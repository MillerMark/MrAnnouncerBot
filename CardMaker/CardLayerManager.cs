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
			image.DataContext = layer;
			if (layer.X != 0)
				Canvas.SetLeft(image, layer.X);
			if (layer.Y != 0)
				Canvas.SetTop(image, layer.Y);
			canvas.Children.Add(image);
			BindToCanvasPosition(layer, image);
		}

		private static void BindToCanvasPosition(CardImageLayer layer, Image image)
		{
			CreateAttachedPropertyBinding(layer, CardImageLayer.XProperty, image, Canvas.LeftProperty);
			CreateAttachedPropertyBinding(layer, CardImageLayer.YProperty, image, Canvas.TopProperty);
		}

		private static void CreateAttachedPropertyBinding(CardImageLayer source, DependencyProperty sourceProperty, DependencyObject target, DependencyProperty targetProperty)
		{
			Binding binding = new Binding
			{
				Path = new PropertyPath(sourceProperty),
				Source = source
			};
			BindingOperations.SetBinding(target, targetProperty, binding);
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
