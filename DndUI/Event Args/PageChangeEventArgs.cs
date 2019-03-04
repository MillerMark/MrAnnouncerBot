using System;
using System.Windows;

namespace DndUI
{
	public class PageChangeEventArgs: RoutedEventArgs
	{
		public PageChangeEventArgs(RoutedEvent routedEvent, int pageIndex) : base(routedEvent)
		{
			PageIndex = pageIndex;
		}

		public int PageIndex { get; private set; }
	}
}
