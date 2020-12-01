using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CardMaker
{
	public class CardLayerManager
	{
		public List<CardImageLayer> CardLayers { get; private set; } = new List<CardImageLayer>();

		public CardLayerManager()
		{
		}

		public void Clear()
		{
			CardLayers.Clear();
		}

		public void Add(CardImageLayer cardImageLayer)
		{
			CardLayers.Add(cardImageLayer);
		}

		void AddLayerToCanvas(CardImageLayer layer, Canvas canvas)
		{
			UIElement image = layer.CreateImage();
			if (layer.X != 0)
				Canvas.SetLeft(image, layer.X);
			if (layer.Y != 0)
				Canvas.SetTop(image, layer.Y);
			canvas.Children.Add(image);
		}

		public void AddLayersToCanvas(Canvas canvas)
		{
			foreach (CardImageLayer cardImageLayer in CardLayers)
				AddLayerToCanvas(cardImageLayer, canvas);
		}
		public void SortByLayerIndex()
		{
			CardLayers = CardLayers.OrderBy(x => x.Index).Reverse().ToList();
		}
	}
}
