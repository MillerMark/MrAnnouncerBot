﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using DndCore;
using Imaging;
using Microsoft.Extensions.Configuration;
using Streamloots;
using SysPath = System.IO.Path;
using GoogleHelper;
using System.Threading.Tasks;
using BotCore;

namespace CardMaker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class CardMakerMain : Window
	{
		Card activeCard;
		Deck activeDeck;
		Field activeField;
		CardImageLayer activeLayer;
		LayerTextOptions activeLayerTextOptions;
		string activeStyle;
		CardGenerationOptions cardGenerationOptions = new CardGenerationOptions();
		CardLayerManager cardLayerManager = new CardLayerManager();
		bool ignoreFirstDeckNameChange = true;
		int internalChangeCount;
		Color lastSampleColor;
		BitmapImage sample;
		bool samplingColor;
		bool selectingAlternate;
		string tempFileName;
		StreamlootsClient streamlootsClient;
		static readonly IConfigurationRoot configuration;
		CloudinaryClient cloudinaryClient;

		static CardMakerMain()
		{
			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			configuration = builder.Build();
		}

		public static IConfigurationRoot Configuration { get => configuration; }

		public CardMakerMain()
		{
			RegisterSpreadsheetIDs();
			InitializeStreamlootsClient();
			InitializeCloudinaryClient();
			InitializeComponent();
			codeCardPlayed.Load();
			codeCardReceived.Load();
			CardData = new CardData();
			CardData.LoadData();
			foreach (Card card in CardData.AllKnownCards)
				HookCardEvents(card);

			if (CardData.AllKnownDecks != null && CardData.AllKnownDecks.Count > 0)
				SetActiveDeck(CardData.AllKnownDecks[0]);
			else
				SetActiveDeck(null);

			lbDecks.ItemsSource = CardData.AllKnownDecks;
		}

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

		public CardData CardData { get; set; }

		private bool ChangingDataInternally { get { return internalChangeCount > 0; } }

		private static PropertyLink GetPropertyLink(string propertyName, Card card, CardImageLayer cardImageLayer)
		{
			if (cardImageLayer == null)
				return null;

			if (!cardImageLayer.Groups.ContainsKey(propertyName))
				return null;

			int groupNumber = cardImageLayer.Groups[propertyName];
			return card.linkedProperties.FirstOrDefault(x => x.GroupNumber == groupNumber && x.Name == propertyName);
		}

		private static Slider GetSliderFromContextMenu(ContextMenu contextMenu)
		{
			if (!(contextMenu.Parent is System.Windows.Controls.Primitives.Popup popup))
				return null;

			if (!(popup.PlacementTarget is Slider slider))
				return null;

			return slider;
		}

		private static bool HasMoreStyles(string[] subDirectories)
		{
			if (subDirectories.Length == 0)
				return false;
			foreach (string subDirectory in subDirectories)
			{
				string directoryName = SysPath.GetFileName(subDirectory);
				if (!directoryName.StartsWith("[") && !directoryName.StartsWith("Shared"))
					return true;
			}
			return false;
		}

		private void AddBackgroundLayers(Card card, string stylePath)
		{
			const string backgroundFolderName = Folders.SharedBackAdornments;
			string basePath = SysPath.GetDirectoryName(stylePath);
			string backFolder = SysPath.Combine(Folders.Assets, basePath, backgroundFolderName);
			if (!Directory.Exists(backFolder))
				return;
			string[] pngFiles = Directory.GetFiles(backFolder, "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile).Index -= 100;
		}

		void AddCardStyleMenuItems(ItemCollection items, string[] directories)
		{
			foreach (string directory in directories)
			{
				string folderName = SysPath.GetFileName(directory);
				if (folderName.StartsWith("Shared") || folderName.StartsWith("["))
					continue;
				CardStyleMenuItem newMenuItem = new CardStyleMenuItem() { Header = folderName };
				newMenuItem.StylePath = directory.Substring(Folders.Assets.Length + 1);
				items.Add(newMenuItem);
				string[] subDirectories = Directory.GetDirectories(directory);
				if (HasMoreStyles(subDirectories))
					AddCardStyleMenuItems(newMenuItem.Items, subDirectories);
				else
					newMenuItem.Click += ChangeCardStyleItem_Click;
			}
		}

		private void AddCoreLayers(Card card, string stylePath)
		{
			string[] pngFiles = Directory.GetFiles(SysPath.Combine(Folders.Assets, stylePath), "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile);
		}

		private void AddForegroundLayers(Card card, string stylePath)
		{
			const string foregroundFolderName = Folders.TopBackAdornments;
			string basePath = SysPath.GetDirectoryName(stylePath);
			string foregroundFolder = SysPath.Combine(Folders.Assets, basePath, foregroundFolderName);
			if (!Directory.Exists(foregroundFolder))
				return;
			string[] pngFiles = Directory.GetFiles(foregroundFolder, "*.png");
			foreach (string pngFile in pngFiles)
				AddLayer(card, pngFile).Index += 100;
		}

		private void AddImageItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			if (!(menuItem.Tag is string fileName && fileName != null))
				return;

			string spellName = SysPath.GetFileNameWithoutExtension(fileName);
			if (ActiveCard != null)
			{
				ActiveCard.Text = spellName;
				ActiveCard.Placeholder = fileName;
				cardLayerManager.ReplaceImageInPlaceHolder(fileName, cvsLayers);
			}
		}

		void AddImageMenuItems(ItemCollection items, string[] files)
		{
			foreach (string file in files)
			{
				MenuItem menuItem = new MenuItem() { Header = SysPath.GetFileNameWithoutExtension(file), Tag = file };
				menuItem.Click += AddImageItem_Click;
				items.Add(menuItem);
			}
		}

		private void AddImageMenuItems(ContextMenu contextMenu, string imageFolder)
		{
			string[] files = Directory.GetFiles(imageFolder, "*.png");
			contextMenu.Items.Clear();
			AddImageMenuItems(contextMenu.Items, files);
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

		private void BeginUpdate()
		{
			internalChangeCount++;
		}

		private void btnAddCard_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				cardLayerManager.Clear();
				ActiveCard = CardData.AddCard(ActiveDeck);
				HookCardEvents(ActiveCard);
				tbxCardName.SelectAll();
				tbxCardName.Focus();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception");
			}
		}

		private void btnAddDeck_Click(object sender, RoutedEventArgs e)
		{
			ActiveDeck = CardData.AddDeck();
		}

		private void btnAddField_Click(object sender, RoutedEventArgs e)
		{
			SetActiveField(CardData.AddNewField(ActiveCard));
			tbxFieldName.Focus();
			tbxFieldName.SelectAll();
		}

		private void btnCardStylePicker_MouseDown(object sender, MouseButtonEventArgs e)
		{
			string[] directories = Directory.GetDirectories(Folders.Assets);
			mnuCardStyle.Items.Clear();
			AddCardStyleMenuItems(mnuCardStyle.Items, directories);
		}

		private void btnCloseStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			statusBar.Visibility = Visibility.Hidden;
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

			if (MessageBox.Show($"Delete card \"{ActiveCard.Name}\"?", "Confirm", MessageBoxButton.YesNo) !=
				MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbCards.Items.IndexOf(ActiveCard);
			CardData.Delete(ActiveCard);
			int newIndexToSelect = lastSelectedIndex;
			SelectCardByIndex(newIndexToSelect);
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

			if (MessageBox.Show($"Delete deck \"{ActiveDeck.Name}\"?", "Confirm", MessageBoxButton.YesNo) !=
				MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbDecks.Items.IndexOf(ActiveDeck);
			CardData.Delete(ActiveDeck);
			int newIndexToSelect = lastSelectedIndex;
			SelectDeckByIndex(newIndexToSelect);
		}

		private void btnDeleteField_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveField == null)
				return;

			if (MessageBox.Show($"Delete field \"{ActiveField.Name}\"?", "Confirm", MessageBoxButton.YesNo) !=
				MessageBoxResult.Yes)
				return;

			int lastSelectedIndex = lbFields.Items.IndexOf(ActiveField);
			CardData.Delete(ActiveField);
			int newIndexToSelect = lastSelectedIndex;
			SelectFieldByIndex(newIndexToSelect);
		}

		private void btnEyeDropper_Click(object sender, RoutedEventArgs e)
		{
			samplingColor = true;
			Panel.SetZIndex(csrEyeDropper, 1000);

			tempFileName = SysPath.ChangeExtension(SysPath.GetTempFileName(), ".png");

			cvsLayers.SaveToPng(new Uri(tempFileName));

			sample = new BitmapImage();
			sample.BeginInit();
			sample.CacheOption = BitmapCacheOption.OnLoad;
			sample.UriSource = new Uri(tempFileName, UriKind.Absolute);
			sample.EndInit();
		}

		private void btnEyeDropper_LostFocus(object sender, RoutedEventArgs e)
		{
			RestoreNormalCursor();
			StopSampling();
		}

		private void btnLoadArmorImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			const string armorFolder = @"D:\Dropbox\DragonHumpers\Monetization\StreamLoots\Card Factory\AllArmor";
			AddImageMenuItems(mnuPickArmor, armorFolder);
		}

		private void btnLoadSpellImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			AddImageMenuItems(mnuPickSpell, Folders.Spells);
		}

		private void btnRandomize_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveCard == null)
				return;

			for (int i = cardLayerManager.CardLayers.Count - 1; i >= 0; i--)
			{
				CardImageLayer cardImageLayer = cardLayerManager.CardLayers[i];
				if (cardImageLayer.Alternates != null)
				{
					int index = new Random().Next(0, cardImageLayer.Alternates.CardLayers.Count);

					CardImageLayer newImageLayer = cardImageLayer.Alternates.CardLayers[index];

					if (cardImageLayer != newImageLayer)
					{
						cardLayerManager.Replace(cardImageLayer, newImageLayer, cvsLayers);
						cardImageLayer = newImageLayer;
					}
				}

				LayerGenerationOptions layerGenerationOptions = cardGenerationOptions.Find(activeStyle, cardImageLayer);
				if (layerGenerationOptions == null)
					continue;

				cardImageLayer.IsVisible = new Random().Next(0, 100) <= layerGenerationOptions.ChancesVisible * 100;

				ActiveCard.BeginUpdate();
				try
				{
					cardImageLayer.RandomlySetProperties(layerGenerationOptions);
				}
				finally
				{
					ActiveCard.EndUpdate();
				}
			}
		}

		private void btnResetContrast_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Contrast = 0;
		}

		private void btnResetHue_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Hue = 0;
		}

		private void btnResetLightness_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Light = 0;
		}

		private void btnResetOffsets_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.OffsetX = 0;
			cardImageLayer.Details.OffsetY = 0;
		}

		private void btnResetSaturation_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.Sat = 0;
		}

		private void btnResetScale_Click(object sender, RoutedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;
			cardImageLayer.Details.ScaleX = 1;
			cardImageLayer.Details.ScaleY = 1;
		}

		private void Card_LinkedPropertyChanged(object sender, LinkedPropertyEventArgs ea)
		{
			if (!(sender is Card card))
				return;
			PropertyLink propertyLink;
			CardImageLayer cardImageLayer;
			GetPropertyLinkAndCardImageLayer(ea.Details, ea.Name, card, out propertyLink, out cardImageLayer);
			if (propertyLink == null)
				return;
			if (propertyLink != null)
				foreach (CardImageLayer imageLayer in propertyLink.Layers)
					if (cardImageLayer != imageLayer)
						imageLayer.ChangeProperty(ea);
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

		private void ChancesVisibleMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			LayerGenerationOptions options = cardGenerationOptions.Find(activeStyle, activeLayer);
			if (options == null)
				return;

			string valueToSet = Math.Round(options.ChancesVisible * 100).ToString();

			foreach (MenuItem menu in menuItem.Items.OfType<MenuItem>())
			{
				menu.IsChecked = menu.Tag.ToString() == valueToSet;
			}
		}

		private void ChangeCardStyleItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is CardStyleMenuItem cardStyleMenuItem))
				return;

			LoadNewStyle(cardStyleMenuItem.StylePath);
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

		private void cvsLayers_MouseDown(object sender, MouseButtonEventArgs e)
		{
			StopSampling();
			ActiveCard.SetTextColor(lastSampleColor);
		}

		private void cvsLayers_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!samplingColor)
				return;

			csrEyeDropper.Visibility = Visibility.Visible;
			Cursor = Cursors.None;
		}

		private void cvsLayers_MouseLeave(object sender, MouseEventArgs e)
		{
			RestoreNormalCursor();
		}

		private void cvsLayers_MouseMove(object sender, MouseEventArgs e)
		{
			if (!samplingColor)
				return;
			Point position = e.GetPosition(cvsLayers);
			Canvas.SetLeft(csrEyeDropper, position.X);
			const double hotSpotOffsetY = 32;
			Canvas.SetTop(csrEyeDropper, position.Y - hotSpotOffsetY);
			if (sample == null)
				return;

			if (position.X >= sample.PixelWidth)
				return;
			if (position.Y >= sample.PixelHeight)
				return;
			if (position.X < 0)
				return;
			if (position.Y < 0)
				return;
			lastSampleColor = ImageUtils.GetPixelColor(sample, (int)position.X, (int)position.Y);
			btnEyeDropper.Background = new SolidColorBrush(lastSampleColor);
		}

		void DeleteAllImages(Canvas canvas)
		{
			for (int i = canvas.Children.Count - 1; i >= 0; i--)
			{
				if (canvas.Children[i] is Image)
					canvas.Children.RemoveAt(i);
			}
		}

		void EndUpdate()
		{
			internalChangeCount--;
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

		private void GetPropertyLinkAndCardImageLayer(
			LayerDetails details,
			string propertyName,
			Card card,
			out PropertyLink propertyLink,
			out CardImageLayer cardImageLayer)
		{
			cardImageLayer = GetImageLayerFromDetails(details);
			propertyLink = GetPropertyLink(propertyName, card, cardImageLayer);
		}

		Slider GetSliderFromSender(object sender)
		{
			if (!(sender is MenuItem menuItem))
				return null;

			if (!(menuItem.Parent is ContextMenu contextMenu))
				return null;

			return GetSliderFromContextMenu(contextMenu);
		}

		private string GetSliderNamePart(Slider slider)
		{
			string sliderNamePart = string.Empty;
			if (slider == sliderHue)
				sliderNamePart = CardGenerationOptions.Hue;
			if (slider == sliderLightness)
				sliderNamePart = CardGenerationOptions.Light;
			if (slider == sliderSaturation)
				sliderNamePart = CardGenerationOptions.Sat;
			if (slider == sliderContrast)
				sliderNamePart = CardGenerationOptions.Contrast;
			if (slider == sliderOffsetX)
				sliderNamePart = CardGenerationOptions.X;
			if (slider == sliderOffsetY)
				sliderNamePart = CardGenerationOptions.Y;
			if (slider == sliderHorizontalStretch)
				sliderNamePart = CardGenerationOptions.Horz;
			if (slider == sliderVerticalStretch)
				sliderNamePart = CardGenerationOptions.Vert;
			return sliderNamePart;
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

		private void lbAlternates_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;

			if (lbAlternates.SelectedItem is CardImageLayer newImageLayer)
			{
				SelectAlternate(newImageLayer);
			}
		}

		private void lbCardLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer cardImageLayer))
				return;

			activeLayer = cardImageLayer;

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
				if (!selectingAlternate)
				{
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
			}
			else
				lbAlternates.Visibility = Visibility.Collapsed;
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

		private void lbDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActiveDeck(lbDecks.SelectedItem as Deck);
		}

		private void lbFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActiveField(lbFields.SelectedItem as Field);
		}

		void LoadNewStyle(string stylePath)
		{
			activeStyle = stylePath;
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
			if (layerTextOptions != null)
				MoveTextLayerTo(numBackgroundLayers + layerTextOptions.TextLayerIndex + 1);
			if (ActiveCard != null && layerTextOptions != null && ActiveCard.TextFontOpacity == 0)
			{
				ActiveCard.SetFontBrush(layerTextOptions.FontBrush);
				activeLayerTextOptions = layerTextOptions;
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

		private void MoveTextLayerTo(int textLayerIndex)
		{
			Panel.SetZIndex(grdCardText, textLayerIndex);
		}

		private void OnlyThisValueForCardGeneration_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetCardGenOptions(GetSliderFromSender(sender), BoundaryKind.Min);
			SetCardGenOptions(GetSliderFromSender(sender), BoundaryKind.Max);
		}

		private void RestoreNormalCursor()
		{
			Cursor = Cursors.Arrow;
			csrEyeDropper.Visibility = Visibility.Hidden;
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

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SelectAlternate(CardImageLayer newAlternateImageLayer)
		{
			if (!(lbCardLayers.SelectedItem is CardImageLayer selectedLayer))
				return;
			SelectAlternate(selectedLayer, newAlternateImageLayer);
		}

		private void SelectAlternate(CardImageLayer selectedLayer, CardImageLayer newAlternateImageLayer)
		{
			if (selectedLayer == newAlternateImageLayer)
				return;

			cardLayerManager.Replace(selectedLayer, newAlternateImageLayer, cvsLayers);

			SetDetailsIsSelected(newAlternateImageLayer);

			if (selectingAlternate)
				return;

			selectingAlternate = true;

			try
			{
				lbCardLayers.SelectedItem = newAlternateImageLayer;
				ActiveCard.InvalidateLayerMods();
				ActiveCard.LayerPropertiesHaveChanged();
				SelectLinkedAlternates(newAlternateImageLayer);
			}
			finally
			{
				selectingAlternate = false;
			}
		}

		private void SelectByIndex(ListBox listBox, int index)
		{
			if (index >= listBox.Items.Count)
				index = listBox.Items.Count - 1;

			if (index < 0)
				return;
			listBox.SelectedItem = listBox.Items[index];
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

		void SelectCardByIndex(int index)
		{
			ActiveCard = null;
			SelectByIndex(lbCards, index);
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

		void SelectDeck(Deck deck)
		{
			lbDecks.SelectedItem = deck;
			lbCards.ItemsSource = deck?.Cards;
			UpdateCardsLabel(deck);
		}

		void SelectDeckByIndex(int index)
		{
			ActiveDeck = null;
			SelectByIndex(lbDecks, index);
		}

		void SelectField(Field field)
		{
			lbFields.SelectedItem = field;
		}

		void SelectFieldByIndex(int index)
		{
			ActiveField = null;
			SelectByIndex(lbFields, index);
		}

		private void SelectLinkedAlternates(CardImageLayer newAlternateImageLayer)
		{
			BeginUpdate();
			try
			{
				string displayName = newAlternateImageLayer.DisplayName;
				string alternateName = newAlternateImageLayer.AlternateName;

				PropertyLink propertyLink = GetPropertyLink(CardImageLayer.SubLayerNameStr, ActiveCard, newAlternateImageLayer);
				if (propertyLink == null)
					return;
				foreach (CardImageLayer cardImageLayer in propertyLink.Layers)
				{
					string displayNameToMatch = cardImageLayer.DisplayName;
					if (displayNameToMatch == displayName)  // Don't worry about alternates in this layer.
						continue;

					bool foundAlternateCousin = cardImageLayer.AlternateName == alternateName;

					if (foundAlternateCousin)
					{
						// If we are setting to "Glow - Torch", and alternate cousin might be named "Inner Glow - To
						CardImageLayer cousinToSelect = cardImageLayer;
						List<CardImageLayer> alternateCousinLayers = cousinToSelect.Alternates.CardLayers;

						CardImageLayer selectedAlternate = alternateCousinLayers.FirstOrDefault(x => x.Details.IsSelected);

						if (selectedAlternate != null)
							cardLayerManager.Replace(selectedAlternate, cousinToSelect, cvsLayers);

						SetDetailsIsSelected(cardImageLayer);
					}
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		void SetActiveCard(Card card)
		{
			BeginUpdate();
			try
			{
				ActiveCard = card;
				if (card != null && !string.IsNullOrWhiteSpace(card.StreamlootsImageFileUri))
					imgUploadedImage.Source = new BitmapImage(new Uri(card.StreamlootsImageFileUri));
				else
					imgUploadedImage.Source = null;

				if (ActiveCard != null)
				{
					codeCardPlayed.SetText(ActiveCard.CardPlayed);
					codeCardReceived.SetText(ActiveCard.CardReceived);
				}
				else
				{
					codeCardPlayed.SetText(string.Empty);
					codeCardReceived.SetText(string.Empty);
				}

				lbCards.SelectedItem = card;
				spSelectedCard.DataContext = card;
				spCardEvents.DataContext = card;
				SetActiveFields(card?.Fields);
				if (card == null)
					SetActiveLayerTextOptions(null);
				else
				{
					if (activeLayerTextOptions != null && ActiveCard.TextFontOpacity == 0)
						ActiveCard.SetFontBrush(activeLayerTextOptions.FontBrush);
					SetActiveLayerTextOptionsForCard(card);
				}

				tbItemName.DataContext = card;
				bool isValidCard = card != null;
				btnDeleteCard.IsEnabled = isValidCard;
				spSelectedCard.Visibility = isValidCard ? Visibility.Visible : Visibility.Collapsed;
				spCardEvents.Visibility = isValidCard ? Visibility.Visible : Visibility.Collapsed;
			}
			finally
			{
				EndUpdate();
			}
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

		void SetActiveFields(ObservableCollection<Field> fields)
		{
			lbFields.ItemsSource = fields;
			if (fields == null || fields.Count == 0)
				SetActiveField(null);
			else
				SetActiveField(fields[0]);
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

		void SetCardGenOptions(BoundaryKind boundaryKind, string key, double value)
		{
			cardGenerationOptions.Set(activeStyle, activeLayer, boundaryKind, key, value);
		}

		private void SetCardGenOptions(Slider slider, BoundaryKind boundaryKind)
		{
			if (slider == null)
				return;

			if (boundaryKind == BoundaryKind.FullRange)
			{
				SetCardGenOptions(BoundaryKind.Min, GetSliderNamePart(slider), slider.Minimum);
				SetCardGenOptions(BoundaryKind.Max, GetSliderNamePart(slider), slider.Maximum);
			}
			else
				SetCardGenOptions(boundaryKind, GetSliderNamePart(slider), slider.Value);
		}

		private void SetChancesVisibleMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			if (!int.TryParse(menuItem.Tag.ToString(), out int percentVisible))
				return;

			LayerGenerationOptions options = cardGenerationOptions.Find(activeStyle, activeLayer);
			if (options == null)
				return;

			options.ChancesVisible = percentVisible / 100.0;

			GoogleHelper.GoogleSheets.SaveChanges(options);
		}

		private void SetDetailsIsSelected(CardImageLayer newAlternateImageLayer)
		{
			foreach (CardImageLayer cardImageLayer in newAlternateImageLayer.Alternates.CardLayers)
				cardImageLayer.Details.IsSelected = cardImageLayer == newAlternateImageLayer;
		}

		private void SetFullRangeForCardGeneration_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetCardGenOptions(GetSliderFromSender(sender), BoundaryKind.FullRange);
		}

		private void SetMaxForCardGeneration_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetCardGenOptions(GetSliderFromSender(sender), BoundaryKind.Max);
		}

		private void SetMinForCardGeneration_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetCardGenOptions(GetSliderFromSender(sender), BoundaryKind.Min);
		}

		void ShowStatus(string message)
		{
			statusBar.Visibility = Visibility.Visible;
			sbMessage.Content = message;
		}

		private void sliderHorizontalStretch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				sliderVerticalStretch.Value = sliderHorizontalStretch.Value;
		}

		private void sliderHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
		}

		private void sliderOffsetX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
		}

		private void sliderOffsetY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
		}

		private void sliderSaturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
		}

		private void sliderVerticalStretch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				sliderHorizontalStretch.Value = sliderVerticalStretch.Value;
		}

		private void StopSampling()
		{
			if (samplingColor)
			{
				samplingColor = false;
				btnEyeDropper.Background = Brushes.LightGray;
				if (File.Exists(tempFileName))
					File.Delete(tempFileName);
			}
		}

		private void SynchronizeHorizontalAndVertical_ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			if (!(sender is ContextMenu contextMenu))
				return;

			Slider slider = GetSliderFromContextMenu(contextMenu);
			string sliderNamePart = GetSliderNamePart(slider);
			if (sliderNamePart == CardGenerationOptions.Horz || sliderNamePart == CardGenerationOptions.Vert)
				foreach (MenuItem menuItem in contextMenu.Items)
					if (menuItem.Name == "mnuSyncStretch")
						menuItem.IsChecked = cardGenerationOptions.GetSyncStretch(activeStyle, activeLayer);
		}

		private void SynchronizeHorizontalAndVertical_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			menuItem.IsChecked = !menuItem.IsChecked;
			cardGenerationOptions.SetSyncStretch(activeStyle, activeLayer, menuItem.IsChecked);
		}

		private void tbxAdditionalInstructions_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void tbxCardName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ChangingDataInternally)
				return;
			if (ActiveCard == null)
				return;
			if (tbxCardName.Text == ActiveCard.Name)
				return;
			ActiveCard.IsDirty = true;
			if (ActiveCard.ParentDeck != null)
				ActiveCard.ParentDeck.IsDirty = true;
			lbCards.Items.Refresh();
		}

		private void tbxCooldown_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

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

		private void tbxDescription_TextChanged(object sender, TextChangedEventArgs e)
		{
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

		private void tbxFragmentsRequired_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void TestStatusBarMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowStatus("Menu item clicked!");
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

		void UpdateCardsLabel(Deck deck)
		{
			tbCards.Text = deck == null ? string.Empty : $"Cards in {deck.Name}:";
		}

		bool SpellLevelMatches(SpellDto x, int spellLevel, bool checkCast)
		{
			if (spellLevel != 0 || x.level.ToLower() != "cantrip")
				if (!int.TryParse(x.level, out int thisSpellLevel) || thisSpellLevel != spellLevel)
					return false;
			if (checkCast)
				return x.viewerCanCast != null && x.viewerCanCast.ToUpper() != "FALSE";
			else
				return x.viewerCanGift != null && x.viewerCanGift.ToUpper() != "FALSE";
		}

		public class AvailableSpells
		{
			public List<SpellDto> ViewerCanCast;
			public List<SpellDto> ViewerCanGift;
			public AvailableSpells(List<SpellDto> viewerCanCast, List<SpellDto> viewerCanGift)
			{
				ViewerCanCast = viewerCanCast;
				ViewerCanGift = viewerCanGift;
			}
		}

		void AddSpellLevelMenuItems(int spellLevel, ContextMenu contextMenu, List<SpellDto> spellsViewerCanCast, List<SpellDto> spellsViewerCanGift)
		{
			if (spellsViewerCanCast.Count == 0 && spellsViewerCanGift.Count == 0)
				return;
			MenuItem menuItem = new MenuItem() { Header = $"Level {spellLevel} Spells ({spellsViewerCanCast.Count} castable, {spellsViewerCanGift.Count} giftable)" };
			menuItem.Click += AddLevelSpellsMenuItem_Click;
			menuItem.Tag = new AvailableSpells(spellsViewerCanCast, spellsViewerCanGift);
			contextMenu.Items.Add(menuItem);
		}

		private void HookCardEvents(Card card)
		{
			card.LinkedPropertyChanged += Card_LinkedPropertyChanged;
			card.PropertyChanged += Card_PropertyChanged;
		}

		private void Card_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (ChangingDataInternally || e.PropertyName == "IsDirty" || e.PropertyName == "Uploaded")
				return;

			if (!(sender is Card card))
				return;

			card.Uploaded = false;
		}

		private void AddLevelSpellsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			if (!(menuItem.Tag is AvailableSpells availableSpells))
				return;

			foreach (SpellDto spellDto in availableSpells.ViewerCanCast)
			{
				Card card = SpellFactory.AddCastSpellCard(spellDto, CardData, ActiveDeck);
				HookCardEvents(card);
			}

			foreach (SpellDto spellDto in availableSpells.ViewerCanGift)
			{
				Card card = SpellFactory.AddGiftSpellCard(spellDto, CardData, ActiveDeck);
				HookCardEvents(card);
			}
		}

		private void btnAddSpells_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			mnuPickSpellLevel.Items.Clear();
			for (int spellLevel = 0; spellLevel < 9; spellLevel++)
			{
				List<SpellDto> spellsViewerCanCast = AllSpells.Spells.FindAll(x => SpellLevelMatches(x, spellLevel, true));
				List<SpellDto> spellsViewerCanGift = AllSpells.Spells.FindAll(x => SpellLevelMatches(x, spellLevel, false));
				AddSpellLevelMenuItems(spellLevel, mnuPickSpellLevel, spellsViewerCanCast, spellsViewerCanGift);
			}
		}

		private CardImageLayer GetImageLayerFromDetails(LayerDetails details)
		{
			return cardLayerManager.CardLayers.FirstOrDefault(x => x.Details == details);
		}



		void CreateBonusPenaltyCard(string rollKind, int modifier, int powerLevel, bool isSecret = true)
		{
			Card card = DieModFactory.CreateBonusPenaltyCard(rollKind, modifier, powerLevel, isSecret, CardData, ActiveDeck);
			HookCardEvents(card);
		}

		void CreateRollModCardsAtPowerLevel(int powerLevel)
		{
			int topEnd = Math.Min(powerLevel + 3, 10);
			for (int i = powerLevel; i <= topEnd; i++)
			{
				CreateBonusPenaltyCard(DieModFactory.Skill, i, powerLevel, false);
				CreateBonusPenaltyCard(DieModFactory.Skill, i, powerLevel);
				CreateBonusPenaltyCard(DieModFactory.Skill, -i, powerLevel);
				CreateBonusPenaltyCard(DieModFactory.Save, i, powerLevel, false);
				CreateBonusPenaltyCard(DieModFactory.Save, i, powerLevel);
				CreateBonusPenaltyCard(DieModFactory.Save, -i, powerLevel);
				CreateBonusPenaltyCard(DieModFactory.Attack, i, powerLevel, false);
				CreateBonusPenaltyCard(DieModFactory.Attack, i, powerLevel);
				CreateBonusPenaltyCard(DieModFactory.Attack, -i, powerLevel);
			}
		}

		void CreateSayAnythingCardsAtPowerLevel(int powerLevel)
		{
			int topEnd = Math.Min(powerLevel + 3, 16);
			for (int i = powerLevel; i <= topEnd; i++)
			{
				Card card = SayAnythingFactory.CreateSayAnythingCard(i, powerLevel, CardData, ActiveDeck);
				HookCardEvents(card);
			}
		}

		private void RollModPowerLevelMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;
			if (!int.TryParse(menuItem.Tag.ToString(), out int powerLevel))
				return;
			CreateRollModCardsAtPowerLevel(powerLevel);
		}

		private void SayAnythingPowerLevelMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;
			if (!int.TryParse(menuItem.Tag.ToString(), out int powerLevel))
				return;
			CreateSayAnythingCardsAtPowerLevel(powerLevel);
		}

		void InitializeStreamlootsClient()
		{
			streamlootsClient = new StreamlootsClient(new MySecureString(Configuration["Secrets:StreamlootsBearerToken"]));
		}

		void InitializeCloudinaryClient()
		{
			cloudinaryClient = new CloudinaryClient(Folders.Images, new MySecureString($"cloudinary://812749384489784:{Configuration["Secrets:cloudinaryApiSecret"]}@dragonhumpers"));
		}

		string GetFileName(Card activeCard)
		{
			return CardDto.GetFileName(activeCard.Name) + ".png";
		}

		private void btnUploadImage_Click(object sender, RoutedEventArgs e)
		{
			SaveAndUploadImage(ActiveCard);
		}

		private void SaveAndUploadImage(Card card)
		{
			string fullPath = SaveCardImageToFile(card);
			ActiveCard.CloudinaryImageUrl = cloudinaryClient.UploadImage(SysPath.GetFileName(fullPath));
			streamlootsClient.UploadImageFile(card, fullPath);
		}

		private string SaveCardImageToFile(Card card)
		{
			string fileName = GetFileName(card);
			string fullPath = Path.Combine(Folders.Images, fileName);
			cvsLayers.SaveToPng(new Uri(fullPath));
			return fullPath;
		}

		private async void btnUploadCard_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveCard == null)
				return;
			await UploadCard(ActiveCard);
		}

		private async Task UploadCard(Card card)
		{
			if (string.IsNullOrWhiteSpace(card.StreamlootsImageFileUri))
				SaveAndUploadImage(card);

			if (!string.IsNullOrWhiteSpace(card.StreamlootsCardId))
				await streamlootsClient.UpdateCard(card);
			else
			{
				SetCardViewModel setCardViewModel = await streamlootsClient.AddCard(card);
				card.StreamlootsCardId = setCardViewModel._id;
			}
			card.Uploaded = true;
		}

		private void codeCardReceived_CodeChanged(object sender, EventArgs e)
		{
			if (ChangingDataInternally || ActiveCard == null)
				return;
			ActiveCard.CardReceived = codeCardReceived.TextEditor.Text;
		}

		private void codeCardPlayed_CodeChanged(object sender, EventArgs e)
		{
			if (ChangingDataInternally || ActiveCard == null)
				return;
			ActiveCard.CardPlayed = codeCardPlayed.TextEditor.Text;
		}

		void RegisterSpreadsheetIDs()
		{
			GoogleSheets.RegisterSpreadsheetID("DnD", "13g0mcruC1gLcSfkVESIWW9Efrn0MyaKw0hqCiK1Rg8k");
			GoogleSheets.RegisterSpreadsheetID("IDE", "1q-GuDx91etsKO0HzX0MCojq24PGZbPIcTZX-V6arpTQ");
		}

		private void SaveSelectedCardStyle_Click(object sender, RoutedEventArgs e)
		{
			CardStyles.Save(ActiveCard);
		}



		private void StartingProgress(string caption, int count)
		{
			Dispatcher.Invoke(() =>
			{
				prgUploading.Value = 0;
				prgUploading.Maximum = count;
				tbProgressLabel.Text = caption;
				spProgress.Visibility = Visibility.Visible;
			});
		}

		void UpdateProgress(int value)
		{
			Dispatcher.Invoke(() =>
			{
				prgUploading.Value = value;
			});
		}

		void HideProgress()
		{
			Dispatcher.Invoke(() =>
			{
				spProgress.Visibility = Visibility.Collapsed;
				prgUploading.Value = 0;
			});
		}

		async void UploadUpdateAllCardsInDeck()
		{
			if (ActiveDeck == null)
				return;
			int cardsUploadedSoFar = 0;
			StartingProgress("Uploading cards...", ActiveDeck.Cards.Count);
			foreach (Card card in ActiveDeck.Cards)
			{
				await UploadCard(card);
				UpdateProgress(cardsUploadedSoFar++);
				if (cardsUploadedSoFar % 5 == 0)
				{
					
				}
			}
			HideProgress();
		}
		private void UploadUpdateAllCardsInDeck_Click(object sender, RoutedEventArgs e)
		{
			UploadUpdateAllCardsInDeck();
		}

		// TODO: Prompt for save if dirty on close!!!
	}
}

