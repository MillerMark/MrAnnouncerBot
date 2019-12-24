using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class Layers
	{
		List<Layer> AllLayers { get; set; } = new List<Layer>();
		public int Count
		{
			get
			{
				return AllLayers.Count;
			}
		}
		public Layers()
		{

		}
		public void Add(Layer layer)
		{
			AllLayers.Add(layer);
		}
		public void SetSize(int widthPx, int heightPx)
		{
			foreach (Layer layer in AllLayers)
				layer.SetSize(widthPx, heightPx);
		}
		public void AddImagesToCanvas(Canvas canvas)
		{
			foreach (Layer layer in AllLayers)
				layer.AddImageToCanvas(canvas);
		}
		public void SetZIndex(int count)
		{
			foreach (Layer layer in AllLayers)
				layer.SetZIndex(count);
		}
	}
}

