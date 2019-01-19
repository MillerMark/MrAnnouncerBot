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
	public enum StatBoxState
	{
		DisplayOnly,
		Focused,
		Editing
	}

	/// <summary>
	/// Interaction logic for StatBox.xaml
	/// </summary>
	public partial class StatBox : UserControl
	{
		//TextBox txtEdit;
		public static readonly DependencyProperty StatBoxStateProperty = DependencyProperty.Register(
			nameof(StatBoxState), typeof(StatBoxState), typeof(StatBox), 
			new FrameworkPropertyMetadata(StatBoxState.DisplayOnly, new PropertyChangedCallback(OnStatBoxStateChanged)));

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			nameof(Text), typeof(string), typeof(StatBox), 
			new FrameworkPropertyMetadata("stat", new PropertyChangedCallback(OnStatBoxTextChanged)));

		public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
			nameof(TextAlignment), typeof(TextAlignment), typeof(StatBox), 
			new FrameworkPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnStatBoxTextAlignmentChanged)));
		

		static StatBox()
		{
			// Change defaults for inherited dependency properties...
			HorizontalAlignmentProperty.OverrideMetadata(typeof(StatBox), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			VerticalAlignmentProperty.OverrideMetadata(typeof(StatBox), new FrameworkPropertyMetadata(VerticalAlignment.Top));
		}

		//TextBox txtEdit;
		public StatBox()
		{
			InitializeComponent();
			txtDisplay.TextAlignment = TextAlignment.Center;
			txtEdit.TextAlignment = TextAlignment.Center;
			TextAlignment = TextAlignment.Center;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public StatBoxState StatBoxState
		{
			get => (StatBoxState)GetValue(StatBoxStateProperty);
			set => SetValue(StatBoxStateProperty, value);
		}

		public TextAlignment TextAlignment
		{
			get => (TextAlignment)GetValue(TextAlignmentProperty);
			set => SetValue(TextAlignmentProperty, value);
		}

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		void OnTextChanged(string oldValue, string newValue)
		{
			txtDisplay.Text = newValue;
			txtEdit.Text = newValue;
		}

		private static void OnStatBoxTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			StatBox statBox = o as StatBox;
			if (statBox != null)
				statBox.OnTextChanged((string)e.OldValue, (string)e.NewValue);
		}

		void OnTextAlignmentChanged(TextAlignment oldValue, TextAlignment newValue)
		{
			txtDisplay.TextAlignment = newValue;
			txtEdit.TextAlignment = newValue;
		}

		private static void OnStatBoxTextAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			StatBox statBox = o as StatBox;
			if (statBox != null)
				statBox.OnTextAlignmentChanged((TextAlignment)e.OldValue, (TextAlignment)e.NewValue);
		}

		private static void OnStatBoxStateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			StatBox statBox = o as StatBox;
			if (statBox != null)
				statBox.OnStatBoxStateChanged((StatBoxState)e.OldValue, (StatBoxState)e.NewValue);
		}

		protected virtual void OnStatBoxStateChanged(StatBoxState oldValue, StatBoxState newValue)
		{
			switch (StatBoxState)
			{
				case StatBoxState.DisplayOnly:
					txtDisplay.Background = Brushes.Transparent;
					txtDisplay.Visibility = Visibility.Visible;
					txtEdit.Visibility = Visibility.Hidden;
					break;
				case StatBoxState.Focused:
					txtDisplay.Background = Brushes.LightGoldenrodYellow;
					txtDisplay.Visibility = Visibility.Visible;
					txtEdit.Visibility = Visibility.Hidden;
					break;
				case StatBoxState.Editing:
					txtDisplay.Visibility = Visibility.Hidden;
					txtEdit.Visibility = Visibility.Visible;
					if (oldValue == StatBoxState.Focused)
						txtEdit.Focus();
					break;
			}
		}
		private void TxtDisplay_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (StatBoxState == StatBoxState.DisplayOnly)
				StatBoxState = StatBoxState.Focused;
			else if (StatBoxState == StatBoxState.Focused)
				StatBoxState = StatBoxState.Editing;
			
		}

		private void TxtEdit_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				StatBoxState = StatBoxState.Focused;
		}

		private void TxtDisplay_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				StatBoxState = StatBoxState.Editing;
		}

		private void TxtEdit_TextChanged(object sender, TextChangedEventArgs e)
		{
			Text = txtEdit.Text;
		}
	}
}
