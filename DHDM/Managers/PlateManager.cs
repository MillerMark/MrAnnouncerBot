//#define profiling
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using GoogleHelper;
using System.Linq;
using ObsControl;

namespace DHDM
{
	/// <summary>
	/// Manages the foreground and background plates
	/// </summary>
	public class PlateManager
	{
		const string BackgroundSceneName = "DH.Background";
		const string ForegroundSceneName = "DH.Foreground";
		const string DungeonWalkDoor = "DungeonWalkDoor.mov";
		const string Nebula = "Nebula.mp4";
		const string WaitingToStartTunnels = "WaitingToStartTunnels.mp4";
		const string TaleSpireNdi = "TaleSpire.NDI";
		const string TheVoid_WhiteBlack = "TheVoid_WhiteBlack.mov";
		const string TheVoid_PinkPurple = "TheVoid_PinkPurple.mov";
		const string TheVoid_Lava = "TheVoid_Lava.mov";
		const string TheVoid_Rainbow = "TheVoid_Rainbow.mov";
		const string TopMonitor = "TopMonitor";
		const string BothComputers = "CR.BothComputers";
		
		List<string> voidItems;
		List<string> allBackgroundItems;

		public PlateManager()
		{
			InitializeSceneLists();
		}

		void InitializeSceneLists()
		{
			voidItems = new List<string>();
			allBackgroundItems = new List<string>();
			AddVoidItems();
			AddNonVoidItems();
		}

		private void AddNonVoidItems()
		{
			allBackgroundItems.Add(DungeonWalkDoor);
			allBackgroundItems.Add(WaitingToStartTunnels);
			allBackgroundItems.Add(Nebula);
			allBackgroundItems.Add(TaleSpireNdi);
			allBackgroundItems.Add(TopMonitor);
			allBackgroundItems.Add(BothComputers);
		}

		private void AddVoidItems()
		{
			AddVoidItem(TheVoid_WhiteBlack);
			AddVoidItem(TheVoid_PinkPurple);
			AddVoidItem(TheVoid_Lava);
			AddVoidItem(TheVoid_Rainbow);
			AddVoidItem(WaitingToStartTunnels);
		}

		private void AddVoidItem(string item)
		{
			voidItems.Add(item);
			allBackgroundItems.Add(item);
		}

		public void SetObsSourceVisibility(string sceneName, string sourceName, bool visible)
		{
			try
			{
				ObsManager.SetSourceRender(sourceName, visible, sceneName);
			}
			catch (Exception ex)
			{
				
			}
		}

		bool HasItem(string[] keepItems, string compareItem)
		{
			foreach (string item in keepItems)
			{
				if (item == compareItem)
					return true;
			}
			return false;
		}

		void HideAllBackgroundItemsExcept(params string[] keepItems)
		{
			foreach (string backgroundItem in allBackgroundItems)
			{
				if (!HasItem(keepItems, backgroundItem))
					SetObsSourceVisibility(BackgroundSceneName, backgroundItem, false);
			}
		}

		public void ShowBackground(string sourceName)
		{
			if (sourceName == "Clear")
			{
				HideAllBackgroundItems();
				return;	
			}
			SetObsSourceVisibility(BackgroundSceneName, "Black Background", true);
			//OBSScene currentScene = obsWebsocket.GetCurrentScene();
			bool isVoidScene = sourceName.StartsWith("TheVoid_");
			SetObsSourceVisibility(BackgroundSceneName, DungeonWalkDoor, isVoidScene);
			if (isVoidScene)
				HideAllBackgroundItemsExcept(sourceName, DungeonWalkDoor);
			else
				HideAllBackgroundItemsExcept(sourceName);
			SetObsSourceVisibility(BackgroundSceneName, sourceName, true);
		}

		private void HideAllBackgroundItems()
		{
			HideAllBackgroundItemsExcept();
			SetObsSourceVisibility(BackgroundSceneName, "Black Background", false);
		}

		internal void ShowForeground(string sourceName)
		{
			SetObsSourceVisibility(ForegroundSceneName, sourceName, true);
		}
	}
}