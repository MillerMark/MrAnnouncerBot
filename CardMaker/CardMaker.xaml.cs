using System;
using System.Collections.Generic;
using System.IO;
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
using CardMaker;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardMaker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class CardMakerMain : Window
	{

		public CardData CardData { get; set; }
		Deck activeDeck;
		public Deck ActiveDeck
		{
			get => activeDeck; 
			set
			{
				if (activeDeck == value)
					return;
				activeDeck = value;
				SelectDeck(activeDeck);
			}
		}

		void SelectDeck(Deck deck)
		{
			lbDecks.SelectedItem = deck;
		}

		public CardMakerMain()
		{
			InitializeComponent();
			CardData = new CardData();
			CardData.LoadData();
			lbDecks.ItemsSource = CardData.AllKnownDecks;
		}



		void ShowStatus(string message)
		{
			statusBar.Visibility = Visibility.Visible;
			sbMessage.Content = message;
		}

		private void btnCloseStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			statusBar.Visibility = Visibility.Hidden;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowStatus("Menu item clicked!");
		}

		private void btnAddDeck_Click(object sender, RoutedEventArgs e)
		{
			ActiveDeck = CardData.AddDeck();
		}

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			CardData.Save();
		}
	}
}
