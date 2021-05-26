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
			string prefabName = FrmSelectPrefab.SelectPrefab(this);
			if (prefabName == null)
				return;

			GameObjectNode node = new GameObjectNode()
			{
				Text = prefabName,
				CompositeEffect = new CompositeEffect()
				{
					Prefab = prefabName
				}
			};

			TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
			if (selectedNode != null)
				selectedNode.Nodes.Add(node);
			else
				trvEffectHierarchy.Nodes.Add(node);
			node.GameObject = Talespire.Prefabs.Clone(prefabName, GetNewInstanceId());
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

		void AddChildren(GameObjectNode node)
		{
			if (node == null)
				return;
			if (node.GameObject == null)
				return;
			Transform transform = node.GameObject.transform;
			if (transform == null)
				return;
			int childCount = transform.childCount;
			if (childCount == 0)
				return;

			Talespire.Log.Debug($"Adding {childCount} children to {node.Text}");

			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = transform.GetChild(i);
				if (childTransform != null)
				{
					GameObject gameObject = childTransform.gameObject;
					if (gameObject != null)
					{
						Talespire.Log.Debug($"Adding child {gameObject.name}");
						AddChild(node, gameObject);
					}
				}
			}
		}

		List<GameObject> allGameObjectsAdded = new List<GameObject>();

		void AddChild(GameObjectNode node, GameObject gameObject)
		{
			if (node == null)
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
			childNode.GameObject = gameObject;
			childNode.Text = gameObject.name;
			childNode.Checked = gameObject.activeSelf;
			node.Nodes.Add(childNode);
			AddChildren(childNode);
		}

		void AddVirtualNode(TreeNode treeNode)
		{
			treeNode.Nodes.Add(STR_VirtualNodeKey, "...");
		}

		void ShowProperties(GameObject gameObject)
		{
			FlatNodes = null;
			TreeNodeCollection nodes = trvProperties.Nodes;
			AddPropertiesToNodes(nodes, gameObject.GetType(), gameObject);
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
			if (e.Node is TreeNode treeNode)
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
						e.Node.Nodes.Clear();
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
			AddValueEditor(allValueEditors.Register(new EdtBool()));
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
					ShowSpecialEditor(materialColorPropertyNode);
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
			Talespire.Log.Debug("ShowValueEditor...");
			Type type = propertyNode.ValueInstance?.GetType();
			if (type == null)
			{
				Talespire.Log.Error("type is null!");
				return;
			}

			IValueEditor valueEditor = allValueEditors.Get(type);
			if (valueEditor == null)
			{
				valueEditor = noneEditor;
				Talespire.Log.Debug($"(no valueEditor yet for {type.FullName})");
			}
			else
				Talespire.Log.Debug($"valueEditor found for {type.FullName}!");

			if (valueEditor is UserControl userControl)
				userControl.Visible = true;

			if (propertyNode.PropertyInfo != null)
			{
				Talespire.Log.Debug("Setting property value...");
				valueEditor.SetValue(propertyNode.PropertyValue);
			}
			else if (propertyNode.FieldInfo != null)
			{
				Talespire.Log.Debug("Setting field value...");
				valueEditor.SetValue(propertyNode.FieldValue);
			}
		}

		void ShowSpecialEditor(MaterialColorPropertyNode materialColorPropertyNode)
		{
			IValueEditor valueEditor = allValueEditors.Get(typeof(UnityEngine.Color));
			if (valueEditor == null)
			{
				valueEditor = noneEditor;
				Talespire.Log.Debug($"(no valueEditor yet for UnityEngine.Color)");
			}
			else
				Talespire.Log.Debug($"valueEditor found for UnityEngine.Color!");

			if (valueEditor is UserControl userControl)
				userControl.Visible = true;

			valueEditor.SetValue(materialColorPropertyNode.Material.GetColor(materialColorPropertyNode.Name));
		}

		private void HideExistingEditors()
		{
			noneEditor.Visible = false;
			foreach (IValueEditor valueEditor in allValueEditors.Editors.Values)
				if (valueEditor is UserControl userControl)
					userControl.Visible = false;
		}

		private void AllValueEditors_ValueChanged(object sender, ValueChangedEventArgs ea)
		{
			try
			{
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

		private void btnTestEffect_Click(object sender, EventArgs e)
		{
			ClearTestEffects();
			CharacterPosition sourcePosition = new CharacterPosition() { Position = new VectorDto(10, 0.5f, 0) };
			CharacterPosition targetPosition = new CharacterPosition() { Position = new VectorDto(0, 0.5f, 0) };
			AddEffectsForNodes(sourcePosition, targetPosition, trvEffectHierarchy.Nodes);
		}

		private void AddEffectsForNodes(CharacterPosition sourcePosition, CharacterPosition targetPosition, TreeNodeCollection nodes)
		{
			try
			{
				foreach (TreeNode treeNode in nodes)
					if (treeNode is GameObjectNode gameObjectNode)
					{
						if (gameObjectNode.CompositeEffect != null)
							gameObjectNode.GameObject = gameObjectNode.CompositeEffect.Create(sourcePosition, targetPosition, GetNewInstanceId());

						if (gameObjectNode.Nodes != null)
							AddEffectsForNodes(sourcePosition, targetPosition, gameObjectNode.Nodes);
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

		public List<TreeNode> FlatNodes {
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
			ClearTestEffects();
		}

		private void ClearTestEffects()
		{
			foreach (string instanceId in instanceIds)
				Talespire.Instances.Delete(instanceId);
			instanceIds.Clear();
		}

		private void FrmEffectEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			ClearTestEffects();
		}

		private void trvEffectHierarchy_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node is GameObjectNode gameObjectNode && gameObjectNode.GameObject != null)
				gameObjectNode.GameObject.SetActive(e.Node.Checked);
		}
	}
}
