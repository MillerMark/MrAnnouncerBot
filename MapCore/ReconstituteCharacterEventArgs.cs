using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class ReconstituteCharacterEventArgs : EventArgs
	{
		public List<IItemProperties> Characters { get; set; }
		public SerializedCharacter SerializedCharacter { get; set; }
		public ReconstituteCharacterEventArgs()
		{

		}
	}
}
