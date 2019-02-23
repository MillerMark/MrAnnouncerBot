using System;
using System.Linq;

namespace DHDM
{
	public class RadioEnumViewModel : ViewModelBase
	{
		public RadioEnumViewModel(object value) => Value = value;

		public object Value { get; }

		private bool isChecked;

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
