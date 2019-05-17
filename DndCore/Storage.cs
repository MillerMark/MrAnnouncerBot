using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DndCore
{
	public static class Storage
	{
		const string STR_Data = "D:\\Dropbox\\DragonHumpers\\Data";

		public static string GetDataFileName(string baseFileName)
		{
			return Path.Combine(STR_Data, baseFileName);
		}

		public static T Load<T>(string baseFileName)
		{
			string fileName = GetDataFileName(baseFileName);
			if (!File.Exists(fileName))
				return default(T);
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
		}

		public static ObservableCollection<T> LoadEntriesFromFile<T>(string fileName) where T : ListEntry
		{
			List<T> loadedEntries = null;
			if (fileName != null)
				loadedEntries = Load<List<T>>(fileName);

			foreach (T entry in loadedEntries)
				entry.AfterLoad();

			ObservableCollection<T> results;
			if (loadedEntries != null)
				results = new ObservableCollection<T>(loadedEntries);
			else
				results = new ObservableCollection<T>();
			return results;
		}

		public static void Save(string baseFileName, object data)
		{
			try
			{
				File.WriteAllText(GetDataFileName(baseFileName), JsonConvert.SerializeObject(data));
			}
			catch (Exception ex)
			{
				// TODO: Alert and offer to try again.
				Console.WriteLine("Exception thrown in SaveText: " + ex);
			}
		}

		public static void SaveAllItems(string fileName, IEnumerable itemsSource)
		{
			if (fileName == null)
			{
				if (fileName == null)
					return;
			}

			if (itemsSource != null)
			{
				object firstItem = null;
				IEnumerator enumerator = itemsSource.GetEnumerator();
				if (enumerator.MoveNext())
					firstItem = enumerator.Current;

				if (firstItem == null)
					return;

				Type entryType = firstItem.GetType();
				MethodInfo method = typeof(Enumerable).GetMethod("ToList");
				if (method == null)
					return;

				MethodInfo genericMethod = method.MakeGenericMethod(entryType);
				if (genericMethod == null)
					return;

				object[] parameters = { itemsSource };

				object data = genericMethod.Invoke(itemsSource, parameters);
				if (data is IEnumerable enumerable)
				{
					IEnumerator entryEnumerator = enumerable.GetEnumerator();
					while (entryEnumerator.MoveNext())
					{
						if (entryEnumerator.Current is ListEntry listEntry)
							listEntry.PrepForSerialization();
					}
				}

				//if (data is List<ItemViewModel> items)
				//{
				//	foreach (ItemViewModel itemViewModel in items)
				//		itemViewModel.PrepForSerialization();
				//}
				Storage.Save(fileName, data);
			}
		}
	}
}
