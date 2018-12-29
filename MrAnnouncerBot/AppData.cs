using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MrAnnouncerBot
{
	public static class AppData
	{
		public static string GetDataFolder()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MrAnnouncerBot");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		public static string GetDataFileName(string fileName)
		{
			return Path.Combine(GetDataFolder(), fileName);
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