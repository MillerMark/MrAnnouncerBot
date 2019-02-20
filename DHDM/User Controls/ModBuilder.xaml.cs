using DndCore;
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
	/// Interaction logic for ModBuilder.xaml
	/// </summary>
	public partial class ModBuilder : UserControl
	{
		PlayerProperty PlayerProperty;
		public static readonly DependencyProperty PlayerPropertyIndexProperty = DependencyProperty.Register("PlayerPropertyIndex", typeof(int), typeof(ModBuilder), new FrameworkPropertyMetadata(0));

		public int PlayerPropertyIndex
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(PlayerPropertyIndexProperty);
			}
			set
			{
				SetValue(PlayerPropertyIndexProperty, value);
			}
		}

		public ModBuilder()
		{
			InitializeComponent();
		}
	}
}
