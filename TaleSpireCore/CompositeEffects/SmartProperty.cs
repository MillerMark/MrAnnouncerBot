using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public class SmartProperty
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public List<string> PropertyPaths { get; set; } = new List<string>();
		
		public SmartProperty()
		{

		}

		public SmartProperty(string name, string type)
		{
			Type = type;
			Name = name;
		}
	}
}
