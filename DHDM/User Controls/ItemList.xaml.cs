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
	/// Interaction logic for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl
	{
		ObservableCollection<ItemViewModel> items;

		bool loading;
		int itemsCreated;

		public ItemList()
		{
			InitializeComponent();
		}

		private void EditableListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is EditableListBox editableListBox)
				if (editableListBox.SelectedItem is ItemViewModel entry)
					if (itemBuilder != null)
					{
						loading = true;
						try
						{
							itemBuilder.LoadFromItem(entry);
						}
						finally
						{
							loading = false;
						}
					}
		}

		private void EditableListBox_Loaded(object sender, RoutedEventArgs e)
		{
			items = lbItems.LoadEntries<ItemViewModel>();
		}

		private void EditableListBox_ClickAdd(object sender, RoutedEventArgs e)
		{
			items.Add(new ItemViewModel("New Item" + itemsCreated));
			itemsCreated++;
		}

		private void ItemBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (loading)
				return;

			lbItems.SetDirty();

			if (lbItems.SelectedItem is ItemViewModel item)
				if (itemBuilder != null)
					itemBuilder.SaveToItem(item, e.PropertyName);
		}
		public void Save()
		{
			lbItems.SaveEntries();
		}
	}
}
