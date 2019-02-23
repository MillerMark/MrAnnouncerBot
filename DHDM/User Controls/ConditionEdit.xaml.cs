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
	/// Interaction logic for ConditionEdit.xaml
	/// </summary>
	public partial class ConditionEdit : UserControl
	{
		public static readonly DependencyProperty ConditionsProperty = DependencyProperty.Register("Conditions", typeof(Conditions), typeof(ConditionEdit), new FrameworkPropertyMetadata(Conditions.None, new PropertyChangedCallback(OnConditionsChanged)));
		
		public Conditions Conditions
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (Conditions)GetValue(ConditionsProperty);
			}
			set
			{
				SetValue(ConditionsProperty, value);
			}
		}

		public ConditionEdit()
		{
			InitializeComponent();
		}

		private static void OnConditionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ConditionEdit conditionEdit = o as ConditionEdit;
			if (conditionEdit != null)
				conditionEdit.OnConditionsChanged((Conditions)e.OldValue, (Conditions)e.NewValue);
		}

		protected virtual void OnConditionsChanged(Conditions oldValue, Conditions newValue)
		{
			ckbBlinded.IsChecked = newValue.HasFlag(Conditions.Blinded);
			ckbCharmed.IsChecked = newValue.HasFlag(Conditions.Charmed);
			ckbDeafened.IsChecked = newValue.HasFlag(Conditions.Deafened);
			ckbFatigued.IsChecked = newValue.HasFlag(Conditions.Fatigued);
			ckbGrappled.IsChecked = newValue.HasFlag(Conditions.Grappled);
			ckbIncapacitated.IsChecked = newValue.HasFlag(Conditions.Incapacitated);
			ckbInvisible.IsChecked = newValue.HasFlag(Conditions.Invisible);
			ckbParalyzed.IsChecked = newValue.HasFlag(Conditions.Paralyzed);
			ckbPetrified.IsChecked = newValue.HasFlag(Conditions.Petrified);
			ckbPoisoned.IsChecked = newValue.HasFlag(Conditions.Poisoned);
			ckbProne.IsChecked = newValue.HasFlag(Conditions.Prone);
			ckbRestrained.IsChecked = newValue.HasFlag(Conditions.Restrained);
			ckbSleep.IsChecked = newValue.HasFlag(Conditions.Sleep);
			ckbStunned.IsChecked = newValue.HasFlag(Conditions.Stunned);
			ckbUnconscious.IsChecked = newValue.HasFlag(Conditions.Unconscious);
		}

		private void AnyCheckbox_CheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			Conditions condition = Conditions.None;

			condition |= ckbBlinded.IsChecked == true ? Conditions.Blinded : Conditions.None;
			condition |= ckbCharmed.IsChecked == true ? Conditions.Charmed : Conditions.None;
			condition |= ckbDeafened.IsChecked == true ? Conditions.Deafened : Conditions.None;
			condition |= ckbFatigued.IsChecked == true ? Conditions.Fatigued : Conditions.None;
			condition |= ckbGrappled.IsChecked == true ? Conditions.Grappled : Conditions.None;
			condition |= ckbIncapacitated.IsChecked == true ? Conditions.Incapacitated : Conditions.None;
			condition |= ckbInvisible.IsChecked == true ? Conditions.Invisible : Conditions.None;
			condition |= ckbParalyzed.IsChecked == true ? Conditions.Paralyzed : Conditions.None;
			condition |= ckbPetrified.IsChecked == true ? Conditions.Petrified : Conditions.None;
			condition |= ckbPoisoned.IsChecked == true ? Conditions.Poisoned : Conditions.None;
			condition |= ckbProne.IsChecked == true ? Conditions.Prone : Conditions.None;
			condition |= ckbRestrained.IsChecked == true ? Conditions.Restrained : Conditions.None;
			condition |= ckbSleep.IsChecked == true ? Conditions.Sleep : Conditions.None;
			condition |= ckbStunned.IsChecked == true ? Conditions.Stunned : Conditions.None;
			condition |= ckbUnconscious.IsChecked == true ? Conditions.Unconscious : Conditions.None;

			Conditions = condition;
		}
	}
}
