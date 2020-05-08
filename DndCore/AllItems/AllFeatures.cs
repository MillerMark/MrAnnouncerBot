using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllFeatures
	{

		static AllFeatures()
		{
			DndGame.Instance.PlayerStateChanged += Instance_PlayerStateChanged;
		}

		public static void LoadData()
		{
			LoadData(GoogleSheets.Get<FeatureDto>(Folders.InCoreData("DnD - Features.csv")));
		}

		private static void Instance_PlayerStateChanged(object sender, PlayerStateEventArgs ea)
		{
			ea.Player.ActivateConditionallySatisfiedFeatures();
		}

		static List<Feature> features;
		public static List<Feature> Features
		{
			get
			{
				if (features == null)
					LoadData();
				return features;
			}
			private set
			{
				features = value;
			}
		}

		public static Feature Get(string featureName)
		{
			return Features.FirstOrDefault(x => x.Name == featureName);
		}
		static void LoadData(List<FeatureDto> data)
		{
			features = new List<Feature>();
			foreach (FeatureDto featureDto in data)
			{
				features.Add(Feature.FromDto(featureDto));
			}
		}
		public static void Invalidate()
		{
			features = null;
		}
	}
}
