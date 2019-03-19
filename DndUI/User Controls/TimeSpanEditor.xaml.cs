using DndCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
	/// Interaction logic for TimeSpanEditor.xaml
	/// </summary>
	public partial class TimeSpanEditor : UserControl, INotifyPropertyChanged
	{
		public TimeSpanEditor()
		{
			Label = "TimeSpan: ";
			Amount = 0.0;
			MeasureIndex = 0;
			InitializeComponent();
		}

		public static readonly DependencyProperty MeasureIndexProperty = DependencyProperty.Register("MeasureIndex", typeof(int), typeof(TimeSpanEditor), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(MeasureIndexChanged)));
		public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(TimeSpanEditor), new FrameworkPropertyMetadata(0.0));
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TimeSpanEditor), new FrameworkPropertyMetadata("Time Span:"));

		public TimeMeasure TimeMeasure
		{
			get
			{
				return (TimeMeasure)MeasureIndex;
			}
			set
			{
				MeasureIndex = (int)value;
			}
		}

		public int MeasureIndex
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(MeasureIndexProperty);
			}
			set
			{
				SetValue(MeasureIndexProperty, value);
			}
		}
		public double Amount
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(AmountProperty);
			}
			set
			{
				SetValue(AmountProperty, value);
			}
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

		private void TbxAmount_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (double.TryParse(tbxAmount.Text, out double result))
				Amount = result;
			OnPropertyChanged("Amount");
		}

		private void CmbMeasure_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MeasureIndex = cmbMeasure.SelectedIndex;
			OnPropertyChanged("Measure");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		static void MeasureIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			TimeSpanEditor timeSpanEditor = o as TimeSpanEditor;
			if (timeSpanEditor != null)
				timeSpanEditor.OnMeasureIndexChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnMeasureIndexChanged(int oldValue, int newValue)
		{
			cmbMeasure.SelectedIndex = newValue;
		}
		static void AmountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			TimeSpanEditor timeSpanEditor = o as TimeSpanEditor;
			if (timeSpanEditor != null)
				timeSpanEditor.OnAmountChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnAmountChanged(int oldValue, int newValue)
		{
			tbxAmount.Text = newValue.ToString();
		}
	}
}
