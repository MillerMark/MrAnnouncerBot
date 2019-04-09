using System;
using System.Linq;

namespace DndCore
{
	public static class Dice
	{
		public const string NoRoll = "";

		public const string d4x1 = "1d4";
		public const string d4x2 = "2d4";
		public const string d4x3 = "3d4";
		public const string d4x4 = "4d4";
		public const string d4x5 = "5d4";
		public const string d4x6 = "6d4";
		public const string d4x7 = "7d4";
		public const string d4x8 = "8d4";
		public const string d4x9 = "9d4";
		public const string d4x10 = "10d4";
		public const string d4x11 = "11d4";
		public const string d4x12 = "12d4";

		public const string d6x1 = "1d6";
		public const string d6x2 = "2d6";
		public const string d6x3 = "3d6";
		public const string d6x4 = "4d6";
		public const string d6x5 = "5d6";
		public const string d6x6 = "6d6";
		public const string d6x7 = "7d6";
		public const string d6x8 = "8d6";
		public const string d6x9 = "9d6";
		public const string d6x10 = "10d6";
		public const string d6x11 = "11d6";
		public const string d6x12 = "12d6";

		public const string d8x1 = "1d8";
		public const string d8x2 = "2d8";
		public const string d8x3 = "3d8";
		public const string d8x4 = "4d8";
		public const string d8x5 = "5d8";
		public const string d8x6 = "6d8";
		public const string d8x7 = "7d8";
		public const string d8x8 = "8d8";
		public const string d8x9 = "9d8";
		public const string d8x10 = "10d8";
		public const string d8x11 = "11d8";
		public const string d8x12 = "12d8";

		public const string d10x1 = "1d10";
		public const string d10x2 = "2d10";
		public const string d10x3 = "3d10";
		public const string d10x4 = "4d10";
		public const string d10x5 = "5d10";
		public const string d10x6 = "6d10";
		public const string d10x7 = "7d10";
		public const string d10x8 = "8d10";
		public const string d10x9 = "9d10";
		public const string d10x10 = "10d10";
		public const string d10x11 = "11d10";
		public const string d10x12 = "12d10";

		public const string d12x1 = "1d12";
		public const string d12x2 = "2d12";
		public const string d12x3 = "3d12";
		public const string d12x4 = "4d12";
		public const string d12x5 = "5d12";
		public const string d12x6 = "6d12";
		public const string d12x7 = "7d12";
		public const string d12x8 = "8d12";
		public const string d12x9 = "9d12";
		public const string d12x10 = "10d12";
		public const string d12x11 = "11d12";
		public const string d12x12 = "12d12";

		public const string d20x1 = "1d20";
		public const string d20x2 = "2d20";
		public const string d20x3 = "3d20";
		public const string d20x4 = "4d20";
		public const string d20x5 = "5d20";
		public const string d20x6 = "6d20";
		public const string d20x7 = "7d20";
		public const string d20x8 = "8d20";
		public const string d20x9 = "9d20";
		public const string d20x10 = "10d20";
		public const string d20x11 = "11d20";
		public const string d20x12 = "12d20";

		public static string Plus(this string str, int offset)
		{
			if (offset == 0)
				return str;

			return str + "+" + offset.ToString();
		}
	}
}

