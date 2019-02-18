using System;
using DndCore;
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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for TargetValueEdit.xaml
	/// </summary>
	public partial class TargetValueEdit : UserControl
	{
		public static readonly RoutedEvent ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TargetValueEdit));
		public static readonly RoutedEvent PreviewChangedEvent = EventManager.RegisterRoutedEvent("PreviewChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(TargetValueEdit));

		public event RoutedEventHandler Changed
		{
			add { AddHandler(ChangedEvent, value); }
			remove { RemoveHandler(ChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewChanged
		{
			add { AddHandler(PreviewChangedEvent, value); }
			remove { RemoveHandler(PreviewChangedEvent, value); }
		}
		protected virtual void OnChanged()
		{
			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(ChangedEvent);
			RaiseEvent(eventArgs);
		}
		

		public static readonly DependencyProperty ShowMinProperty = DependencyProperty.Register("ShowMin", typeof(bool), typeof(TargetValueEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowMinChanged)));

		public bool ShowMin
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(ShowMinProperty);
			}
			set
			{
				SetValue(ShowMinProperty, value);
			}
		}

		static void ShowMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				if ((bool)e.NewValue)
					targetValueEdit.spMin.Visibility = Visibility.Visible;
				else
					targetValueEdit.spMin.Visibility = Visibility.Collapsed;
		}

		public static readonly DependencyProperty ShowMaxProperty = DependencyProperty.Register("ShowMax", typeof(bool), typeof(TargetValueEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowMaxChanged)));

		public bool ShowMax
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(ShowMaxProperty);
			}
			set
			{
				SetValue(ShowMaxProperty, value);
			}
		}

		static void ShowMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				if ((bool)e.NewValue)
					targetValueEdit.spMax.Visibility = Visibility.Visible;
				else
					targetValueEdit.spMax.Visibility = Visibility.Collapsed;
		}

		public static readonly DependencyProperty ShowDriftProperty = DependencyProperty.Register("ShowDrift", typeof(bool), typeof(TargetValueEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDriftChanged)));

		public bool ShowDrift
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(ShowDriftProperty);
			}
			set
			{
				SetValue(ShowDriftProperty, value);
			}
		}

		static void ShowDriftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				if ((bool)e.NewValue)
					targetValueEdit.spDrift.Visibility = Visibility.Visible;
				else
					targetValueEdit.spDrift.Visibility = Visibility.Collapsed;
		}

		public static readonly DependencyProperty ShowBindingProperty = DependencyProperty.Register("ShowBinding", typeof(bool), typeof(TargetValueEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowBindingChanged)));

		public bool ShowBinding
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(ShowBindingProperty);
			}
			set
			{
				SetValue(ShowBindingProperty, value);
			}
		}

		static void ShowBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				if ((bool)e.NewValue)
					targetValueEdit.spBinding.Visibility = Visibility.Visible;
				else
					targetValueEdit.spBinding.Visibility = Visibility.Collapsed;
		}

		public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register("Units", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("cnt", new PropertyChangedCallback(UnitsChanged)));

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

		static void UnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbUnits.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("0", new PropertyChangedCallback(MinChanged)));

		public string Min
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(MinProperty);
			}
			set
			{
				SetValue(MinProperty, value);
			}
		}

		static void MinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbxMin.Text = (string)e.NewValue;
		}


		public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("Inf.", new PropertyChangedCallback(MaxChanged)));

		public string Max
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(MaxProperty);
			}
			set
			{
				SetValue(MaxProperty, value);
			}
		}

		static void MaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbxMax.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty DriftProperty = DependencyProperty.Register("Drift", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("0", new PropertyChangedCallback(DriftChanged)));

		public string Drift
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(DriftProperty);
			}
			set
			{
				SetValue(DriftProperty, value);
			}
		}

		static void DriftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbxDrift.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("Label: ", new PropertyChangedCallback(LabelChanged)));

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

		static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbLabel.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("0", new PropertyChangedCallback(ValueChanged)));

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

		static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.tbxValue.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("truncate", new PropertyChangedCallback(BindingChanged)));

		public string Binding
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(BindingProperty);
			}
			set
			{
				SetValue(BindingProperty, value);
			}
		}

		static void BindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
				targetValueEdit.cmbBinding.Text = (string)e.NewValue;
		}

		public static readonly DependencyProperty VarianceProperty = DependencyProperty.Register("Variance", typeof(string), typeof(TargetValueEdit), new FrameworkPropertyMetadata("0%", new PropertyChangedCallback(VarianceChanged)));

		public string Variance
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(VarianceProperty);
			}
			set
			{
				SetValue(VarianceProperty, value);
			}
		}

		static void VarianceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TargetValueEdit targetValueEdit)
			{
				targetValueEdit.tbxVariance.Text = (string)e.NewValue;
				targetValueEdit.ShowMinMaxBasedOnVariance();
			}
		}

		void ShowMinMaxBasedOnVariance()
		{
			string variance = tbxVariance.Text.Trim();
			bool canAdjustMinMax = variance != "0%" && variance != "0";
			ShowMin = canAdjustMinMax;
			ShowMax = canAdjustMinMax;
		}

		public TargetValueEdit()
		{
			InitializeComponent();
		}

		private void TbxNumber_TextChanged(object sender, TextChangedEventArgs e)
		{
			Value = tbxValue.Text;
			OnChanged();
		}

		private void TbxVariance_TextChanged(object sender, TextChangedEventArgs e)
		{
			Variance = tbxVariance.Text;
			ShowMinMaxBasedOnVariance();

			OnChanged();
		}

		private void TbxMin_TextChanged(object sender, TextChangedEventArgs e)
		{
			Min = tbxMin.Text;
			OnChanged();
		}

		private void TbxMax_TextChanged(object sender, TextChangedEventArgs e)
		{
			Max = tbxMax.Text;
			OnChanged();
		}

		private void TbxDrift_TextChanged(object sender, TextChangedEventArgs e)
		{
			Drift = tbxDrift.Text;
			OnChanged();
		}

		private void CmbBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnChanged();
		}

		private void ControlBack_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				bool shouldShow = !ShowMin;
				ShowMin = shouldShow;
				ShowMax = shouldShow;
				ShowDrift = shouldShow;
				ShowBinding = shouldShow;
			}
		}
		public TargetBinding GetTargetBinding()
		{
			if (!(cmbBinding.SelectedValue is ComboBoxItem cbi))
				return TargetBinding.truncate;

			string content = cbi.Content.ToString();

			if (content == "wrap")
				return TargetBinding.wrap;
			if (content == "rock")
				return TargetBinding.rock;
			return TargetBinding.truncate;
		}
	}
}
