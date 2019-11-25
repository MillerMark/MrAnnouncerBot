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

		public List<EventGroup> Groups { get => groups; set => groups = value; }
		public string CategoryName { get; set; }
		public Type Type { get; set; }
	}
}