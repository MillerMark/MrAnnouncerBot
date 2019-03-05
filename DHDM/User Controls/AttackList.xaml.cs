using DndUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for AttackList.xaml
	/// </summary>
	public partial class AttackList : UserControl
	{
		ObservableCollection<AttackViewModel> attacks;
		int attacksCreated;
		bool loading;
		public AttackList()
		{
			InitializeComponent();
		}

		private void EditableListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is EditableListBox editableListBox)
				if (editableListBox.SelectedItem is AttackViewModel attackViewModel)
				{
					if (attackBuilder != null)
					{
						loading = true;
						try
						{
							attackBuilder.LoadFromItem(attackViewModel);
						}
						finally
						{
							loading = false;
						}
					}
				}
		}

		private void EditableListBox_ClickAdd(object sender, RoutedEventArgs e)
		{
			attacks.Add(new AttackViewModel());
			attacksCreated++;
		}

		private void EditableListBox_Loaded(object sender, RoutedEventArgs e)
		{
			attacks = lbAttacksList.LoadEntries<AttackViewModel>();
		}
	}
}
