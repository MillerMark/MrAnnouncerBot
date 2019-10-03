using System;
using System.Linq;

namespace DndCore
{
	public class FeatureDto
	{
		public string Name { get; set; }
		public string RequiresPlayerActivation { get; set; }
		public string OnActivate { get; set; }
		public string OnDeactivate { get; set; }
		public string OnPlayerCastsSpell { get; set; }
		public string Duration { get; set; }
		public string Conditions { get; set; }
		public string Limit { get; set; }
		public string Per { get; set; }
		public FeatureDto()
		{

		}
	}
}
