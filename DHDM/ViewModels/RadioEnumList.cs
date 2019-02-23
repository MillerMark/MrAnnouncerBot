using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DHDM
{
	public class RadioEnumList : ViewModelBase
	{
		private void Initialize(Type enumType)
		{
			_enumType = enumType;

			Items = new ObservableCollection<RadioEnumViewModel>();
		}

		public RadioEnumList(Type enumType, object filterItems, EnumListOption enumListOption = EnumListOption.Add)
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
					var item = new RadioEnumViewModel(enumValue);
					item.PropertyChanged += HandleItemPropertyChanged;
					Items.Add(item);
				}
			}
		}

		public RadioEnumList(Type enumType)
		{
			Initialize(enumType);
			foreach (object value in Enum.GetValues(_enumType))
			{
				var item = new RadioEnumViewModel(value);
				item.PropertyChanged += HandleItemPropertyChanged;
				Items.Add(item);
			}
		}

		private object _value;

		public object Value
		{
			get => _value;
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<RadioEnumViewModel> Items { get; private set; }

		private Type _enumType;

		private object CalcValue()
		{
			// Assumes the enums are ints.  Can change if needed (can dynamically determine as well).
			int result = 0;

			foreach (RadioEnumViewModel item in Items)
			{
				if (item.IsChecked)
					result |= Convert.ToInt32(item.Value);
			}

			return Enum.ToObject(_enumType, result);
		}

		private void HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Value = CalcValue();
		}
	}
}
