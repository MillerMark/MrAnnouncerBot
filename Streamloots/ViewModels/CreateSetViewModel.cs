using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class CreateSetViewModel : UpdateSetViewModel
	{
		public string slug { get; set; }
		public List<SetLootViewModel> loots { get; set; }
		public List<ProfileViewModel> rarities { get; set; }
		public CreateSetViewModel()
		{

		}
	}
}
