using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using GoogleHelper;

namespace CardMaker
{
	public class CardGenerationOptions
	{
		public const string Hue = "Hue";
		public const string Sat = "Sat";
		public const string Light = "Light";
		public const string Contrast = "Contrast";
		public const string X = "X";
		public const string Y = "Y";
		public const string Vert = "Vert";
		public const string Horz = "Horz";
		List<LayerGenerationOptions> layerOptions;
		public List<LayerGenerationOptions> LayerOptions
		{
			get
			{
				if (layerOptions == null)
				{
					Load();
				}
				return layerOptions;
			}
			private set => layerOptions = value;
		}

		public void Load()
		{
			layerOptions = GoogleSheets.Get<LayerGenerationOptions>();
		}

		public LayerGenerationOptions FindOrCreate(string layoutName, CardImageLayer cardImageLayer)
		{
			// TODO: Be sure to save any unsaved changes on app shut down.
			LayerGenerationOptions options = Find(layoutName, cardImageLayer);
			if (options == null)
			{
				options = new LayerGenerationOptions();
				options.LayerName = cardImageLayer.LayerName;
				options.LayoutName = layoutName;
				LayerOptions.Add(options);
				GoogleSheets.SaveChanges(options);
			}
			return options;
		}

		public LayerGenerationOptions Find(string layoutName, CardImageLayer cardImageLayer)
		{
			return LayerOptions.FirstOrDefault(x => x.LayerName == cardImageLayer.LayerName && x.LayoutName == layoutName);
		}

		string GetBoundaryStr(BoundaryKind boundaryKind)
		{
			switch (boundaryKind)
			{
				case BoundaryKind.Min:
					return "Min";
				case BoundaryKind.Max:
					return "Max";
			}
			throw new Exception("Um... No!");
		}

		public void Set(string layoutName, CardImageLayer cardImageLayer, BoundaryKind boundaryKind, string key, double value)
		{
			LayerGenerationOptions options = FindOrCreate(layoutName, cardImageLayer);

			string boundaryStr = GetBoundaryStr(boundaryKind);
			string propertyName = $"{boundaryStr}{key}";
			PropertyInfo propInfo = typeof(LayerGenerationOptions).GetProperty(propertyName);
			if (propInfo == null)
				return;

			propInfo.SetValue(options, value);
			GoogleSheets.SaveChanges(options);
		}

		public bool GetSyncStretch(string layoutName, CardImageLayer cardImageLayer)
		{
			LayerGenerationOptions options = Find(layoutName, cardImageLayer);
			if (options == null)
				return true; // Default for SyncStretch
			return options.SyncStretch;
		}

		public void SetSyncStretch(string style, CardImageLayer cardImageLayer, bool syncStretch)
		{
			LayerGenerationOptions options = FindOrCreate(style, cardImageLayer);
			if (options == null)
				return;
			options.SyncStretch = syncStretch;
			GoogleSheets.SaveChanges(options);
		}

		public CardGenerationOptions()
		{

		}
	}
}
