using Imaging;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Runtime.CompilerServices;

namespace CardMaker
{
	public class CardImageLayer : DependencyObject, INotifyPropertyChanged
	{
		public string Category { get; set; }
		public event EventHandler NeedToReloadImage;
		string alternateName;
		
		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d));
		public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d));
		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d));

		protected void OnNeedToReloadImage(object sender, EventArgs e)
		{
			NeedToReloadImage?.Invoke(sender, e);
		}


		public string DisplayName
		{
			get => Details.DisplayName;
			set
			{
				Details.DisplayName = value;
				OnPropertyChanged();
			}
		}


		public double Height
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(HeightProperty);
			}
			set
			{
				SetValue(HeightProperty, value);
			}
		}
		public double Width
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(WidthProperty);
			}
			set
			{
				SetValue(WidthProperty, value);
			}
		}
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
			else if (e.PropertyName == nameof(Details.ScaleX))
			{
				Width = Details.ScaleX * OriginalWidth;
				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(ScaleChanged));
			}
			else if (e.PropertyName == nameof(Details.ScaleY))
			{
				Height = Details.ScaleY * OriginalHeight;
				OnPropertyChanged(nameof(Height));
				OnPropertyChanged(nameof(ScaleChanged));
			}
			else if (e.PropertyName == nameof(Details.Hue))
			{
				OnPropertyChanged(nameof(HueChanged));
				ReloadImage();
			}
			else if (e.PropertyName == nameof(Details.Light))
			{
				OnPropertyChanged(nameof(LightChanged));
				ReloadImage();
			}
			else if (e.PropertyName == nameof(Details.Sat))
			{
				OnPropertyChanged(nameof(SatChanged));
				ReloadImage();
			}
			else if (e.PropertyName == nameof(Details.Contrast))
			{
				OnPropertyChanged(nameof(ContrastChanged));
				ReloadImage();
			}
			else
				ReloadImage();
		}

		void OnPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public string PngFile { get; set; }
		public int Index { get; set; }
		public double StartX { get; set; }
		public double StartY { get; set; }


		public event PropertyChangedEventHandler PropertyChanged;

		public double OriginalWidth { get; set; }
		public double OriginalHeight { get; set; }


		public string LayerName { get; private set; }
		public bool IsVisible
		{
			get => Details.IsVisible;
			set
			{
				if (Details.IsVisible == value)
					return;
				Details.IsVisible = value;
				PropagateVisibility();
				SetImageVisibility();
				OnPropertyChanged();
				OnPropertyChanged("IsHidden");
			}
		}

		public bool IsHidden
		{
			get => Details.IsHidden;
			set
			{
				if (Details.IsHidden == value)
					return;
				Details.IsHidden = value;
				PropagateVisibility();
				SetImageVisibility();
				OnPropertyChanged();
				OnPropertyChanged("IsVisible");
			}
		}

		private void SetImageVisibility()
		{
			if (image != null)
				if (IsVisible)
					image.Visibility = Visibility.Visible;
				else
					image.Visibility = Visibility.Collapsed;
		}

		void PropagateVisibility()
		{
			if (Alternates == null)
				return;
			foreach (CardImageLayer cardImageLayer in Alternates.CardLayers)
				cardImageLayer.IsVisible = IsVisible;
		}
		public AlternateCardImageLayers Alternates { get; set; }
		
		public string AlternateName
		{
			get => alternateName;
			set
			{
				if (alternateName == value)
					return;
				alternateName = value;
				OnPropertyChanged();
			}
		}
		

		public CardImageLayer()
		{
			
		}

		public override string ToString()
		{
			return LayerName;
		}

		Image image;
		public Image CreateImage(double overrideWidth = 0, double overrideHeight = 0)
		{
			image = ImageUtils.CreateImage(Details.Angle, Details.Hue, Details.Sat, Details.Light, Details.Contrast, 1, 1, PngFile);
			if (overrideWidth > 0 && overrideHeight > 0)
			{
				OriginalWidth = overrideWidth;
				OriginalHeight = overrideHeight;
			}
			else if (image.Source is System.Windows.Media.Imaging.BitmapSource bitmapSource)
			{
				OriginalWidth = bitmapSource.PixelWidth;
				OriginalHeight = bitmapSource.PixelHeight;
			}
			else
			{
				OriginalWidth = 280;
				OriginalHeight = 424;
			}

			image.Width = OriginalWidth * Details.ScaleX;
			image.Height = OriginalHeight * Details.ScaleY;

			X = StartX + Details.OffsetX;
			Y = StartY + Details.OffsetY;
			Width = image.Width;
			Height = image.Height;

			if (IsVisible)
				image.Visibility = Visibility.Visible;
			else
				image.Visibility = Visibility.Hidden;

			image.DataContext = this;
			image.Stretch = System.Windows.Media.Stretch.Fill;
			BindingHelper.BindProperties(this, CardImageLayer.WidthProperty, image, Image.WidthProperty);
			BindingHelper.BindProperties(this, CardImageLayer.HeightProperty, image, Image.HeightProperty);
			BindingHelper.BindToCanvasPosition(this, image);
			image.Tag = this;

			return image;
		}

		void ReloadImage()
		{
			OnNeedToReloadImage(this, EventArgs.Empty);
		}

		public void ToggleVisibility()
		{
			IsVisible = !IsVisible;
		}
		public bool ImageMatches(UIElement uIElement)
		{
			return uIElement == image;
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

		public void ReplaceImage(string fileName, Canvas canvas)
		{
			PngFile = fileName;
			
			for (int i = 0; i < canvas.Children.Count; i++)
			{
				if (canvas.Children[i] == image)
				{
					canvas.Children.RemoveAt(i);
					image = CreateImage();
					canvas.Children.Insert(i, image);
					break;
				}
			}
		}
	}
}
