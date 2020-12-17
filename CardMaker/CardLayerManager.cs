using System;
using DndCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace CardMaker
{
	public class AlternateCardImageLayers
	{
		public List<CardImageLayer> CardLayers { get; private set; } = new List<CardImageLayer>();
		public AlternateCardImageLayers()
		{

		}
	}
	public class CardLayerManager
	{
		public BindingList<CardImageLayer> CardLayers { get; private set; } = new BindingList<CardImageLayer>();

		public CardLayerManager()
		{
		}

		public void Clear()
		{
			CardLayers.Clear();
		}

		CardImageLayer GetLayerForCategory(string category)
		{
			return CardLayers.FirstOrDefault(x => x.Category == category);
		}

		public void Add(CardImageLayer cardImageLayer)
		{
			if (cardImageLayer.Details.Name.IndexOf("-") > 0)
			{
				string category = GetCategory(cardImageLayer);
				cardImageLayer.Category = category;
				cardImageLayer.Details.DisplayName = category;
				cardImageLayer.AlternateName = cardImageLayer.Details.Name.EverythingAfter("-").Trim();
				CardImageLayer existingLayer = GetLayerForCategory(category);
				if (existingLayer != null)
				{
					if (existingLayer.Alternates == null)
					{
						existingLayer.Alternates = new AlternateCardImageLayers();
						existingLayer.Alternates.CardLayers.Add(existingLayer);
					}
					existingLayer.Alternates.CardLayers.Add(cardImageLayer);
					cardImageLayer.Alternates = existingLayer.Alternates;
					return;
				}
			}
			CardLayers.Add(cardImageLayer);
		}

		private static string GetCategory(CardImageLayer cardImageLayer)
		{
			return cardImageLayer.Details.Name.EverythingBefore("-").Trim();
		}

		void AddImageLayerToCanvas(CardImageLayer layer, Canvas canvas)
		{
			Image image;
			if (layer.LayerName == "Placeholder")
			{
				if (layer.PlaceholderWidth == 0)
					layer.SetPlaceholderDetails();
				image = layer.PlaceImageIntoPlaceholder();
			}
			else
			{
				image = layer.CreateImage();
			}

			canvas.Children.Add(image);
			Panel.SetZIndex(image, layer.Index);
		}

		public void AddImageLayersToCanvas(Canvas canvas)
		{
			//! For-loop needed because as layers are added, we may need to replace a layer with its alternate (which would break a foreach).
			for (int i = CardLayers.Count - 1; i >= 0; i--)
				AddImageLayerToCanvas(CardLayers[i], canvas);
		}
		public void SortByLayerIndex()
		{
			CardLayers = new BindingList<CardImageLayer>(CardLayers.OrderBy(x => x.Index).Reverse().ToList());

			for (int i = 0; i < CardLayers.Count; i++)
				CardLayers[i].Index = CardLayers.Count - i;
		}

		public void HideAllLayersExcept(CardImageLayer notThisCardImageLayer)
		{
			foreach (CardImageLayer cardImageLayer in CardLayers)
				cardImageLayer.IsVisible = cardImageLayer == notThisCardImageLayer;
		}
		public void ShowAllLayersExcept(CardImageLayer notThisCardImageLayer)
		{
			foreach (CardImageLayer cardImageLayer in CardLayers)
				cardImageLayer.IsHidden = cardImageLayer == notThisCardImageLayer;
		}

		public void ShowAllLayers()
		{
			ShowAllLayersExcept(null);
		}
		public void Replace(CardImageLayer selectedLayer, CardImageLayer newImageLayer, Canvas canvas)
		{
			if (selectedLayer == newImageLayer)
				return;
			int indexOfExisting = GetIndexOfExisting(selectedLayer);
			if (indexOfExisting < 0)
				return;

			for (int i = canvas.Children.Count - 1; i >= 0; i--)
				if (selectedLayer.ImageMatches(canvas.Children[i]))
				{
					canvas.Children.RemoveAt(i);
					canvas.Children.Insert(i, newImageLayer.CreateImage());
					break;
				}

			CardLayers.RemoveAt(indexOfExisting);
			CardLayers.Insert(indexOfExisting, newImageLayer);
		}

		private int GetIndexOfExisting(CardImageLayer selectedLayer)
		{
			for (int i = 0; i < CardLayers.Count; i++)
				if (CardLayers[i].LayerName == selectedLayer.LayerName)
					return i;
			return -1;
		}

		public void ReplaceImageInPlaceHolder(string fileName, Canvas canvas)
		{
			foreach (CardImageLayer cardImageLayer in CardLayers)
				if (cardImageLayer.LayerName == "Placeholder")
				{
					cardImageLayer.ReplaceImage(fileName, canvas);
					return;
				}
		}
	}
}
