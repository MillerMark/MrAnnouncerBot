using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndCore
{
	public class CheckEnumList : ViewModelBase
	{

		private Type _enumType;


		private object _value;
		bool settingInternally;

		public CheckEnumList(Type enumType)
		{
			Initialize(enumType);
			foreach (object value in Enum.GetValues(_enumType))
			{
				var item = new CheckEnumViewModel(value);
				item.PropertyChanged += HandleItemPropertyChanged;
				Items.Add(item);
			}
		}

		public CheckEnumList(Type enumType, object filterItems, EnumListOption enumListOption = EnumListOption.Add)
		{
			Initialize(enumType);

			int filterValue = Convert.ToInt32(filterItems);

			foreach (object enumValue in Enum.GetValues(_enumType))
			{
				int enumValueAsInt = Convert.ToInt32(enumValue);
				bool shouldAddItem = (filterValue & enumValueAsInt) != 0;
				if (enumListOption == EnumListOption.Exclude)
					if (filterValue == 0)
						shouldAddItem = enumValueAsInt != 0;
					else
						shouldAddItem = !shouldAddItem;

				if (shouldAddItem)
				{
					var item = new CheckEnumViewModel(enumValue);
					item.PropertyChanged += HandleItemPropertyChanged;
					Items.Add(item);
				}
			}
		}

		public ObservableCollection<CheckEnumViewModel> Items { get; private set; }

		public object Value
		{
			get => _value;
			set
			{
				_value = value;
				SetCheckedFromValue(Convert.ToInt32(_value));
				OnPropertyChanged();
			}
		}

		public static object CalcValue(object itemSource, Type enumType)
		{
			if (itemSource is ObservableCollection<CheckEnumViewModel> items)
			{
				int result = 0;

				foreach (CheckEnumViewModel item in items)
				{
					if (item.IsChecked)
						result |= Convert.ToInt32(item.Value);
				}

				return Enum.ToObject(enumType, result);
			}

			return Enum.ToObject(enumType, 0);
		}

		public static void SetValue(object itemSource, object valueObject)
		{
			int value = Convert.ToInt32(valueObject);
			if (itemSource is ObservableCollection<CheckEnumViewModel> items)
				foreach (CheckEnumViewModel item in items)
				{
					int itemValue = Convert.ToInt32(item.Value);
					item.IsChecked = (value & itemValue) == itemValue;
				}
		}

		private object CalcValue()
		{
			// Assumes the enums are ints.  Can change if needed (can dynamically determine as well).
			Type enumType = _enumType;
			ObservableCollection<CheckEnumViewModel> items = Items;
			return CalcValue(items, enumType);
		}

		private void HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (settingInternally)
				return;
			Value = CalcValue();
		}
		private void Initialize(Type enumType)
		{
			_enumType = enumType;

			Items = new ObservableCollection<CheckEnumViewModel>();
			Items.CollectionChanged += Items_CollectionChanged;
		}

		private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{

		}

		void SetCheckedFromValue(int value)
		{
			settingInternally = true;
			try
			{
				ObservableCollection<CheckEnumViewModel> items = Items;
				SetValue(items, value);
			}
			finally
			{
				settingInternally = false;
			}
		}
	}
}
