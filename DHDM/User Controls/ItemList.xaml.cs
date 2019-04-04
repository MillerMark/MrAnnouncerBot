using DndCore;
using DndUI;
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
using TimeLineControl;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl
	{
		ObservableCollection<ItemViewModel> items;

		bool loading;
		int itemsCreated;

		public ItemList()
		{
			InitializeComponent();
		}

		private void EditableListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is EditableListBox editableListBox)
				if (editableListBox.SelectedItem is ItemViewModel entry)
					if (itemBuilder != null)
					{
						loading = true;
						try
						{
							itemBuilder.LoadFromItem(entry);
						}
						finally
						{
							loading = false;
						}
					}
		}

		private void EditableListBox_Loaded(object sender, RoutedEventArgs e)
		{
			items = lbItems.LoadEntries<ItemViewModel>();
		}

		private void EditableListBox_ClickAdd(object sender, RoutedEventArgs e)
		{
			items.Add(new ItemViewModel("New Item" + itemsCreated));
			itemsCreated++;
		}

		private void ItemBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (loading)
				return;

			lbItems.SetDirty();

			if (lbItems.SelectedItem is ItemViewModel item)
				if (itemBuilder != null)
					itemBuilder.SaveToItem(item, e.PropertyName);
		}
		public void Save()
		{
			lbItems.SaveEntries();
		}

		private void ItemBuilder_TestEffect(object sender, RoutedEffectEventArgs ea)
		{
			if (ea.TimeLineData == null)
				return;

			EffectGroup effectGroup = new EffectGroup();
			foreach (TimeLineEffect timeLineEffects in ea.TimeLineData.Entries)
			{
				Effect effect = null;

				if (timeLineEffects.Effect != null)
					effect = timeLineEffects.Effect.GetPrimaryEffect();

				if (effect != null)
				{
					effect.timeOffsetMs = (int)Math.Round(timeLineEffects.Start.TotalMilliseconds);
					effectGroup.Add(effect);
				}
			}

			string serializedObject = JsonConvert.SerializeObject(effectGroup);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}
	}
}
