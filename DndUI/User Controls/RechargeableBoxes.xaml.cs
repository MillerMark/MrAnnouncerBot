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
		public static readonly RoutedEvent ChargesChangedEvent = EventManager.RegisterRoutedEvent("ChargesChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RechargeableBoxes));

		public event RoutedEventHandler ChargesChanged
		{
			add { AddHandler(ChargesChangedEvent, value); }
			remove { RemoveHandler(ChargesChangedEvent, value); }
		}

		protected virtual void TriggerChargesChanged()
		{
			RoutedEventArgs eventArgs = new RoutedEventArgs(ChargesChangedEvent);
			RaiseEvent(eventArgs);
		}
		
		public static readonly DependencyProperty MinBeginLabelWidthProperty = DependencyProperty.Register("MinBeginLabelWidth", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnMinBeginLabelWidthChanged)));
		
		public static readonly DependencyProperty BeginLabelProperty = DependencyProperty.Register("BeginLabel", typeof(string), typeof(RechargeableBoxes), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnBeginLabelChanged)));
		public static readonly DependencyProperty EndLabelProperty = DependencyProperty.Register("EndLabel", typeof(string), typeof(RechargeableBoxes), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnEndLabelChanged)));


		public static readonly DependencyProperty MaxChargesProperty = DependencyProperty.Register("MaxCharges", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnMaxChargesChanged)));


		public static readonly DependencyProperty ChargesUsedProperty = DependencyProperty.Register("ChargesUsed", typeof(int), typeof(RechargeableBoxes), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnChargesUsedChanged)));
		bool updatingInternally;


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
			for (int i = spRechargeable.Children.Count - 1; i > 0; i--)
			{
				UIElement child = spRechargeable.Children[i];
				if (child is CheckBox)
					spRechargeable.Children.RemoveAt(i);
			}

			for (int i = 0; i < newValue; i++)
			{
				CheckBox checkbox = new CheckBox();
				checkbox.Checked += Checkbox_Checked;
				checkbox.Unchecked += Checkbox_Unchecked;
				spRechargeable.Children.Insert(i + 1, checkbox);
			}

			UpdateCharges();
		}

		private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (updatingInternally)
				return;
			ChargesUsed--;
		}

		private void Checkbox_Checked(object sender, RoutedEventArgs e)
		{
			if (updatingInternally)
				return;
			ChargesUsed++;
		}

		private static void OnChargesUsedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			RechargeableBoxes rechargeableBoxes = o as RechargeableBoxes;
			if (rechargeableBoxes != null)
				rechargeableBoxes.OnChargesUsedChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnChargesUsedChanged(int oldValue, int newValue)
		{
			TriggerChargesChanged();
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
		public int ChargesUsed
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(ChargesUsedProperty);
			}
			set
			{
				SetValue(ChargesUsedProperty, value);
			}
		}

		public string Key { get; set; }
		public int PlayerId { get; set; }

		public RechargeableBoxes()
		{
			InitializeComponent();
		}

		void UpdateCharges()
		{
			updatingInternally = true;
			try
			{
				int checkboxIndex = 0;
				for (int i = 0; i < spRechargeable.Children.Count; i++)
				{
					UIElement uIElement = spRechargeable.Children[i];
					if (uIElement is CheckBox checkBox)
					{
						checkBox.IsChecked = checkboxIndex < ChargesUsed;
						checkboxIndex++;
					}
				}
			}
			finally
			{
				updatingInternally = false;
			}
		}
	}
}
