using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace DndCore
{
	public static class CsvData
	{
		public static List<T> Get<T>(string dataFileName, bool validateHeaders = true)
		{
			List<T> result = new List<T>();
			try
			{
				var textReader = File.OpenText(dataFileName);

				using (var csvReader = new CsvReader(textReader))
				{
					if (!validateHeaders)
					{
						csvReader.Configuration.HeaderValidated = null;
						csvReader.Configuration.MissingFieldFound = null;
					}
					result = csvReader.GetRecords<T>().ToList();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading scene data file ({dataFileName}): {ex.Message}");
			}
			return result;
		}
	}
}
