using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DHDM.User_Controls
{
	public class EnumCheckBox<T> : CheckBox //where T : struct
	{
		public static readonly DependencyProperty EnumBindingProperty = DependencyProperty.Register("EnumBinding", typeof(T), typeof(EnumCheckBox<T>), new FrameworkPropertyMetadata(default(T), new PropertyChangedCallback(OnEnumBindingChanged), new CoerceValueCallback(OnCoerceEnumBinding)));
		

		private static object OnCoerceEnumBinding(DependencyObject o, object value)
		{
			EnumCheckBox<T> enumCheckBox = o as EnumCheckBox<T>;
			if (enumCheckBox != null)
				return enumCheckBox.OnCoerceEnumBinding((T)value);
			else
				return value;
		}

		private static void OnEnumBindingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			EnumCheckBox<T> enumCheckBox = o as EnumCheckBox<T>;
			if (enumCheckBox != null)
				enumCheckBox.OnEnumBindingChanged((T)e.OldValue, (T)e.NewValue);
		}

		protected virtual T OnCoerceEnumBinding(T value)
		{
			// TODO: Keep the proposed value within the desired range.
			return value;
		}

		protected virtual void OnEnumBindingChanged(T oldValue, T newValue)
		{
			// TODO: Add your property changed side-effects. Descendants can override as well.
		}

		public T EnumBinding
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (T)GetValue(EnumBindingProperty);
			}
			set
			{
				SetValue(EnumBindingProperty, value);
			}
		}

	}
}
