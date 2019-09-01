using System;
using System.Linq;

namespace DndCore
{
	public class SorcererDetail: BaseRow
	{
		public int Level { get; set; }
		public int ProficiencyBonus { get; set; }
		public int SorceryPoints { get; set; }
		public string Features { get; set; }
		public int CantripsKnown { get; set; }
		public int SpellsKnown { get; set; }
		public int Slot1Spells { get; set; }
		public int Slot2Spells { get; set; }
		public int Slot3Spells { get; set; }
		public int Slot4Spells { get; set; }
		public int Slot5Spells { get; set; }
		public int Slot6Spells { get; set; }
		public int Slot7Spells { get; set; }
		public int Slot8Spells { get; set; }
		public int Slot9Spells { get; set; }
		public SorcererDetail()
		{

		}

		public static SorcererDetail From(SorcererDetailDto sorcererDetailDto)
		{
			SorcererDetail result = new SorcererDetail();
			result.CantripsKnown = MathUtils.GetInt(sorcererDetailDto.CantripsKnown);
			result.Level = MathUtils.GetInt(sorcererDetailDto.Level);
			result.ProficiencyBonus = MathUtils.GetInt(sorcererDetailDto.ProficiencyBonus);
			result.Slot1Spells = MathUtils.GetInt(sorcererDetailDto.Slot1Spells);
			result.Slot2Spells = MathUtils.GetInt(sorcererDetailDto.Slot2Spells);
			result.Slot3Spells = MathUtils.GetInt(sorcererDetailDto.Slot3Spells);
			result.Slot4Spells = MathUtils.GetInt(sorcererDetailDto.Slot4Spells);
			result.Slot5Spells = MathUtils.GetInt(sorcererDetailDto.Slot5Spells);
			result.Slot6Spells = MathUtils.GetInt(sorcererDetailDto.Slot6Spells);
			result.Slot7Spells = MathUtils.GetInt(sorcererDetailDto.Slot7Spells);
			result.Slot8Spells = MathUtils.GetInt(sorcererDetailDto.Slot8Spells);
			result.Slot9Spells = MathUtils.GetInt(sorcererDetailDto.Slot9Spells);
			return result;
		}
	}
}
