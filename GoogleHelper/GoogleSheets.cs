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
		public static List<T> Get<T>(string dataFileName, bool validateHeaders = true) where T : new()
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

		public static List<T> Get<T>() where T : new()
		{
			SheetNameAttribute sheetAttribute = typeof(T).GetCustomAttribute<SheetNameAttribute>();
			if (sheetAttribute == null)
				throw new InvalidDataException($"SheetNameAttribute not found on (\"{typeof(T).Name}\").");
			TabNameAttribute tabAttribute = typeof(T).GetCustomAttribute<TabNameAttribute>();
			if (tabAttribute == null)
				throw new InvalidDataException($"TabNameAttribute not found on (\"{typeof(T).Name}\").");

			return Get<T>(sheetAttribute.SheetName, tabAttribute.TabName);
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

		static void SetEnumValue(PropertyInfo enumProperty, object instance, string valueStr)
		{
			int value = 0;
			if (!string.IsNullOrWhiteSpace(valueStr))
				try
				{
					string[] parts = valueStr.Split('|');
					foreach (string part in parts)
					{
						value += (int)Enum.Parse(enumProperty.PropertyType, part.Trim());
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debugger.Break();
					return;
				}
			enumProperty.SetValue(instance, value);
		}

		static void SetValue(PropertyInfo property, object instance, string value)
		{
			if (property.PropertyType.IsEnum)
				SetEnumValue(property, instance, value);
			else
				System.Diagnostics.Debugger.Break();
		}

		static PropertyInfo GetCorrespondingPropertyInfo(Type type, string header)
		{
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
				if (columnAttribute != null && columnAttribute.ColumnName == header)
					return propertyInfo;

			}
			return type.GetProperty(header);
		}
		static void TransferValues<T>(T instance, Dictionary<int, string> headers, IList<object> row) where T : new()
		{
			Type type = instance.GetType();

			for (int i = 0; i < row.Count; i++)  // There may be fewer rows than headers.
			{
				//! Not using ColumName here!!!
				PropertyInfo property = GetCorrespondingPropertyInfo(type, headers[i]);
				if (property == null)
					continue;

				string fullName = property.PropertyType.FullName;
				string value = (string)row[i];
				switch (fullName)
				{
					case "System.Int32":
						if (!int.TryParse(value, out int intValue))
							intValue = 0;
						property.SetValue(instance, intValue);
						break;
					case "System.Decimal":
						if (decimal.TryParse(value, out decimal decimalValue))
							property.SetValue(instance, decimalValue);
						else
						{
							System.Diagnostics.Debugger.Break();
						}
						break;
					case "System.String":
						property.SetValue(instance, row[i]);
						break;
					case "System.Boolean":
						string compareValue = value.ToLower().Trim();
						bool newValue = compareValue == "true" || compareValue == "x";
						property.SetValue(instance, newValue);
						break;
					default:
						SetValue(property, instance, value);
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
					case "System.Decimal":
						property.SetValue(instance, default(decimal));
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

		static T newItem<T>(Dictionary<int, string> headers, IList<object> row) where T : new()
		{
			T result = new T();
			TransferValues(result, headers, row);
			return result;
		}

		static List<T> Get<T>(string docName, string tabName) where T : new()
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
			spreadsheetIDs.Add("DnD Game", "1GhONDxF4NU6sU0cqxwtvTyQ6HKlIGNEYb8Z_lcqeWKY");
			spreadsheetIDs.Add("IDE", "1q-GuDx91etsKO0HzX0MCojq24PGZbPIcTZX-V6arpTQ");
		}

		static MemberInfo[] GetFields<TAttribute>(Type instanceType) where TAttribute : Attribute
		{
			List<MemberInfo> memberInfo = new List<MemberInfo>();
			IEnumerable<PropertyInfo> properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<TAttribute>() != null);
			IEnumerable<FieldInfo> fields = instanceType.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<TAttribute>() != null);
			memberInfo.AddRange(properties);
			memberInfo.AddRange(fields);
			return memberInfo.ToArray();
		}

		static string GetColumnName<TColumnAttribute>(MemberInfo memberInfo) where TColumnAttribute: ColumnNameAttribute
		{
			TColumnAttribute customAttribute = memberInfo.GetCustomAttribute<TColumnAttribute>();
			if (customAttribute == null || string.IsNullOrEmpty(customAttribute.ColumnName))
				return memberInfo.Name;
			return customAttribute.ColumnName;
		}

		static int GetColumnIndex(List<string> headerRow, string indexColumnName)
		{
			for (int i = 0; i < headerRow.Count; i++)
				if (string.Compare(headerRow[i], indexColumnName, StringComparison.OrdinalIgnoreCase) == 0)
					return i;
			return -1;
		}

		static string GetValue(object obj, MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo propInfo)
			{
				object value = propInfo.GetValue(obj);
				if (value == null)
					return null;
				return value.ToString();
			}

			if (memberInfo is FieldInfo fieldInfo)
				return fieldInfo.GetValue(obj).ToString();
			return null;
		}

		static int GetInstanceRowIndex(object obj, MemberInfo[] indexFields, IList<IList<object>> allRows, List<string> headerRow)
		{
			if (indexFields == null || indexFields.Length == 0)
				throw new InvalidDataException($"indexFields must contain data!");

			for (int rowIndex = 1; rowIndex < allRows.Count; rowIndex++)
			{

				bool found = false;
				for (int i = 0; i < indexFields.Length; i++)
				{
					string indexColumnName = GetColumnName<IndexerAttribute>(indexFields[i]);
					int column = GetColumnIndex(headerRow, indexColumnName);
					if (column == -1)
						throw new Exception($"Header not found in sheet: {indexColumnName}!");
					IList<object> thisRow = allRows[rowIndex];
					if (thisRow[column].ToString() == GetValue(obj, indexFields[i]))
						found = true;
					else
					{
						found = false;
						break;
					}
				}
				if (found)
					return rowIndex;
			}

			return -1;
		}

		static string GetColumnId(int column)
		{
			int secondDigit = column / 26;
			int firstDigit = column % 26;
			string secondDigitStr = "";
			if (secondDigit > 0)
				secondDigitStr = GetColumnId(secondDigit - 1);
			return secondDigitStr + ((char)((byte)firstDigit + 65)).ToString();
		}

		static string GetRange(int columnIndex, int rowIndex)
		{
			return $"{GetColumnId(columnIndex)}{rowIndex + 1}";
		}

		static string GetExistingValue(IList<IList<object>> allRows, int columnIndex, int rowIndex)
		{
			return allRows[rowIndex][columnIndex].ToString();
		}

		static bool HasMember(string[] filterMembers, string memberName)
		{
			if (filterMembers == null)
				return true;

			foreach (string filterMember in filterMembers)
				if (filterMember == memberName)
					return true;

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="docName"></param>
		/// <param name="tabName"></param>
		/// <param name="instances"></param>
		/// <param name="instanceType"></param>
		/// <param name="filterMembersStr">Comma-separated list of names of member properties to save.</param>
		public static void SaveChanges(string docName, string tabName, object[] instances, Type instanceType, string filterMembersStr = null)
		{
			if (instances == null || instances.Length == 0)
				return;

			if (!spreadsheetIDs.ContainsKey(docName))
				throw new InvalidDataException($"docName (\"{docName}\") not found!");

			if (sheetTabMap[docName].IndexOf(tabName) < 0)
				throw new InvalidDataException($"tabName (\"{tabName}\") not found!");

			string[] filterMembers = null;
			if (filterMembersStr != null)
			{
				filterMembers = filterMembersStr.Split(',');
			}

			string spreadsheetId = spreadsheetIDs[docName];
			List<string> headerRow;
			IList<IList<object>> allRows;
			GetHeaderRow(docName, tabName, out headerRow, out allRows);

			MemberInfo[] indexFields = GetFields<IndexerAttribute>(instanceType);

			MemberInfo[] serializableFields = GetFields<ColumnAttribute>(instanceType);

			for (int i = 0; i < instances.Length; i++)
			{
				int rowIndex = GetInstanceRowIndex(instances[i], indexFields, allRows, headerRow);
				for (int j = 0; j < serializableFields.Length; j++)
				{
					// TODO: Filtering on the serializableFields would go here.
					MemberInfo memberInfo = serializableFields[j];

					if (!HasMember(filterMembers, memberInfo.Name))
						continue;

					int columnIndex = GetColumnIndex(headerRow, GetColumnName<ColumnAttribute>(memberInfo));

					string existingValue = null;
					if (columnIndex < allRows[rowIndex].Count)  // Some rows may have fewer columns because efficiency of the Google Sheets engine.
						existingValue = GetExistingValue(allRows, columnIndex, rowIndex);

					string range = GetRange(columnIndex, rowIndex);
					
					ValueRange body = new ValueRange();
					body.MajorDimension = "ROWS";
					body.Range = $"{tabName}!{range}";
					body.Values = new List<IList<object>>();
					body.Values.Add(new List<object>());
					string value = GetValue(instances[i], memberInfo);
					
					if (existingValue == null /* New */ || value != existingValue /* Mod */)
					{
						body.Values[0].Add(value);
						SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(body, spreadsheetId, body.Range);
						request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
						try
						{
							UpdateValuesResponse response = request.Execute();
							if (response != null)
							{

							}
						}
						catch (Exception ex)
						{
							string msg = ex.Message;
							System.Diagnostics.Debugger.Break();
						}
					}
				}
			}
		}

		private static void GetHeaderRow(string docName, string tabName, out List<string> headerRow, out IList<IList<object>> allRows)
		{
			allRows = GetCells(docName, tabName);
			Dictionary<int, string> headers = new Dictionary<int, string>();
			IList<object> headerRowObjects = allRows[0];
			headerRow = headerRowObjects.Select(x => x.ToString()).ToList();
		}

		public static void SaveChanges(object[] instances, string filterMembers = null)
		{
			if (instances == null || instances.Length == 0)
				return;
			Type instanceType = instances[0].GetType();
			SheetNameAttribute sheetNameAttribute = instanceType.GetCustomAttribute<SheetNameAttribute>();
			if (sheetNameAttribute == null)
				throw new InvalidDataException($"{instanceType.Name} needs to specify the \"SheetName\" attribute.");
			TabNameAttribute tabNameAttribute = instanceType.GetCustomAttribute<TabNameAttribute>();
			if (tabNameAttribute == null)
				throw new InvalidDataException($"{instanceType.Name} needs to specify the \"TabName\" attribute.");

			SaveChanges(sheetNameAttribute.SheetName, tabNameAttribute.TabName, instances, instanceType, filterMembers);
		}

		public static void SaveChanges(object instance, string filterMember = null)
		{
			object[] array = { instance };
			SaveChanges(array, filterMember);
		}
	}
}
