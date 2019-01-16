using System;
using System.Windows;

namespace DHDM
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
