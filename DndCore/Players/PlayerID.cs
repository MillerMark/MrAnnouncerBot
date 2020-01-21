using System;
using System.Linq;

namespace DndCore
{
	public static class PlayerID
	{
		public const int Lady = 0;
		public const int Shemo = 1;
		public const int Merkin = 2;
		public const int LilCutie = 3;
		public const int Fred = 4;
		public const int Willy = 5;
		
		public static int FromName(string name)
		{
			if (name == null)
				return -1;

			string lowerCaseName = name.ToLower();
			if (lowerCaseName.StartsWith("willy"))
				return Willy;
			if (lowerCaseName.StartsWith("shemo"))
				return Shemo;
			if (lowerCaseName.StartsWith("merkin"))
				return Merkin;
			if (lowerCaseName.StartsWith("ava") || lowerCaseName.StartsWith("l'il"))
				return LilCutie;
			if (lowerCaseName.StartsWith("fred"))
				return Fred;
			if (lowerCaseName.StartsWith("lady"))
				return Lady;
			return -1;
		}
	}
}
