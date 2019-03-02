using System;
using System.Linq;

namespace DHDM
{
	public class RadioEnumViewModel : ViewModelBase
	{
		public event EventHandler EntryClicked;

		internal void OnEntryClicked(object sender, EventArgs e)
		{
			EntryClicked?.Invoke(sender, e);
		}

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
				if (isChecked == value)
					return;

				isChecked = value;
				if (isChecked)
					OnEntryClicked(this, null);
				OnPropertyChanged();
			}
		}
	}
}
