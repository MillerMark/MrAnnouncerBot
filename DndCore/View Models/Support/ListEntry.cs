using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DndCore.ViewModels.Support
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
