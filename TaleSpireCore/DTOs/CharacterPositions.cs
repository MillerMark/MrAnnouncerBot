using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public class CharacterPositions
	{
		public List<CharacterPosition> Characters { get; set; }
		public CharacterPositions()
		{
			Characters = new List<CharacterPosition>();
		}
	}
}
