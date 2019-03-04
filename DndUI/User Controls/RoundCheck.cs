using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndUI
{
	public class RoundCheck : CheckBox
	{
		public static readonly DependencyProperty FocusItemProperty = DependencyProperty.Register("FocusItem", typeof(string), typeof(RoundCheck), new FrameworkPropertyMetadata(string.Empty));

		public string FocusItem
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(FocusItemProperty);
			}
			set
			{
				SetValue(FocusItemProperty, value);
			}
		}

		public RoundCheck() : base()
		{

		}
	}
}
