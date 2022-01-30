using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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

			SpreadsheetsResource.ValuesResource.GetRequest request = Service.Spreadsheets.Values.Get(spreadsheetId, range);
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
				// TODO: Consider re-throwing the error before publishing.
#pragma warning disable CS0168  // Used for diagnostics/debugging.
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
					case "System.Double":
						if (double.TryParse(value, out double doubleValue))
							property.SetValue(instance, doubleValue);
						else
						{
							// TODO: Consider specifying default values through attributes for given properties.
							property.SetValue(instance, 0);
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
					case "System.DateTime":
						if (DateTime.TryParse(value, out DateTime date))
							property.SetValue(instance, date);
						break;
					default:
						SetValue(property, instance, value);
						break;
				}
			}

			SetDefaultsForEmptyCells(instance, headers, row, type);
		}

		static object GetDefaultValue(PropertyInfo property)
		{
			IEnumerable<DefaultAttribute> customAttributes = property.GetCustomAttributes<DefaultAttribute>();

			DefaultAttribute defaultAttribute = null;
			if (customAttributes.Any())
				defaultAttribute = customAttributes.First();
			if (defaultAttribute != null)
				return defaultAttribute.DefaultValue;

			switch (property.PropertyType.FullName)
			{
				case "System.Int32":
					return default(int);
				case "System.String":
					return string.Empty;
				case "System.Boolean":
					return default(bool);
				case "System.Decimal":
					return default(decimal);
				case "System.Double":
					return default(double);
				case "System.DateTime":
					return default(DateTime);
				default:
					if (property.PropertyType.BaseType.FullName == "System.Enum")
						return 0;
					else
						System.Diagnostics.Debugger.Break();
					break;
			}
			return null;
		}
		private static void SetDefaultsForEmptyCells<T>(T instance, Dictionary<int, string> headers, IList<object> row, Type type) where T : new()
		{
			for (int i = row.Count; i < headers.Count; i++)  // There may be fewer rows than headers.
			{
				PropertyInfo property = type.GetProperty(headers[i]);
				if (property == null)
					continue;

				object defaultValue = GetDefaultValue(property);
				property.SetValue(instance, defaultValue);
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
				string title = $"{ex.GetType()} reading google sheet ({docName} - {tabName}): ";
				while (ex != null)
				{
					Console.WriteLine($"{title} \"{ex.Message}\"");
					ex = ex.InnerException;
					if (ex != null)
						title = $"Inner {ex.GetType()}: ";
				}
			}
			return result;
		}

		static SheetsService service;

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
				ClientSecrets secrets = GoogleClientSecrets.FromStream(stream).Secrets;
				FileDataStore dataStore = new FileDataStore(credentialPath, true);
				Debug.WriteLine("GoogleWebAuthorizationBroker.AuthorizeAsync...");
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, "user", CancellationToken.None, dataStore).Result;
				Debug.WriteLine("Credential file saved to: " + credentialPath);
			}

			return credential;
		}

		public static void RegisterSpreadsheetID(string sheetName, string sheetID)
		{
			spreadsheetIDs[sheetName] = sheetID;
		}

		static MemberInfo[] GetSerializableFields<TAttribute>(Type instanceType) where TAttribute : Attribute
		{
			List<MemberInfo> memberInfo = new List<MemberInfo>();
			IEnumerable<PropertyInfo> properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<TAttribute>() != null);
			IEnumerable<FieldInfo> fields = instanceType.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<TAttribute>() != null);
			memberInfo.AddRange(properties);
			memberInfo.AddRange(fields);
			return memberInfo.ToArray();
		}

		static string GetColumnName<TColumnAttribute>(MemberInfo memberInfo) where TColumnAttribute : ColumnNameAttribute
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
					if (column >= thisRow.Count)
					{
						found = false;
						break;
					}
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
			object obj = allRows[rowIndex][columnIndex];
			if (obj == null)
				return string.Empty;
			return obj.ToString();
		}

		static bool HasMember(string[] memberList, string memberName)
		{
			if (memberList == null)
				return true;

			foreach (string filterMember in memberList)
				if (filterMember == memberName)
					return true;

			return false;
		}

		static void AddInstance(IList<IList<object>> allRows, List<string> headerRow, MemberInfo[] serializableFields, object instance)
		{
			object[] row = new object[headerRow.Count];
			foreach (MemberInfo memberInfo in serializableFields)
			{
				int columnIndex = GetColumnIndex(headerRow, GetColumnName<ColumnAttribute>(memberInfo));
				row[columnIndex] = GetValue(instance, memberInfo);
			}
			allRows.Add(row.ToList());
		}

		static int AddRow(string tabName, MemberInfo[] indexFields, List<string> headerRow, MemberInfo[] serializableFields, IList<IList<object>> allRows, object instance, BatchUpdateValuesRequest requestBody)
		{
			foreach (MemberInfo memberInfo in indexFields)
			{
				int columnIndex = GetColumnIndex(headerRow, GetColumnName<ColumnAttribute>(memberInfo));
				int rowIndex = allRows.Count;
				ValueRange body = GetValueRange(tabName, columnIndex, rowIndex, GetValue(instance, memberInfo));

				requestBody.Data.Add(body);
			}

			AddInstance(allRows, headerRow, serializableFields, instance);
			return allRows.Count - 1;
		}

		private static ValueRange GetValueRange(string tabName, int columnIndex, int rowIndex, string value)
		{
			string range = GetRange(columnIndex, rowIndex);

			ValueRange body = new ValueRange();
			body.MajorDimension = "ROWS";
			body.Range = $"{tabName}!{range}";
			body.Values = new List<IList<object>>();
			body.Values.Add(new List<object>());
			body.Values[0].Add(value);
			return body;
		}

		private static void ExecuteBatchUpdate(string spreadsheetId, BatchUpdateValuesRequest requestBody)
		{
			SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = Service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);
			Execute(request);
		}

		private static BatchUpdateValuesRequest GetBatchUpdateRequest()
		{
			BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
			requestBody.Data = new List<ValueRange>();
			requestBody.ValueInputOption = "USER_ENTERED";
			return requestBody;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="docName"></param>
		/// <param name="tabName"></param>
		/// <param name="instances"></param>
		/// <param name="instanceType"></param>
		/// <param name="saveOnlyTheseMembersStr">Comma-separated list of the names of the member properties to save.</param>
		public static void SaveChanges(string docName, string tabName, object[] instances, Type instanceType, string saveOnlyTheseMembersStr = null)
		{
			if (instances == null || instances.Length == 0)
				return;

			ValidateSheetAndTabNames(docName, tabName, true);

			string[] saveOnlyTheseMembers = null;
			if (saveOnlyTheseMembersStr != null)
			{
				saveOnlyTheseMembers = saveOnlyTheseMembersStr.Split(',');
			}

			string spreadsheetId = spreadsheetIDs[docName];
			List<string> headerRow;
			IList<IList<object>> allRows;
			GetHeaderRow(docName, tabName, out headerRow, out allRows);

			MemberInfo[] indexFields = GetSerializableFields<IndexerAttribute>(instanceType);

			MemberInfo[] serializableFields = GetSerializableFields<ColumnAttribute>(instanceType);

			BatchUpdateValuesRequest requestBody = GetBatchUpdateRequest();
			for (int i = 0; i < instances.Length; i++)
			{
				int rowIndex = GetInstanceRowIndex(instances[i], indexFields, allRows, headerRow);
				if (rowIndex < 0)
					AddRow(tabName, serializableFields, headerRow, serializableFields, allRows, instances[i], requestBody);
				else
					UpdateRow(tabName, instances, saveOnlyTheseMembers, headerRow, allRows, serializableFields, i, rowIndex, requestBody);
			}

			ExecuteBatchUpdate(spreadsheetId, requestBody);
		}

		private static void ValidateSheetAndTabNames(string sheetName, string tabName, bool trackTabIfMissing = false)
		{
			ValidateSheetName(sheetName);

			if (trackTabIfMissing)
				Track(sheetName, tabName);
			else if (sheetTabMap[sheetName].IndexOf(tabName) < 0)
				throw new InvalidDataException($"tabName (\"{tabName}\") not found!");
		}

		private static void ValidateSheetName(string sheetName)
		{
			if (!spreadsheetIDs.ContainsKey(sheetName))
				throw new InvalidDataException($"docName (\"{sheetName}\") not found!");
		}

		private static void UpdateRow(string tabName, object[] instances, string[] saveOnlyTheseMembers, List<string> headerRow, IList<IList<object>> allRows, MemberInfo[] serializableFields, int i, int rowIndex, BatchUpdateValuesRequest requestBody)
		{
			for (int j = 0; j < serializableFields.Length; j++)
			{
				MemberInfo memberInfo = serializableFields[j];

				if (!HasMember(saveOnlyTheseMembers, memberInfo.Name))
					continue;

				if (instances[i] is ITrackPropertyChanges trackPropertyChanges && trackPropertyChanges.ChangedProperties != null)
				{
					if (trackPropertyChanges.ChangedProperties.IndexOf(memberInfo.Name) < 0)
						continue;
				}

				int columnIndex = GetColumnIndex(headerRow, GetColumnName<ColumnAttribute>(memberInfo));

				string existingValue = null;
				if (columnIndex < allRows[rowIndex].Count)  // Some rows may have fewer columns because the Google Sheets engine will efficiently return only the columns holding data.
					existingValue = GetExistingValue(allRows, columnIndex, rowIndex);

				string value = GetValue(instances[i], memberInfo);

				if (existingValue == null /* New */ || value != existingValue /* Mod */)
				{
					//ValueRange body = new ValueRange();
					//body.MajorDimension = "ROWS";
					//body.Range = $"{tabName}!{range}";
					//body.Values = new List<IList<object>>();
					//body.Values.Add(new List<object>());
					//body.Values[0].Add(value);
					ValueRange body = GetValueRange(tabName, columnIndex, rowIndex, value);

					requestBody.Data.Add(body);
					//SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(body, spreadsheetId, body.Range);
					//Execute(request);
				}
			}
		}

		private static void Execute(SpreadsheetsResource.ValuesResource.UpdateRequest request)
		{
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

		private static void Execute(SpreadsheetsResource.ValuesResource.BatchUpdateRequest request)
		{
			try
			{
				BatchUpdateValuesResponse response = request.Execute();
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

		private static void GetHeaderRow(string docName, string tabName, out List<string> headerRow, out IList<IList<object>> allRows)
		{
			allRows = GetCells(docName, tabName);
			Dictionary<int, string> headers = new Dictionary<int, string>();
			IList<object> headerRowObjects = allRows[0];
			headerRow = headerRowObjects.Select(x => x.ToString()).ToList();
		}

		public static void SaveChanges(object[] instances, string saveOnlyTheseMembersStr = null)
		{
			if (instances == null || instances.Length == 0)
				return;
			Type instanceType = instances[0].GetType();

			GetSheetTabAttributes(instanceType, out SheetNameAttribute sheetNameAttribute, out TabNameAttribute tabNameAttribute);

			SaveChanges(sheetNameAttribute.SheetName, tabNameAttribute.TabName, instances, instanceType, saveOnlyTheseMembersStr);
		}

		private static void GetSheetTabAttributes(Type instanceType, out SheetNameAttribute sheetNameAttribute, out TabNameAttribute tabNameAttribute)
		{
			sheetNameAttribute = GetSheetAttributes(instanceType);

			tabNameAttribute = instanceType.GetCustomAttribute<TabNameAttribute>();
			if (tabNameAttribute == null)
				throw new InvalidDataException($"{instanceType.Name} needs to specify the \"TabName\" attribute.");
		}

		private static SheetNameAttribute GetSheetAttributes(Type instanceType)
		{
			SheetNameAttribute sheetNameAttribute = instanceType.GetCustomAttribute<SheetNameAttribute>();
			if (sheetNameAttribute == null)
				throw new InvalidDataException($"{instanceType.Name} needs to specify the \"SheetName\" attribute.");
			return sheetNameAttribute;
		}

		public static void SaveChanges(object instance, string filterMember = null)
		{
			object[] array = { instance };
			SaveChanges(array, filterMember);
		}

		static int? GetTabId(string spreadsheetId, string tabName)
		{
			Spreadsheet spreadsheet = Service.Spreadsheets.Get(spreadsheetId).Execute();

			foreach (Sheet sheet in spreadsheet.Sheets)
				if (sheet.Properties.Title == tabName)
					return sheet.Properties.SheetId;
			return null;
		}

		static void DeleteRowByIndexInSheet(string spreadsheetId, string tabName, int rowIndex)
		{
			if (rowIndex < 0)
				return;

			int? sheetId = GetTabId(spreadsheetId, tabName);
			Request RequestBody = new Request()
			{
				DeleteDimension = new DeleteDimensionRequest()
				{
					Range = new DimensionRange()
					{
						SheetId = sheetId,
						Dimension = "ROWS",
						StartIndex = Convert.ToInt32(rowIndex),
						EndIndex = Convert.ToInt32(rowIndex + 1)
					}
				}
			};

			List<Request> RequestContainer = new List<Request>();
			RequestContainer.Add(RequestBody);

			BatchUpdateSpreadsheetRequest deleteRequest = new BatchUpdateSpreadsheetRequest();
			deleteRequest.Requests = RequestContainer;

			SpreadsheetsResource.BatchUpdateRequest batchUpdate = Service.Spreadsheets.BatchUpdate(deleteRequest, spreadsheetId);
			batchUpdate.Execute();
		}

		static void DeleteRowInSheet(object instance, string sheetName, string tabName)
		{
			Type instanceType = instance.GetType();
			ValidateSheetAndTabNames(sheetName, tabName);
			string spreadsheetId = spreadsheetIDs[sheetName];
			List<string> headerRow;
			IList<IList<object>> allRows;
			GetHeaderRow(sheetName, tabName, out headerRow, out allRows);

			MemberInfo[] indexFields = GetSerializableFields<IndexerAttribute>(instanceType);

			int rowIndex = GetInstanceRowIndex(instance, indexFields, allRows, headerRow);
			DeleteRowByIndexInSheet(spreadsheetId, tabName, rowIndex);
		}

		public static void DeleteRow(object instance)
		{
			GetSheetTabAttributes(instance.GetType(), out SheetNameAttribute sheetNameAttribute, out TabNameAttribute tabNameAttribute);
			DeleteRowInSheet(instance, sheetNameAttribute.SheetName, tabNameAttribute.TabName);
		}

		static void ExecuteAppendRows(string spreadsheetId, string tabName, IList<IList<object>> values)
		{
			SpreadsheetsResource.ValuesResource.AppendRequest request = GetAppendRequest(spreadsheetId, tabName, values);
			try
			{
				var response = request.Execute();
			}
			catch (Exception ex)
			{

			}
		}

		private static SpreadsheetsResource.ValuesResource.AppendRequest GetAppendRequest(string spreadsheetId, string tabName, IList<IList<object>> values)
		{
			SpreadsheetsResource.ValuesResource.AppendRequest request =
												 Service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, tabName);
			request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
			return request;
		}

		static void ExecuteInsertRows(string spreadsheetId, string tabName, IList<IList<object>> values, int rowStartIndex = 0)
		{
			// TODO: Untested. Test this.
			BatchUpdateValuesRequest requestBody = GetBatchUpdateRequest();
			InsertDimensionRequest insertDimensionRequest = new InsertDimensionRequest();
			insertDimensionRequest.InheritFromBefore = false;
			insertDimensionRequest.Range.StartIndex = rowStartIndex;
			insertDimensionRequest.Range.StartIndex = rowStartIndex + values.Count;
			insertDimensionRequest.Range.Dimension = "ROWS";
			SpreadsheetsResource.ValuesResource.AppendRequest appendRequest =
									 Service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, tabName);
			appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
			try
			{
				var response = appendRequest.Execute();
			}
			catch (Exception ex)
			{
				ExecuteBatchUpdate(spreadsheetId, requestBody);
			}
		}

		public static void MakeSureTabExists<T>(string tabName)
		{
			if (!TabExists<T>(tabName))
				AddTab<T>(tabName);
		}

		private static string GetSheetName<T>()
		{
			SheetNameAttribute sheetNameAttribute = GetSheetAttributes(typeof(T));
			return sheetNameAttribute.SheetName;
		}

		public static bool TabExists<T>(string tabName)
		{
			return TabExists(GetSheetName<T>(), tabName);
		}

		public static bool TabExists(string sheetName, string tabName)
		{
			ValidateSheetName(sheetName);
			return GetTabId(spreadsheetIDs[sheetName], tabName) != null;
		}

		/// <summary>
		/// Adds a new specified tab to the spreadsheet connected with this element.
		/// </summary>
		/// <param name="tabName"></param>
		public static void AddTab<T>(string tabName)
		{
			string sheetName = GetSheetName<T>();
			AddSheetRequest addSheetRequest = new AddSheetRequest();
			SheetProperties sheetProperties = new SheetProperties() { Title = tabName };

			addSheetRequest.Properties = sheetProperties;

			BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();

			//Create requestList and set it on the batchUpdateSpreadsheetRequest
			List<Request> requestsList = new List<Request>();
			batchUpdateSpreadsheetRequest.Requests = requestsList;

			//Create a new request with containing the addSheetRequest and add it to the requestList
			Request request = new Request();
			request.AddSheet = addSheetRequest;
			requestsList.Add(request);

			//Add the requestList to the batchUpdateSpreadsheetRequest
			batchUpdateSpreadsheetRequest.Requests = requestsList;

			//Call the sheets API to execute the batchUpdate
			string spreadsheetId = spreadsheetIDs[sheetName];
			Service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId).Execute();

			// TODO: To optimize for performance, consider rolling this next ExecuteAppendRow call into the previous batch update.

			List<object> columns = new List<object>();
			MemberInfo[] serializableFields = GetSerializableFields<ColumnAttribute>(typeof(T));
			foreach (MemberInfo memberInfo in serializableFields)
			{
				ColumnAttribute columnAttribute = memberInfo.GetCustomAttribute<ColumnAttribute>();
				if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.ColumnName))
					columns.Add(columnAttribute.ColumnName);
				else
					columns.Add(memberInfo.Name);
			}

			List<IList<object>> rows = new List<IList<object>>();
			rows.Add(columns);
			Console.WriteLine("ExecuteAppendRows(spreadsheetId, tabName, rows);");
			ExecuteAppendRows(spreadsheetId, tabName, rows);
			Console.WriteLine("AddColumnComments(spreadsheetId, tabName, serializableFields);");
			AddColumnComments(spreadsheetId, tabName, serializableFields);
			Console.WriteLine("Done!");
		}

		private static void AddColumnComments(string spreadsheetId, string tabName, MemberInfo[] serializableFields)
		{
			int columnIndex = 0;
			IList<Request> requests = new List<Request>();

			foreach (MemberInfo memberInfo in serializableFields)
			{
				NoteAttribute commentAttribute = memberInfo.GetCustomAttribute<NoteAttribute>();
				if (commentAttribute != null && !string.IsNullOrEmpty(commentAttribute.ColumnNote))
					AddNote(requests, columnIndex, commentAttribute.ColumnNote, spreadsheetId, tabName);
				columnIndex++;
			}

			if (requests != null && requests.Count > 0)
			{
				BatchUpdateSpreadsheetRequest body = new BatchUpdateSpreadsheetRequest();
				body.Requests = requests;

				SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = Service.Spreadsheets.BatchUpdate(body, spreadsheetId);
				if (batchUpdateRequest != null)
				{
					BatchUpdateSpreadsheetResponse response = batchUpdateRequest.Execute();
				}
			}
		}

		private static void AddNote(IList<Request> requests, int columnIndex, string comment, string spreadsheetId, string tabName)
		{
			Request commentRequest = new Request();
			commentRequest.UpdateCells = new UpdateCellsRequest();
			commentRequest.UpdateCells.Range = new GridRange();
			commentRequest.UpdateCells.Range.SheetId = GetTabId(spreadsheetId, tabName);
			commentRequest.UpdateCells.Range.StartRowIndex = 0;
			commentRequest.UpdateCells.Range.EndRowIndex = 1;
			commentRequest.UpdateCells.Range.StartColumnIndex = columnIndex;
			commentRequest.UpdateCells.Range.EndColumnIndex = columnIndex + 1;
			commentRequest.UpdateCells.Rows = new List<RowData>();
			commentRequest.UpdateCells.Rows.Add(new RowData());
			commentRequest.UpdateCells.Rows[0].Values = new List<CellData>();
			commentRequest.UpdateCells.Rows[0].Values.Add(new CellData());
			commentRequest.UpdateCells.Rows[0].Values[0].Note = comment;
			commentRequest.UpdateCells.Fields = "note";

			//CellData cellData = new CellData();
			//cellData.Note = comment;
			//commentRequest.RepeatCell.Cell = cellData;
			//commentRequest.RepeatCell.Fields = "note";
			requests.Add(commentRequest);


			//// Building a request for update cells.
			//request.UpdateCells = new Google.Apis.Sheets.v4.Data.UpdateCellsRequest();
			//request.UpdateCells.Range = gridRange;
			//request.UpdateCells.Fields = "note";
			//request.UpdateCells.Rows = new List<Google.Apis.Sheets.v4.Data.RowData>();
			//request.UpdateCells.Rows.Add(new Google.Apis.Sheets.v4.Data.RowData());
			//request.UpdateCells.Rows[0].Values = new List<Google.Apis.Sheets.v4.Data.CellData>();
			//request.UpdateCells.Rows[0].Values.Add(new Google.Apis.Sheets.v4.Data.CellData());
			//request.UpdateCells.Rows[0].Values[0].Note = noteMessage;
		}

		/// <summary>
		/// Appends a row representing the specified instance, writing its field values out to the spreadsheet. Make sure 
		/// your instance class has the [SheetName] attribute (and that sheet has been registered with <cref>RegisterSpreadsheetID</cref>), 
		/// and add the [Column] attribute to any members that you want to write out to the spreadsheet.
		/// Messages sent here are throttled so as not to exceed the Google sheets per-minute usage limits. The time to wait 
		/// between message bursts is determined by the <cref>TimeBetweenThrottledUpdates</cref> property.
		/// </summary>
		/// <typeparam name="T">The type to append</typeparam>
		/// <param name="instance">The instance of the type containing the writable data (in its public fields and properties marked with the [Column] attribute).</param>
		/// <param name="tabNameOverride">An optional override for the tab name.</param>
		public static void AppendRow<T>(T instance, string tabNameOverride = null) where T : class
		{
			if (!messageThrottlers.ContainsKey(typeof(T)))
				messageThrottlers[typeof(T)] = new MessageThrottler<T>(TimeBetweenThrottledUpdates);

			if (messageThrottlers[typeof(T)] is MessageThrottler<T> throttler)
				throttler.AppendRow(instance, tabNameOverride);
		}

		/// <summary>
		/// Sends any throttled messages at once to GoogleSheets. Call when shutting down to make sure any messages in the 
		/// queue are written to the spreadsheet.
		/// </summary>
		public static void FlushAllMessages()
		{
			foreach (Type type in messageThrottlers.Keys)
			{
				Type throttlerType = messageThrottlers[type].GetType();
				MethodInfo methodDefinition = throttlerType.GetMethod(nameof(MessageThrottler<object>.FlushAllMessages), new Type[] { });
				methodDefinition.Invoke(messageThrottlers[type], new object[] { });
			}
		}

		static GoogleSheets()
		{
			InitialService(GetUserCredentials());
		}

		public static TimeSpan TimeBetweenThrottledUpdates { get; set; } = TimeSpan.FromSeconds(5);
		public static SheetsService Service
		{
			get
			{
				if (service == null)
				{
					InitialService(GetUserCredentials());
				}
				return service;
			}
		}

		static Dictionary<Type, object> messageThrottlers = new Dictionary<Type, object>();


		internal static void InternalAppendRows<T>(T[] instances, string tabNameOverride = null, string sheetNameOverride = null) where T : class
		{
			if (instances == null || instances.Length == 0)
				return;

			GetSheetTabAttributes(typeof(T), out SheetNameAttribute sheetNameAttribute, out TabNameAttribute tabNameAttribute);

			string sheetName;
			string tabName;
			if (sheetNameOverride != null)
				sheetName = sheetNameOverride;
			else
				sheetName = sheetNameAttribute.SheetName;

			if (tabNameOverride != null)
				tabName = tabNameOverride;
			else
				tabName = tabNameAttribute.TabName;

			Track(sheetName, tabName);
			ValidateSheetAndTabNames(sheetName, tabName);
			string spreadsheetId = spreadsheetIDs[sheetName];

			IList<IList<Object>> values = new List<IList<Object>>();
			foreach (object instance in instances)
			{
				IList<Object> row = new List<Object>();
				MemberInfo[] serializableFields = GetSerializableFields<ColumnAttribute>(typeof(T));
				foreach (MemberInfo memberInfo in serializableFields)
					row.Add(GetValue(instance, memberInfo));
				values.Add(row);
			}

			ExecuteAppendRows(spreadsheetId, tabName, values);
		}
	}
}

