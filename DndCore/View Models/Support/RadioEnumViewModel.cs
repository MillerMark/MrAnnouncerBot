using System;
using System.Linq;

namespace DndCore
{
	public class RadioEnumViewModel : ViewModelBase
	{

		private string groupName;

		private bool isChecked;

		public RadioEnumViewModel(object value) => Value = value;

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

		public object Value { get; }

		internal void OnEntryClicked(object sender, EventArgs e)
		{
			EntryClicked?.Invoke(sender, e);
		}

		public event EventHandler EntryClicked;
	}
}
