using Imaging;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Windows;
using System.Runtime.CompilerServices;

namespace CardMaker
{
	public class CardImageLayer : DependencyObject, INotifyPropertyChanged
	{
		public double X
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(XProperty);
			}
			set
			{
				SetValue(XProperty, value);
			}
		}

		public double Y
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(YProperty);
			}
			set
			{
				SetValue(YProperty, value);
			}
		}

		LayerDetails details;
		public LayerDetails Details
		{
			get => details;
			set
			{
				if (details == value)
					return;
				details = value;
				if (details != null)
					details.PropertyChanged += Details_PropertyChanged;
			}
		}

		public bool OffsetChanged => Details.OffsetX != 0 || Details.OffsetY != 0;
		public bool HueChanged => Details.Hue != 0;
		public bool SatChanged => Details.Sat != 0;
		public bool LightChanged => Details.Light != 0;
		public bool ContrastChanged => Details.Contrast != 0;
		public bool ScaleChanged => Details.ScaleX != 1 || Details.ScaleY != 1;

		private void Details_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Details.OffsetX))
			{
				X = StartX + Details.OffsetX;
				OnPropertyChanged(nameof(OffsetChanged));
			}
			else if (e.PropertyName == nameof(Details.OffsetY))
			{
				Y = StartY + Details.OffsetY;
				OnPropertyChanged(nameof(OffsetChanged));
			}
			else if (e.PropertyName == nameof(Details.Hue))
				OnPropertyChanged(nameof(HueChanged));
			else if (e.PropertyName == nameof(Details.Light))
				OnPropertyChanged(nameof(LightChanged));
			else if (e.PropertyName == nameof(Details.Sat))
				OnPropertyChanged(nameof(SatChanged));
			else if (e.PropertyName == nameof(Details.Contrast))
				OnPropertyChanged(nameof(ContrastChanged));
			else if (e.PropertyName == nameof(Details.ScaleX) || e.PropertyName == nameof(Details.ScaleY))
				OnPropertyChanged(nameof(ScaleChanged));
		}

		void OnPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public string PngFile { get; set; }
		public int Index { get; set; }
		public double StartX { get; set; }
		public double StartY { get; set; }


		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(null));

		public event PropertyChangedEventHandler PropertyChanged;



		public string LayerName { get; private set; }
		public CardImageLayer()
		{

		}

		public override string ToString()
		{
			return LayerName;
		}

		public Image CreateImage()
		{
			Image image = ImageUtils.CreateImage(0, 0, 0, 0, 0, 1, 1, PngFile);
			if (image.Source is System.Windows.Media.Imaging.BitmapSource bitmapSource)
			{
				image.Width = bitmapSource.PixelWidth;
				image.Height = bitmapSource.PixelHeight;
			}
			else
			{
				image.Width = 280;
				image.Height = 424;
			}
			X = StartX + Details.OffsetX;
			Y = StartY + Details.OffsetY;
			return image;
		}

		public CardImageLayer(string pngFile)
		{
			PngFile = pngFile;
			LayerName = Path.GetFileNameWithoutExtension(pngFile);
			int closeBracketPos = LayerName.IndexOf("]");
			if (closeBracketPos > 0)
			{
				string indexStr = LayerName.Substring(1, closeBracketPos - 1);
				LayerName = LayerName.Substring(closeBracketPos + 1).Trim();
				if (int.TryParse(indexStr, out int index))
					Index = index;
			}
			int openParenPos = LayerName.IndexOf("(");
			if (openParenPos > 0)
			{
				string posStr = LayerName.Substring(openParenPos + 1).Trim(')');
				LayerName = LayerName.Substring(0, openParenPos).Trim();
				string[] parts = posStr.Split(',');
				if (double.TryParse(parts[0], out double x))
					StartX = x;
				if (double.TryParse(parts[1], out double y))
					StartY = y;
			}
		}
	}
}
