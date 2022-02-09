using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace CardMaker
{

	[DocumentName(Constants.DocumentName_DeckData)]
	[SheetName("Random Gen Rules")]
	public class LayerGenerationOptions : TrackPropertyChanges
	{
		bool syncStretch = true;
		double minX;
		double minY;
		double maxX;
		double maxY;
		double maxHorz;
		double maxVert;
		double minHorz;
		double minVert;
		double maxSat = 0;
		double minLight = 0;
		double maxLight = 0;
		double minContrast = 0;
		double maxContrast = 0;
		double minSat = 0;
		double maxHue = 0;
		double minHue = 0;
		double chancesVisible = 1;
		string layerName;
		string layoutName;

		[Column]
		[Indexer]
		public string LayoutName
		{
			get => layoutName;
			set
			{
				if (layoutName == value)
					return;
				layoutName = value;
				OnPropertyChanged();
			}
		}


		[Column]
		[Indexer]
		public string LayerName
		{
			get => layerName;
			set
			{
				if (layerName == value)
					return;
				layerName = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double ChancesVisible
		{
			get => chancesVisible;
			set
			{
				if (chancesVisible == value)
					return;
				chancesVisible = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinHue
		{
			get => minHue;
			set
			{
				if (minHue == value)
					return;
				minHue = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double MaxHue
		{
			get => maxHue;
			set
			{
				if (maxHue == value)
					return;
				maxHue = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double MinSat
		{
			get => minSat;
			set
			{
				if (minSat == value)
					return;
				minSat = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxSat
		{
			get => maxSat;
			set
			{
				if (maxSat == value)
					return;
				maxSat = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double MinLight
		{
			get => minLight;
			set
			{
				if (minLight == value)
					return;
				minLight = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxLight
		{
			get => maxLight;
			set
			{
				if (maxLight == value)
					return;
				maxLight = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinContrast
		{
			get => minContrast;
			set
			{
				if (minContrast == value)
					return;
				minContrast = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxContrast
		{
			get => maxContrast;
			set
			{
				if (maxContrast == value)
					return;
				maxContrast = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinX
		{
			get => minX;
			set
			{
				if (minX == value)
					return;
				minX = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxX
		{
			get => maxX;
			set
			{
				if (maxX == value)
					return;
				maxX = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinY
		{
			get => minY;
			set
			{
				if (minY == value)
					return;
				minY = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxY
		{
			get => maxY;
			set
			{
				if (maxY == value)
					return;
				maxY = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinHorz
		{
			get => minHorz;
			set
			{
				if (minHorz == value)
					return;
				minHorz = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxHorz
		{
			get => maxHorz;
			set
			{
				if (maxHorz == value)
					return;
				maxHorz = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MinVert
		{
			get => minVert;
			set
			{
				if (minVert == value)
					return;
				minVert = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public double MaxVert
		{
			get => maxVert;
			set
			{
				if (maxVert == value)
					return;
				maxVert = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public bool SyncStretch
		{
			get => syncStretch;
			set
			{
				if (syncStretch == value)
					return;
				syncStretch = value;
				OnPropertyChanged();
			}
		}
		
		public LayerGenerationOptions()
		{

		}
	}
}
