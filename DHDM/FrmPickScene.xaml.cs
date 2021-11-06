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
using System.Windows.Shapes;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for FrmPickScene.xaml
	/// </summary>
	public partial class FrmPickScene : Window
	{
		public FrmPickScene()
		{
			InitializeComponent();
		}

		public FrmPickScene(List<string> allScenes)
		{
			InitializeComponent();
			lbScenes.ItemsSource = allScenes;
			btnOkay.IsEnabled = false;
		}

		private void btnOkay_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void lbScenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			btnOkay.IsEnabled = true;
			SelectedScene = lbScenes.SelectedItem as string;
		}

		public string SelectedScene { get; set; }
	}
}
