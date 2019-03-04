using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DndUI
{
	public class NumTextBox : TextBox
	{
		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(NumTextBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChanged)));


		public bool IsActive
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(IsActiveProperty);
			}
			set
			{
				SetValue(IsActiveProperty, value);
			}
		}


		public NumTextBox() : base()
		{
			PreviewMouseDown += NumTextBox_PreviewMouseDown;
			Loaded += NumTextBox_Loaded;
			PreviewMouseMove += NumTextBox_PreviewMouseMove;
			PreviewMouseUp += NumTextBox_PreviewMouseUp;
			//LostFocus += NumTextBox_LostFocus;
			//Cursor = Cursors.ScrollWE;
		}

		private void NumTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			SetAppearanceBasedOnState(IsActive);
			TextBox textBox = this.FindVisualChild<TextBox>("TextBox");
			if (textBox != null)
				textBox.LostFocus += TextBox_LostFocus;
			//VisualStateManager.GoToState(this, "CanDrag", false);
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//IsActive = false;
		}

		private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			NumTextBox numTextBox = o as NumTextBox;
			if (numTextBox != null)
				numTextBox.OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
		{
			SetAppearanceBasedOnState(newValue);
		}

		private void SetAppearanceBasedOnState(bool active)
		{
			const bool useTransitions = false;
			if (active)
			{
				VisualStateManager.GoToState(this, "CanEdit", useTransitions);
				TextBox textBox = this.FindVisualChild<TextBox>("TextBox");
				if (textBox != null)
					textBox.Focus();
			}
			else
			{
				VisualStateManager.GoToState(this, "CanDrag", useTransitions);
			}
		}
		private void NumTextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (IsActive)
				return;

			string text = Text.Trim();
			isPercent = text.EndsWith("%");
			if (isPercent)
				text = text.Substring(0, text.Length - 1);
			if (double.TryParse(text, out startValue))
			{
				haveMoved = false;
				CaptureMouse();
				mouseDownX = e.GetPosition(this).X;
			}
		}

		bool haveMoved;
		double mouseDownX;
		double startValue;
		bool isPercent;

		private void NumTextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (IsMouseCaptured)
			{
				ReleaseMouseCapture();
				if (haveMoved)
				{
					haveMoved = false;
					return;
				}
			}

			if (IsActive)
				return;

			IsActive = true;

			TextBox textBox = this.FindVisualChild<TextBox>("TextBox");
			if (textBox != null)
			{
				Focus();
				//if (textBox.InputHitTest(e.GetPosition(this)) != null)
				//MouseButtonEventArgs mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				//mouseButtonEventArgs.RoutedEvent = TextBox.PreviewMouseDownEvent;
				//textBox.RaiseEvent(mouseButtonEventArgs);

				//mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				//mouseButtonEventArgs.RoutedEvent = TextBox.MouseDownEvent;
				//textBox.RaiseEvent(mouseButtonEventArgs);

				//mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				//mouseButtonEventArgs.RoutedEvent = TextBox.MouseUpEvent;
				//textBox.RaiseEvent(mouseButtonEventArgs);

				//mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				//mouseButtonEventArgs.RoutedEvent = TextBox.PreviewMouseUpEvent;
				//textBox.RaiseEvent(mouseButtonEventArgs);

				//textBox.Focus();
				textBox.SelectAll();
				e.Handled = true;
			}
			//textBox.mousedown();
		}

		private void NumTextBox_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (IsActive || (!IsMouseCaptured))
				return;

			double x = e.GetPosition(this).X;
			double distanceMoved = x - mouseDownX;
			const double minDistanceForMove = 10;
			if (!haveMoved && Math.Abs(distanceMoved) > minDistanceForMove)
				haveMoved = true;

			if (haveMoved)
			{
				const double pixelsPerUnit = 2;
				double valueAdjust = Math.Round(Math.Sign(distanceMoved) * (Math.Abs(distanceMoved) - minDistanceForMove) / pixelsPerUnit);

				string suffix = string.Empty;
				if (isPercent)
					suffix = "%";
				Text = (valueAdjust + startValue).ToString() + suffix;
			}
		}
	}
}
