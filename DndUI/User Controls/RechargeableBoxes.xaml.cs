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
	/// Interaction logic for RechargeableBoxes.xaml
	/// </summary>
	public partial class RechargeableBoxes : UserControl
	{
		public static readonly DependencyProperty MinBeginLabelWidthProperty = DependencyProperty.Register("MinBeginLabelWidth", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnMinBeginLabelWidthChanged)));
		
		public static readonly DependencyProperty BeginLabelProperty = DependencyProperty.Register("BeginLabel", typeof(string), typeof(RechargeableBoxes), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnBeginLabelChanged)));
		public static readonly DependencyProperty EndLabelProperty = DependencyProperty.Register("EndLabel", typeof(string), typeof(RechargeableBoxes), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnEndLabelChanged)));


		public static readonly DependencyProperty MaxChargesProperty = DependencyProperty.Register("MaxCharges", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnMaxChargesChanged)));


		public static readonly DependencyProperty ChargesProperty = DependencyProperty.Register("Charges", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnChargesChanged)));


		private static void OnMinBeginLabelWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnMinBeginLabelWidthChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnMinBeginLabelWidthChanged(int oldValue, int newValue)
		{
			tbLabelBegin.MinWidth = newValue;
		}

		private static void OnBeginLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnBeginLabelChanged((string)e.OldValue, (string)e.NewValue);
		}
		private static void OnEndLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnEndLabelChanged((string)e.OldValue, (string)e.NewValue);
		}


		protected virtual void OnBeginLabelChanged(string oldValue, string newValue)
		{
			tbLabelBegin.Text = newValue;
		}
		protected virtual void OnEndLabelChanged(string oldValue, string newValue)
		{
			tbLabelEnd.Text = newValue;
		}

		private static void OnMaxChargesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnMaxChargesChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnMaxChargesChanged(int oldValue, int newValue)
		{
			spBoxes.Children.Clear();
			for (int i = 0; i < newValue; i++)
				spBoxes.Children.Add(new CheckBox());

			UpdateCharges();
		}

		private static void OnChargesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnChargesChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnChargesChanged(int oldValue, int newValue)
		{
			// TODO: Add your property changed side-effects. Descendants can override as well.
			UpdateCharges();
		}

		public int MinBeginLabelWidth
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(MinBeginLabelWidthProperty);
			}
			set
			{
				SetValue(MinBeginLabelWidthProperty, value);
			}
		}
		public string BeginLabel
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(BeginLabelProperty);
			}
			set
			{
				SetValue(BeginLabelProperty, value);
			}
		}
		public string EndLabel
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(EndLabelProperty);
			}
			set
			{
				SetValue(EndLabelProperty, value);
			}
		}

		public int MaxCharges
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(MaxChargesProperty);
			}
			set
			{
				SetValue(MaxChargesProperty, value);
			}
		}
		public int Charges
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(ChargesProperty);
			}
			set
			{
				SetValue(ChargesProperty, value);
			}
		}

		public RechargeableBoxes()
		{
			InitializeComponent();
		}

		void UpdateCharges()
		{
			for (int i = 0; i < spBoxes.Children.Count; i++)
			{
				UIElement uIElement = spBoxes.Children[i];
				if (uIElement is CheckBox checkBox)
					checkBox.IsChecked = i < Charges;
			}
		}
	}
}
