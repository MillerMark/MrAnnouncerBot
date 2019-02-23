using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DHDM
{
	public interface IListEntry
	{
		string Name { get; set; }
	}

	public class ListEntry : IListEntry, INotifyPropertyChanged
	{
		string name;

		public string Name
		{
			get { return name; }
			set
			{
				if (name == value)
				{
					return;
				}

				name = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
