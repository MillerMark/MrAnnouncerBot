using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace CardMaker
{
	public static class WpfExtensionMethods
	{
		private static Action EmptyDelegate = delegate () { };


		public static void Refresh(this UIElement uiElement)

		{
			uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		}
	}
}
