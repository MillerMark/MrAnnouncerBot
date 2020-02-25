using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class TextEditor: StackPanel
	{
		TextChangedEventArgs eventArgs = new TextChangedEventArgs();
		public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs ea);
		public event TextChangedEventHandler TextChanged;
		public string Value { get; set; }
		public string PropertyName { get; set; }
		public PropertyType PropertyType { get; set; }
		string originalText;
		public TextEditor()
		{
			Orientation = Orientation.Horizontal;
		}
		public void AddLabel(string displayText)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Margin = new Thickness(4, 0, 4, 4);
			textBlock.Text = displayText;
			textBlock.Foreground = System.Windows.Media.Brushes.White;
			Children.Insert(0, textBlock);
		}

		public void AddText(string text)
		{
			originalText = text;
			lastTextChangeText = text;
			TextBox textBox = new TextBox();
			textBox.MinWidth = 80;
			textBox.Margin = new Thickness(0, 0, 4, 4);
			textBox.Text = text;
			//textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
			textBox.KeyDown += TextBox_KeyDown;
			textBox.LostFocus += TextBox_LostFocus;
			Children.Add(textBox);
		}

		private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (!(sender is TextBox textBox))
				return;
			if (e.Key == System.Windows.Input.Key.Escape)
			{
				textBox.Text = originalText;
			}
			else if (e.Key == System.Windows.Input.Key.Enter)
			{
				OnTextChanged(textBox.Text);
			}
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				return;
			OnTextChanged(textBox.Text);
		}

		private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			
		}

		string lastTextChangeText;
		protected virtual void OnTextChanged(string text)
		{
			Value = text;
			originalText = text;
			if (string.IsNullOrEmpty(lastTextChangeText) && string.IsNullOrEmpty(text))
				return;

			if (lastTextChangeText == text)
				return;

			lastTextChangeText = text;
			eventArgs.Value = text;
			TextChanged?.Invoke(this, eventArgs);
		}
	}

	public class EnumEditor : StackPanel
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

