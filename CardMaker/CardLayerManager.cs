using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
			Image image = layer.CreateImage();
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
