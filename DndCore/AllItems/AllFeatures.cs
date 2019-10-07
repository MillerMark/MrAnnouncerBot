using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllFeatures
	{

		static AllFeatures()
		{
			LoadData();
			DndGame.Instance.PlayerStateChanged += Instance_PlayerStateChanged;
		}

		public static void LoadData()
		{
			LoadData(CsvData.Get<FeatureDto>(Folders.InCoreData("DnD - Features.csv")));
		}

		private static void Instance_PlayerStateChanged(object sender, PlayerStateEventArgs ea)
		{
			ea.Player.ActivateFeaturesByConditions();
		}

		public static List<Feature> Features { get; private set; }

		public static Feature Get(string featureName)
		{
			return Features.FirstOrDefault(x => x.Name == featureName);
		}
		static void LoadData(List<FeatureDto> data)
		{
			if (Features == null)
				Features = new List<Feature>();
			Features.Clear();
			foreach (FeatureDto featureDto in data)
			{
				Features.Add(Feature.FromDto(featureDto));
			}
		}
	}
}
