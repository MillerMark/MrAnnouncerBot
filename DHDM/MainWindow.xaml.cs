using Microsoft.AspNetCore.SignalR.Client;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		HubConnection hubConnection;
		public MainWindow()
		{
			InitializeComponent();
			ConnectToHub();
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PlayerPageChanged(tabPlayers.SelectedIndex, (int)ScrollPage.main, string.Empty);
		}

		void ConnectToHub()
		{
			hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:44303/MrAnnouncerBotHub").Build();
			if (hubConnection != null)
			{
				//hubConnection.Closed += HubConnection_Closed;
				// TODO: Check out benefits of stopping gracefully with a cancellation token.
				hubConnection.StartAsync();
			}
		}



		void PlayerPageChanged(int playerID, int pageID, string playerData)
		{
			hubConnection.InvokeAsync("PlayerPageChanged", playerID, pageID, playerData);
		}

		void FocusItem(int playerID, int pageID, string itemID)
		{
			hubConnection.InvokeAsync("FocusItem", playerID, pageID, itemID);
		}

		private void CharacterSheets_PageChanged(object sender, RoutedEventArgs ea)
		{
			CharacterSheets characterSheets = sender as CharacterSheets;
			if (characterSheets != null)
				PlayerPageChanged(tabPlayers.SelectedIndex, (int)characterSheets.Page, string.Empty);
		}
	}
}
