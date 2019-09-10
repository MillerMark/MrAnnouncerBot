using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllProperties
	{
		static AllProperties()
		{
			Properties = CsvData.Get<PropertyDto>(Folders.InCoreData("DnD - Properties.csv"));
		}

		public static List<PropertyDto> Properties { get; private set; }

		public static PropertyDto Get(string propertyName)
		{
			return Properties.FirstOrDefault(x => x.Name == propertyName);
		}

	}
}
