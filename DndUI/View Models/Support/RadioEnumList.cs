using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DndUI
{
	public class RadioEnumList : ViewModelBase
	{
		private void Initialize(Type enumType)
		{
			_enumType = enumType;
			Items = new ObservableCollection<RadioEnumViewModel>();
		}

		public RadioEnumList(Type enumType, string groupName, object filterItems, EnumListOption enumListOption = EnumListOption.Add)
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
					item.GroupName = groupName;
					item.EntryClicked += Item_EntryClicked;
					Items.Add(item);
				}
			}
		}

		public RadioEnumList(Type enumType, string groupName)
		{
			Initialize(enumType);
			foreach (object value in Enum.GetValues(_enumType))
			{
				var item = new RadioEnumViewModel(value);
				item.GroupName = groupName;
				item.EntryClicked += Item_EntryClicked;
				Items.Add(item);
			}
		}

		private object _value;

		void SetCheckedFromValue(int value, ObservableCollection<RadioEnumViewModel> items)
		{
			settingInternally = true;
			try
			{
				SetValue(items, value);
			}
			finally
			{
				settingInternally = false;
			}
		}

		public object Value
		{
			get => _value;
			set
			{
				_value = value;
				if (_value is RadioEnumViewModel vm)
					SetCheckedFromValue((int)vm.Value, Items);
				else
					SetCheckedFromValue(Convert.ToInt32(_value), Items);
				OnPropertyChanged();
			}
		}

		public ObservableCollection<RadioEnumViewModel> Items { get; private set; }

		private Type _enumType;
		bool settingInternally;

		void ClearAllBut(RadioEnumViewModel radioEnumViewModel)
		{
			if (radioEnumViewModel == null)
				return;
			foreach (RadioEnumViewModel item in Items)
				if (item != radioEnumViewModel)
					item.IsChecked = false;
		}

		private void Item_EntryClicked(object sender, EventArgs e)
		{
			if (settingInternally)
				return;
			ClearAllBut(sender as RadioEnumViewModel);
		}

		private void HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (settingInternally)
				return;
			Value = CalcValue(Items, _enumType);
		}

		public static int CalcValue(object itemSource, Type enumType)
		{
			if (itemSource is ObservableCollection<RadioEnumViewModel> items)
				if (items != null)
					foreach (RadioEnumViewModel item in items)
						if (item.IsChecked)
							return (int)Enum.ToObject(enumType, Convert.ToInt32(item.Value));

			return (int)Enum.ToObject(enumType, 0);
		}

		public static void SetValue(object itemSource, object valueObject)
		{
			int value = Convert.ToInt32(valueObject);
			if (itemSource is ObservableCollection<RadioEnumViewModel> items)
				foreach (RadioEnumViewModel item in items)
					item.IsChecked = Convert.ToInt32(item.Value) == value;
		}
	}
}
