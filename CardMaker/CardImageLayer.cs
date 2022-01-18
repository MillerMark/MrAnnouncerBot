using Imaging;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace CardMaker
{
	public class CardImageLayer : DependencyObject, INotifyPropertyChanged
	{
		public const string SubLayerNameStr = "SubLayerName";
		Dictionary<string, int> groups = new Dictionary<string, int>();
		static Dictionary<char, string> propertyMap = new Dictionary<char, string>();
		static CardImageLayer()
		{
			propertyMap.Add('h', "Hue");
			propertyMap.Add('l', "Light");
			propertyMap.Add('s', "Sat");
			propertyMap.Add('n', SubLayerNameStr);
			propertyMap.Add('c', "Contrast");
			propertyMap.Add('x', "X");
			propertyMap.Add('y', "Y");
			propertyMap.Add('z', "ScaleX");  // Horizontal stretch.
			propertyMap.Add('v', "ScaleY");  // Vertical stretch.
			propertyMap.Add('i', "IsVisible");
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

		public string Category { get; set; }
		public event EventHandler NeedToReloadImage;
		string alternateName;

		public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnXChanged)));
		public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(CardImageLayer), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnYChanged)));
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

		private void Details_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ChangingInternally)
				return;
			if (e.PropertyName == nameof(Details.OffsetX))
			{
				CalculateX();
				OnPropertyChanged(nameof(OffsetChanged));
			}
			else if (e.PropertyName == nameof(Details.OffsetY))
			{
				CalculateY();
				OnPropertyChanged(nameof(OffsetChanged));
			}
			else if (e.PropertyName == nameof(Details.ScaleX))
			{
				Width = GetScaledWidth();
				CalculateX();
				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(ScaleChanged));
			}
			else if (e.PropertyName == nameof(Details.ScaleY))
			{
				Height = GetScaledHeight();
				CalculateY();
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


		public double GetScaledWidth()
		{
			return OriginalWidth * Details.ScaleX;
		}
		public double GetScaledHeight()
		{
			return OriginalHeight * Details.ScaleY;
		}

		public double AnchorX { get; set; } = 0.5;
		public double AnchorY { get; set; } = 0.5;

		private double StartWithAnchorX => StartX + GetAnchorOffsetX();

		private double GetAnchorOffsetX()
		{
			return (OriginalWidth - GetScaledWidth()) * AnchorX;
		}

		private double StartWithAnchorY => StartY + GetAnchorOffsetY();

		private double GetAnchorOffsetY()
		{
			return (OriginalHeight - GetScaledHeight()) * AnchorY;
		}

		private void CalculateX()
		{
			X = StartWithAnchorX + Details.OffsetX;
		}

		private void CalculateY()
		{
			Y = StartWithAnchorY + Details.OffsetY;
		}

		private void CalculateOffsetX()
		{
			Details.OffsetX = X - StartWithAnchorX;
		}

		private void CalculateOffsetY()
		{
			Details.OffsetY = Y - StartWithAnchorY;
		}

		void OnPropertyChanged([CallerMemberName] string name = "")
		{
			// This CardImageLayer PropertyChanged event is not subscribed to by the code, but used by WPF for updating data-bound controls.
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public string PngFile { get; set; }
		int index;
		public int Index
		{
			get => index;
			set
			{
				if (index == value)
					return;
				index = value;
				if (Alternates != null && Alternates.CardLayers != null && Alternates.CardLayers.Count > 0)
					foreach (CardImageLayer cardImageLayer in Alternates.CardLayers)
						cardImageLayer.Index = value;
			}
		}

		/// <summary>
		/// The left position of this layer when loaded from the file.
		/// </summary>
		public double StartX { get; set; }

		/// <summary>
		/// The top position of this layer when loaded from the file.
		/// </summary>
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

		public Dictionary<string, int> Groups { get => groups; set => groups = value; }
		public double PlaceholderX { get; set; }
		public double PlaceholderY { get; set; }
		public int PlaceholderWidth { get; set; }
		public int PlaceholderHeight { get; set; }
		public string PlaceholderFile { get; set; }

		public CardImageLayer()
		{

		}

		public override string ToString()
		{
			return LayerName;
		}

		Image image;
		int changingInternally;
		void BeginUpdate()
		{
			changingInternally++;
		}

		public bool ChangingInternally
		{
			get
			{
				return changingInternally > 0;
			}
		}

		public Image Image { get => image; }

		void EndUpdate()
		{
			changingInternally--;
		}
		public Image CreateImage(double overrideWidth = 0, double overrideHeight = 0, string fileOverride = null)
		{
			string fileName;
			if (fileOverride != null)
				fileName = fileOverride;
			else
				fileName = PngFile;

			image = ImageUtils.CreateImage(Details.Angle, Details.Hue, Details.Sat, Details.Light, Details.Contrast, 1, 1, fileName);

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

			BeginUpdate();
			try
			{
				CalculateX();
				CalculateY();
				Width = image.Width;
				Height = image.Height;
			}
			finally
			{
				EndUpdate();
			}

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
			Panel.SetZIndex(image, Index);
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
			if (!(uIElement is FrameworkElement frameworkElement))
				return false;

			if (!(frameworkElement.Tag is CardImageLayer cardImageLayer))
				return false;

			return cardImageLayer.LayerName == LayerName;
		}


		void AddPropertyLink(Card card, string propertyName, int groupNumber)
		{
			if (!groups.ContainsKey(propertyName))
				groups.Add(propertyName, groupNumber);
			card.AddPropertyLink(this, propertyName, groupNumber);
		}

		void AddPropertyLink(Card card, char propertyInitial, int groupNumber)
		{
			char propertyInitialLower = char.ToLower(propertyInitial);
			if (!propertyMap.ContainsKey(propertyInitialLower))
				return;

			AddPropertyLink(card, propertyMap[propertyInitialLower], groupNumber);
		}

		void AddPropertyLink(Card card, string link)
		{
			if (link == null || link.Length <= 1)
				return;

			string trimmedLink = link.Trim();
			char propertyInitial = trimmedLink[0];
			string groupNumberStr = trimmedLink.Substring(1);
			if (!int.TryParse(groupNumberStr, out int groupNumber))
				return;

			AddPropertyLink(card, propertyInitial, groupNumber);
		}

		public CardImageLayer(Card card, string pngFile, int indexOffset = 0)
		{
			PngFile = pngFile;
			LayerName = Path.GetFileNameWithoutExtension(pngFile);

			int closeBracketPos = LayerName.IndexOf("]");
			if (closeBracketPos > 0)
			{
				string indexStr = LayerName.Substring(1, closeBracketPos - 1);
				LayerName = LayerName.Substring(closeBracketPos + 1).Trim();
				if (int.TryParse(indexStr, out int index))
					Index = indexOffset + index;
			}
			int caretSymbolPos = LayerName.IndexOf("^");
			if (caretSymbolPos > 0)
			{
				string linkStr = LayerName.Substring(caretSymbolPos + 1).Trim();
				LayerName = LayerName.Substring(0, caretSymbolPos).Trim();
				string[] links = linkStr.Split(',');
				foreach (string link in links)
					AddPropertyLink(card, link);
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

			Details = card.GetLayerDetails(LayerName);
		}

		public Image ReplaceImage(string fileName, Canvas canvas, double overrideWidth = 0, double overrideHeight = 0)
		{
			PngFile = fileName;

			for (int i = 0; i < canvas.Children.Count; i++)
			{
				if (canvas.Children[i] == image)
				{
					canvas.Children.RemoveAt(i);
					double saveWidth = image.ActualWidth;
					double saveHeight = image.ActualHeight;
					if (overrideWidth != 0)
						saveWidth = overrideWidth;
					if (overrideHeight != 0)
						saveHeight = overrideHeight;
					double centerX = StartX + saveWidth / 2;
					double centerY = StartY + saveHeight / 2;
					image = CreateImage();
					double newWidth = Width;
					double newHeight = Height;
					Details.ScaleX *= saveWidth / newWidth;
					Details.ScaleY *= saveHeight / newHeight;

					StartX = centerX - newWidth / 2;
					StartY = centerY - newHeight / 2;

					Width = saveWidth;
					Height = saveHeight;
					CalculateX();
					CalculateY();
					canvas.Children.Insert(i, image);
					return image;
				}
			}
			return null;
		}
		public void ChangeProperty(LinkedPropertyEventArgs ea)
		{
			switch (ea.Name)
			{
				case "Hue":
					Details.Hue = ea.DoubleValue;
					break;
				case "Sat":
					Details.Sat = ea.DoubleValue;
					break;
				case "Light":
					Details.Light = ea.DoubleValue;
					break;
				case "Contrast":
					Details.Contrast = ea.DoubleValue;
					break;
				case "X":
					if (X != ea.DoubleValue)
					{
						X = ea.DoubleValue;
						CalculateOffsetX();
					}
					break;
				case "Y":
					if (Y != ea.DoubleValue)
					{
						Y = ea.DoubleValue;
						CalculateOffsetY();
					}
					break;
				case "ScaleX":
					Details.ScaleX = ea.DoubleValue;
					break;
				case "ScaleY":
					Details.ScaleY = ea.DoubleValue;
					break;
				case "IsVisible":
					IsVisible = ea.BoolValue;
					break;
			}
		}

		void OnXChanged(DependencyPropertyChangedEventArgs e)
		{
			Details.OnPossibleLinkedPropertyChanged(this, new LinkedPropertyEventArgs(X, "X", Details));
		}

		void OnYChanged(DependencyPropertyChangedEventArgs e)
		{
			Details.OnPossibleLinkedPropertyChanged(this, new LinkedPropertyEventArgs(Y, "Y", Details));
		}

		static void OnXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is CardImageLayer cardImageLayer)
				cardImageLayer.OnXChanged(e);
		}

		static void OnYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is CardImageLayer cardImageLayer)
				cardImageLayer.OnYChanged(e);
		}

		public void SetPlaceholderDetails()
		{
			if (PlaceholderHeight != 0)
				return;
			PlaceholderX = StartX;
			PlaceholderY = StartY;
			PlaceholderFile = PngFile;
			Size imageSize = ImageUtils.GetImageSize(PlaceholderFile);
			PlaceholderHeight = (int)imageSize.Height;
			PlaceholderWidth = (int)imageSize.Width;
		}

		public Image PlaceImageIntoPlaceholder()
		{
			//image = CreateImage(0, 0, PlaceholderFile);
			//StartX = PlaceholderX;
			//StartY = PlaceholderY;
			//canvas.Children.Add(image);
			//return ReplaceImage(PngFile, canvas, PlaceholderWidth, PlaceholderHeight);


			double centerX = StartX + PlaceholderWidth / 2.0;
			double centerY = StartY + PlaceholderHeight / 2.0;

			Image image = CreateImage();
			double newWidth = Width;
			double newHeight = Height;
			if (Width != OriginalWidth)
				Details.ScaleX = newWidth / OriginalWidth;

			if (Height != OriginalHeight)
				Details.ScaleY = newHeight / OriginalHeight;

			StartX = centerX - newWidth / 2.0 - GetAnchorOffsetX();
			StartY = centerY - newHeight / 2.0 - GetAnchorOffsetY();

			CalculateX();
			CalculateY();
			return image;
		}

		Random random = new Random();
		public void RandomlySetProperties(LayerGenerationOptions options)
		{
			LayerDetails.BeginUpdate();
			try
			{
				Details.Hue = Between(options.MinHue, options.MaxHue);
				Details.Sat = Between(options.MinSat, options.MaxSat);
				Details.Light = Between(options.MinLight, options.MaxLight);
				Details.Contrast = Between(options.MinContrast, options.MaxContrast);
				Details.OffsetX = Between(options.MinX, options.MaxX);
				Details.OffsetY = Between(options.MinY, options.MaxY);
			}
			finally
			{
				LayerDetails.EndUpdate();
			}
		}

		private int Between(double min, double max)
		{
			if (max <= min)
				return (int)Math.Round(min);
			return random.Next((int)Math.Round(min), (int)Math.Round(max));
		}
	}
}
