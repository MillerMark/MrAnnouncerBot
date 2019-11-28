using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class EventCategory
	{
		List<EventGroup> groups = new List<EventGroup>();
		public EventCategory()
		{

		}

		public EventCategory(string categoryName, Type type)
		{
			Type = type;
			CategoryName = categoryName;
		}

		public void Add(EventGroup eventGroup)
		{
			groups.Add(eventGroup);
			eventGroup.ParentCategory = this;
		}

		public void Clear()
		{
			groups.Clear();
		}
		public EventData FindEvent(EventType eventType, string parentName, string eventName)
		{
			foreach (EventGroup eventGroup in Groups)
			{
				if (eventGroup.Name != parentName)
					continue;
				EventData foundEvent = eventGroup.FindEvent(eventType, eventName);
				if (foundEvent != null)
					return foundEvent;
			}
			return null;
		}

		public List<EventGroup> Groups { get => groups; set => groups = value; }
		public string CategoryName { get; set; }
		public Type Type { get; set; }
	}
}