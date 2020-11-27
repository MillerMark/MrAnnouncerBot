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
		Card activeCard;
		public Card ActiveCard
		{
			get => activeCard;
			set
			{
				if (activeCard == value)
					return;
				activeCard = value;
				SelectCard(activeCard);
			}
		}

		void SelectDeck(Deck deck)
		{
			lbDecks.SelectedItem = deck;
			lbCards.ItemsSource = deck.Cards;
			tbCards.Text = $"Cards in {deck.Name}:";
		}

		void SelectCard(Card card)
		{
			lbCards.SelectedItem = card;
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

		private void TestStatusBarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowStatus("Menu item clicked!");
		}

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			CardData.Save(ActiveDeck);
		}

		void SelectDeckByIndex(int index)
		{
			ActiveDeck = null;
			if (index >= lbDecks.Items.Count)
				index = lbDecks.Items.Count - 1;

			if (index < 0)
				return;
			lbDecks.SelectedItem = lbDecks.Items[index];
		}

		private void HandleDeckSelectionChange()
		{
			ActiveDeck = lbDecks.SelectedItem as Deck;
		}

		private void btnDeleteDeck_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDeck == null)
				return;
			
			if (MessageBox.Show($"Delete deck \"{ActiveDeck.Name}\"?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbDecks.Items.IndexOf(ActiveDeck);
			CardData.Delete(ActiveDeck);
			int newIndexToSelect = lastSelectedIndex;
			SelectDeckByIndex(newIndexToSelect);
		}

		private void lbDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			HandleDeckSelectionChange();
		}

		private void btnAddDeck_Click(object sender, RoutedEventArgs e)
		{
			ActiveDeck = CardData.AddDeck();
		}
		
		private void btnAddCard_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ActiveCard = CardData.AddCard(ActiveDeck);
				tbxCardName.SelectAll();
				tbxCardName.Focus();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception");
			}
		}

		private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
		{

		}

		private void lbCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbCards.SelectedItem != null)
			{
				ActiveCard = lbCards.SelectedItem as Card;
				spSelectedCard.DataContext = ActiveCard;
				spSelectedCard.Visibility = Visibility.Visible;
			}
			else
			{
				ActiveCard = null;
				spSelectedCard.Visibility = Visibility.Collapsed;
			}
		}

		private void lbCardLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void sliderSaturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void sliderHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void sliderOffsetY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void sliderOffsetX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void tbxCardName_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void tbxAdditionalInstructions_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void tbxDescription_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void tbxFragmentsRequired_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void tbxCooldown_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}
