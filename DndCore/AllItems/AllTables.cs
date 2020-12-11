using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllTables
	{
		static List<object> tables;

		public static void Invalidate()
		{
			tables = null;
		}

		public static List<object> Tables
		{
			get
			{
				if (tables == null)
					LoadData();

				return tables;
			}
			set
			{
				tables = value;
			}
		}
		
		public static void LoadData()
		{
			tables = new List<object>();
			LoadTable<Barbarian>();
			LoadTable<Sorcerer>();
			LoadTable<ArcaneTrickster>();
			LoadTable<Paladin>();
			LoadTable<Rogue>();
			LoadTable<Bard>();
			LoadTable<Cleric>();
			LoadTable<Ranger>();
			LoadTable<Druid>();
		}

		static void LoadTable<T>() where T : BaseRow, new()
		{
			string tableName = typeof(T).ToString().EverythingAfter(".");

			Table<T> table = new Table<T>();
			table.Name = tableName;
			table.Rows = LoadTable<T>(tableName).ToArray();
			tables.Add(table);
		}

		public static List<T> LoadTable<T>(string tableName) where T : new()
		{
			return GoogleSheets.Get<T>(Folders.InCoreData($"DnD Table - {tableName}.csv"));
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
			foreach (object item in Tables)
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
		public static List<string> GetColumns(string tableName)
		{
			List<string> columns = new List<string>();
			foreach (object table in tables)
			{
				PropertyInfo nameProp = table.GetType().GetProperty("Name");
				if (nameProp != null && (string)nameProp.GetValue(table) == tableName)
				{
					Type tableType = table.GetType();
					if (tableType.IsGenericType)
					{
						Type[] typeParameters = tableType.GetGenericArguments();
						if (typeParameters.Length == 1)
						{
							Type tableEntryType = typeParameters[0];
							PropertyInfo[] properties = tableEntryType.GetProperties();
							foreach (PropertyInfo propertyInfo in properties)
							{
								columns.Add(propertyInfo.Name);
							}
						}
					}
					break;
				}
			}
			return columns;
		}
	}
}

