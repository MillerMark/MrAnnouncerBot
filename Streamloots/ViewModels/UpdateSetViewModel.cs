using System;
using System.Linq;

namespace Streamloots
{
	public class UpdateSetViewModel
	{
		public bool craftableCards { get; set; }
		public string name { get; set; }
		public bool @default { get; set; }
		public string imageUrl { get; set; }

		public UpdateSetViewModel()
		{

		}
	}
}
