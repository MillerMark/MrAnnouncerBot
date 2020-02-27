using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class TextEditor : StackPanel, IHasDependentProperty, IPropertyEditor
	{
		TextChangedEventArgs eventArgs = new TextChangedEventArgs();
		public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs ea);
		public event TextChangedEventHandler TextChanged;
		public string Value { get; set; }
		public string PropertyName { get; set; }
		public PropertyType PropertyType { get; set; }
		public string DependentProperty { get; set; }

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
			textBox.Margin = new Thickness(0, -1, 4, 4);
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
		protected virtual void ConditionText(ref string text)
		{
			// Do nothing. Let descendants override.
		}
		protected virtual void OnTextChanged(string text)
		{
			ConditionText(ref text);
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

		public virtual void Initialize(PropertyValueData propertyValueData)
		{
			PropertyName = propertyValueData.Name;
			PropertyType = propertyValueData.Type;
			AddLabel(propertyValueData.DisplayText);
			DependentProperty = propertyValueData.DependentProperty;
		}
	}
}

