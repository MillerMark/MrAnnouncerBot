using Newtonsoft.Json;
using System;

namespace CommonCore
{
	public class Point2dConverter : JsonConverter<Point2d>
	{
		public override void WriteJson(JsonWriter writer, Point2d value, JsonSerializer serializer)
		{
			writer.WriteValue($"{value.X}, {value.Y}");
		}

		double StrToDouble(string str)
		{
			if (double.TryParse(str.Trim(), out double result))
				return result;
			return 0d;
		}

		public override Point2d ReadJson(JsonReader reader, Type objectType, Point2d existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string str = (string)reader.Value;
			string[] parts = str.Split(',');
			if (parts.Length == 2)
			{
				double x = StrToDouble(parts[0]);
				double y = StrToDouble(parts[1]);
				return new Point2d(x, y);
			}
			throw new Exception($"Invalid Point2d format - \"{str}\"");
		}

	}
}
