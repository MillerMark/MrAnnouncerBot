using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;
using Newtonsoft.Json;

namespace TaleSpireExplore
{
	public partial class FrmEffectEditor : Form
	{
		const string STR_VirtualNodeKey = "VirtualNode";
		public FrmEffectEditor()
		{
			InitializeComponent();
			RegisterValueEditors();
		}

		List<string> instanceIds = new List<string>();

		private void btnAddPrefab_Click(object sender, EventArgs e)
		{
			string prefabName = FrmSelectItem.SelectPrefab(this);
			if (prefabName == null)
				return;

			GameObjectNode node = new GameObjectNode()
			{
				Text = prefabName,
				CompositeEffect = new CompositeEffect()
				{
					PrefabToCreate = prefabName
				}
			};

			TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
			GameObjectNode parentNode = null;
			if (selectedNode != null)
			{
				selectedNode.Nodes.Add(node);
				parentNode = selectedNode as GameObjectNode;
			}
			else
				trvEffectHierarchy.Nodes.Add(node);
			
			node.GameObject = Talespire.Prefabs.Clone(prefabName, GetNewInstanceId());
			if (parentNode != null)
			{
				node.GameObject.transform.SetParent(parentNode.GameObject.transform);
			}
			node.Checked = node.GameObject.activeSelf;
			try
			{
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private string GetNewInstanceId()
		{
			string instanceId = Guid.NewGuid().ToString();
			instanceIds.Add(instanceId);
			return instanceId;
		}

		void AddChildren(GameObjectNode parentNode)
		{
			if (parentNode == null)
				return;
			if (parentNode.GameObject == null)
				return;
			Transform transform = parentNode.GameObject.transform;
			if (transform == null)
				return;
			int childCount = transform.childCount;
			if (childCount == 0)
				return;

			Talespire.Log.Debug($"Adding {childCount} children to {parentNode.Text}");

			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = transform.GetChild(i);
				if (childTransform != null)
				{
					GameObject gameObject = childTransform.gameObject;
					if (gameObject != null)
					{
						Talespire.Log.Debug($"Adding child {gameObject.name}");
						AddChild(parentNode, gameObject);
					}
				}
			}
		}

		List<GameObject> allGameObjectsAdded = new List<GameObject>();

		void AddChild(GameObjectNode parentNode, GameObject gameObject)
		{
			if (parentNode == null)
				return;
			if (gameObject == null)
				return;

			if (allGameObjectsAdded.Contains(gameObject))
			{
				//Talespire.Log.Error("allGameObjectsAdded already contains this gameObject!");
				return;
			}

			allGameObjectsAdded.Add(gameObject);

			GameObjectNode childNode = new GameObjectNode();
			childNode.CompositeEffect = new CompositeEffect();

			// TODO: Support adding empty GameObjects, and renaming them later!!!
			childNode.CompositeEffect.ExistingChildName = gameObject.name;
			parentNode.CompositeEffect.Children.Add(childNode.CompositeEffect);

			childNode.GameObject = gameObject;
			childNode.Text = gameObject.name;
			childNode.Checked = gameObject.activeSelf;
			parentNode.Nodes.Add(childNode);
			AddChildren(childNode);
		}

		void AddVirtualNode(TreeNode treeNode)
		{
			treeNode.Nodes.Add(STR_VirtualNodeKey, "...");
		}

		void EnableJumpButtons()
		{
			btnParticleSystemRenderer.Enabled = false;
			btnMeshRendererMaterial.Enabled = false;
			btnLocalPosition.Enabled = false;
			btnLocalScale.Enabled = false;
			foreach (TreeNode treeNode in trvProperties.Nodes)
			{
				if (treeNode.Text == "<ParticleSystemRenderer>")
					btnParticleSystemRenderer.Enabled = true;
				else if (treeNode.Text == "<MeshRenderer>")
					btnMeshRendererMaterial.Enabled = true;
				else if (treeNode.Text == "<Transform>")
				{
					btnLocalScale.Enabled = true;
					btnLocalPosition.Enabled = true;
				}
			}
		}

		void ShowProperties(GameObject gameObject)
		{
			FlatNodes = null;
			TreeNodeCollection nodes = trvProperties.Nodes;
			AddPropertiesToNodes(nodes, gameObject.GetType(), gameObject);
			HideExistingEditors();
			EnableJumpButtons();
		}

		void AddSpecialProperties(List<TreeNode> nodes, Type type, object parentInstance)
		{
			try
			{
				if (!(parentInstance is Material material))
					return;

				if (!(material.shader is Shader shader))
					return;

				int propertyCount = shader.GetPropertyCount();
				for (int i = 0; i < propertyCount; i++)
				{
					if (shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
					{
						string propertyName = shader.GetPropertyName(i);
						nodes.Add(new MaterialColorPropertyNode()
						{
							Name = propertyName,
							Text = propertyName,
							Material = material
						});
					}
				}
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					Talespire.Log.Error($"{ex.Message} in AddSpecialProperties....");
					MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", $"{ex.GetType().Name}!");
					ex = ex.InnerException;
				}
			}
		}

		private void AddPropertiesToNodes(TreeNodeCollection nodes, Type type, object parentInstance)
		{
			nodes.Clear();
			Talespire.Log.Debug($"AddPropertiesToNodes for type {type.FullName}...");
			try
			{
				List<TreeNode> newNodes = new List<TreeNode>();
				AddProperties(newNodes, type, parentInstance);
				AddFields(newNodes, type, parentInstance);
				AddComponents(newNodes, parentInstance);
				AddSpecialProperties(newNodes, type, parentInstance);

				nodes.AddRange(newNodes.OrderBy(x => x.Text).ToArray());
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					Talespire.Log.Error($"{ex.GetType().Name} in AddPropertiesToNodes....");
					MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", $"{ex.GetType().Name}!");
					ex = ex.InnerException;
				}
			}
		}

		static bool CanSkipComponent(UnityEngine.Component component)
		{
			if (component == null)
				return true;
			if (component.name == "ParticleSystemRenderer")
				return true;
			return false;
		}

		private void AddComponents(List<TreeNode> nodes, object parentInstance)
		{
			if (parentInstance is GameObject gameObject)
			{
				UnityEngine.Component[] components = gameObject.GetComponents<UnityEngine.Component>();
				if (components != null)
					foreach (UnityEngine.Component component in components)
					{
						if (CanSkipComponent(component))
							continue;
						ComponentNode componentNode = new ComponentNode()
						{
							Component = component,
							Text = $"<{component.GetType().Name}>",
							Type = component.GetType(),
						};
						nodes.Add(componentNode);
						if (WillHaveChildNodes(component, componentNode.Text))
							AddVirtualNode(componentNode);
					}
			}
		}

		private void AddFields(List<TreeNode> nodes, Type type, object parentInstance)
		{
			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			if (fieldInfos != null)
				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					if (CanSkipField(fieldInfo))
					{
						Talespire.Log.Debug($"Skipping field {fieldInfo.Name}...");
						continue;
					}

					PropertyNode field = new PropertyNode()
					{
						PropertyName = fieldInfo.Name,
						Text = fieldInfo.Name,
						FieldInfo = fieldInfo,
						ValueInstance = fieldInfo.GetValue(parentInstance),
						ParentInstance = parentInstance
					};
					if (WillHaveChildNodes(field.ValueInstance, field.PropertyName))
						AddVirtualNode(field);
					nodes.Add(field);
				}
		}

		private static bool CanSkipField(FieldInfo fieldInfo)
		{
			return fieldInfo.IsInitOnly || fieldInfo.Name.StartsWith("m_");
		}

		static bool WillHaveChildNodes(object instance, string memberName)
		{
			if (instance == null)
				return false;

			try
			{
				Type type = instance.GetType();
				if (HasAnyProperties(type, instance, false))
				{
					Talespire.Log.Debug($"{type.Name}.{memberName} has properties (child nodes)!");
					return true;
				}

				if (HasAnyFields(type))
				{
					Talespire.Log.Debug($"{type.Name}.{memberName} has fields (child nodes)!");
					return true;
				}

				if (instance is GameObject gameObject)
					if (HasAnyComponents(gameObject))
					{
						Talespire.Log.Debug($"{type.Name}.{memberName} has components (child nodes)!");
						return true;
					}
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					MessageBox.Show(ex.Message, $"{ex.GetType()} in WillHaveChildNodes!");
					ex = ex.InnerException;
				}
				return false;
			}

			return false;
		}

		private static bool HasAnyProperties(Type type, object instance, bool checkForChildNodes)
		{
			PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfos == null)
				return false;

			foreach (PropertyInfo propertyInfo in propertyInfos)
				if (!CanSkipProperty(propertyInfo, instance, checkForChildNodes))
					return true;

			return false;
		}

		private static bool HasAnyFields(Type type)
		{
			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			if (fieldInfos == null)
				return false;

			foreach (FieldInfo fieldInfo in fieldInfos)
				if (!CanSkipField(fieldInfo))
					return true;

			return false;
		}

		private static bool HasAnyComponents(GameObject gameObject)
		{
			if (gameObject == null)
				return false;

			UnityEngine.Component[] components = gameObject.GetComponents<UnityEngine.Component>();
			if (components == null)
				return false;

			foreach (UnityEngine.Component component in components)
			{
				if (!CanSkipComponent(component))
					return true;
			}

			return false;
		}

		private void AddProperties(List<TreeNode> nodes, Type type, object parentInstance)
		{
			PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfos != null)
				Talespire.Log.Debug($"AddProperties - count == {propertyInfos.Length}");
			else
				Talespire.Log.Debug($"no properties found");
			if (propertyInfos != null)
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					if (CanSkipProperty(propertyInfo, parentInstance, true))
					{
						Talespire.Log.Debug($"Skipping property {propertyInfo.Name} because it has no set method.");
						continue;
					}

					PropertyNode prop = new PropertyNode()
					{
						PropertyName = propertyInfo.Name,
						Text = propertyInfo.Name,
						PropertyInfo = propertyInfo,
						ValueInstance = propertyInfo.GetValue(parentInstance),
						ParentInstance = parentInstance
					};

					PropertyInfo enabledProperty = propertyInfo.PropertyType.GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
					if (enabledProperty != null)
					{
						Talespire.Log.Debug($"Found an enabledProperty.");
						bool enabled = (bool)enabledProperty.GetValue(prop.ValueInstance);
						Talespire.Log.Debug($"enabled = {enabled}");
						if (!enabled)
							prop.IsDisabled = true;
					}

					if (WillHaveChildNodes(prop.ValueInstance, prop.PropertyName))
						AddVirtualNode(prop);
					nodes.Add(prop);
				}
		}

		private static bool CanSkipProperty(PropertyInfo propertyInfo, object instance, bool checkForChildNodes)
		{
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters != null && indexParameters.Length > 0)
				return true;

			if (checkForChildNodes)
				if (propertyInfo.SetMethod == null && !WillHaveChildNodes(instance, propertyInfo.Name))
					return true;

			return false;
		}

		private void trvEffectHierarchy_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node is GameObjectNode prefabNode)
				ShowProperties(prefabNode.GameObject);
		}

		private void trvProperties_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			AddChildNodesIfNecessary(e.Node);
		}

		private void AddChildNodesIfNecessary(TreeNode treeNode)
		{
			if (treeNode.Nodes.ContainsKey(STR_VirtualNodeKey))
			{
				try
				{
					treeNode.Nodes.Clear();

					if (treeNode is PropertyNode propertyNode)
						FillInPropertyNodes(propertyNode);
					else if (treeNode is ComponentNode componentNode)
						FillInComponentNodes(componentNode);
				}
				catch
				{
					treeNode.Nodes.Clear();
				}
			}
		}

		private void FillInPropertyNodes(PropertyNode propertyNode)
		{
			if (propertyNode.ValueInstance != null)
				Talespire.Log.Debug($"FillInPropertyNodes for {propertyNode.ValueInstance.GetType()}");
			else
				Talespire.Log.Error($"propertyNode.ValueInstance == null!!");

			if (propertyNode.PropertyInfo != null)
				AddPropertiesToNodes(propertyNode.Nodes, propertyNode.PropertyInfo.PropertyType, propertyNode.ValueInstance);
			else if (propertyNode.FieldInfo != null)
				AddPropertiesToNodes(propertyNode.Nodes, propertyNode.FieldInfo.FieldType, propertyNode.ValueInstance);
		}

		private void FillInComponentNodes(ComponentNode componentNode)
		{
			AddPropertiesToNodes(componentNode.Nodes, componentNode.Type, componentNode.Component);
		}

		AllValueEditors allValueEditors;
		void AddValueEditor(UserControl userControl)
		{
			if (userControl == null)
				return;
			pnlProperties.Controls.Add(userControl);
			userControl.Visible = false;
		}

		void RegisterValueEditors()
		{
			allValueEditors = new AllValueEditors();
			AddValueEditor(allValueEditors.Register(new EdtFloat()));
			AddValueEditor(allValueEditors.Register(new EdtInt()));
			AddValueEditor(allValueEditors.Register(new EdtMaterial()));
			AddValueEditor(allValueEditors.Register(new EdtBool()));
			AddValueEditor(allValueEditors.Register(new EdtString()));
			AddValueEditor(allValueEditors.Register(new EdtColor()));
			AddValueEditor(allValueEditors.Register(new EdtVector3()));
			AddValueEditor(allValueEditors.Register(new EdtMinMaxGradient()));
			allValueEditors.ValueChanged += AllValueEditors_ValueChanged;
		}

		private void trvProperties_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				HideExistingEditors();

				string typeName;

				if (e.Node is PropertyNode propertyNode)
				{
					typeName = propertyNode.ParentInstance?.GetType().Name;
					if (typeName == null)
						typeName = "(unknown type)";
					ShowValueEditor(propertyNode);
				}
				else if (e.Node is MaterialColorPropertyNode materialColorPropertyNode)
				{
					typeName = "Color";
					ShowMaterialColorEditor(materialColorPropertyNode);
				}
				else
					return;

				lblPropertyName.Text = $"{typeName}.{e.Node.Text}:";


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception");
			}
		}


		EdtNone noneEditor = new EdtNone();
		private void ShowValueEditor(PropertyNode propertyNode)
		{
			activeValueEditor = null;
			Talespire.Log.Debug("ShowValueEditor...");
			Type type = propertyNode?.ValueInstance?.GetType();
			if (type == null)
			{
				Talespire.Log.Error("type is null!");
				return;
			}

			activeValueEditor = allValueEditors.Get(type);
			if (activeValueEditor == null)
			{
				activeValueEditor = noneEditor;
				Talespire.Log.Debug($"(no valueEditor yet for {type.FullName})");
			}
			else
				Talespire.Log.Debug($"valueEditor found for {type.FullName}!");

			if (activeValueEditor is UserControl userControl)
				userControl.Visible = true;

			if (propertyNode.PropertyInfo != null)
			{
				Talespire.Log.Debug("Setting property value...");
				activeValueEditor.SetValue(propertyNode.PropertyValue);
			}
			else if (propertyNode.FieldInfo != null)
			{
				Talespire.Log.Debug("Setting field value...");
				activeValueEditor.SetValue(propertyNode.FieldValue);
			}
		}

		IValueEditor activeValueEditor;

		void ShowMaterialColorEditor(MaterialColorPropertyNode materialColorPropertyNode)
		{
			activeValueEditor = allValueEditors.Get(typeof(UnityEngine.Color));
			if (activeValueEditor == null)
			{
				activeValueEditor = noneEditor;
				Talespire.Log.Debug($"(no valueEditor yet for UnityEngine.Color)");
			}
			else
				Talespire.Log.Debug($"valueEditor found for UnityEngine.Color!");

			if (activeValueEditor is UserControl userControl)
				userControl.Visible = true;

			activeValueEditor.SetValue(materialColorPropertyNode.Material.GetColor(materialColorPropertyNode.Name));
		}

		private void HideExistingEditors()
		{
			noneEditor.Visible = false;
			foreach (IValueEditor valueEditor in allValueEditors.Editors.Values)
				if (valueEditor is UserControl userControl)
					userControl.Visible = false;
		}

		string GetFullPropertyName(TreeNode node)
		{
			List<string> propNames = new List<string>();
			while (node != null)
			{
				propNames.Insert(0, node.Text);
				node = node.Parent;
			}
			return string.Join(".", propNames);
		}

		void JumpTo(string propertyJump)
		{
			Talespire.Log.Debug($"Jump to: {propertyJump}");
			string[] parts = propertyJump.Split('.');
			TreeNodeCollection searchNodes = trvProperties.Nodes;
			TreeNode foundNode = null;
			foreach (string part in parts)
			{
				foreach (TreeNode treeNode in searchNodes)
				{
					if (treeNode.Text == part)
					{
						foundNode = treeNode;
						AddChildNodesIfNecessary(treeNode);
						searchNodes = treeNode.Nodes;
						break;
					}
				}
			}
			if (foundNode != null)
			{
				trvProperties.SelectedNode = foundNode;
				AddChildNodesIfNecessary(foundNode);
				if (propertyJump.EndsWith(".material"))
				{
					foundNode.Expand();
					if (foundNode.Nodes != null && foundNode.Nodes.Count > 0)
						trvProperties.SelectedNode = foundNode.Nodes[0];
				}
			}
		}

		private void AllValueEditors_ValueChanged(object sender, ValueChangedEventArgs ea)
		{
			// TODO: Add a property modification to the Composites structure
			try
			{
				if (!(trvEffectHierarchy.SelectedNode is GameObjectNode gameObjectNode))
					return;

				if (trvProperties.SelectedNode is PropertyNode propertyNode)
				{
					if (propertyNode.PropertyInfo != null)
						propertyNode.PropertyInfo.SetValue(propertyNode.ParentInstance, ea.Value);
					else if (propertyNode.FieldInfo != null)
						propertyNode.FieldInfo.SetValue(propertyNode.ParentInstance, ea.Value);
				}
				else if (trvProperties.SelectedNode is MaterialColorPropertyNode materialColorPropertyNode)
				{
					materialColorPropertyNode.Material.SetColor(materialColorPropertyNode.Name, (UnityEngine.Color)ea.Value);
				}
				else
					return;

				Talespire.Log.Debug($"Getting a property changer for {trvProperties.SelectedNode.Name}...");

				BasePropertyChanger propertyChanger = activeValueEditor.GetPropertyChanger();
				if (propertyChanger != null)
				{
					Talespire.Log.Debug($"Found propertyChanger!!!");
					propertyChanger.Name = GetFullPropertyName(trvProperties.SelectedNode);
					gameObjectNode.CompositeEffect.AddProperty(propertyChanger);
				}
				CreateJson();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception");
			}
		}

		private void btnApplyChange_Click(object sender, EventArgs e)
		{
			// TODO: add a new node to the effect tree, or change an existing node.
		}

		const string effectEditorTestId = "EffectEditorTest";

		private void btnTestEffect_Click(object sender, EventArgs e)
		{
			//CharacterPosition sourcePosition = new CharacterPosition() { Position = new VectorDto(10, 0.5f, 0) };
			//CharacterPosition targetPosition = new CharacterPosition() { Position = new VectorDto(0, 0.5f, 0) };
			//CreateEffectsForNodes(sourcePosition, targetPosition, trvEffectHierarchy.Nodes);

			DeserializeJsonToCreateEffect();

		}

		private GameObject DeserializeJsonToCreateEffect()
		{
			try
			{
				Talespire.Instances.Delete(effectEditorTestId);
				Talespire.GameObjects.InvalidateFound();
				CompositeEffect compositeEffect = JsonConvert.DeserializeObject<CompositeEffect>(tbxJson.Text);
				compositeEffect.RebuildPropertiesAfterLoad();
				CharacterPosition janusPosition = Talespire.Minis.GetPosition(FrmExplorer.JanusId);
				CharacterPosition merkinPosition = Talespire.Minis.GetPosition(FrmExplorer.MerkinId);
				return compositeEffect.CreateOrFind(effectEditorTestId, merkinPosition, janusPosition);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
				return null;
			}
		}

		private void CreateEffectsForNodes(CharacterPosition sourcePosition, CharacterPosition targetPosition, TreeNodeCollection nodes)
		{
			try
			{
				foreach (TreeNode treeNode in nodes)
					if (treeNode is GameObjectNode gameObjectNode)
					{
						if (gameObjectNode.CompositeEffect != null)
							gameObjectNode.GameObject = gameObjectNode.CompositeEffect.CreateOrFind(GetNewInstanceId(), sourcePosition, targetPosition);

						if (gameObjectNode.Nodes != null)
							CreateEffectsForNodes(sourcePosition, targetPosition, gameObjectNode.Nodes);
					}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void tbxSearch_TextChanged(object sender, EventArgs e)
		{

		}

		List<TreeNode> flatNodes;

		public List<TreeNode> FlatNodes
		{
			get
			{
				if (flatNodes == null)
				{
					flatNodes = new List<TreeNode>();
					AddNodesToFlatList(flatNodes, trvProperties.Nodes);
				}
				return flatNodes;
			}
			set => flatNodes = value;
		}

		private void btnFindNext_Click(object sender, EventArgs e)
		{
			string searchText = tbxSearch.Text.ToLower();
			if (string.IsNullOrWhiteSpace(searchText))
				return;

			int startSearchIndex = -1;
			if (trvProperties.SelectedNode != null)
				startSearchIndex = FlatNodes.IndexOf(trvProperties.SelectedNode);

			for (int i = startSearchIndex + 1; i < FlatNodes.Count; i++)
				if (FlatNodes[i].Text.ToLower().Contains(searchText))
				{
					trvProperties.SelectedNode = FlatNodes[i];
					return;
				}
		}

		private void btnFindPrevious_Click(object sender, EventArgs e)
		{
			string searchText = tbxSearch.Text.ToLower();
			if (string.IsNullOrWhiteSpace(searchText))
				return;

			int startSearchIndex = FlatNodes.Count;
			if (trvProperties.SelectedNode != null)
				startSearchIndex = FlatNodes.IndexOf(trvProperties.SelectedNode);

			for (int i = startSearchIndex - 1; i >= 0; i--)
				if (FlatNodes[i].Text.ToLower().Contains(searchText))
				{
					trvProperties.SelectedNode = FlatNodes[i];
					return;
				}
		}

		private static void AddNodesToFlatList(List<TreeNode> flatNodes, TreeNodeCollection nodes)
		{
			if (nodes == null)
				return;
			foreach (TreeNode treeNode in nodes)
			{
				flatNodes.Add(treeNode);
				if (treeNode.Nodes.ContainsKey(STR_VirtualNodeKey))
					continue;
				AddNodesToFlatList(flatNodes, treeNode.Nodes);
			}
		}

		private void trvProperties_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (!(sender is TreeView treeView))
				return;

			Brush fontBrush = Brushes.Black;
			if (e.Node is PropertyNode propertyNode)
				if (propertyNode.IsDisabled)
					fontBrush = Brushes.Gray;

			if (e.Node is MaterialColorPropertyNode)
				fontBrush = Brushes.DarkRed;

			Font nodeFont = e.Node.NodeFont;
			if (nodeFont == null)
				nodeFont = treeView.Font;
			e.Graphics.DrawString(e.Node.Text, nodeFont, fontBrush, Rectangle.Inflate(e.Bounds, 2, 0));


			if ((e.State & TreeNodeStates.Focused) != 0)
			{
				using (Pen focusPen = new Pen(System.Drawing.Color.Black))
				{
					focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
					Rectangle focusBounds = NodeBounds(treeView, e.Node, nodeFont);
					focusBounds.Size = new Size(focusBounds.Width - 1,
					focusBounds.Height - 1);
					e.Graphics.DrawRectangle(focusPen, focusBounds);
				}
			}

			e.DrawDefault = false;


		}

		// Returns the bounds of the specified node, including the region 
		// occupied by the node label and any node tag displayed.
		private Rectangle NodeBounds(TreeView treeView, TreeNode node, Font font)
		{
			// Set the return value to the normal node bounds.
			Rectangle bounds = node.Bounds;
			if (node.Tag != null)
			{
				// Retrieve a Graphics object from the TreeView handle
				// and use it to calculate the display width of the tag.
				System.Drawing.Graphics g = treeView.CreateGraphics();
				int tagWidth = (int)g.MeasureString
						(node.Tag.ToString(), font).Width + 6;

				// Adjust the node bounds using the calculated value.
				bounds.Offset(tagWidth / 2, 0);
				bounds = Rectangle.Inflate(bounds, tagWidth / 2, 0);
				g.Dispose();
			}

			return bounds;
		}

		private void btnClearEffect_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Delete Everything? Are You Sure?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
				DeleteEverything();
		}

		private void DeleteEverything()
		{
			foreach (string instanceId in instanceIds)
				Talespire.Instances.Delete(instanceId);
			instanceIds.Clear();
			trvEffectHierarchy.Nodes.Clear();
			trvProperties.Nodes.Clear();
			HideExistingEditors();
		}

		private void FrmEffectEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			DeleteEverything();
		}

		private void trvEffectHierarchy_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node is GameObjectNode gameObjectNode && gameObjectNode.GameObject != null)
			{
				try
				{
					string boolVal;
					if (e.Node.Checked)
						boolVal = "true";
					else
						boolVal = "false";
					gameObjectNode.CompositeEffect.AddProperty(new ChangeBool("active", boolVal));
					gameObjectNode.GameObject.SetActive(e.Node.Checked);
				}
				catch (Exception ex)
				{
					while (ex != null)
					{
						MessageBox.Show(ex.Message, $"{ex.GetType()} Changing active Property!");
						ex = ex.InnerException;
					}
				}
				CreateJson();
			}
		}

		private void CreateJson()
		{
			try
			{
				if (trvEffectHierarchy.Nodes.Count > 0)
				{
					GameObjectNode topNode = trvEffectHierarchy.Nodes[0] as GameObjectNode;
					if (topNode != null && topNode.CompositeEffect != null)
					{
						string serializeObject = JsonConvert.SerializeObject(topNode.CompositeEffect, Formatting.Indented);
						tbxJson.Text = serializeObject;
					}
				}
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					MessageBox.Show(ex.Message, $"{ex.GetType()} Serializing to JSON!");
					ex = ex.InnerException;
				}
			}
		}

		private void btnCopyJson_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(tbxJson.Text);
		}

		List<GameObject> gameObjectsFound = new List<GameObject>();

		void EnableAllParticleSystems(GameObject gameObject, bool value)
		{
			if (gameObjectsFound.Contains(gameObject))
				return;

			gameObjectsFound.Add(gameObject);

			ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.enabled = value;
			}

			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			if (componentsInChildren != null)
				foreach (Transform transform in componentsInChildren)
					EnableAllParticleSystems(transform.gameObject, value);
		}

		void StartAllParticleSystems(GameObject gameObject)
		{
			gameObjectsFound.Clear();
			EnableAllParticleSystems(gameObject, true);
		}

		void StopAllParticleSystems(GameObject gameObject)
		{
			gameObjectsFound.Clear();
			EnableAllParticleSystems(gameObject, false);
		}

		private void btnStartAllParticleSystems_Click(object sender, EventArgs e)
		{
			foreach (TreeNode treeNode in trvEffectHierarchy.Nodes)
				if (treeNode is GameObjectNode gameObjectNode)
					StartAllParticleSystems(gameObjectNode.GameObject);
		}

		private void btnStopAllParticleSystems_Click(object sender, EventArgs e)
		{
			foreach (TreeNode treeNode in trvEffectHierarchy.Nodes)
				if (treeNode is GameObjectNode gameObjectNode)
					StopAllParticleSystems(gameObjectNode.GameObject);
		}

		private void btnAttackJanus_Click(object sender, EventArgs e)
		{
			try
			{
				GameObject projectile = DeserializeJsonToCreateEffect();
				if (projectile == null)
				{
					MessageBox.Show("Need Json to Deserialize!", "Error");
				}

				Translator translator = projectile.AddComponent<Translator>();

				CharacterPosition janusPosition = Talespire.Minis.GetPosition(FrmExplorer.JanusId);
				CharacterPosition merkinPosition = Talespire.Minis.GetPosition(FrmExplorer.MerkinId);

				if (janusPosition == null)
				{
					Talespire.Log.Error("janusPosition is null!");
					return;
				}

				if (merkinPosition == null)
				{
					Talespire.Log.Error("merkinPosition is null!");
					return;
				}

				projectile.transform.position = merkinPosition.Position.GetVector3();
				translator.StartPosition = GetChest(merkinPosition);
				translator.StopPosition = GetChest(janusPosition);
				float distance = Math.Abs((translator.StartPosition - translator.StopPosition).magnitude);
				float travelTime = 0.06f;
				if (float.TryParse(tbxTravelTime.Text, out float result))
					travelTime = result;
				translator.TravelTime = travelTime * distance;
				translator.easing = EasingOption.EaseInQuint;
				translator.StartTravel();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().ToString());
			}
		}

		private static Vector3 GetChest(CharacterPosition position)
		{
			Vector3 basePosition = position.Position.GetVector3();
			return new Vector3(basePosition.x, basePosition.y + 0.7f, basePosition.z);
		}

		private void JumpToButton_Click(object sender, EventArgs e)
		{
			if (sender is Button button)
				JumpTo(button.Text);
		}

		private void addScriptToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{

		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void btnCreateEmpty_Click(object sender, EventArgs e)
		{

		}

		private void btnCloneExisting_Click(object sender, EventArgs e)
		{
			string existingItemName = FrmSelectItem.SelectGameObject(this);
			if (existingItemName == null)
				return;

			try
			{
				GameObjectNode node = new GameObjectNode()
				{
					Text = existingItemName,
					CompositeEffect = new CompositeEffect()
					{
						ItemToClone = existingItemName
					}
				};

				TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
				if (selectedNode != null)
					selectedNode.Nodes.Add(node);
				else
					trvEffectHierarchy.Nodes.Add(node);
				node.GameObject = Talespire.GameObjects.Clone(existingItemName, GetNewInstanceId());
				node.Checked = node.GameObject.activeSelf;
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnExploreMini_Click(object sender, EventArgs e)
		{
			CreatureBoardAsset mini = FrmSelectItem.SelectMini(this);
			if (mini == null)
				return;

			try
			{
				GameObjectNode node = new GameObjectNode()
				{
					Text = mini.name,
					CompositeEffect = new CompositeEffect()
					{
						Mini = mini,
					}
				};

				TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
				if (selectedNode != null)
					selectedNode.Nodes.Add(node);
				else
					trvEffectHierarchy.Nodes.Add(node);
				node.GameObject = mini.gameObject;
				node.Checked = node.GameObject.activeSelf;
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnCreateEffect_Click(object sender, EventArgs e)
		{
			string existingEffectName = FrmSelectItem.SelectEffectName(this);
			if (existingEffectName == null)
				return;

			try
			{
				GameObjectNode node = new GameObjectNode()
				{
					Text = existingEffectName,
					CompositeEffect = new CompositeEffect()
					{
						EffectNameToCreate = existingEffectName
					}
				};

				TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
				if (selectedNode != null)
					selectedNode.Nodes.Add(node);
				else
					trvEffectHierarchy.Nodes.Add(node);
				node.GameObject = KnownEffects.Create(existingEffectName, GetNewInstanceId());
				node.Checked = node.GameObject.activeSelf;
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnExploreExisting_Click(object sender, EventArgs e)
		{
			string existingItemName = FrmSelectItem.SelectGameObject(this);
			if (existingItemName == null)
				return;

			try
			{
				GameObjectNode node = new GameObjectNode()
				{
					Text = existingItemName,
					CompositeEffect = new CompositeEffect()
					{
						ItemToBorrow = existingItemName
					}
				};

				TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
				if (selectedNode != null)
					selectedNode.Nodes.Add(node);
				else
					trvEffectHierarchy.Nodes.Add(node);
				node.GameObject = Talespire.GameObjects.Get(existingItemName);
				node.Checked = node.GameObject.activeSelf;
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}
	}
}
