using Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for FrmColorPicker.xaml
	/// </summary>
	public partial class FrmColorPicker : UserControl
	{
		public static readonly RoutedEvent ColorChangedEvent = System.Windows.EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FrmColorPicker));
		public static readonly RoutedEvent PreviewColorChangedEvent = System.Windows.EventManager.RegisterRoutedEvent("PreviewColorChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(FrmColorPicker));

		public event RoutedEventHandler ColorChanged
		{
			add { AddHandler(ColorChangedEvent, value); }
			remove { RemoveHandler(ColorChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewColorChanged
		{
			add { AddHandler(PreviewColorChangedEvent, value); }
			remove { RemoveHandler(PreviewColorChangedEvent, value); }
		}

		public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(FrmColorPicker), new FrameworkPropertyMetadata(0d, OnHueChanged));
		public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(FrmColorPicker), new FrameworkPropertyMetadata(100d, OnSaturationChanged));
		public static readonly DependencyProperty LightnessProperty = DependencyProperty.Register("Lightness", typeof(double), typeof(FrmColorPicker), new FrameworkPropertyMetadata(100d, OnLightnessChanged));

		public double Hue
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(HueProperty);
			}
			set
			{
				SetValue(HueProperty, value);
			}
		}

		private static void OnHueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			FrmColorPicker frmColorPicker = o as FrmColorPicker;
			if (frmColorPicker != null)
				frmColorPicker.OnHueChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnHueChanged(double oldValue, double newValue)
		{
			sldHue.Value = newValue;
		}
		
		public double Saturation
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(SaturationProperty);
			}
			set
			{
				SetValue(SaturationProperty, value);
			}
		}

		private static void OnSaturationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			FrmColorPicker frmColorPicker = o as FrmColorPicker;
			if (frmColorPicker != null)
				frmColorPicker.OnSaturationChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnSaturationChanged(double oldValue, double newValue)
		{
			sldSaturation.Value = newValue;
		}
		
		public double Lightness
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(LightnessProperty);
			}
			set
			{
				SetValue(LightnessProperty, value);
			}
		}

		private static void OnLightnessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			FrmColorPicker frmColorPicker = o as FrmColorPicker;
			if (frmColorPicker != null)
				frmColorPicker.OnLightnessChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnLightnessChanged(double oldValue, double newValue)
		{
			sldLightness.Value = newValue;
		}

		protected virtual void OnColorChanged()
		{
			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewColorChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(ColorChangedEvent);
			RaiseEvent(eventArgs);
		}

		public FrmColorPicker()
		{
			InitializeComponent();
		}

		void UpdateColor()
		{
			Hue = sldHue.Value;
			Saturation = sldSaturation.Value;
			Lightness = sldLightness.Value;
			HueSatLight hueSatLight = new HueSatLight(Hue / 360.0, Saturation / 100, Lightness / 100);
			rctSample.Fill = new SolidColorBrush(hueSatLight.AsRGB);
			tbHue.Text = Math.Round(sldHue.Value).ToString();
			tbSaturation.Text = Math.Round(sldSaturation.Value).ToString();
			tbLightness.Text = Math.Round(sldLightness.Value).ToString();
		}

		private bool dragStarted = false;

		private void DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			dragStarted = true;
		}

		private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			UpdateColor();
			OnColorChanged();
			dragStarted = false;
		}

		private void ColorSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded)
			{
				UpdateColor();
				if (!dragStarted)
					OnColorChanged();
			}
		}
	}
}
