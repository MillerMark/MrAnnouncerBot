using System;
using System.Linq;

namespace DndCore
{
	public class MonsterDto
	{
		public string Name { get; set; }
		public string Meta { get; set; }
		public string ArmorClass { get; set; }
		public string HitPoints { get; set; }
		public string Speed { get; set; }
		public string STR { get; set; }
		public string STR_mod { get; set; }
		public string DEX { get; set; }
		public string DEX_mod { get; set; }
		public string CON { get; set; }
		public string CON_mod { get; set; }
		public string INT { get; set; }
		public string INT_mod { get; set; }
		public string WIS { get; set; }
		public string WIS_mod { get; set; }
		public string CHA { get; set; }
		public string CHA_mod { get; set; }
		public string SavingThrows { get; set; }
		public string Skills { get; set; }
		public string Senses { get; set; }
		public string Languages { get; set; }
		public string Challenge { get; set; }
		public string Traits { get; set; }
		public string Actions { get; set; }
		public string LegendaryActions { get; set; }
		public string img_url { get; set; }
		public string DamageImmunities { get; set; }
		public string ConditionImmunities { get; set; }
		public string DamageResistances { get; set; }
		public string DamageVulnerabilities { get; set; }
		public string Reactions { get; set; }
		public MonsterDto()
		{

		}
	}
}
