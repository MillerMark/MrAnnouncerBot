using System;
using System.Linq;

namespace DndCore
{
	public class DndRoom
	{

		public DndRoom()
		{
		}

		public DndRoom(string name)
		{
			Name = name;
		}

		public DndMap Map { get; set; }

		public string Name { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		Vector GetWorldPosition(int x, int y)
		{
			return Map.GetWorldPosition(X + x, Y + y);
		}
		public void PositionCreatures(string str)
		{
			string[] crlf = { "\r\n" };
			string[] lines = str.Split(crlf, StringSplitOptions.None);

			bool firstLineIsEmpty = lines[0] == string.Empty;
			int yOffset = 0;
			if (firstLineIsEmpty)
				yOffset = -1;
			for (int y = 0; y < lines.Length; y++)
			{
				string line = lines[y];
				for (int x = 0; x < line.Length; x++)
				{
					char creatureName = line[x];
					if (char.IsLetterOrDigit(creatureName))
					{
						Character foundCharacter = Map.Game.Players.FirstOrDefault(player => player.name == creatureName.ToString());
						if (foundCharacter != null)
						{
							foundCharacter.SetWorldPosition(GetWorldPosition(x, y + yOffset));
						}

					}
				}
			}
			// TODO: Implement this!
		}
	}
}
