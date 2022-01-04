using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;
using UnityEngine;

namespace TaleSpireExplore
{
	public partial class EdtMesh : UserControl, IValueEditor
	{
		public EdtMesh()
		{
			InitializeComponent();
		}

		public IValueChangedListener ValueChangedListener { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public void ValueChanged(object newValue)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, true);
		}

		public Type GetValueType()
		{
			return typeof(Mesh);
		}

		bool changingInternally;
		
		void SelectMaterial(string name)
		{
			foreach (object item in cmbMesh.Items)
				if (item is Mesh mesh && mesh.name == name)
				{
					cmbMesh.SelectedItem = item;
					return;
				}
		}

		public void SetValue(object newValue)
		{
			changingInternally = true;
			try
			{
				if (newValue is Mesh mesh)
				{
					SelectMaterial(mesh.name);
				}
				else if (newValue is string meshName)
				{
					SelectMaterial(meshName);
				}
			}
			finally
			{
				changingInternally = false;
			}
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeMesh result = new ChangeMesh();
			if (cmbMesh.SelectedItem != null)
				result.SetValue(cmbMesh.SelectedItem as string);
			return result;
		}

		private void cmbMesh_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			string selectedMeshName = cmbMesh.SelectedItem as string;
			if (selectedMeshName == null)
				return;
			Mesh existingMesh = Talespire.Mesh.Get(selectedMeshName);
			if (existingMesh != null)
				ValueChanged(existingMesh);
		}

		void LoadComboboxWithAllMeshes()
		{
			cmbMesh.Items.Clear();
			Talespire.Mesh.Invalidate();
			List<string> allNames = Talespire.Mesh.GetAllNames();
			cmbMesh.Items.AddRange(allNames.ToArray());
		}

		private void cmbMesh_DropDown(object sender, EventArgs e)
		{
			LoadComboboxWithAllMeshes();
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
