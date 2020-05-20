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
	/// Interaction logic for FrmMonsterPicker.xaml
	/// </summary>
	public partial class FrmMonsterPicker : Window
	{
		public FrmMonsterPicker()
		{
			InitializeComponent();
		}

		private void TbxFilter_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void BtnOkay_Click(object sender, RoutedEventArgs e)
		{
			SelectedMonster = lbMonsters.SelectedItem as DndCore.Monster;
			Close();
		}
		public DndCore.Monster SelectedMonster { get; set; }
	}
}
