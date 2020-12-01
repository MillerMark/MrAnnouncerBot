using System;
using System.Linq;
using System.Windows.Media;
using GoogleHelper;

namespace CardMaker
{
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("LayerTextOptions")]
	public class LayerTextOptions: TrackPropertyChanges
	{
		string colorStr;
		double gridHeight;
		double gridWidth;
		double gridTop;
		double gridLeft;
		double textAngle;
		string name;

		[Column]
		[Indexer]
		public string Name
		{
			get => name;
			set
			{
				if (name == value)
					return;
				name = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double TextAngle
		{
			get => textAngle;
			set
			{
				if (textAngle == value)
					return;
				textAngle = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double GridLeft
		{
			get => gridLeft;
			set
			{
				if (gridLeft == value)
					return;
				gridLeft = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double GridTop
		{
			get => gridTop;
			set
			{
				if (gridTop == value)
					return;
				gridTop = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double GridWidth
		{
			get => gridWidth;
			set
			{
				if (gridWidth == value)
					return;
				gridWidth = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public double GridHeight
		{
			get => gridHeight;
			set
			{
				if (gridHeight == value)
					return;
				gridHeight = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public string TextColor
		{
			get => colorStr;
			set
			{
				if (colorStr == value)
					return;
				colorStr = value;
				fontBrush = null;
				OnPropertyChanged();
				OnPropertyChanged("FontBrush");
			}
		}

		Brush fontBrush;
		public Brush FontBrush
		{
			get
			{
				if (fontBrush == null)
					fontBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr));
				return fontBrush;
			}
		}

		public LayerTextOptions(string name)
		{
			Name = name;
		}

		public LayerTextOptions()
		{
		}
	}
}
