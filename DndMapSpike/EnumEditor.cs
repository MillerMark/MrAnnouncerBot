using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class EnumEditor : StackPanel, IPropertyEditor
	{
		ValueChangedEventArgs eventArgs = new ValueChangedEventArgs();
		public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs ea);
		public event ValueChangedEventHandler ValueChanged;
		public int? Value { get; set; }
		public string PropertyName { get; set; }
		public PropertyType PropertyType { get; set; }
		public EnumEditor()
		{
			Orientation = Orientation.Horizontal;
		}
		public void AddDisplayText(string displayText)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Margin = new Thickness(4, 0, 4, 4);
			textBlock.Text = displayText;
			textBlock.Foreground = System.Windows.Media.Brushes.White;
			Children.Insert(0, textBlock);
		}

		public void AddOption(string name, int value, bool isChecked)
		{
			RadioButton radioButton = new RadioButton();
			Style defaultStyle = Application.Current.TryFindResource(typeof(RadioButton)) as Style;
			radioButton.Content = name;
			radioButton.Foreground = System.Windows.Media.Brushes.White;
			radioButton.Style = defaultStyle;
			radioButton.Margin = new Thickness(4, 0, 4, 4);
			radioButton.IsChecked = isChecked;
			radioButton.Tag = value;
			radioButton.Click += RadioButton_Click;
			Children.Add(radioButton);
		}

		void OnValueChanged(int value)
		{
			Value = value;
			eventArgs.Value = value;
			ValueChanged?.Invoke(this, eventArgs);
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton radioButton)
			{
				if (radioButton.Tag is int)
				{
					int newValue = (int)radioButton.Tag;
					if (newValue != Value)
						OnValueChanged(newValue);
				}
			}
		}
	}
}

