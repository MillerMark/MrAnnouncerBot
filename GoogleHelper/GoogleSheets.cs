using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

namespace GoogleHelper
{
	public static class GoogleSheets
	{
		static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static Dictionary<string, string> spreadsheetIDs = new Dictionary<string, string>();

		static string ApplicationName = "Google Sheets .NET Helper";
		public static List<T> Get<T>(string dataFileName, bool validateHeaders = true) where T: new()
		{
			string fileNameOnly = Path.GetFileNameWithoutExtension(dataFileName);

			const string DocTabSeparator = " - ";
			int docTabSeparatorPosition = fileNameOnly.IndexOf(DocTabSeparator);
			if (docTabSeparatorPosition > 0)
			{
				string docFileName = fileNameOnly.Substring(0, docTabSeparatorPosition);
				string tabName = fileNameOnly.Substring(docTabSeparatorPosition + DocTabSeparator.Length);
				return Get<T>(docFileName, tabName);
			}

			throw new InvalidDataException($"DocTabSeparator (\"{DocTabSeparator}\") not found.");
		}

		static IList<IList<object>> GetCells(string docName, string tabName, string cellRange = "")
		{
			if (!spreadsheetIDs.ContainsKey(docName))
				throw new InvalidDataException($"docName (\"{docName}\") not found!");

			string spreadsheetId = spreadsheetIDs[docName];

			string range;
			if (string.IsNullOrEmpty(cellRange))
				range = tabName;  // Reading the entire file.
			else
				range = $"{tabName}!{cellRange}";

			SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
			ValueRange response = request.Execute();
			return response.Values;
		}

		static void TransferValues<T>(T instance, Dictionary<int, string> headers, IList<object> row) where T : new()
		{
			Type type = instance.GetType();

			for (int i = 0; i < row.Count; i++)  // There may be fewer rows than headers.
			{
				PropertyInfo property = type.GetProperty(headers[i]);
				if (property == null)
					continue;

				string fullName = property.PropertyType.FullName;
				string value = (string)row[i];
				switch (fullName)
				{
					case "System.Int32":
						if (int.TryParse(value, out int intValue))
							property.SetValue(instance, intValue);
						else
						{
							System.Diagnostics.Debugger.Break();
						}
						break;
					case "System.String":
						property.SetValue(instance, row[i]);
						break;
					case "System.Boolean":
						bool newValue = value.ToLower().Trim() == "true";
						property.SetValue(instance, newValue);
						break;
					default:
						System.Diagnostics.Debugger.Break();
						break;
				}
			}

			SetDefaultsForEmptyCells(instance, headers, row, type);
		}

		private static void SetDefaultsForEmptyCells<T>(T instance, Dictionary<int, string> headers, IList<object> row, Type type) where T : new()
		{
			for (int i = row.Count; i < headers.Count; i++)  // There may be fewer rows than headers.
			{
				PropertyInfo property = type.GetProperty(headers[i]);
				if (property == null)
					continue;

				switch (property.PropertyType.FullName)
				{
					case "System.Int32":
						property.SetValue(instance, default(int));
						break;
					case "System.String":
						property.SetValue(instance, string.Empty);
						break;
					case "System.Boolean":
						property.SetValue(instance, default(bool));
						break;
					default:
						System.Diagnostics.Debugger.Break();
						break;
				}
			}
		}
		static Dictionary<string, List<string>> sheetTabMap = new Dictionary<string, List<string>>();
		static void Track(string docName, string tabName)
		{
			if (!sheetTabMap.ContainsKey(docName))
				sheetTabMap.Add(docName, new List<string>());
			List<string> tabs = sheetTabMap[docName];
			if (!tabs.Contains(tabName))
				tabs.Add(tabName);
		}

		static string GetDocumentName(string tabName)
		{
			foreach (string docName in sheetTabMap.Keys)
			{
				List<string> tabs = sheetTabMap[docName];
				if (tabs.Contains(tabName))
					return docName;
			}
			return null;
		}

		static T newItem<T>(Dictionary<int, string> headers, IList<object> row) where T: new()
		{
			T result = new T();
			TransferValues(result, headers, row);
			return result;
		}

		static List<T> Get<T>(string docName, string tabName) where T: new()
		{
			Track(docName, tabName);
			List<T> result = new List<T>();
			try
			{
				IList<IList<object>> allCells = GetCells(docName, tabName);
				Dictionary<int, string> headers = new Dictionary<int, string>();
				IList<object> headerRow = allCells[0];
				for (int i = 0; i < headerRow.Count; i++)
				{
					headers.Add(i, (string)headerRow[i]);
				}
				for (int row = 1; row < allCells.Count; row++)
				{
					result.Add(newItem<T>(headers, allCells[row]));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading google sheet ({docName} - {tabName}): {ex.Message}");
			}
			return result;
		}

		static SheetsService service;

		static GoogleSheets()
		{
			InitialService(GetUserCredentials());
			RegisterSpreadsheetIDs();
		}

		private static void InitialService(UserCredential credential)
		{
			service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
		}

		private static UserCredential GetUserCredentials()
		{
			UserCredential credential;
			using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credentialPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
						GoogleClientSecrets.Load(stream).Secrets,
						Scopes,
						"user",
						CancellationToken.None,
						new FileDataStore(credentialPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credentialPath);
			}

			return credential;
		}
		static void RegisterSpreadsheetIDs()
		{
			spreadsheetIDs.Add("DnD", "13g0mcruC1gLcSfkVESIWW9Efrn0MyaKw0hqCiK1Rg8k");
			spreadsheetIDs.Add("DnD Table", "1SktOjs8_E8lTuU1ao9M1H44UGR9fDOnWSvdbpVgMIuw");
		}

		public static void Update<T>(string tabName, List<T> dtos)
		{
			string docName = GetDocumentName(tabName);

			if (docName == null)
				throw new InvalidDataException($"tabName (\"{tabName}\") not found!");

			if (!spreadsheetIDs.ContainsKey(docName))
				throw new InvalidDataException($"docName (\"{docName}\") not found!");

			string spreadsheetId = spreadsheetIDs[docName];

			ValueRange body = new ValueRange();
			SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(body, spreadsheetId, tabName);
			UpdateValuesResponse response = request.Execute();
		}
	}
}
