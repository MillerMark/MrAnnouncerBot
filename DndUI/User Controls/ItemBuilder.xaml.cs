using DndCore;
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

namespace DndUI.User_Controls
{
	/// <summary>
	/// Interaction logic for ItemBuilder.xaml
	/// </summary>
	public partial class ItemBuilder : UserControl
	{
		public static readonly DependencyProperty WeaponCategoryProperty = DependencyProperty.Register("WeaponCategory", typeof(WeaponCategories), typeof(ItemBuilder), new FrameworkPropertyMetadata(WeaponCategories.None, new PropertyChangedCallback(OnWeaponCategoryChanged), new CoerceValueCallback(OnCoerceWeaponCategory)));
		
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

		}

		private void LbModsList_ClickAdd(object sender, RoutedEventArgs e)
		{

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
	}
}
