using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace DndUI
{
	public static class Storage
	{
		const string STR_Data = "D:\\Dropbox\\DragonHumpers\\Data";

		static string GetDataFileName(string baseFileName)
		{
			return Path.Combine(STR_Data, baseFileName);
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

		public static T Load<T>(string baseFileName)
		{
			string fileName = GetDataFileName(baseFileName);
			if (!File.Exists(fileName))
				return default(T);
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
		}
	}
}
