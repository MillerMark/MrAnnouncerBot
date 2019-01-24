using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for Sheet.xaml
	/// </summary>
	public partial class Sheet : UserControl
	{
		public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent("Activated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sheet));
		public static readonly RoutedEvent PreviewActivatedEvent = EventManager.RegisterRoutedEvent("PreviewActivated", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(Sheet));

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
		
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Sheet), new FrameworkPropertyMetadata(default(ImageSource), new PropertyChangedCallback(OnImageSourceChanged)));

		public ImageSource ImageSource
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (ImageSource)GetValue(ImageSourceProperty);
			}
			set
			{
				SetValue(ImageSourceProperty, value);
			}
		}

		public Sheet()
		{
			InitializeComponent();
		}

		static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Sheet sheet = d as Sheet;
			if (sheet != null)
				sheet.imageBack.Source = (ImageSource)e.NewValue;
		}

		private void ImageBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

		private void UserControl_Initialized(object sender, EventArgs e)
		{
			List<StatBox> statBoxes = new List<StatBox>();
			FindChildren<StatBox>(statBoxes, this);
			foreach (StatBox statBox in statBoxes)
			{
				statBox.Activated += StatBox_Activated;
			}
		}

		private void StatBox_Activated(object sender, RoutedEventArgs e)
		{
			OnActivated();
		}
	}
}
