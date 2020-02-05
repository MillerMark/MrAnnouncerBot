using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public interface IStamp: IStampProperties
	{
		[JsonIgnore]
		Image Image { get; }
		void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0);
		void CreateFloating(Canvas canvas, int left = 0, int top = 0);
	}
}

