using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllTables
	{
		static List<object> tables = new List<object>();
		static AllTables()
		{
			LoadData();
		}

		public static void LoadData()
		{
			LoadTable<Barbarian>();
			LoadTable<Sorcerer>();
		}

		static void LoadTable<T>() where T : BaseRow
		{
			string tableName = typeof(T).ToString().EverythingAfter(".");

			Table<T> table = new Table<T>();
			table.Name = tableName;
			table.Rows = LoadTable<T>(tableName).ToArray();
			tables.Add(table);
		}

		public static List<T> LoadTable<T>(string tableName)
		{
			return CsvData.Get<T>(Folders.InCoreData($"DnD Table - {tableName}.csv"));
		}
		static object GetValue(object entry, string fieldLookup)
		{
			PropertyInfo property = entry.GetType().GetProperty(fieldLookup);
			if (property == null)
				return null;
			return property.GetValue(entry);
		}
		static bool ValuesMatch(object entry, string matchColumn, object matchValue)
		{
			PropertyInfo property = entry.GetType().GetProperty(matchColumn);
			if (property == null)
				return false;
			return matchValue.Equals(property.GetValue(entry));
		}
		public static object GetData(string tableName, string fieldLookup, string matchColumn, object matchValue)
		{
			object foundTable = FindTable(tableName);
			if (foundTable == null)
				return null;
			PropertyInfo rowsProperty = foundTable.GetType().GetProperty("Rows");
			if (rowsProperty == null)
				return null;

			object[] rowsArray = (object[])rowsProperty.GetValue(foundTable);

			PropertyInfo lengthProperty = rowsProperty.PropertyType.GetProperty("Length");
			int arrayLength = (int)lengthProperty.GetValue(rowsArray);

			for (int i = 0; i < arrayLength; i++)
			{
				object entry = rowsArray[i];
				if (ValuesMatch(entry, matchColumn, matchValue))
					return GetValue(entry, fieldLookup);
			}

			return null;
		}

		private static object FindTable(string tableName)
		{
			object foundTable = null;
			foreach (object item in tables)
			{
				PropertyInfo nameProperty = item.GetType().GetProperty("Name");
				if (nameProperty != null)
				{
					object nameValue = nameProperty.GetValue(item);
					if (nameValue is string)
					{
						string thisTableName = (string)nameValue;
						if (thisTableName == tableName)
						{
							foundTable = item;
							break;
						}
					}
				}
			}

			return foundTable;
		}
	}
}

