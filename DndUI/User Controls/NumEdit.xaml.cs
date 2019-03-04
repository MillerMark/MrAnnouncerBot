using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DndUI
{
	/// <summary>
	/// Interaction logic for NumEdit.xaml
	/// </summary>
	public partial class NumEdit : UserControl
	{
		public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register("Units", typeof(string), typeof(NumEdit), new FrameworkPropertyMetadata("px", new PropertyChangedCallback(UnitsChanged)));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(NumEdit), new FrameworkPropertyMetadata("100", new PropertyChangedCallback(ValueChanged)));
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(NumEdit), new FrameworkPropertyMetadata("Label: ", new PropertyChangedCallback(LabelChanged)));

		public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumEdit));
		public static readonly RoutedEvent PreviewTextChangedEvent = EventManager.RegisterRoutedEvent("PreviewTextChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(NumEdit));

		public event RoutedEventHandler TextChanged
		{
			add { AddHandler(TextChangedEvent, value); }
			remove { RemoveHandler(TextChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewTextChanged
		{
			add { AddHandler(PreviewTextChangedEvent, value); }
			remove { RemoveHandler(PreviewTextChangedEvent, value); }
		}
		protected virtual void OnTextChanged()
		{
			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewTextChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(TextChangedEvent);
			RaiseEvent(eventArgs);
		}
		

		public string Units
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(UnitsProperty);
			}
			set
			{
				SetValue(UnitsProperty, value);
			}
		}

		public string Value
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		static void UnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is NumEdit numEdit)
				numEdit.tbUnits.Text = (string)e.NewValue;
		}

		static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is NumEdit numEdit)
				numEdit.tbxNumber.Text = (string)e.NewValue;
		}

		public string Label
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(LabelProperty);
			}
			set
			{
				SetValue(LabelProperty, value);
			}
		}
		public double ValueAsDouble
		{
			get
			{
				if (Value.StartsWith("Inf"))
					return double.PositiveInfinity;
				if (double.TryParse(Value, out double result))
					return result;
				return 0d;
			}
			set
			{
				Value = value.ToString();
			}
		}
		static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is NumEdit numEdit)
				numEdit.tbLabel.Text = (string)e.NewValue;
		}

		public NumEdit()
		{
			InitializeComponent();
		}

		private void TbxNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			Value = tbxNumber.Text;
			OnTextChanged();
		}
	}
}
