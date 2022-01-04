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
		List<string> rotateNames = new List<string>();
		List<string> scaleNames = new List<string>();
		public EdtVector3()
		{
			InitializeComponent();

			rotateNames.Add("rotate");
			rotateNames.Add("rotation");
			rotateNames.Add("angle");
			rotateNames.Add("direction");

			scaleNames.Add("scale");
		}

		public IValueChangedListener ValueChangedListener { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public void ValueChanged(object newValue, bool committedChange = true)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
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

		private void UpdateValue(float multiplier = 1, bool committedChange = true)
		{
			if (changingInternally)
				return;
			try
			{
				if (GetXYZ(out float x, out float y, out float z, multiplier))
					ValueChanged(new Vector3(x, y, z), committedChange);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
			}
		}

		private bool GetXYZ(out float x, out float y, out float z, float multiplier = 1)
		{
			x = 0;
			y = 0;
			z = 0;
			string[] xyz = tbxVector3.Text.Split(',');
			if (xyz.Length == 1 && float.TryParse(xyz[0].Trim(), out x))
			{
				y = x;
				z = x;
			}
			else if (xyz.Length != 3)
				return false;
			else if (!(float.TryParse(xyz[0].Trim(), out x) && float.TryParse(xyz[1].Trim(), out y) && float.TryParse(xyz[2].Trim(), out z)))
				return false;

			x += multiplier * trkOffsetX.Value / 10f;
			y += multiplier * trkOffsetY.Value / 10f;
			z += multiplier * trkOffsetZ.Value / 10f;
			return true;
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
			UpdateValue(trackBarMultiplier, false);
		}

		private void trkOffset_Leave(object sender, EventArgs e)
		{
			ApplyAndResetTrackbarOffsets();
		}

		private void ApplyAndResetTrackbarOffsets()
		{
			if (GetXYZ(out float x, out float y, out float z, trackBarMultiplier))
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
			ApplyAndResetTrackbarOffsets();
		}

		private void btnPreset_Click(object sender, EventArgs e)
		{
			if (sender is Button button)
				tbxVector3.Text = button.Text;
		}

		float trackBarMultiplier = 1;

		private void ChangeMultiplier(object sender, EventArgs e)
		{
			if (sender is RadioButton radioButton)
			{
				string multiplierStr = radioButton.Text.Substring(2).Trim();  // Removes the leading "x ".
				if (!float.TryParse(multiplierStr, out trackBarMultiplier))
					trackBarMultiplier = 1;
			}
		}

		private static float GetAmountToChange(object sender)
		{
			float amountToChange = 0;
			if (sender is Button button)
				float.TryParse(button.Text, out amountToChange);

			return amountToChange;
		}
		
		private void changeY_Click(object sender, EventArgs e)
		{
			float amountToChange = GetAmountToChange(sender);
			if (GetXYZ(out float x, out float y, out float z))
				tbxVector3.Text = $"{x}, {y + amountToChange}, {z}";
		}

		private void changeX_Click(object sender, EventArgs e)
		{
			float amountToChange = GetAmountToChange(sender);
			if (GetXYZ(out float x, out float y, out float z))
				tbxVector3.Text = $"{x + amountToChange}, {y}, {z}";
		}

		private void changeZ_Click(object sender, EventArgs e)
		{
			float amountToChange = GetAmountToChange(sender);
			if (GetXYZ(out float x, out float y, out float z))
				tbxVector3.Text = $"{x}, {y}, {z + amountToChange}";
		}

		private void rbTranslate_CheckedChanged(object sender, EventArgs e)
		{
			if (rbTranslate.Checked)
			{
				btnAllZeros.Visible = true;
				btnAllHalves.Visible = false;
				btnAllOnes.Visible = true;
				btnAllTwos.Visible = true;
				btn90x.Visible = false;
				btn90y.Visible = false;
				btn90z.Visible = false;
				btnX270.Visible = false;
				btnY270.Visible = false;
				btnZ270.Visible = false;
				btnX90.Visible = false;
				btnY90.Visible = false;
				btnZ90.Visible = false;
				btnX315.Visible = false;
				btnY315.Visible = false;
				btnZ315.Visible = false;
				btnX45.Visible = false;
				btnY45.Visible = false;
				btnZ45.Visible = false;
			}
		}

		private void rbRotate_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRotate.Checked)
			{
				btnAllZeros.Visible = true;
				btnAllHalves.Visible = false;
				btnAllOnes.Visible = false;
				btnAllTwos.Visible = false;
				btn90x.Visible = true;
				btn90y.Visible = true;
				btn90z.Visible = true;
				btnX270.Visible = true;
				btnY270.Visible = true;
				btnZ270.Visible = true;
				btnX90.Visible = true;
				btnY90.Visible = true;
				btnZ90.Visible = true;
				btnX315.Visible = true;
				btnY315.Visible = true;
				btnZ315.Visible = true;
				btnX45.Visible = true;
				btnY45.Visible = true;
				btnZ45.Visible = true;
			}
		}


		private void rbScale_CheckedChanged(object sender, EventArgs e)
		{
			if (rbScale.Checked)
			{
				btnAllZeros.Visible = false;
				btnAllHalves.Visible = true;
				btnAllOnes.Visible = true;
				btnAllTwos.Visible = true;
				btn90x.Visible = false;
				btn90y.Visible = false;
				btn90z.Visible = false;
				btnX270.Visible = false;
				btnY270.Visible = false;
				btnZ270.Visible = false;
				btnX90.Visible = false;
				btnY90.Visible = false;
				btnZ90.Visible = false;
				btnX315.Visible = false;
				btnY315.Visible = false;
				btnZ315.Visible = false;
				btnX45.Visible = false;
				btnY45.Visible = false;
				btnZ45.Visible = false;
			}
		}

		public void EditingProperty(string name, string paths)
		{
			string lowerCaseName = name.ToLower();
			if (rotateNames.MatchesAnyLower(lowerCaseName))
			{
				rbRotate.Checked = true;
				rbRotate_CheckedChanged(null, null);
			}
			else if (scaleNames.MatchesAnyLower(lowerCaseName))
			{
				rbScale.Checked = true;
				rbScale_CheckedChanged(null, null);
			}
			else
			{
				rbTranslate.Checked = true;
				rbTranslate_CheckedChanged(null, null);
			}
		}
	}
}
