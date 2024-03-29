﻿using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Convert
		{
			const float FeetPerTile = 5f;
			
			public static float FeetToTiles(float feet)
			{
				return feet / FeetPerTile;
			}

			public static float TilesToFeet(float distanceTiles)
			{
				return distanceTiles * FeetPerTile;
			}

			public static Vector3 ToVector3(string vectorStr)
			{
				VectorDto vectorDto = ToVectorDto(vectorStr);
				return vectorDto.GetVector3();
			}

			public static VectorDto ToVectorDto(string vectorStr)
			{
				string[] parts = vectorStr.Split(',');
				float x = 0;
				float y = 0;
				float z = 0;

				if (parts.Length == 3)
					if (float.TryParse(parts[0], out x))
						if (float.TryParse(parts[1], out y))
							if (float.TryParse(parts[2], out z))
								return new VectorDto(x, y, z);

				Log.Error($"Error parsing vector string \"{vectorStr}\" to Vector.");
				return new VectorDto(x, y, z);
			}

			private static T GetElement<T>(string elementName) where T : struct
			{
				if (Enum.TryParse(elementName.Trim().Replace(" ", ""), true, out T result))
					return result;
				else
					return default;
			}

			public static TargetVolume ToTargetVolume(string shapeName)
			{
				return GetElement<TargetVolume>(shapeName);
			}

			public static FireCollisionEventOn ToFireCollisionEventOn(string str)
			{
				return GetElement<FireCollisionEventOn>(str);
			}

			public static ProjectileKind ToProjectileKind(string str)
			{
				return GetElement<ProjectileKind>(str);
			}

			public static ProjectileSizeOption ToProjectileSizeOption(string str)
			{
				return GetElement<ProjectileSizeOption>(str);
			}

			public static float ToFloat(string str, float defaultValue = 0)
			{
				if (float.TryParse(str, out float result))
					return result;
				return defaultValue;
			}

			public static int ToInt(string str, int defaultValue = 0)
			{
				if (int.TryParse(str, out int result))
					return result;
				return defaultValue;
			}

			/// <summary>
			/// Converts the specified string to a number. If the string ends with "ft", 
			/// this method also converts from feet to tiles before returning that number.
			/// </summary>
			public static float ToDistanceTiles(string str)
			{
				bool inFeet = false;
				if (str.EndsWith("ft"))
				{
					inFeet = true;
					str = str.Substring(0, str.Length - 2);
				}
				if (float.TryParse(str, out float rightSideValue))
				{
					if (inFeet)
						return FeetToTiles(rightSideValue);
					else
						return rightSideValue;
				}
				Log.Error($"Unable to convert \"{str}\" to a number.");
				return 0;
			}

			public static float HoursToNormalizedTime(double totalHours)
			{
				float normalizedTime = (float)(totalHours / 24.0 + 0.25);
				if (normalizedTime > 1)
					normalizedTime -= 1;
				return normalizedTime;
			}
		}
	}
}