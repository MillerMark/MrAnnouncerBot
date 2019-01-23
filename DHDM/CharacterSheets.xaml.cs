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
	public enum ScrollPage
	{
		deEmphasis = 0,
		main = 1,
		skills = 2,
		equipment = 3
	}

	/// <summary>
	/// Interaction logic for CharacterSheets.xaml
	/// </summary>
	public partial class CharacterSheets : UserControl
	{
		
		public ScrollPage Page
		{
			get { return scrollPage; }
			set
			{
				scrollPage = value;
			}
		}
		

		ScrollPage scrollPage;
		public static readonly RoutedEvent PageChangedEvent = EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CharacterSheets));
		public static readonly RoutedEvent PreviewPageChangedEvent = EventManager.RegisterRoutedEvent("PreviewPageChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CharacterSheets));

		public event RoutedEventHandler PageChanged
		{
			add { AddHandler(PageChangedEvent, value); }
			remove { RemoveHandler(PageChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewPageChanged
		{
			add { AddHandler(PreviewPageChangedEvent, value); }
			remove { RemoveHandler(PreviewPageChangedEvent, value); }
		}

		protected virtual void OnPageChanged(ScrollPage newPage)
		{
			scrollPage = newPage;
			FocusHelper.ClearActiveStatBoxes();

			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewPageChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(PageChangedEvent);
			RaiseEvent(eventArgs);
		}
		
		public CharacterSheets()
		{
			InitializeComponent();
		}

		private void PageMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.main);
		}

		private void PageSkills_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.skills);
		}

		private void PageEquipment_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.equipment);
		}
	}
}
