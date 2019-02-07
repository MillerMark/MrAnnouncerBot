using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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
		ScrollPage activePage = ScrollPage.main;
		HubConnection hubConnection;
		public MainWindow()
		{
			InitializeComponent();
			ConnectToHub();
			FocusHelper.FocusedControlsChanged += FocusHelper_FocusedControlsChanged;
		}

		public int PlayerID
		{
			get
			{
				return tabPlayers.SelectedIndex;
			}
		}

		private void FocusHelper_FocusedControlsChanged(object sender, FocusedControlsChangedEventArgs e)
		{
			foreach (StatBox statBox in e.Active)
			{
				FocusItem(PlayerID, activePage, statBox.FocusItem);
			}

			foreach (StatBox statBox in e.Deactivated)
			{
				UnfocusItem(PlayerID, activePage, statBox.FocusItem);
			}
		}

		private void TabControl_PlayerChanged(object sender, SelectionChangedEventArgs e)
		{
			activePage = ScrollPage.main;
			FocusHelper.ClearActiveStatBoxes();
			PlayerPageChanged(PlayerID, activePage, string.Empty);
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

		void PlayerPageChanged(int playerID, ScrollPage pageID, string playerData)
		{
			hubConnection.InvokeAsync("PlayerPageChanged", playerID, (int)pageID, playerData);
		}

		void FocusItem(int playerID, ScrollPage pageID, string itemID)
		{
			hubConnection.InvokeAsync("FocusItem", playerID, (int)pageID, itemID);
		}

		void UnfocusItem(int playerID, ScrollPage pageID, string itemID)
		{
			hubConnection.InvokeAsync("UnfocusItem", playerID, (int)pageID, itemID);
		}

		void TriggerEffect(string effectData)
		{
			hubConnection.InvokeAsync("TriggerEffect", effectData);
		}

		private void CharacterSheets_PageChanged(object sender, RoutedEventArgs ea)
		{
			if (sender is CharacterSheets characterSheets && activePage != characterSheets.Page)
			{
				activePage = characterSheets.Page;
				PlayerPageChanged(tabPlayers.SelectedIndex, activePage, string.Empty);
			}
		}

		private void btnSampleEffect_Click(object sender, RoutedEventArgs e)
		{
			AnimationEffect animationEffect = new AnimationEffect("DenseSmoke", new VisualEffectTarget(960, 1080), 0, 220, 100, 100);
			string serializedObject = JsonConvert.SerializeObject(animationEffect);
			TriggerEffect(serializedObject);
		}

		private void BtnTestEffect_Click(object sender, RoutedEventArgs e)
		{
			Effect activeEffect = effectBuilder.GetEffect();
			if (activeEffect == null)
				return;
			string serializedObject = JsonConvert.SerializeObject(activeEffect);
			TriggerEffect(serializedObject);
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			lbEffectsComposite.Items.Add(new EffectEntry(EffectKind.Animation, "New Effect"));
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{

		}

		private void LbEffectsComposite_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox)
				if (listBox.SelectedItem is EffectEntry effectEntry)
					effectBuilder.LoadFromItem(effectEntry);
		}
	}
}
