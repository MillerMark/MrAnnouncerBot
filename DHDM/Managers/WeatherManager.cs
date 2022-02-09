//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using SheetsPersist;
using OBSWebsocketDotNet;
using ObsControl;

namespace DHDM
{
	public class WeatherManager
	{
		System.Timers.Timer timer;
		private const string DH_WeatherIcons = "DH.WeatherIcons";
		private const string DH_WeatherScenesBack = "DH.WeatherScenesBack";
		private const string DH_WeatherScenesFront = "DH.WeatherScenesFront";
		private const string WeatherIcon_Sun = "WeatherIcon_Sun";
		private const string WeatherIcon_Moon = "WeatherIcon_Moon";

		List<string> allWeatherIcons = new List<string>();
		List<string> allBackItems = new List<string>();
		List<string> allFrontItems = new List<string>();

		List<WeatherSceneSettingsDto> weatherSettings;
		
		public WeatherManager(DndGame game)
		{
			Game = game;
		}

		void LoadAllKnownItems()
		{
			foreach (WeatherSceneSettingsDto weatherSceneSettingsDto in weatherSettings)
			{
				AddIconItem(weatherSceneSettingsDto);
				AddBackItem(weatherSceneSettingsDto);
				AddFrontItem(weatherSceneSettingsDto);
			}
		}

		private void AddIconItem(WeatherSceneSettingsDto weatherSceneSettingsDto)
		{
			string iconItem = weatherSceneSettingsDto.IconItem;

			if (!string.IsNullOrWhiteSpace(iconItem))
				if (allWeatherIcons.IndexOf(iconItem) < 0)
					allWeatherIcons.Add(iconItem);
		}

		private void AddBackItem(WeatherSceneSettingsDto weatherSceneSettingsDto)
		{
			string backItems = weatherSceneSettingsDto.BackItem;
			AddItem(backItems, allBackItems);
		}

		private void AddFrontItem(WeatherSceneSettingsDto weatherSceneSettingsDto)
		{
			string frontItems = weatherSceneSettingsDto.FrontItem;
			AddItem(frontItems, allFrontItems);
		}

		private void AddItem(string items, List<string> allItems)
		{
			if (!string.IsNullOrWhiteSpace(items))
			{
				string[] parts = items.Split(';');
				foreach (string backItem in parts)
				{
					string trimmedBackItem = backItem.Trim();
					if (!string.IsNullOrWhiteSpace(trimmedBackItem))
						if (allItems.IndexOf(trimmedBackItem) < 0)
							allItems.Add(trimmedBackItem);
				}
			}
		}

		public void Load()
		{
			allWeatherIcons.Clear();
			allBackItems.Clear();
			allFrontItems.Clear();
			weatherSettings = GoogleSheets.Get<WeatherSceneSettingsDto>();
			LoadAllKnownItems();
		}
		//void HideAll(string sceneName, IEnumerable<string> items)
		//{
		//	throw new NotImplementedException();
		//}
		//void HideAll()
		//{
		//	HideAll(DH_WeatherScenesBack, weatherSettings.Select(x => x.BackItem).Where(x => !string.IsNullOrWhiteSpace(x)));
		//	HideAll(DH_WeatherScenesFront, weatherSettings.Select(x => x.FrontItem).Where(x => !string.IsNullOrWhiteSpace(x)));
		//}

		WeatherSceneSettingsDto GetWeather(string weatherKeyword)
		{
			return weatherSettings.FirstOrDefault(x => x.Keyword == weatherKeyword);
		}

		public void SetObsSourceVisibility(string sceneName, string sourceName, bool visible)
		{
			if (sourceName == null)
				return;
			try
			{
				ObsManager.SetSourceRender(sourceName, visible, sceneName);
			}
			catch //(Exception ex)
			{
				
			}
		}

		void ShowOnly(string sceneName, List<string> allSceneItems, string item)
		{
			List<string> scenesToShow = new List<string>();
			if (item != null)
			{
				string[] parts = item.Split(';');
				foreach (string part in parts)
				{
					string trimmedPart = part.Trim();
					scenesToShow.Add(trimmedPart);
				}
			}

			foreach (string sceneItem in allSceneItems)
			{
				bool visible = scenesToShow.IndexOf(sceneItem) >= 0;
				SetObsSourceVisibility(sceneName, sceneItem, visible);
			}
		}

		List<string> iconsToClear;
		void ShowWeatherIcon(string iconItem)
		{
			if (timer == null)
			{
				timer = new System.Timers.Timer();
				timer.Elapsed += Timer_Elapsed;
				timer.Interval = 6 * 1000;
			}

			timer.Start();
			if (iconsToClear == null)
				iconsToClear = new List<string>();
			lock (iconsToClear)
				iconsToClear.Add(iconItem);

			if (iconItem == "None")
				return;

			if (Game.Clock.IsDaytime())
			{
				SetObsSourceVisibility(DH_WeatherIcons, WeatherIcon_Sun, true);
			}
			else
			{
				SetObsSourceVisibility(DH_WeatherIcons, WeatherIcon_Moon, true);
			}

			if (iconItem != null)
			{
				SetObsSourceVisibility(DH_WeatherIcons, iconItem, true);
			}
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop();
			SetObsSourceVisibility(DH_WeatherIcons, WeatherIcon_Sun, false);
			SetObsSourceVisibility(DH_WeatherIcons, WeatherIcon_Moon, false);
			if (iconsToClear == null)
				return;
			lock (iconsToClear)
				foreach (string icon in iconsToClear)
					SetObsSourceVisibility(DH_WeatherIcons, icon, false);
			iconsToClear = null;
		}

		public void ShowWeather(string weatherKeyword)
		{
			WeatherSceneSettingsDto weather = GetWeather(weatherKeyword);
			ShowOnly(DH_WeatherScenesBack, allBackItems, weather?.BackItem);
			ShowOnly(DH_WeatherScenesFront, allFrontItems, weather?.FrontItem);
			string icon = weather?.IconItem;
			if (icon == null)
				icon = weatherKeyword;
			ShowWeatherIcon(icon);
		}
		public DndGame Game { get; set; }
	}
}