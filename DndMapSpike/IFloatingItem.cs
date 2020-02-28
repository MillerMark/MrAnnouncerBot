using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public interface IFloatingItem: IItemProperties
	{
		[JsonIgnore]
		Image Image { get; }
		void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0);
		void CreateFloating(Canvas canvas, double left = 0, double top = 0);
	}
}

