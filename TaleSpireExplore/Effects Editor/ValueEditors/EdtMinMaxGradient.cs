﻿using System;
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

namespace TaleSpireExplore
{
	public partial class EdtMinMaxGradient : UserControl, IValueEditor
	{
		public EdtMinMaxGradient()
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
			return typeof(MinMaxGradient);
		}

		public void SetValue(object newValue)
		{
			if (!(newValue is MinMaxGradient minMaxGradient))
				return;
			switch (minMaxGradient.mode)
			{
				case ParticleSystemGradientMode.Color:
					rbColor.Checked = true;
					btnSetColor1.BackColor = ToSysDrawColor(minMaxGradient.color);
					btnSetColor1.Visible = true;
					break;
				case ParticleSystemGradientMode.Gradient:
					rbGradient.Checked = true;
					if (minMaxGradient.gradient.colorKeys != null)
						if (minMaxGradient.gradient.colorKeys.Length > 1)
						{
							btnSetColor1.BackColor = ToSysDrawColor(minMaxGradient.gradient.colorKeys[0].color);
							btnSetColor2.BackColor = ToSysDrawColor(minMaxGradient.gradient.colorKeys[1].color);
						}
					btnSetColor1.Visible = true;
					btnSetColor2.Visible = true;
					break;
				case ParticleSystemGradientMode.TwoColors:
					rbTwoColor.Checked = true;
					btnSetColor1.BackColor = ToSysDrawColor(minMaxGradient.colorMin);
					btnSetColor2.BackColor = ToSysDrawColor(minMaxGradient.colorMax);
					btnSetColor1.Visible = true;
					btnSetColor2.Visible = true;
					break;
				case ParticleSystemGradientMode.TwoGradients:
					rbTwoGradients.Checked = true;
					if (minMaxGradient.gradientMin.colorKeys != null)
						{
							// Not supported yet.
						}
					btnSetColor1.Visible = false;
					btnSetColor2.Visible = false;
					break;
				case ParticleSystemGradientMode.RandomColor:
					rbRandomColor.Checked = true;
					btnSetColor1.Visible = false;
					btnSetColor2.Visible = false;
					break;
			}
			//tbxValue.Text = @float.ToString();
		}

		UnityEngine.Color ToUnityColor(System.Drawing.Color backColor)
		{
			return new UnityEngine.Color(backColor.R / 255f, backColor.G / 255f, backColor.B / 255f);
		}

		System.Drawing.Color ToSysDrawColor(UnityEngine.Color backColor)
		{
			return System.Drawing.Color.FromArgb((int)Math.Round(backColor.r * 255), (int)Math.Round(backColor.g * 255), (int)Math.Round(backColor.b * 255));
		}


		private void SomethingChanged()
		{
			UnityEngine.Color color1 = ToUnityColor(btnSetColor1.BackColor);
			UnityEngine.Color color2 = ToUnityColor(btnSetColor2.BackColor);
			MinMaxGradient minMaxGradient = null;
			if (rbColor.Checked)
			{
				minMaxGradient = new MinMaxGradient(ToUnityColor(btnSetColor1.BackColor));
				minMaxGradient.mode = ParticleSystemGradientMode.Color;
				minMaxGradient.color = color1;
			}
			else if (rbGradient.Checked)
			{
				minMaxGradient = new MinMaxGradient();
				minMaxGradient.mode = ParticleSystemGradientMode.Gradient;
				minMaxGradient.gradient = new Gradient();
				minMaxGradient.gradient.mode = GradientMode.Blend;
				
				minMaxGradient.gradient.SetKeys(
					new GradientColorKey[]
					{ new GradientColorKey(color1, 0.0f),
						new GradientColorKey(color2, 1.0f)
					},

					new GradientAlphaKey[] {
						new GradientAlphaKey(0f, 0.0f),
						new GradientAlphaKey(1f, 0.1f),
						new GradientAlphaKey(1f, 0.9f),
						new GradientAlphaKey(0f, 1.0f) }
					);
			}
			else if (rbRandomColor.Checked)
			{
				minMaxGradient = new MinMaxGradient();
				minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
			}
			else if (rbTwoColor.Checked)
			{
				minMaxGradient = new MinMaxGradient(color1, color2);
				minMaxGradient.mode = ParticleSystemGradientMode.TwoColors;
				minMaxGradient.colorMin = color1;
				minMaxGradient.colorMax = color2;
			}
			else if (rbTwoGradients.Checked)
			{
				minMaxGradient = new MinMaxGradient();
				minMaxGradient.mode = ParticleSystemGradientMode.TwoGradients;
				// not supported yet.
			}

			ValueChanged(minMaxGradient);
		}

		private void ColorMode_CheckedChanged(object sender, EventArgs e)
		{
			SomethingChanged();
		}

		private void ChangeColor(Button button)
		{
			ColorDialog colorDialog = new ColorDialog();
			colorDialog.Color = button.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				button.BackColor = colorDialog.Color;
				SomethingChanged();
			}
		}

		private void btnSetColor1_Click(object sender, EventArgs e)
		{
			ChangeColor(btnSetColor1);
		}

		private void btnSetColor2_Click(object sender, EventArgs e)
		{
			ChangeColor(btnSetColor2);
		}
	}
}
