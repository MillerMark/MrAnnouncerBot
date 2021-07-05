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
using TaleSpireCore;

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

		bool changingInternally;
		public void SetValue(object newValue)
		{
			if (!(newValue is Vector3 vector3))
				return;
			changingInternally = true;
			try
			{
				tbxVector3.Text = $"{vector3.x}, {vector3.y}, {vector3.z}";
				trkOffsetX.Value = 0;
				trkOffsetY.Value = 0;
				trkOffsetZ.Value = 0;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void tbxVector3_TextChanged(object sender, EventArgs e)
		{
			UpdateValue();
		}

		private void UpdateValue()
		{
			if (changingInternally)
				return;
			try
			{
				if (GetXYZ(out float x, out float y, out float z))
					ValueChanged(new Vector3(x, y, z));
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
			}
		}

		private bool GetXYZ(out float x, out float y, out float z)
		{
			x = 0;
			y = 0;
			z = 0;
			string[] xyz = tbxVector3.Text.Split(',');
			if (xyz.Length != 3)
				return false;

			if (float.TryParse(xyz[0].Trim(), out x))
				if (float.TryParse(xyz[1].Trim(), out y))
					if (float.TryParse(xyz[2].Trim(), out z))
					{
						x += trkOffsetX.Value / 10f;
						y += trkOffsetY.Value / 10f;
						z += trkOffsetZ.Value / 10f;
						return true;
					}

			return false;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeVector3 result = new ChangeVector3();
			result.SetValue(tbxVector3.Text);
			return result;
		}

		private void trkOffset_Scroll(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			UpdateValue();
		}

		private void trkOffset_Leave(object sender, EventArgs e)
		{
			ApplyAndResetOffsets();
		}

		private void ApplyAndResetOffsets()
		{
			if (GetXYZ(out float x, out float y, out float z))
			{
				changingInternally = true;
				try
				{
					tbxVector3.Text = $"{x}, {y}, {z}";
					trkOffsetX.Value = 0;
					trkOffsetY.Value = 0;
					trkOffsetZ.Value = 0;
				}
				finally
				{
					changingInternally = false;
				}
				UpdateValue();
			}
		}

		private void trkOffset_MouseUp(object sender, MouseEventArgs e)
		{
			ApplyAndResetOffsets();
		}

		private void btnAllZeros_Click(object sender, EventArgs e)
		{
			tbxVector3.Text = "0, 0, 0";
		}

		private void btnAllOnes_Click(object sender, EventArgs e)
		{
			tbxVector3.Text = "1, 1, 1";
		}
	}
}
