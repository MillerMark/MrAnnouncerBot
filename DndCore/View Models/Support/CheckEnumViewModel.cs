using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndCore
{
	// Wraps an enum value so we can expose an IsChecked property.
	public class CheckEnumViewModel : ViewModelBase
	{
		public CheckEnumViewModel(object value) => Value = value;

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
