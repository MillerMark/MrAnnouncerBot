using DndCore;
using System;
using System.Collections.Generic;
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
	/// Interaction logic for ModBuilder.xaml
	/// </summary>
	public partial class ModBuilder : UserControl, INotifyPropertyChanged
	{
		//public static readonly DependencyProperty PlayerPropertyIndexProperty = DependencyProperty.Register("PlayerPropertyIndex", typeof(int), typeof(ModBuilder), new FrameworkPropertyMetadata(0));

		//public int PlayerPropertyIndex
		//{
		//	// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
		//	get
		//	{
		//		return (int)GetValue(PlayerPropertyIndexProperty);
		//	}
		//	set
		//	{
		//		SetValue(PlayerPropertyIndexProperty, value);
		//	}
		//}

		public ModBuilder()
		{
			InitializeComponent();
		}

		void SelectCombo(ComboBox comboBox, object targetEntry)
		{
			if (targetEntry == null)
			{
				comboBox.SelectedIndex = -1;
				return;
			}
			for (int i = 0; i < comboBox.Items.Count; i++)
			{
				object item = comboBox.Items[i];
				if (item.Equals(targetEntry))
				{
					comboBox.SelectedIndex = i;
					return;
				}
			}
		}
		public void LoadFromItem(ModViewModel entry)
		{
			tbxAbsolute.Text = entry.Absolute.ToString();
			lbAddModifier.SelectedValue = entry.ModAddAbilityModifier.Value;
			lbConditions.SelectedValue = entry.ModConditions.Value;
			lbDamageEdit.SelectedValue = entry.DamageTypeFilter.DamageType.Value;
			lbAttackFilter.SelectedValue = entry.DamageTypeFilter.AttackKind.Value;
			lbModType.SelectedValue = entry.ModType.Value;
			tbxMultiplier.Text = entry.Multiplier.ToString();
			tbxOffset.Text = entry.Offset.ToString();
			tmRepeats.TimeMeasure = entry.Repeats.TimeMeasure;
			tmRepeats.Amount = entry.Repeats.Count;
			ckRequiresConsumed.IsChecked = entry.RequiresConsumption;
			ckRequiresEquipped.IsChecked = entry.RequiresEquipped;
			SelectCombo(cbTargetName, entry.TargetName);
			SelectCombo(cbVantageSkillFilter, entry.VantageSkillFilter);
			ckbAddsAdvantage.IsChecked = entry.AddsAdvantage;
			ckbAddsDisadvantage.IsChecked = entry.AddsDisadvantage;
		}
		private void BtnTest_Click(object sender, RoutedEventArgs e)
		{
			ModViewModel modViewModel = (ModViewModel)TryFindResource("vm");
			if ((ModType)modViewModel.ModType.Value == ModType.condition)
				modViewModel.ModType.Value = ModType.incomingAttack;
			else
				modViewModel.ModType.Value = ModType.condition;

			if ((Conditions)modViewModel.ModConditions.Value == Conditions.Charmed)
				modViewModel.ModConditions.Value = Conditions.Invisible;
			else 
				modViewModel.ModConditions.Value = Conditions.Charmed;

			modViewModel.RequiresConsumption = !modViewModel.RequiresConsumption;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void AnyPropertyChanged(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("AnyProperty");
		}

		private void AnyTextChanged(object sender, TextChangedEventArgs e)
		{
			OnPropertyChanged("AnyText");
		}

		private void CkbAddsAdvantage_Checked(object sender, RoutedEventArgs e)
		{
			ckbAddsDisadvantage.IsChecked = false;
			OnPropertyChanged("AddsAdvantage");
		}

		private void CkbAddsDisadvantage_Checked(object sender, RoutedEventArgs e)
		{
			ckbAddsAdvantage.IsChecked = false;
			OnPropertyChanged("AddsDisadvantage");
		}

		private void TmRepeats_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged("Repeats");
		}

		private void CbTargetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnPropertyChanged("TargetName");
		}

		private void CbVantageSkillFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnPropertyChanged("VantageSkillFilter");
		}
		public void SaveToMod(ModViewModel mod, string propertyName)
		{
			ModViewModel vm = FindResource("vm") as ModViewModel;
			if (vm == null)
				return;
			if (double.TryParse(tbxAbsolute.Text, out double result))
				mod.Absolute = result;
			//mod.AddModifier.Value = lbAddModifier.
			mod.AddsAdvantage = ckbAddsAdvantage.IsChecked ?? false;
			mod.AddsDisadvantage = ckbAddsDisadvantage.IsChecked ?? false;
			if (lbConditions.ItemsSource is ModViewModel)
				mod.ModConditions.Value = (lbConditions.ItemsSource as ModViewModel).Conditions;
			mod.DamageTypeFilter.AttackKind = vm.DamageTypeFilter.AttackKind;
			mod.DamageTypeFilter.DamageType = vm.DamageTypeFilter.DamageType;
			mod.ModifierLimit = vm.ModifierLimit;
			mod.ModType = vm.ModType;
			mod.Multiplier = vm.Multiplier;
			//mod.Name = vm.Name;
			mod.Offset = vm.Offset;
			mod.Repeats = new DndTimeSpan(tmRepeats.TimeMeasure, (int)tmRepeats.Amount);
			mod.RequiresConsumption = vm.RequiresConsumption;
			mod.RequiresEquipped = vm.RequiresEquipped;
			mod.TargetName = vm.TargetName;
			mod.VantageSkillFilter = vm.VantageSkillFilter;
		}
	}
}
