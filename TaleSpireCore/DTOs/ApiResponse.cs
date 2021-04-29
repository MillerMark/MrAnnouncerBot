
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ApiResponse
	{
		public ApiResponse()
		{
		}

		public object Data { get; set; }
		
		public static string Good(string message, object data)
		{
			ApiResponse apiResponse = new ApiResponse(message);
			apiResponse.Data = data;
			return apiResponse.ToString();
		}

		public ApiResponse(string message)
		{
			Message = message;
		}

		public ApiResponse(string errorMessage, string message)
		{
			Message = message;
			ErrorMessage = errorMessage;
		}

		public ApiResponse(string errorMessage, string message, object data)
		{
			Data = data;
			ErrorMessage = errorMessage;
			Message = message;
		}

		public string ErrorMessage { get; set; }

		public string Message { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
		}

		public static ApiResponse FromException(Exception e, string message = "")
		{
			if (string.IsNullOrEmpty(message))
				message = "Exception!";
			return new ApiResponse(e.Message, message);
		}

		public T GetData<T>() where T : class
		{
			if (Data is Newtonsoft.Json.Linq.JObject jobject)
				return jobject.ToObject<T>();
			return default;
		}
	}
}
