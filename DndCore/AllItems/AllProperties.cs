using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllProperties
	{
		static void LoadData()
		{
			Properties = CsvToSheetsHelper.Get<PropertyDto>(Folders.InCoreData("DnD - Properties.csv"));
		}
		static List<PropertyDto> properties;

		public static List<PropertyDto> Properties
		{
			get
			{
				if (properties == null)
					LoadData();
				return properties;
			}
			private set
			{
				properties = value;
			}
		}

		public static PropertyDto Get(string propertyName)
		{
			return Properties.FirstOrDefault(x => x.Name == propertyName);
		}

		public static void Invalidate()
		{
			properties = null;
		}
	}
}
