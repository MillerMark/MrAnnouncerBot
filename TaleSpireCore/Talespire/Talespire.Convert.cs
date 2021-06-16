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
		}
	}
}