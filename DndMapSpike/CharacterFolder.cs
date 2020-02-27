using System;
using MapCore;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public static class CharacterFinder
	{
		public static void FindCharacters(string path, List<MapCharacter> Characters)
		{
			string[] files = System.IO.Directory.GetFiles(path, "*_Stand7.png");
			foreach (string file in files)
				Characters.Add(new MapCharacter(file));
		}
	}
}

