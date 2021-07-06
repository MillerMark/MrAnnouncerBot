using System;
using System.Linq;

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
		}
	}
}