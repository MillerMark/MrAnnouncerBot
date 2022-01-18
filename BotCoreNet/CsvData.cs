using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotCore
{
	public static class CsvData
	{
		public static List<T> Get<T>(string dataFileName)
		{
			List<T> result = new List<T>();
			try
			{
				StreamReader textReader = File.OpenText(dataFileName);

				using (var csvReader = new CsvReader(textReader, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)))
					result = csvReader.GetRecords<T>().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading scene data file ({dataFileName}): {ex.Message}");
			}
			return result;
		}
	}
}