using System;
using System.Linq;

namespace DHDM
{
	public class RadioEnumViewModel : ViewModelBase
	{
		public RadioEnumViewModel(object value) => Value = value;

		public object Value { get; }

		private bool isChecked;

		private string groupName;
		public string GroupName
		{
			get
			{
				return groupName;
			}
			set
			{
				if (groupName == value)
					return;

				groupName = value;
				OnPropertyChanged();
			}
		}

		public bool IsChecked
		{
			get => isChecked;
			set
			{
				isChecked = value;
				OnPropertyChanged();
			}
		}
	}
}
