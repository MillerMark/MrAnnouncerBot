using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class FileViewModel
	{
		public string _id { get; set; }
		public string createdAt { get; set; }
		public string encoding { get; set; }
		public string fileName { get; set; }
		public string filePath { get; set; }
		public string fileUri { get; set; }
		public string metadata { get; set; }  // not seeing this in the card data coming back from Streamloots
		public string mimeType { get; set; }
		public string modifiedAt { get; set; }
		public string name { get; set; }
		public string originalName { get; set; }
		public string ownerId { get; set; }
		public int size { get; set; }
		public FileViewModel()
		{

		}
	}
}
