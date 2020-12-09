using System;
using DndCore;
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
using System.Collections.ObjectModel;
using CardMaker;
using Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SysPath = System.IO.Path;

namespace CardMaker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class CardMakerMain : Window
	{
		const string assetsFolder = @"D:\Dropbox\DragonHumpers\Monetization\StreamLoots\Card Factory\Assets";

		int internalChangeCount;
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
		LayerTextOptions activeLayerTextOptions;
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
		Field activeField;
		public Field ActiveField
		{
			get => activeField;
			set
			{
				if (activeField == value)
					return;
				activeField = value;
				SelectField(activeField);
			}
		}

		void SelectDeck(Deck deck)
		{
			lbDecks.SelectedItem = deck;
			lbCards.ItemsSource = deck?.Cards;
			UpdateCardsLabel(deck);
		}
		
		void UpdateCardsLabel(Deck deck)
		{
			tbCards.Text = deck == null ? "" : $"Cards in {deck.Name}:";
		}

		void SelectCard(Card card)
		{
			lbCards.SelectedItem = card;
			if (card == null)
				return;
			lbCardLayers.ItemsSource = cardLayerManager.CardLayers;
			lbAlternates.Visibility = Visibility.Collapsed;
			lbAlternates.ItemsSource = null;
			card.BeginUpdate();
			try
			{
				LoadNewStyle(card.StylePath);
			}
			finally
			{
				card.EndUpdate();
			}
		}

		void SelectField(Field field)
		{
			lbFields.SelectedItem = field;
		}

		public CardMakerMain()
		{
			InitializeComponent();
			CardData = new CardData();
			CardData.LoadData();
			foreach (Card card in CardData.AllKnownCards)
				card.LinkedPropertyChanged += Card_LinkedPropertyChanged;

			if (CardData.AllKnownDecks != null && CardData.AllKnownDecks.Count > 0)
				SetActiveDeck(CardData.AllKnownDecks[0]);
			else
				SetActiveDeck(null);

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
		}

		void SelectDeckByIndex(int index)
		{
			ActiveDeck = null;
			SelectByIndex(lbDecks, index);
		}

		private void SelectByIndex(ListBox listBox, int index)
		{
			if (index >= listBox.Items.Count)
				index = listBox.Items.Count - 1;

			if (index < 0)
				return;
			listBox.SelectedItem = listBox.Items[index];
		}

		void SelectCardByIndex(int index)
		{
			ActiveCard = null;
			SelectByIndex(lbCards, index);
		}

		void SelectFieldByIndex(int index)
		{
			ActiveField = null;
			SelectByIndex(lbFields, index);
		}

		void SetActiveLayerTextOptions(LayerTextOptions layerTextOptions)
		{
			activeLayerTextOptions = layerTextOptions;
			grdCardText.DataContext = layerTextOptions;
			if (layerTextOptions == null)
			{
				grdLayerDetails.Visibility = Visibility.Collapsed;
				grdLayerDetail.Visibility = Visibility.Collapsed;
			}
			else
			{
				grdLayerDetails.Visibility = Visibility.Visible;
				grdLayerDetail.Visibility = Visibility.Visible;
			}
		}

		void SetActiveLayerTextOptionsForCard(Card card)
		{
			SetActiveLayerTextOptions(CardData.GetLayerTextOptions(card?.StylePath));
		}

		void SetActiveCard(Card card)
		{
			BeginUpdate();
			try
			{
				ActiveCard = card;

				lbCards.SelectedItem = card;
				spSelectedCard.DataContext = card;
				SetActiveFields(card?.Fields);
				if (card == null)
					SetActiveLayerTextOptions(null);
				else
				{
					if (activeLayerTextOptions != null)
					{
						ActiveCard.BeginUpdate();
						try
						{
							ActiveCard.FontBrush = activeLayerTextOptions.FontBrush;
						}
						finally
						{
							ActiveCard.EndUpdate();
						}
					}
					SetActiveLayerTextOptionsForCard(card);
				}

				tbItemName.DataContext = card;
				bool isValidCard = card != null;
				btnDeleteCard.IsEnabled = isValidCard;
				spSelectedCard.Visibility = isValidCard ? Visibility.Visible : Visibility.Collapsed;
			}
			finally
			{
				EndUpdate();
			}
		}

		private void BeginUpdate()
		{
			internalChangeCount++;
		}

		void EndUpdate()
		{
			internalChangeCount--;
		}

		void SetActiveDeck(Deck deck)
		{
			ActiveDeck = deck;
			if (deck == null)
				return;

			BeginUpdate();
			try
			{
				spActiveDeck.DataContext = deck;
				if (deck.Cards.Count == 0)
					SetActiveCard(null);
				else
					SetActiveCard(deck.Cards[0]);
			}
			finally
			{
				EndUpdate();
			}
		}

		private void btnDeleteDeck_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveDeck == null)
				return;

			if (ActiveDeck.Cards == null || ActiveDeck.Cards.Count > 0)
			{
				ShowStatus("You can only delete empty decks! Delete all cards first!");
				return;
			}

			if (MessageBox.Show($"Delete deck \"{ActiveDeck.Name}\"?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbDecks.Items.IndexOf(ActiveDeck);
			CardData.Delete(ActiveDeck);
			int newIndexToSelect = lastSelectedIndex;
			SelectDeckByIndex(newIndexToSelect);
		}

		private void lbDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActiveDeck(lbDecks.SelectedItem as Deck);
		}

		private void btnAddDeck_Click(object sender, RoutedEventArgs e)
		{
			ActiveDeck = CardData.AddDeck();
		}
		
		private void btnAddCard_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				cardLayerManager.Clear();
				ActiveCard = CardData.AddCard(ActiveDeck);
				ActiveCard.LinkedPropertyChanged += Card_LinkedPropertyChanged;
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
			if (ActiveCard == null)
				return;

			if (ActiveCard.Fields == null || ActiveCard.Fields.Count > 0)
			{
				ShowStatus("You can only delete cards with no fields! Delete all fields first!");
				return;
			}

			if (MessageBox.Show($"Delete card \"{ActiveCard.Name}\"?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbCards.Items.IndexOf(ActiveCard);
			CardData.Delete(ActiveCard);
			int newIndexToSelect = lastSelectedIndex;
			SelectCardByIndex(newIndexToSelect);
		}

		void SetActiveFields(ObservableCollection<Field> fields)
		{
			lbFields.ItemsSource = fields;
			if (fields == null || fields.Count == 0)
				SetActiveField(null);
			else
				SetActiveField(fields[0]);
		}

		private void lbCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			BeginUpdate();
			try
			{
				SetActiveCard(lbCards.SelectedItem as Card);
			}
			finally
			{
				EndUpdate();
			}
		}

		private void lbCardLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			sliderOffsetX.DataContext = cardImageLayer.Details;
			sliderOffsetY.DataContext = cardImageLayer.Details;
			sliderHorizontalStretch.DataContext = cardImageLayer.Details;
			sliderVerticalStretch.DataContext = cardImageLayer.Details;
			sliderHue.DataContext = cardImageLayer.Details;
			sliderLightness.DataContext = cardImageLayer.Details;
			sliderSaturation.DataContext = cardImageLayer.Details;
			sliderContrast.DataContext = cardImageLayer.Details;
			tbLayerName.DataContext = cardImageLayer.Details;

			btnResetOffsets.DataContext = cardImageLayer;
			btnResetScale.DataContext = cardImageLayer;
			btnResetHue.DataContext = cardImageLayer;
			btnResetLightness.DataContext = cardImageLayer;
			btnResetSaturation.DataContext = cardImageLayer;
			btnResetContrast.DataContext = cardImageLayer;

			if (cardImageLayer.Alternates != null && cardImageLayer.Alternates.CardLayers.Count > 0)
			{
				lbAlternates.Visibility = Visibility.Visible;
				lbAlternates.ItemsSource = cardImageLayer.Alternates.CardLayers;
				BeginUpdate();
				try
				{
					SelectCorrectAlternateImageLayer(cardImageLayer);
				}
				finally
				{
					EndUpdate();
				}
			}
			else
				lbAlternates.Visibility = Visibility.Collapsed;

		}

		private void SelectCorrectAlternateImageLayer(CardImageLayer cardImageLayer)
		{
			if (cardImageLayer.Alternates == null || cardImageLayer.Alternates.CardLayers.Count == 0)
				return;

			foreach (CardImageLayer alternateCardImageLayer in cardImageLayer.Alternates.CardLayers)
				if (alternateCardImageLayer.Details.IsSelected)
				{
					lbAlternates.SelectedItem = alternateCardImageLayer;
					if (alternateCardImageLayer != cardImageLayer)
						cardLayerManager.Replace(cardImageLayer, alternateCardImageLayer, cvsLayers);
					break;
				}
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

		void SetActiveField(Field field)
		{
			BeginUpdate();
			try
			{
				ActiveField = field;
				lbFields.SelectedItem = field;
				spActiveField.DataContext = field;
				bool isValidField = field != null;
				btnDeleteField.IsEnabled = isValidField;
				spActiveField.Visibility = isValidField ? Visibility.Visible : Visibility.Collapsed;
			}
			finally
			{
				EndUpdate();
			}
		}

		private void lbFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActiveField(lbFields.SelectedItem as Field);
		}

		private void btnAddField_Click(object sender, RoutedEventArgs e)
		{
			SetActiveField(CardData.AddField(ActiveCard));
			tbxFieldName.Focus();
			tbxFieldName.SelectAll();
		}

		private void btnDeleteField_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveField == null)
				return;

			if (MessageBox.Show($"Delete field \"{ActiveField.Name}\"?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbFields.Items.IndexOf(ActiveField);
			CardData.Delete(ActiveField);
			int newIndexToSelect = lastSelectedIndex;
			SelectFieldByIndex(newIndexToSelect);
		}

		private void cmMoveCardTo_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			// Delete this.
		}

		private void cmMoveCardTo_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			cmMoveCardTo.Items.Clear();
			foreach (Deck deck in CardData.AllKnownDecks)
				if (deck != ActiveDeck)
				{
					MenuItem menuItem = new MenuItem() { Header = deck.Name, Tag = deck };
					cmMoveCardTo.Items.Add(menuItem);
					menuItem.Click += MoveCardToMenuItem_Click;
				}

		}

		private void MoveCardToMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;
			if (!(menuItem.Tag is Deck deck))
				return;

			CardData.MoveCardToDeck(ActiveCard, deck);
		}

		bool ignoreFirstDeckNameChange = true;
		private void tbxDeckName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;
			if (ignoreFirstDeckNameChange)
			{
				ignoreFirstDeckNameChange = false;
				return;
			}
			ActiveDeck.IsDirty = true;
			lbDecks.Items.Refresh();
			UpdateCardsLabel(ActiveDeck);
		}

		private bool ChangingDataInternally
		{
			get
			{
				return internalChangeCount > 0;
			}
		}

		private void tbxCardName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;
			if (ActiveCard == null)
				return;
			ActiveCard.IsDirty = true;
			if (ActiveCard.ParentDeck != null)
				ActiveCard.ParentDeck.IsDirty = true;
			lbCards.Items.Refresh();
		}

		private void tbxFieldName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;
			if (ActiveField == null)
				return;
			ActiveField.IsDirty = true;
			if (ActiveField.ParentCard != null)
			{
				ActiveField.ParentCard.IsDirty = true;
				if (ActiveField.ParentCard.ParentDeck != null)
					ActiveField.ParentCard.ParentDeck.IsDirty = true;
			}
			lbFields.Items.Refresh();
		}

		void AddCardStyleMenuItems(ItemCollection items, string[] directories)
		{
			foreach (string directory in directories)
			{
				string folderName = System.IO.Path.GetFileName(directory);
				if (folderName.StartsWith("Shared") || folderName.StartsWith("["))
					continue;
				CardStyleMenuItem newMenuItem = new CardStyleMenuItem() { Header = folderName };
				newMenuItem.StylePath = directory.Substring(assetsFolder.Length + 1);
				items.Add(newMenuItem);
				string[] subDirectories = Directory.GetDirectories(directory);
				if (HasMoreStyles(subDirectories))
					AddCardStyleMenuItems(newMenuItem.Items, subDirectories);
				else
					newMenuItem.Click += ChangeCardStyleItem_Click;
			}
		}

		private static bool HasMoreStyles(string[] subDirectories)
		{
			if (subDirectories.Length == 0)
				return false;
			foreach (string subDirectory in subDirectories)
				if (!System.IO.Path.GetFileName(subDirectory).StartsWith("["))
					return true;
			return false;
		}

		private void ChangeCardStyleItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is CardStyleMenuItem cardStyleMenuItem))
				return;

			LoadNewStyle(cardStyleMenuItem.StylePath);
		}

		private void btnCardStylePicker_MouseDown(object sender, MouseButtonEventArgs e)
		{
			string[] directories = Directory.GetDirectories(assetsFolder);
			mnuCardStyle.Items.Clear();
			AddCardStyleMenuItems(mnuCardStyle.Items, directories);
		}

		CardLayerManager cardLayerManager = new CardLayerManager();

		void DeleteAllImages(Canvas canvas)
		{
			for (int i = canvas.Children.Count - 1; i >= 0; i--)
			{
				if (canvas.Children[i] is Image)
					canvas.Children.RemoveAt(i);
			}
		}

		void LoadNewStyle(string stylePath)
		{
			if (string.IsNullOrWhiteSpace(stylePath))
			{
				btnCardStylePicker.Content = "Card Style";
				return;
			}

			btnCardStylePicker.Content = $"Card Style: {stylePath}";

			cardLayerManager.Clear();

			if (ActiveCard != null)
				ActiveCard.StylePath = stylePath;
			DeleteAllImages(cvsLayers);
			AddBackgroundLayers(ActiveCard, stylePath);
			int numBackgroundLayers = cardLayerManager.CardLayers.Count;
			AddCoreLayers(ActiveCard, stylePath);
			AddForegroundLayers(ActiveCard, stylePath);
			cardLayerManager.SortByLayerIndex();

			LayerDetails.BeginUpdate();
			try
			{
				cardLayerManager.AddImageLayersToCanvas(cvsLayers);
			}
			finally
			{
				LayerDetails.EndUpdate();
			}

			lbCardLayers.ItemsSource = cardLayerManager.CardLayers;

			for (int i = cardLayerManager.CardLayers.Count - 1; i >= 0; i--)
				SelectCorrectAlternateImageLayer(cardLayerManager.CardLayers[i]);

			if (cardLayerManager.CardLayers.Count > 0)
				lbCardLayers.SelectedItem = cardLayerManager.CardLayers[0];
			SetActiveLayerTextOptionsForCard(ActiveCard);
			LayerTextOptions layerTextOptions = CardData.GetLayerTextOptions(stylePath);
			MoveTextLayerTo(numBackgroundLayers + layerTextOptions.TextLayerIndex + 1);
			if (ActiveCard != null && layerTextOptions != null)
			{
				ActiveCard.BeginUpdate();
				try
				{
					ActiveCard.FontBrush = layerTextOptions.FontBrush;
				}
				finally
				{
					ActiveCard.EndUpdate();
				}
				activeLayerTextOptions = layerTextOptions;
			}
		}

		private CardImageLayer AddLayer(Card card, string pngFile)
		{
			CardImageLayer cardImageLayer = new CardImageLayer(card, pngFile);
			if (cardImageLayer.LayerName == "Placeholder" && !string.IsNullOrWhiteSpace(ActiveCard.Placeholder))
			{
				cardImageLayer.SetPlaceholderDetails();
				cardImageLayer.PngFile = ActiveCard.Placeholder;
			}
			cardImageLayer.NeedToReloadImage += CardImageLayer_NeedToReloadImage;
			cardLayerManager.Add(cardImageLayer);
			return cardImageLayer;
		}
		
		private void AddCoreLayers(Card card, string stylePath)
		{
			string[] pngFiles = Directory.GetFiles(System.IO.Path.Combine(assetsFolder, stylePath), "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile);
		}

		private void AddBackgroundLayers(Card card, string stylePath)
		{
			const string backgroundFolderName = "Shared Back Layer Adornments";
			string basePath = System.IO.Path.GetDirectoryName(stylePath);
			string backFolder = System.IO.Path.Combine(assetsFolder, basePath, backgroundFolderName);
			if (!Directory.Exists(backFolder))
				return;
			string[] pngFiles = Directory.GetFiles(backFolder, "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile).Index -= 100;
		}

		private void AddForegroundLayers(Card card, string stylePath)
		{
			const string foregroundFolderName = "Shared Top Layer Adornments";
			string basePath = System.IO.Path.GetDirectoryName(stylePath);
			string foregroundFolder = System.IO.Path.Combine(assetsFolder, basePath, foregroundFolderName);
			if (!Directory.Exists(foregroundFolder))
				return;
			string[] pngFiles = Directory.GetFiles(foregroundFolder, "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile).Index += 100;
		}

		private void CardImageLayer_NeedToReloadImage(object sender, EventArgs e)
		{
			if (!(sender is CardImageLayer cardImageLayer))
				return;
			int index = GetIndexOfChild(cardImageLayer);
			if (index == -1)
				return;

			cvsLayers.Children.RemoveAt(index);
			Image image = cardImageLayer.CreateImage();
			cvsLayers.Children.Insert(index, image);
		}

		private int GetIndexOfChild(object sender)
		{
			int index = -1;
			foreach (UIElement uIElement in cvsLayers.Children)
			{
				if (uIElement is FrameworkElement frameworkElement)
				{
					if (frameworkElement.Tag == sender)
					{
						index = cvsLayers.Children.IndexOf(frameworkElement);
						break;
					}
				}
			}

			return index;
		}

		private void MoveTextLayerTo(int textLayerIndex)
		{
			Panel.SetZIndex(grdCardText, textLayerIndex);
		}

		private void btnResetOffsets_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.OffsetX = 0;
			cardImageLayer.Details.OffsetY = 0;
		}

		private void btnResetScale_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.ScaleX = 1;
			cardImageLayer.Details.ScaleY = 1;
		}

		private void btnResetHue_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Hue = 0;
		}

		private void btnResetSaturation_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Sat = 0;
		}

		private void btnResetLightness_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Light = 0;
		}

		private void btnResetContrast_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Contrast = 0;
		}

		private void Save()
		{
			ShowStatus("Saving...");
			statusBar.Refresh();
			CardData.Save();
			statusBar.Visibility = Visibility.Collapsed;
		}

		private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Save();
		}

		private void ToggleVisibility_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Viewbox viewbox))
				return;

			if (!(viewbox.DataContext is CardImageLayer cardImageLayer))
				return;

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
					cardLayerManager.ShowAllLayers();
				else
					cardLayerManager.HideAllLayersExcept(cardImageLayer);
			else
				cardImageLayer.ToggleVisibility();
		}

		private void lbAlternates_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;

			if (lbAlternates.SelectedItem is CardImageLayer newImageLayer)
			{
				SelectAlternate(newImageLayer);
			}
		}

		private void SelectAlternate(CardImageLayer newAlternateImageLayer)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer selectedLayer))
				return;
			SelectAlternate(selectedLayer, newAlternateImageLayer);
		}

		private void SelectAlternate(CardImageLayer selectedLayer, CardImageLayer newAlternateImageLayer)
		{
			SetDetailsIsSelected(newAlternateImageLayer);

			cardLayerManager.Replace(selectedLayer, newAlternateImageLayer, cvsLayers);
			lbCardLayers.SelectedItem = newAlternateImageLayer;
			ActiveCard.InvalidateLayerMods();
			ActiveCard.LayerPropertiesHaveChanged();
		}

		private void SetDetailsIsSelected(CardImageLayer newAlternateImageLayer)
		{
			foreach (object item in lbAlternates.Items)
				if (item is CardImageLayer cardImageLayer)
					cardImageLayer.Details.IsSelected = cardImageLayer == newAlternateImageLayer;
		}

		void AddSpellMenuItems(ItemCollection items, string[] files)
		{
			foreach (string file in files)
			{
				MenuItem menuItem = new MenuItem() { Header = System.IO.Path.GetFileNameWithoutExtension(file), Tag = file };
				menuItem.Click += AddSpellImageItem_Click;
				items.Add(menuItem);
			}
		}

		private void AddSpellImageItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			if (!(menuItem.Tag is string fileName && fileName != null))
				return;

			string spellName = System.IO.Path.GetFileNameWithoutExtension(fileName);
			if (ActiveCard != null)
			{
				ActiveCard.Text = spellName;
				ActiveCard.Placeholder = fileName;
				cardLayerManager.ReplaceImageInPlaceHolder(fileName, cvsLayers);
			}
		}

		private void btnLoadSpellImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			const string spellFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Scroll\Spells\Icons";
			string[] files = Directory.GetFiles(spellFolder, "*.png");
			mnuPickSpell.Items.Clear();
			AddSpellMenuItems(mnuPickSpell.Items, files);
		}

		private void Card_LinkedPropertyChanged(object sender, LinkedPropertyEventArgs ea)
		{
			if (!(sender is Card card))
				return;

			CardImageLayer cardImageLayer = cardLayerManager.CardLayers.FirstOrDefault(x => x.Details == ea.Details);
			if (cardImageLayer == null)
				return;
			if (!cardImageLayer.Groups.ContainsKey(ea.Name))
				return;
			int groupNumber = cardImageLayer.Groups[ea.Name];
			PropertyLink propertyLink = card.linkedProperties.FirstOrDefault(x => x.GroupNumber == groupNumber && x.Name == ea.Name);
			if (propertyLink != null)
				foreach (CardImageLayer imageLayer in propertyLink.Layers)
					if (cardImageLayer != imageLayer)
						imageLayer.ChangeProperty(ea);
		}

		private void LayerCompressMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;

			ImageCropper imageCropper = new ImageCropper();
			string file = cardImageLayer.PngFile;
			imageCropper.FindCropEdges(file);
			// TODO: Clean this up:
			string directory = SysPath.GetDirectoryName(file);
			string fileName = SysPath.GetFileNameWithoutExtension(file);
			string extension = SysPath.GetExtension(file);
			string layerLinks = string.Empty;
			if (fileName.Contains("^"))
			{
				layerLinks = "^" + fileName.EverythingAfterLast("^");
				fileName = fileName.EverythingBeforeLast("^");
			}

			if (fileName.Contains("("))
				fileName = fileName.EverythingBefore("(");
			
			string positioning = $"({imageCropper.leftMargin}, {imageCropper.topMargin})";

			string newFileName = $"{fileName}{positioning}{layerLinks}";
			string destinationFile = SysPath.Combine(directory, newFileName + extension);

			File.Copy(file, destinationFile);
			imageCropper.ApplyCrop(destinationFile);
		}

		private void sliderVerticalStretch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				sliderHorizontalStretch.Value = sliderVerticalStretch.Value;
		}

		private void sliderHorizontalStretch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				sliderVerticalStretch.Value = sliderHorizontalStretch.Value;
		}
	}
}
