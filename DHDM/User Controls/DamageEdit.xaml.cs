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

namespace DHDM.User_Controls
{
	/// <summary>
	/// Interaction logic for DamageEdit.xaml
	/// </summary>
	public partial class DamageEdit : UserControl
	{
		public static readonly DependencyProperty DamageTypeProperty = DependencyProperty.Register("DamageType", typeof(DamageType), typeof(DamageEdit), new FrameworkPropertyMetadata(DamageType.None, new PropertyChangedCallback(OnDamageTypeChanged), new CoerceValueCallback(OnCoerceDamageType)));
		
		private static object OnCoerceDamageType(DependencyObject o, object value)
		{
			DamageEdit damageEdit = o as DamageEdit;
			if (damageEdit != null)
				return damageEdit.OnCoerceDamageType((DamageType)value);
			else
				return value;
		}

		private static void OnDamageTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			DamageEdit damageEdit = o as DamageEdit;
			if (damageEdit != null)
				damageEdit.OnDamageTypeChanged((DamageType)e.OldValue, (DamageType)e.NewValue);
		}

		protected virtual DamageType OnCoerceDamageType(DamageType value)
		{
			// TODO: Keep the proposed value within the desired range.
			return value;
		}

		protected virtual void OnDamageTypeChanged(DamageType oldValue, DamageType newValue)
		{
			ckbAcid.IsChecked = newValue.HasFlag(DamageType.Acid);
			ckbBludgeoning.IsChecked = newValue.HasFlag(DamageType.Bludgeoning);
			ckbCold.IsChecked = newValue.HasFlag(DamageType.Cold);
			ckbFire.IsChecked = newValue.HasFlag(DamageType.Fire);
			ckbForce.IsChecked = newValue.HasFlag(DamageType.Force);
			ckbLightning.IsChecked = newValue.HasFlag(DamageType.Lightning);
			ckbNecrotic.IsChecked = newValue.HasFlag(DamageType.Necrotic);
			ckbPiercing.IsChecked = newValue.HasFlag(DamageType.Piercing);
			ckbPoison.IsChecked = newValue.HasFlag(DamageType.Poison);
			ckbPsychic.IsChecked = newValue.HasFlag(DamageType.Psychic);
			ckbRadiant.IsChecked = newValue.HasFlag(DamageType.Radiant);
			ckbSlashing.IsChecked = newValue.HasFlag(DamageType.Slashing);
			ckbThunder.IsChecked = newValue.HasFlag(DamageType.Thunder);
		}

		public DamageType DamageType
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (DamageType)GetValue(DamageTypeProperty);
			}
			set
			{
				SetValue(DamageTypeProperty, value);
			}
		}

		public DamageEdit()
		{
			InitializeComponent();
		}

		private void AnyCheckbox_CheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			DamageType damage = DamageType.None;
			
			damage |= ckbAcid.IsChecked == true? DamageType.Acid: DamageType.None;
			damage |= ckbBludgeoning.IsChecked == true ? DamageType.Bludgeoning : DamageType.None;
			damage |= ckbCold.IsChecked == true ? DamageType.Cold : DamageType.None;
			damage |= ckbFire.IsChecked == true ? DamageType.Fire : DamageType.None;
			damage |= ckbForce.IsChecked == true ? DamageType.Force : DamageType.None;
			damage |= ckbLightning.IsChecked == true ? DamageType.Lightning : DamageType.None;
			damage |= ckbNecrotic.IsChecked == true ? DamageType.Necrotic : DamageType.None;
			damage |= ckbPiercing.IsChecked == true ? DamageType.Piercing : DamageType.None;
			damage |= ckbPoison.IsChecked == true ? DamageType.Poison : DamageType.None;
			damage |= ckbPsychic.IsChecked == true ? DamageType.Psychic : DamageType.None;
			damage |= ckbRadiant.IsChecked == true ? DamageType.Radiant : DamageType.None;
			damage |= ckbSlashing.IsChecked == true ? DamageType.Slashing : DamageType.None;
			damage |= ckbThunder.IsChecked == true ? DamageType.Thunder : DamageType.None;

			DamageType = damage;
		}
	}
}
