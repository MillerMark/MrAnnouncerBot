using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UnityEngine.ParticleSystem;
using UnityEngine;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public partial class EdtColor : UserControl, IValueEditor
	{
		public EdtColor()
		{
			InitializeComponent();
		}

		public IValueChangedListener ValueChangedListener { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeColor result = new ChangeColor();
			HueSatLight hueSatLight = GetHsl();
			float multiplier = trkMultiplier.Value / 10.0f;
			result.SetValue(hueSatLight.AsHtml, multiplier);
			return result;
		}

		public void ValueChanged(object newValue)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue);
		}

		public Type GetValueType()
		{
			return typeof(UnityEngine.Color);
		}

		public void SetValue(object newValue)
		{
			Talespire.Log.Debug($"EdtColor.SetValue - {newValue}");
			if (!(newValue is UnityEngine.Color newColor))
				return;
			SetMultiplier(newColor);
			Talespire.Log.Debug($"btnSetColor.BackColor = ColorUtils.ToSysDrawColor(newColor);");
			btnSetColor.BackColor = ColorUtils.ToSysDrawColor(newColor);
			Talespire.Log.Debug($"UpdateTrackbars();");
			UpdateTrackbars();
			Talespire.Log.Debug($"UpdateHtml();");
			UpdateHtml();
		}

		void SetMultiplier(UnityEngine.Color newColor)
		{
			float maxValue = Math.Max(1, Math.Max(newColor.g, Math.Max(newColor.r, newColor.b)));
			trkMultiplier.Value = (int)Math.Floor(maxValue * 10);
		}

		private void ColorChanged()
		{
			Talespire.Log.Debug($"EdtColor.ColorChanged");
			float multiplier = trkMultiplier.Value / 10.0f;
			UnityEngine.Color color = ColorUtils.ToUnityColor(btnSetColor.BackColor, multiplier);
			Talespire.Log.Debug($"ValueChanged(color);");
			ValueChanged(color);
		}

		private void UpdateTrackbars()
		{
			changingTrackbarsInternally = true;
			try
			{
				HueSatLight hueSatLight = new HueSatLight(btnSetColor.BackColor);
				trkHue.Value = (int)(hueSatLight.Hue * 100);
				trkSat.Value = (int)(hueSatLight.Saturation * 100);
				trkLight.Value = (int)(hueSatLight.Lightness * 100);
			}
			finally
			{
				changingTrackbarsInternally = false;
			}
		}

		private void UpdateHtml()
		{
			changingTextInternally = true;
			try
			{
				HueSatLight hueSatLight = new HueSatLight(btnSetColor.BackColor);
				tbxHtml.Text = hueSatLight.AsHtml;
			}
			finally
			{
				changingTextInternally = false;
			}
		}

		IWin32Window GetParentForm()
		{
			Control parent = Parent;
			Control parentForm = parent;
			while (parentForm.Parent != null)
			{
				parentForm = parentForm.Parent;
			}
			return parentForm;
		}

		private void ChangeColor(Button button)
		{
			ColorDialog colorDialog = new ColorDialog();
			colorDialog.Color = button.BackColor;
			if (colorDialog.ShowDialog(GetParentForm()) == DialogResult.OK)
			{
				button.BackColor = colorDialog.Color;
				ColorChanged();
				UpdateHtml();
				UpdateTrackbars();
			}
		}

		private void btnSetColor_Click(object sender, EventArgs e)
		{
			ChangeColor(btnSetColor);
		}

		private void trkColor_Scroll(object sender, EventArgs e)
		{
			if (changingTrackbarsInternally)
				return;
			HueSatLight hueSatLight = GetHsl();
			//Talespire.Log.Debug($"hueSatLight - hue = {hueSatLight.Hue}, saturation = {hueSatLight.Saturation}, lightness = {hueSatLight.Lightness} ");
			btnSetColor.BackColor = hueSatLight.AsRGB;
			ColorChanged();
			UpdateHtml();
		}

		private HueSatLight GetHsl()
		{
			HueSatLight hueSatLight = new HueSatLight();
			hueSatLight.Hue = trkHue.Value / 100.0;
			hueSatLight.Saturation = trkSat.Value / 100.0;
			hueSatLight.Lightness = trkLight.Value / 100.0;
			return hueSatLight;
		}

		bool changingTrackbarsInternally;
		bool changingTextInternally;

		private void tbxHtml_TextChanged(object sender, EventArgs e)
		{
			if (changingTextInternally)
				return;

			try
			{
				HueSatLight hueSatLight = new HueSatLight(tbxHtml.Text);
				btnSetColor.BackColor = hueSatLight.AsRGB;
				ColorChanged();
				UpdateTrackbars();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().ToString());
			}
		}

		private void trkMultiplier_Scroll(object sender, EventArgs e)
		{
			ColorChanged();
		}
	}
}
