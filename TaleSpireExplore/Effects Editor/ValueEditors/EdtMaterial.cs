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
	public partial class EdtMaterial : UserControl, IValueEditor
	{
		public EdtMaterial()
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
				ValueChangedListener.ValueHasChanged(this, newValue);
		}

		public Type GetValueType()
		{
			return typeof(Material);
		}

		bool changingInternally;
		
		void SelectMaterial(string name)
		{
			foreach (object item in cmbMaterial.Items)
				if (item is Material material && material.name == name)
				{
					cmbMaterial.SelectedItem = item;
					return;
				}
		}

		public void SetValue(object newValue)
		{
			changingInternally = true;
			try
			{
				if (newValue is Material material)
				{
					SelectMaterial(material.name);
				}
				else if (newValue is string materialName)
				{
					SelectMaterial(materialName);
				}
			}
			finally
			{
				changingInternally = false;
			}
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeMaterial result = new ChangeMaterial();
			if (cmbMaterial.SelectedItem != null)
				result.SetValue(cmbMaterial.SelectedItem as string);
			return result;
		}

		private void cmbMaterial_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			string selectedMaterialName = cmbMaterial.SelectedItem as string;
			if (selectedMaterialName == null)
				return;
			Material existingMaterial = Talespire.Materials.GetMaterial(selectedMaterialName);
			if (existingMaterial != null)
				ValueChanged(existingMaterial);
		}

		void LoadComboboxWithAllMaterials()
		{
			cmbMaterial.Items.Clear();
			Talespire.Materials.Invalidate();
			List<string> allNames = Talespire.Materials.GetAllMaterialNames();
			cmbMaterial.Items.AddRange(allNames.ToArray());
		}

		private void cmbMaterial_DropDown(object sender, EventArgs e)
		{
			LoadComboboxWithAllMaterials();
		}

		public void EditingProperty(string name)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
