using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DndCore
{
	public class EventData : INotifyPropertyChanged
	{
		public string Name { get; set; }
		bool breakAtStart;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool BreakAtStart
		{
			get
			{
				return breakAtStart;
			}
			set
			{
				if (breakAtStart == value)
				{
					return;
				}

				breakAtStart = value;
				OnPropertyChanged();
			}
		}
		void OnPropertyChanged([CallerMemberName] string name = "")
		{
			if (PropertyChanged == null)
				return;

			PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
			PropertyChanged.Invoke(this, e);
		}
		public bool HasCode { get; set; }
		public EventGroup ParentGroup { get; set; }
		public EventData()
		{

		}

		public EventData(string eventName)
		{
			Name = eventName;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}