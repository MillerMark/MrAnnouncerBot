using System;
using System.Linq;

namespace MapCore
{
	public class MapCharacter : BaseItemProperties
	{
		public override double Height { get; set; }
		public override double Width { get; set; }

		public MapCharacter()
		{

		}

		public MapCharacter(string fileName)
		{
			FileName = fileName;
		}
	}
}
