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
using DndUI;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for EffectsList.xaml
	/// </summary>
	public partial class EffectsList : UserControl
	{
		ObservableCollection<EffectEntry> effects;
		int entriesCreated = 0;
		bool loading;
		public EffectsList()
		{
			InitializeComponent();
		}

		private void LbEffectsComposite_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is EditableListBox editableListBox)
				if (editableListBox.SelectedItem is EffectEntry entry)
				{
					if (effectBuilder != null)
					{
						loading = true;
						try
						{
							effectBuilder.LoadFromItem(entry);
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

		private void EffectBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// TODO: Set isDirty to true inside lbEffectsList.
			if (loading)
				return;

			lbEffectsList.SetDirty();

			if (lbEffectsList.SelectedItem is EffectEntry effectEntry)
				if (effectBuilder != null)
					effectBuilder.SaveToItem(effectEntry, e.PropertyName);
		}

		private void LbEffectsList_ClickAdd(object sender, RoutedEventArgs e)
		{
			effects.Add(new EffectEntry(EffectKind.Animation, "New Effect" + entriesCreated));
			entriesCreated++;
		}

		private void LbEffectsList_Loaded(object sender, RoutedEventArgs e)
		 {
			effects = lbEffectsList.LoadEntries<EffectEntry>();
		}
	}
}
