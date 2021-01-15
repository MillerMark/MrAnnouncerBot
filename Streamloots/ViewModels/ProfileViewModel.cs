using System;
using System.Linq;

namespace Streamloots
{
	public class ProfileViewModel
	{
		public string label { get; set; }
		public string name { get; set; }
		public float value { get; set; }
		public float defaultCraftingCost { get; set; }
		public float defaultDisenchantingReward { get; set; }
		public ProfileViewModel()
		{

		}
	}
}
