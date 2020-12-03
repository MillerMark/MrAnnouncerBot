using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace CardMaker
{
	public static class BindingHelper
	{
		public static void BindToCanvasPosition(CardImageLayer layer, Image image)
		{
			BindProperties(layer, CardImageLayer.XProperty, image, Canvas.LeftProperty);
			BindProperties(layer, CardImageLayer.YProperty, image, Canvas.TopProperty);
		}

		public static void BindProperties(object source, object sourceProperty, DependencyObject target, DependencyProperty targetProperty)
		{
			Binding binding = new Binding
			{
				Path = new PropertyPath(sourceProperty),
				Source = source
			};
			BindingOperations.SetBinding(target, targetProperty, binding);
		}
	}
}
