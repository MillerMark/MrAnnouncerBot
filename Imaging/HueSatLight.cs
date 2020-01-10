using System;
using System.Windows.Media;

namespace Imaging
{
	public class HueSatLight
	{
		#region private fields...
		private double alpha = 1.0;
		private double hue;
		private double saturation;
		private double lightness;
		#endregion

		// constructors...
		#region HueSatLight
		public HueSatLight()
		{
			hue = 0;
			saturation = 0;
			lightness = 0;
		}
		#endregion
		#region HueSatLight(Color aColor)
		public HueSatLight(Color color)
		{
			Hue = color.GetHue() / 360.0; // Convert from range of 0-360 to 0.0-1.0.
			Lightness = color.GetBrightness();
			Saturation = color.GetSaturation();
		}

		public HueSatLight(double hue, double saturation, double lightness)
		{
			this.hue = hue;
			this.saturation = saturation;
			this.lightness = lightness;
		}

		public HueSatLight(string htmlColorStr) : this((Color)ColorConverter.ConvertFromString(htmlColorStr))
		{
		}
		#endregion

		// public methods...
		#region AsRGB
		/// <summary> 
		/// Returns an equivalent Color instance.
		/// </summary> 
		public Color AsRGB
		{
			get
			{
				double red = 0;
				double green = 0;
				double blue = 0;
				if (Lightness == 0)
				{
					// Completely black.
					red = green = blue = 0;
				}
				else
				{
					if (Saturation == 0)
					{
						// No color; somewhere on the gray scale.
						red = green = blue = Lightness;
					}
					else
					{
						double temp2 = ((Lightness <= 0.5) ? Lightness * (1.0 + Saturation) : Lightness + Saturation - (Lightness * Saturation));
						double temp1 = 2.0 * Lightness - temp2;

						double[] hueShift = new double[] { Hue + 1.0 / 3.0, Hue, Hue - 1.0 / 3.0 };
						double[] colorArray = new double[] { 0, 0, 0 };

						for (int i = 0; i < 3; i++)
						{
							if (hueShift[i] < 0)
								hueShift[i] += 1.0;

							if (hueShift[i] > 1)
								hueShift[i] -= 1.0;

							if (6.0 * hueShift[i] < 1.0)
								colorArray[i] = temp1 + (temp2 - temp1) * hueShift[i] * 6.0;
							else if (2.0 * hueShift[i] < 1.0)
								colorArray[i] = temp2;
							else if (3.0 * hueShift[i] < 2.0)
								colorArray[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - hueShift[i]) * 6.0);
							else
								colorArray[i] = temp1;
						}
						red = colorArray[0];
						green = colorArray[1];
						blue = colorArray[2];
					}
				}
				return Color.FromArgb((byte)(alpha * 255.0), (byte)(255 * red), (byte)(255 * green), (byte)(255 * blue));
			}
		}
		#endregion
		#region AsGrayScale
		/// <summary>
		/// Returns an equivalent Color object converted to gray scale.
		/// </summary>
		public Color AsGrayScale
		{
			get
			{
				Color color = AsRGB;
				//byte grayScaleLevel = (byte)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
				//byte grayScaleLevel = (byte)Math.Sqrt(0.299 * color.R * color.R + 0.587 * color.G * color.G + 0.114 * color.B * color.B);  // http://alienryderflex.com/hsp.html
				byte grayScaleLevel = (byte)Math.Sqrt(0.241 * color.R * color.R + 0.691 * color.G * color.G + 0.068 * color.B * color.B);   // http://www.purplesquirrels.com.au/2012/03/as3-calculate-the-perceived-brightness-of-a-colour/ and http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx

				return Color.FromRgb(grayScaleLevel, grayScaleLevel, grayScaleLevel);
			}
		}
		#endregion
		#region GetRelativeLuminance
		/// <summary>
		/// Returns an the relative luminance (https://www.w3.org/TR/WCAG20/#relativeluminancedef) of this HueSatLight instance.
		/// </summary>
		public double GetRelativeLuminance()
		{
			Color color = AsRGB;
			double RsRGB = color.R / 255.0;
			double GsRGB = color.G / 255.0;
			double BsRGB = color.B / 255.0;

			double R;
			double G;
			double B;

			if (RsRGB <= 0.03928)
				R = RsRGB / 12.92;
			else
				R = Math.Pow(((RsRGB + 0.055) / 1.055), 2.4);

			if (GsRGB <= 0.03928)
				G = GsRGB / 12.92;
			else
				G = Math.Pow(((GsRGB + 0.055) / 1.055), 2.4);

			
			if (BsRGB <= 0.03928)
				B = BsRGB / 12.92;
			else
				B = Math.Pow(((BsRGB + 0.055) / 1.055), 2.4);

			double luminance = 0.2126 * R + 0.7152 * G + 0.0722 * B;
			return luminance;
		}
		#endregion
		void HueShiftDegrees()
		{
			
		}

		// public properties...
		#region AsHtml
		public string AsHtml
		{
			get
			{
				Color color = AsRGB;
				return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
			}
		}
		#endregion

		#region IsLight
		/// <summary>
		/// Returns true if this color is brighter than medium gray.
		/// </summary>
		public bool IsLight
		{
			get
			{
				Color grayScale = AsGrayScale;
				return (grayScale.R >= 128);
			}
		}
		#endregion
		#region IsDark
		/// <summary>
		/// Returns true if this color is darker than medium gray.
		/// </summary>
		public bool IsDark
		{
			get
			{
				Color grayScale = AsGrayScale;
				return (grayScale.R < 128);
			}
		}
		#endregion
		#region IsGrayScale
		/// <summary>
		/// Returns true if saturation is at zero.
		/// </summary>
		public bool IsGrayScale
		{
			get
			{
				return saturation == 0.0;
			}
		}
		#endregion
		#region GrayScale
		/// <summary>
		/// Returns the percentage on the gray scale.
		/// </summary>
		public float GrayScale
		{
			get
			{
				Color color = AsGrayScale;
				return color.R / 255.0f;
			}
		}
		#endregion
		#region Alpha
		public double Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				if (value < 0)
					alpha = 0;
				else if (value > 1.0)
					value = 1.0;
				else
					alpha = value;

			}
		}
		#endregion
		#region Hue
		public double Hue
		{
			get
			{
				return hue;
			}
			set
			{
				if (value > 1)
					hue = 1;
				else if (value < 0)
					hue = 0;
				else
					hue = value;
			}
		}
		#endregion
		#region Saturation
		public double Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				if (value > 1)
					saturation = 1;
				else if (value < 0)
					saturation = 0;
				else
					saturation = value;
			}
		}
		#endregion
		#region Lightness
		public double Lightness
		{
			get
			{
				return lightness;
			}
			set
			{
				if (value > 1)
					lightness = 1;
				else if (value < 0)
					lightness = 0;
				else
					lightness = value;
			}
		}
		#endregion
	}
}
