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

namespace TaleSpireExplore
{
	public partial class EdtFloat : UserControl, IValueEditor
	{
		public EdtFloat()
		{
			InitializeComponent();
		}

		public IValueChangedListener ValueChangedListener { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}


		public void ValueChanged(object newValue, bool committedChange)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
		}

		public Type GetValueType()
		{
			return typeof(float);
		}

		public void SetValue(object newValue)
		{
			if (newValue is float @float)
				tbxValue.Text = @float.ToString();
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeFloat result = new ChangeFloat();
			if (float.TryParse(tbxValue.Text.Trim(), out float value))
				result.SetValue(value);
			return result;
		}

		private void tbxValue_TextChanged(object sender, EventArgs e)
		{
			if (changingTextInternally)
				return;

			if (float.TryParse(tbxValue.Text.Trim(), out float result))
			{
				ValueChanged(result, true);
				if (trkValue.Visible)
				{
					changingTrackbarInternally = true;
					try
					{
						int sliderValue = (int)Math.Round(result * multiplier);
						if (sliderValue > trkValue.Maximum)
							sliderValue = trkValue.Maximum;						
						
						if (sliderValue < trkValue.Minimum)
							sliderValue = trkValue.Minimum;

						trkValue.Value = sliderValue;
					}
					finally
					{
						changingTrackbarInternally = false;
					}
				}
			}
		}

		public void EditingProperty(string name, string paths)
		{
			//Talespire.Log.Warning($"paths = \"{paths}\"");
			string[] allPaths = paths.Split(';');
			List<SlidableFloat> allSlidableFloats = SlidableFloats.GetAll();
			SlidableFloat slidableFloat = allSlidableFloats.FirstOrDefault(x => x.Matches(name, allPaths));
			if (slidableFloat == null)
			{
				trkValue.Visible = false;
				Talespire.Log.Debug($"Did not find a SlidableFloat for \"{name}\" or \"{paths}\".");
				return;
			}

			trkValue.Visible = true;
			multiplier = Math.Pow(10, slidableFloat.DecimalPlaces);
			trkValue.Minimum = (int)Math.Round(slidableFloat.Min * multiplier);
			trkValue.Maximum = (int)Math.Round(slidableFloat.Max * multiplier);
		}

		bool changingTrackbarInternally;
		double multiplier = 1;
		bool changingTextInternally;

		private void trkValue_Scroll(object sender, EventArgs e)
		{
			if (changingTrackbarInternally)
				return;

			SliderChanged(true);
		}

		private void trkValue_MouseUp(object sender, MouseEventArgs e)
		{
			SliderChanged(true);
		}

		private void SliderChanged(bool committedChange)
		{
			changingTextInternally = true;
			try
			{
				tbxValue.Text = (trkValue.Value / multiplier).ToString();
				ValueChanged(trkValue.Value / multiplier, committedChange);
			}
			finally
			{
				changingTextInternally = false;
			}
		}
	}
}
