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

		bool settingInternally;
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
			string targetAsString = targetEntry.ToString();
			for (int i = 0; i < comboBox.Items.Count; i++)
			{
				object item = comboBox.Items[i];
				if (item.Equals(targetEntry) || item.ToString() == targetAsString)
				{
					comboBox.SelectedIndex = i;
					return;
				}
			}
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
			if (settingInternally)
				return;
			OnPropertyChanged("AnyProperty");
		}

		private void AnyTextChanged(object sender, TextChangedEventArgs e)
		{
			if (settingInternally)
				return;
			OnPropertyChanged("AnyText");
		}

		private void CkbAddsAdvantage_Checked(object sender, RoutedEventArgs e)
		{
			if (settingInternally)
				return;
			ckbAddsDisadvantage.IsChecked = false;
			OnPropertyChanged("AddsAdvantage");
		}

		private void CkbAddsDisadvantage_Checked(object sender, RoutedEventArgs e)
		{
			ckbAddsAdvantage.IsChecked = false;
			if (settingInternally)
				return;
			OnPropertyChanged("AddsDisadvantage");
		}

		private void TmRepeats_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (settingInternally)
				return;
			OnPropertyChanged("Repeats");
		}

		private void CbTargetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (settingInternally)
				return;
			OnPropertyChanged("TargetName");
		}

		private void CbVantageSkillFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (settingInternally)
				return;
			OnPropertyChanged("VantageSkillFilter");
		}

		double GetDouble(string text, double defaultValue = 0)
		{
			if (double.TryParse(text, out double result))
				return result;
			return defaultValue;
		}

		int GetInt(string text, int defaultValue = 0)
		{
			if (int.TryParse(text, out int result))
				return result;
			return defaultValue;
		}

		public void LoadFromMod(ModViewModel mod)
		{
			settingInternally = true;
			try
			{
				tbxAbsolute.Text = mod.Absolute.ToString();
				//lbAddModifier.SelectedValue = entry.ModAddAbilityModifier.Value;
				lbConditions.SelectedValue = mod.ModConditions.Value;
				CheckEnumList.SetValue(lbDamageEdit.ItemsSource, mod.DamageTypeFilter.DamageType.Value);
				CheckEnumList.SetValue(lbConditions.ItemsSource, mod.ModConditions.Value);
				RadioEnumList.SetValue(lbAttackFilter.ItemsSource, mod.DamageTypeFilter.AttackKind.Value);
				RadioEnumList.SetValue(lbAddModifier.ItemsSource, mod.ModAddAbilityModifier.Value);
				RadioEnumList.SetValue(lbModType.ItemsSource, mod.ModType.Value);
				lbModType.SelectedValue = mod.ModType.Value;
				tbxMultiplier.Text = mod.Multiplier.ToString();
				tbxOffset.Text = mod.Offset.ToString();
				txbModifierLimit.Text = mod.ModifierLimit.ToString();
				tmRepeats.TimeMeasure = mod.Repeats.TimeMeasure;
				tmRepeats.Amount = mod.Repeats.Count;
				ckRequiresConsumed.IsChecked = mod.RequiresConsumption;
				ckRequiresEquipped.IsChecked = mod.RequiresEquipped;
				SelectCombo(cbTargetName, mod.TargetName);
				SelectCombo(cbVantageSkillFilter, mod.VantageSkillFilter);
				ckbAddsAdvantage.IsChecked = mod.AddsAdvantage;
				ckbAddsDisadvantage.IsChecked = mod.AddsDisadvantage;
			}
			finally
			{
				settingInternally = false;
			}
		}

		public void SaveToMod(ModViewModel mod, string propertyName)
		{
			if (settingInternally)
				return;
			mod.Absolute = GetDouble(tbxAbsolute.Text);
			mod.AddsAdvantage = ckbAddsAdvantage.IsChecked ?? false;
			mod.AddsDisadvantage = ckbAddsDisadvantage.IsChecked ?? false;
			mod.ModifierLimit = GetInt(txbModifierLimit.Text);
			mod.Multiplier = GetDouble(tbxMultiplier.Text, 1);
			//mod.Name = vm.Name;
			mod.Offset = GetDouble(tbxOffset.Text);
			mod.Repeats = new DndTimeSpan(tmRepeats.TimeMeasure, (int)tmRepeats.Amount);
			mod.RequiresConsumption = ckRequiresConsumed.IsChecked == true;
			mod.RequiresEquipped = ckRequiresEquipped.IsChecked == true;
			mod.DamageTypeFilter.AttackKind.Value = RadioEnumList.CalcValue(lbAttackFilter.ItemsSource, typeof(AttackType));
			mod.AddAbilityModifier = (Ability)RadioEnumList.CalcValue(lbAddModifier.ItemsSource, typeof(Ability));
			mod.DamageTypeFilter.DamageType.Value = CheckEnumList.CalcValue(lbDamageEdit.ItemsSource, typeof(DamageType));
			mod.ModConditions.Value = CheckEnumList.CalcValue(lbConditions.ItemsSource, typeof(Conditions));

			if (cbTargetName.SelectedValue != null)
				mod.TargetName = cbTargetName.SelectedValue.ToString();
			else
				mod.TargetName = string.Empty;

			mod.ModType.Value = RadioEnumList.CalcValue(lbModType.ItemsSource, typeof(ModType)); ;
			if (cbVantageSkillFilter.SelectedValue != null)
				mod.VantageSkillFilter = (Skills)cbVantageSkillFilter.SelectedValue;
			else
				mod.VantageSkillFilter = Skills.none;
		}
	}
}
