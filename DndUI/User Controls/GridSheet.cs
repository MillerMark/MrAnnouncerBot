using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DndUI
{
	class GridSheet: Grid
	{

		public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent("Activated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridSheet));
		public static readonly RoutedEvent PreviewActivatedEvent = EventManager.RegisterRoutedEvent("PreviewActivated", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(GridSheet));

		public event RoutedEventHandler Activated
		{
			add { AddHandler(ActivatedEvent, value); }
			remove { RemoveHandler(ActivatedEvent, value); }
		}
		public event RoutedEventHandler PreviewActivated
		{
			add { AddHandler(PreviewActivatedEvent, value); }
			remove { RemoveHandler(PreviewActivatedEvent, value); }
		}

		protected virtual void OnActivated()
		{
			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewActivatedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(ActivatedEvent);
			RaiseEvent(eventArgs);
		}

		public GridSheet(): base()
		{
			this.PreviewMouseDown += GridSheet_PreviewMouseDown;
		}

		private void GridSheet_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OnActivated();
		}

		static GridSheet()
		{
			HorizontalAlignmentProperty.OverrideMetadata(typeof(GridSheet), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			VerticalAlignmentProperty.OverrideMetadata(typeof(GridSheet), new FrameworkPropertyMetadata(VerticalAlignment.Top));
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			ListenToActivatedEvents(this);
		}

		void StatBox_Activated(object sender, RoutedEventArgs e)
		{
			OnActivated();
		}

		internal static void FindChildren<T>(List<T> results, DependencyObject startNode) where T : DependencyObject
		{
			int count = VisualTreeHelper.GetChildrenCount(startNode);
			for (int i = 0; i < count; i++)
			{
				DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
				if ((current.GetType()).Equals(typeof(T)) || (current.GetType().IsSubclassOf(typeof(T))))
				{
					T asType = (T)current;
					results.Add(asType);
				}
				FindChildren<T>(results, current);
			}
		}

		private void ListenToActivatedEvents(DependencyObject startNode)
		{
			List<StatBox> statBoxes = new List<StatBox>();
			FindChildren<StatBox>(statBoxes, startNode);

			foreach (StatBox statBox in statBoxes)
			{
				statBox.Activated += StatBox_Activated;
			}
		}
	}
}
