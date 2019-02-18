using Newtonsoft.Json;
using System;
using DndCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DHDM.User_Controls
{
	/// <summary>
	/// Interaction logic for EffectsList.xaml
	/// </summary>
	public partial class EffectsList : UserControl
	{
		private Timer autoSaveTimer;
		ObservableCollection<EffectEntry> effects;
		int entriesCreated = 0;
		bool loading;
		public EffectsList()
		{
			InitializeComponent();
			LoadEffects();
			Loaded += (s, e) => { // only at this point the control is ready
				Window.GetWindow(this) // get the parent window
							.Closing += (s1, e1) => Disposing(); //disposing logic here
			};
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			effects.Add(new EffectEntry(EffectKind.Animation, "New Effect" + entriesCreated));
			entriesCreated++;
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void BtnDuplicate_Click(object sender, RoutedEventArgs e)
		{
			if (lbEffectsComposite.SelectedValue is EffectEntry effectEntry)
			{
				EffectEntry item = JsonConvert.DeserializeObject<EffectEntry>(JsonConvert.SerializeObject(effectEntry));
				item.Name += " - Copy";
				effects.Add(item);
			}
		}

		private void LbEffectsComposite_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			List<TextBox> list = savedNames.Keys.ToList();
			foreach (object textBox in list)
				TextBox_LostFocus(textBox, null);

			if (sender is ListBox listBox)
				if (listBox.SelectedItem is EffectEntry effectEntry)
				{
					if (effectBuilder != null)
					{
						loading = true;
						try
						{
							effectBuilder.LoadFromItem(effectEntry);
						}
						finally
						{
							loading = false;
						}
					}
				}
		}

		private void BtnTestEffect_Click(object sender, RoutedEventArgs e)
		{
			Effect activeEffect = effectBuilder.GetEffect();
			if (activeEffect == null)
				return;
			string serializedObject = JsonConvert.SerializeObject(activeEffect);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBlock tb = (TextBlock)((Grid)((TextBox)sender).Parent).Children[0];
			tb.Visibility = Visibility.Visible;
			((TextBox)sender).Visibility = Visibility.Collapsed;
		}

		private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			TextBox tb = (TextBox)((Grid)((TextBlock)sender).Parent).Children[1];
			tb.Visibility = Visibility.Visible;
			((TextBlock)sender).Visibility = Visibility.Collapsed;
			if (!savedNames.ContainsKey(tb))
				savedNames.Add(tb, tb.Text);
		}

		private void EffectBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (loading)
				return;
			if (lbEffectsComposite.SelectedItem is EffectEntry effectEntry)
			{
				//EffectBuilder effectBuilder = spControls.FindVisualChild<EffectBuilder>("effectBuilder");
				if (effectBuilder != null)
				{
					effectBuilder.SaveToItem(effectEntry, e.PropertyName);
				}
			}
		}

		Dictionary<TextBox, string> savedNames = new Dictionary<TextBox, string>();
		bool isDirty;

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (sender is TextBox tb)
				{
					tb.Visibility = Visibility.Collapsed;
					return;
				}

			if (e.Key == Key.Escape)
				if (sender is TextBox tb)
				{
					if (savedNames.ContainsKey(tb))
						tb.Text = savedNames[tb];
					tb.Visibility = Visibility.Collapsed;
				}
		}

		private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is TextBox tb)
				if ((bool)e.NewValue)
					savedNames.Add(tb, tb.Text);
				else if (savedNames.ContainsKey(tb))
				{
					tb.Visibility = Visibility.Collapsed;
					savedNames.Remove(tb);
				}
		}

		void LoadEffects()
		{
			List<EffectEntry> loadedEffects = Storage.Load<List<EffectEntry>>("AllEffects.json");
			if (loadedEffects != null)
				effects = new ObservableCollection<EffectEntry>(loadedEffects);
			else
				effects = new ObservableCollection<EffectEntry>();

			effects.CollectionChanged += Effects_CollectionChanged;
			lbEffectsComposite.ItemsSource = effects;
			isDirty = false;
		}

		private void Effects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			isDirty = true;
		}

		void SaveEffects()
		{
			if (!isDirty)
				return;
			// TODO: Only really do this if dirty/changed.
			Storage.Save("AllEffects.json", effects.ToList<EffectEntry>());
			isDirty = false;
		}

		object Disposing()
		{
			SaveEffects();
			return null;
		}
	}
}
