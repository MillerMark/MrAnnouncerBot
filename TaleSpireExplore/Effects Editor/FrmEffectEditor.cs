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

		private void btnAddPrefab_Click(object sender, EventArgs e)
		{
			string prefabName = FrmSelectPrefab.SelectPrefab();
			if (prefabName == null)
				return;

			GameObjectNode node = new GameObjectNode()
			{
				Text = prefabName,
				GameObject = Talespire.Prefabs.Clone(prefabName)
			};

			TreeNode selectedNode = trvEffectHierarchy.SelectedNode;
			if (selectedNode != null)
				selectedNode.Nodes.Add(node);
			else
				trvEffectHierarchy.Nodes.Add(node);
			try
			{
				AddChildren(node);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		void AddChildren(GameObjectNode node)
		{
			if (node == null)
				return;
			Talespire.Log.Debug($"Adding children to {node.Text}");
			if (node.GameObject == null)
				return;
			Transform transform = node.GameObject.transform;
			if (transform == null)
				return;
			int childCount = transform.childCount;
			if (childCount == 0)
				return;

			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = transform.GetChild(i);
				if (childTransform != null)
				{
					GameObject gameObject = childTransform.gameObject;
					if (gameObject != null)
						AddChild(node, gameObject);
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
			node.Nodes.Add(childNode);
			AddChildren(node);
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

		private void AddPropertiesToNodes(TreeNodeCollection nodes, Type type, object parentInstance)
		{
			nodes.Clear();
			Talespire.Log.Debug($"AddPropertiesToNodes for type {type.FullName}...");
			try
			{
				AddProperties(nodes, type, parentInstance);
				AddFields(nodes, type, parentInstance);
				AddComponents(nodes, parentInstance);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
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

		private void AddComponents(TreeNodeCollection nodes, object parentInstance)
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

		private void AddFields(TreeNodeCollection nodes, Type type, object parentInstance)
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

		private void AddProperties(TreeNodeCollection nodes, Type type, object parentInstance)
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
			AddValueEditor(allValueEditors.Register(new EdtVector3()));
			AddValueEditor(allValueEditors.Register(new EdtMinMaxGradient()));
			allValueEditors.ValueChanged += AllValueEditors_ValueChanged;
		}

		private void trvProperties_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				HideExistingEditors();

				if (e.Node is PropertyNode propertyNode)
				{
					string typeName = propertyNode.ParentInstance?.GetType().Name;
					if (typeName == null)
						typeName = "(unknown type)";
					lblPropertyName.Text = $"{propertyNode.ParentInstance.GetType().Name}{propertyNode.Text}:";
					ShowValueEditor(propertyNode);
				}
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
					if (propertyNode.PropertyInfo != null)
						propertyNode.PropertyInfo.SetValue(propertyNode.ParentInstance, ea.Value);
					else if (propertyNode.FieldInfo != null)
						propertyNode.FieldInfo.SetValue(propertyNode.ParentInstance, ea.Value);
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
			// TODO: remove the existing test effect and add a new one.
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
	}
}
