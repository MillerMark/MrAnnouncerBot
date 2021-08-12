
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TaleSpireCore
{
	public class ApiResponse
	{
		public ApiResponse()
		{
		}

		public object Data { get; set; }
		public ResponseType Result { get; set; }

		public static string Good(string message, object data)
		{
			ApiResponse apiResponse = new ApiResponse(message);
			apiResponse.Data = data;
			apiResponse.Result = ResponseType.Success;
			return apiResponse.ToString();
		}

		public static string Good()
		{
			return Good("Success", null);
		}

		public static string InvalidCommand(string command, [CallerMemberName] string memberName = "")
		{
			return new ApiResponse("Failure", $"Invalid command \"{command}\" sent to \"{memberName}\".").ToString();
		}

		public ApiResponse(string message)
		{
			Message = message;
		}

		public ApiResponse(string errorMessage, string message)
		{
			Message = message;
			ErrorMessage = errorMessage;
			Result = ResponseType.Failure;
		}

		public ApiResponse(string errorMessage, string message, object data)
		{
			Data = data;
			ErrorMessage = errorMessage;
			Message = message;
			Result = ResponseType.Failure;
		}

		public string ErrorMessage { get; set; }

		public string Message { get; set; }

		public override string ToString()
		{
			try
			{
				return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
			}
			catch (Exception ex)
			{
				Talespire.Log.Error($"Error serializing (check Data!).");
				Talespire.Log.Exception(ex);
				return string.Empty;
			}
		}

		public static ApiResponse FromException(Exception e, string message = "")
		{
			if (string.IsNullOrEmpty(message))
				message = "Exception!";
			return new ApiResponse(e.Message, message);
		}

		public T GetData<T>() where T : class
		{
			if (Data is Newtonsoft.Json.Linq.JObject jObject)
				return jObject.ToObject<T>();
			return default;
		}

		public List<T> GetList<T>()
		{
			if (Data is Newtonsoft.Json.Linq.JArray jArray)
				return jArray.ToObject<T[]>().ToList();
			return default;
		}

		public static string Bad(string message, [CallerMemberName] string callerName = "")
		{
			Talespire.Log.Error($"Error in {callerName} - " + message);
			ApiResponse apiResponse = new ApiResponse($"Error in {callerName}", message);
			apiResponse.Result = ResponseType.Failure;
			return apiResponse.ToString();
		}

		public int ToInt()
		{
			if (Data is Newtonsoft.Json.Linq.JObject jObject)
				return jObject.ToObject<int>();

			return 0;
		}
	}
}
