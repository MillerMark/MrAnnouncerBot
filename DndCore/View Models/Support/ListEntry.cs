using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DndCore
{
	public class ListEntry : ViewModelBase, IListEntry, INotifyPropertyChanged
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

		public virtual void AfterLoad()
		{

		}
		public virtual void PrepForSerialization()
		{

		}
	}
}
