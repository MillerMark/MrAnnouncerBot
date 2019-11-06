using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class SpellDto
	{
		public string name { get; set; }
		public string level { get; set; }
		public string school { get; set; }
		public string ritual { get; set; }
		public string casting_time { get; set; }
		public string range { get; set; }
		public string description { get; set; }
		public string die_str { get; set; }
		public int ammo_count { get; set; }
		public string attack_type { get; set; }
		public string saving_throw { get; set; }
		public string duration { get; set; }
		public bool components_material { get; set; }
		public bool components_somatic { get; set; }
		public bool components_verbal { get; set; }
		public string components_materials_description { get; set; }
		public string classes_0 { get; set; }
		public string classes_1 { get; set; }
		public string classes_2 { get; set; }
		public string classes_3 { get; set; }
		public string classes_4 { get; set; }
		public string classes_5 { get; set; }
		public string higher_levels { get; set; }
		public string bonus_threshold { get; set; }
		public string bonus_per_level { get; set; }
		public string bonus_type { get; set; }
		public string bonus_max { get; set; }
		public string tags_0 { get; set; }
		public string tags_1 { get; set; }
		public string tags_2 { get; set; }
		public string tags_3 { get; set; }
		public string tags_4 { get; set; }
		public string tags_5 { get; set; }
		public string tags_6 { get; set; }
		public string tags_7 { get; set; }
		public string cast_with { get; set; }
		public string hue1 { get; set; }
		public string bright1 { get; set; }
		public string hue2 { get; set; }
		public string bright2 { get; set; }
		public string onCast { get; set; }
		public string onCasting { get; set; }
		public string onPlayerAttacks { get; set; }
		public string onPlayerHitsTarget { get; set; }
		public string onDispel { get; set; }
		public string availableWhen { get; set; }

		public SpellDto()
		{

		}
	}
}
