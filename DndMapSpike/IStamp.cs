using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public interface IStamp: IStampProperties
	{
		Image Image { get; }
		void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0);
		void CreateFloating(Canvas canvas, int left = 0, int top = 0);
	}
}

