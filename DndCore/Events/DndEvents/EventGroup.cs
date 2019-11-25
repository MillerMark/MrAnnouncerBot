using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DndCore
{
	public class EventGroup : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public bool HasCode { get; set; }
		bool hasBreakpoints;
		public bool HasBreakpoints
		{
			get
			{
				return hasBreakpoints;
			}
			set
			{
				if (hasBreakpoints == value)
				{
					return;
				}

				hasBreakpoints = value;
				OnPropertyChanged();
			}
		}
		public EventGroup()
		{

		}

		void OnPropertyChanged([CallerMemberName] string name = "")
		{
			if (PropertyChanged == null)
				return;

			PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
			PropertyChanged.Invoke(this, e);
		}

		public EventGroup(object instance, string name, EventType type, List<string> events)
		{
			Events = new List<EventData>();
			foreach (string eventName in events)
			{

				EventData eventData = new EventData(eventName);
				eventData.PropertyChanged += EventData_PropertyChanged;
				if (instance != null)
				{
					object value = instance.GetType().GetProperty(eventName).GetValue(instance, null);
					if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
					{
						eventData.HasCode = true;
						HasCode = true;
					}
				}
				eventData.ParentGroup = this;
				Events.Add(eventData);
			}
			Type = type;
			Name = name;
		}

		private void EventData_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			bool newValue = false;
			if (e.PropertyName == "BreakAtStart")
				foreach (EventData eventData in Events)
				{
					if (eventData.BreakAtStart)
					{
						newValue = true;
						break;
					}
				}

			HasBreakpoints = newValue;
		}

		public string Name { get; set; }
		public EventType Type { get; set; }
		public List<EventData> Events { get; set; }
		public EventCategory ParentCategory { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}