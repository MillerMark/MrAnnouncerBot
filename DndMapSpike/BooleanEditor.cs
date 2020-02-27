using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class BooleanEditor : CheckBox, IPropertyEditor
	{

		public BooleanEditor()
		{

		}
		public string PropertyName { get; set; }
		public PropertyType PropertyType { get; set; }
		public UIElement DependentUI { get; set; }
		public void SetDependentUI(UIElement uIElement)
		{
			Checked -= BooleanEditor_Checked;
			Unchecked -= BooleanEditor_Unchecked;
			DependentUI = uIElement;
			if (DependentUI != null)
			{
				SetDependentUI();
				Checked += BooleanEditor_Checked;
				Unchecked += BooleanEditor_Unchecked;
			}
		}

		void SetDependentUI()
		{
			if (DependentUI == null)
				return;

			DependentUI.IsEnabled = IsChecked.HasValue && IsChecked.Value;
		}

		private void BooleanEditor_Unchecked(object sender, RoutedEventArgs e)
		{
			if (DependentUI == null)
				return;
			SetDependentUI();
		}

		private void BooleanEditor_Checked(object sender, RoutedEventArgs e)
		{
			SetDependentUI();
		}
	}
}

