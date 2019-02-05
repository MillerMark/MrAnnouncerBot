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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for GroupBackground.xaml
	/// </summary>
	public partial class GroupBackground : UserControl
	{
		public static readonly DependencyProperty StatBox1Property = DependencyProperty.Register("StatBox1", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty StatBox2Property = DependencyProperty.Register("StatBox2", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty StatBox3Property = DependencyProperty.Register("StatBox3", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty StatBox4Property = DependencyProperty.Register("StatBox4", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty StatBox5Property = DependencyProperty.Register("StatBox5", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty StatBox6Property = DependencyProperty.Register("StatBox6", typeof(StatBox), typeof(GroupBackground), new FrameworkPropertyMetadata(null));


		public StatBox StatBox1
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox1Property);
			}
			set
			{
				SetValue(StatBox1Property, value);
			}
		}
		public StatBox StatBox2
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox2Property);
			}
			set
			{
				SetValue(StatBox2Property, value);
			}
		}

		public StatBox StatBox3
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox3Property);
			}
			set
			{
				SetValue(StatBox3Property, value);
			}
		}

		public StatBox StatBox4
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox4Property);
			}
			set
			{
				SetValue(StatBox4Property, value);
			}
		}

		public StatBox StatBox5
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox5Property);
			}
			set
			{
				SetValue(StatBox5Property, value);
			}
		}
		public StatBox StatBox6
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (StatBox)GetValue(StatBox6Property);
			}
			set
			{
				SetValue(StatBox6Property, value);
			}
		}

		public GroupBackground()
		{
			InitializeComponent();
		}

		static GroupBackground()
		{
			// Change defaults for inherited dependency properties...
			HorizontalAlignmentProperty.OverrideMetadata(typeof(GroupBackground), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			VerticalAlignmentProperty.OverrideMetadata(typeof(GroupBackground), new FrameworkPropertyMetadata(VerticalAlignment.Top));
		}

		private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is GroupBackground groupBackground))
				return;

			if (groupBackground.StatBox1 != null)
				groupBackground.StatBox1.StatBoxState = StatBoxState.Focused;
			if (groupBackground.StatBox2 != null)
				groupBackground.StatBox2.StatBoxState = StatBoxState.Focused;
			if (groupBackground.StatBox3 != null)
				groupBackground.StatBox3.StatBoxState = StatBoxState.Focused;
			if (groupBackground.StatBox4 != null)
				groupBackground.StatBox4.StatBoxState = StatBoxState.Focused;
			if (groupBackground.StatBox5 != null)
				groupBackground.StatBox5.StatBoxState = StatBoxState.Focused;
			if (groupBackground.StatBox6 != null)
				groupBackground.StatBox6.StatBoxState = StatBoxState.Focused;
		}
	}
}
