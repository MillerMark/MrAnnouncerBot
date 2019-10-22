using System;
using DndCore;
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

namespace DndUI
{
	/// <summary>
	/// Interaction logic for FrmPickOne.xaml
	/// </summary>
	public partial class FrmPickOne : Window
	{
		public FrmPickOne()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				return;

			if (!(button.Content is TextBlock textBlock))
				return;

			string itemName = textBlock.Text;

			foreach (object item in lbChoices.ItemsSource)
			{
				if (item is ListEntry listEntry)
					if (listEntry.StandardName == itemName)
					{
						SelectedEntry = listEntry;
						DialogResult = true;
						return;
					}
			}
		}
		public ListEntry SelectedEntry { get; set; }
	}
}
