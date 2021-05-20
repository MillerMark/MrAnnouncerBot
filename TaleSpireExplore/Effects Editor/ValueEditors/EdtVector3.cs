using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireExplore
{
	public partial class EdtVector3 : UserControl, IValueEditor
	{
		public EdtVector3()
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
			return typeof(Vector3);
		}

		public void SetValue(object newValue)
		{
			if (!(newValue is Vector3 vector3))
				return;
			
			tbxVector3.Text = $"{vector3.x}, {vector3.y}, {vector3.z}";
		}

		private void tbxVector3_TextChanged(object sender, EventArgs e)
		{
			string[] xyz = tbxVector3.Text.Split(',');
			if (xyz.Length != 3)
				return;

			if (float.TryParse(xyz[0].Trim(), out float x))
				if (float.TryParse(xyz[1].Trim(), out float y))
					if (float.TryParse(xyz[2].Trim(), out float z))
						ValueChanged(new Vector3(x, y, z));
		}
	}
}
