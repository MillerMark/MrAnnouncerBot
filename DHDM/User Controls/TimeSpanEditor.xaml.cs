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

namespace DHDM.User_Controls
{
	/// <summary>
	/// Interaction logic for TimeSpanEditor.xaml
	/// </summary>
	public partial class TimeSpanEditor : UserControl
	{
		public TimeSpanEditor()
		{
			Label = "TimeSpan: ";
			Amount = 0.0;
			MeasureIndex = 0;
			InitializeComponent();
		}

		public static readonly DependencyProperty MeasureIndexProperty = DependencyProperty.Register("MeasureIndex", typeof(int), typeof(TimeSpanEditor), new FrameworkPropertyMetadata(0));
		public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(TimeSpanEditor), new FrameworkPropertyMetadata(0.0));
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TimeSpanEditor), new FrameworkPropertyMetadata("Time Span:"));


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

	}
}
