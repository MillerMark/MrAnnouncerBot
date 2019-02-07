using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DHDM.User_Controls
{
	/// <summary>
	/// Interaction logic for EffectsList.xaml
	/// </summary>
	public partial class EffectsList : UserControl
	{
		ObservableCollection<EffectEntry> effects = new ObservableCollection<EffectEntry>();
		int entriesCreated = 0;
		public EffectsList()
		{
			InitializeComponent();
			lbEffectsComposite.ItemsSource = effects;
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			effects.Add(new EffectEntry(EffectKind.Animation, "New Effect" + entriesCreated));
			entriesCreated++;
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{

		}

		private void LbEffectsComposite_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox)
				if (listBox.SelectedItem is EffectEntry effectEntry)
					effectBuilder.LoadFromItem(effectEntry);
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
			TextBox txt = (TextBox)((Grid)((TextBlock)sender).Parent).Children[1];
			txt.Visibility = Visibility.Visible;
			((TextBlock)sender).Visibility = Visibility.Collapsed;
		}
	}
}
