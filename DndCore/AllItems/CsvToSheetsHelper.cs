using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class CsvToSheetsHelper
	{
		/// <summary>
		/// Gets/deserializes all objects of type T from a specified *.csv file.
		/// </summary>
		/// <typeparam name="T">The type of the objects stored in the *csv file.</typeparam>
		/// <param name="dataFileName">The full path to the *.csv file.</param>
		/// <returns>Returns a list of T found in the file.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static List<T> Get<T>(string dataFileName) where T : new()
		{
			string fileNameOnly = Path.GetFileNameWithoutExtension(dataFileName);

			const string DocSheetSeparator = " - ";
			int docSheetSeparatorPosition = fileNameOnly.IndexOf(DocSheetSeparator);
			if (docSheetSeparatorPosition > 0)
			{
				string docFileName = fileNameOnly.Substring(0, docSheetSeparatorPosition);
				string sheetName = fileNameOnly.Substring(docSheetSeparatorPosition + DocSheetSeparator.Length);
				return GoogleSheets.Get<T>(docFileName, sheetName);
			}

			throw new InvalidDataException($"DocSheetSeparator (\"{DocSheetSeparator}\") not found.");
		}

	}
}
