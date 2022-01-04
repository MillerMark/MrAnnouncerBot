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
	public partial class EdtEnum : UserControl, IValueEditor
	{
		List<RadioButton> radioButtons = new List<RadioButton>();
		public EdtEnum()
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
			return typeof(Enum);
		}

		bool changingInternally;
		Type enumType;

		public void SetValue(object newValue)
		{
			Talespire.Log.Debug($"EdtEnum - set value: {newValue}");
			changingInternally = true;
			Controls.Clear();
			try
			{
				if (newValue is Enum @enum)
				{
					enumType = @enum.GetType();
					int yPos = 3;
					radioButtons.Clear();
					Array allEnumElements = Enum.GetValues(enumType);
					foreach (object enumElement in allEnumElements)
					{
						string enumName = enumElement.ToString();
						RadioButton radioButton = new RadioButton();
						radioButton.Parent = this;
						radioButton.Text = enumName;
						radioButton.CheckedChanged += RadioButton_CheckedChanged;
						radioButton.Left = 3;
						radioButton.AutoSize = true;
						radioButton.Top = yPos;
						object parsedValue = Enum.Parse(enumType, enumName);

						if ((int)parsedValue == (int)newValue)
							radioButton.Checked = true;

						yPos += 23;
						radioButtons.Add(radioButton);
					}
				}
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;

			if (sender is RadioButton radioButton && radioButton.Checked)
				ValueChanged(GetEnumValue(radioButton.Text));
		}

		private int GetEnumValue(string text)
		{
			return (int)Enum.Parse(enumType, text);
		}

		int GetValueFromRadioButtons()
		{
			foreach (RadioButton radioButton in radioButtons)
			{
				if (radioButton.Checked)
				{
					return GetEnumValue(radioButton.Text);
				}
			}
			return 0;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeEnum result = new ChangeEnum();
			result.SetValue(GetValueFromRadioButtons());
			return result;
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
} 
