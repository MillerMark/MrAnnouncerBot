using DndCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DndUI
{
	/// <summary>
	/// Interaction logic for ItemBuilder.xaml
	/// </summary>
	public partial class ItemBuilder : UserControl, INotifyPropertyChanged
	{
		int modsCreated = 0;
		public static readonly DependencyProperty WeaponCategoryProperty = DependencyProperty.Register("WeaponCategory", typeof(WeaponCategories), typeof(ItemBuilder), new FrameworkPropertyMetadata(WeaponCategories.None, new PropertyChangedCallback(OnWeaponCategoryChanged), new CoerceValueCallback(OnCoerceWeaponCategory)));
		bool loading;

		public WeaponCategories WeaponCategory
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (WeaponCategories)GetValue(WeaponCategoryProperty);
			}
			set
			{
				SetValue(WeaponCategoryProperty, value);
			}
		}

		public ItemBuilder()
		{
			InitializeComponent();
			WeaponCategory = WeaponCategories.None;
		}

		private static object OnCoerceWeaponCategory(DependencyObject o, object value)
		{
			ItemBuilder itemBuilder = o as ItemBuilder;
			if (itemBuilder != null)
				return itemBuilder.OnCoerceWeaponCategory((WeaponCategories)value);
			else
				return value;
		}

		private static void OnWeaponCategoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ItemBuilder itemBuilder = o as ItemBuilder;
			if (itemBuilder != null)
				itemBuilder.OnWeaponCategoryChanged((WeaponCategories)e.OldValue, (WeaponCategories)e.NewValue);
		}

		protected virtual WeaponCategories OnCoerceWeaponCategory(WeaponCategories value)
		{
			// TODO: Keep the proposed value within the desired range.
			return value;
		}

		protected virtual void OnWeaponCategoryChanged(WeaponCategories oldValue, WeaponCategories newValue)
		{
			// TODO: Add your property changed side-effects. Descendants can override as well.
		}
		private void LbModsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is EditableListBox editableListBox)
				if (editableListBox.SelectedItem is ModViewModel entry)
					if (modBuilder != null)
					{
						loading = true;
						try
						{
							modBuilder.LoadFromItem(entry);
							modBuilder.Visibility = Visibility.Visible;
						}
						finally
						{
							loading = false;
						}
					}
		}

		private void LbModsList_ClickAdd(object sender, RoutedEventArgs e)
		{
			if (lbModsList.ItemsSource is ObservableCollection<ModViewModel> itemsSource)
			{
				itemsSource.Add(new ModViewModel("New Mod" + modsCreated));
				modsCreated++;
			}
		}

		private void LbModsList_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void LbAttackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void LbAttackList_ClickAdd(object sender, RoutedEventArgs e)
		{

		}

		private void LbAttackList_Loaded(object sender, RoutedEventArgs e)
		{

		}
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void TbxDescription_TextChanged(object sender, TextChangedEventArgs e)
		{
			OnPropertyChanged("Description");
		}

		private void AnyControlChanged(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("AnyControl");
		}

		private void AnyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged("AnyProperty");
		}
		public void SaveToItem(ItemViewModel entry, string propertyName)
		{
			entry.adamantine = ckbAdamantine.IsChecked ?? false;
			entry.consumable = ckbConsumable.IsChecked ?? false;
			entry.equipped = ckbEquipped.IsChecked ?? false;
			entry.magic = ckbMagic.IsChecked ?? false;
			entry.silvered = ckbSilvered.IsChecked ?? false;

			entry.costValue = nedCostValue.ValueAsDouble;
			entry.count = (int)nedQuantity.ValueAsDouble;
			entry.weight = (int)nedWeight.ValueAsDouble;
			entry.minStrengthToCarry = (int)nedMinStrengthToCarry.ValueAsDouble;
			entry.equipTime = new DndTimeSpan(tseEquipTime.TimeMeasure, (int)tseEquipTime.Amount);
			entry.unequipTime = new DndTimeSpan(tseUnequipTime.TimeMeasure, (int)tseUnequipTime.Amount);
			entry.description = tbxDescription.Text;
			if (lbModsList.ItemsSource is ObservableCollection<ModViewModel> mods)
				entry.mods = mods;

			// entry.acquiredEffects
			// entry.equippedEffects
			// entry.consumedEffects;
			// entry.discardedEffects
			// entry.unequippedEffects

			// entry.mods
			// entry.cursesBlessingsDiseases

			/* 
			 * TODO:
				Dexterity Modifier Limit: [2]
				ModBuilder needs a PropertyChanged event.
				Mods don't seem to have names (on Delete?)
				Attack Filter initial value?
				Equip/Unequip times - saving Combobox Values? */
		}

		public void LoadFromItem(ItemViewModel entry)
		{
			ckbAdamantine.IsChecked = entry.adamantine;
			ckbConsumable.IsChecked = entry.consumable;
			ckbEquipped.IsChecked = entry.equipped;
			ckbMagic.IsChecked = entry.magic;
			ckbSilvered.IsChecked = entry.silvered;

			nedCostValue.ValueAsDouble = entry.costValue;
			nedQuantity.ValueAsDouble = entry.count;
			nedWeight.ValueAsDouble = entry.weight;
			nedMinStrengthToCarry.ValueAsDouble = entry.minStrengthToCarry;
			tseEquipTime.TimeMeasure = entry.equipTime.TimeMeasure;
			tseEquipTime.Amount = entry.equipTime.Count;
			tseUnequipTime.TimeMeasure = entry.unequipTime.TimeMeasure;
			tseUnequipTime.Amount = entry.unequipTime.Count;
			tbxDescription.Text = entry.description;

			lbModsList.ItemsSource = entry.mods;
			modBuilder.Visibility = Visibility.Collapsed;

			// entry.acquiredEffects
			// entry.equippedEffects
			// entry.consumedEffects;
			// entry.discardedEffects
			// entry.unequippedEffects

			
			// entry.cursesBlessingsDiseases
		}

		private void ModBuilder_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (lbModsList.SelectedItem is ModViewModel mod)
				if (modBuilder != null)
					modBuilder.SaveToMod(mod, e.PropertyName);

			OnPropertyChanged("Mod");
		}
	}
}
