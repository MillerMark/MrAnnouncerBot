using System;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class StampFolder
	{
		public string Path { get; set; }
		public string Name { get; set; }
		public List<Stamp> Stamps { get; set; }
		public StampFolder(string path)
		{
			Path = path;
			Name = System.IO.Path.GetFileName(path);
			string[] files = System.IO.Directory.GetFiles(path, "*.png");
			Stamps = new List<Stamp>();
			foreach (string file in files)
			{
				Stamps.Add(new Stamp(file));
			}
			
		}
	}
}

