using System;
using System.Linq;

namespace DndCore
{
	public class Roll
	{
		const string STR_SpellcastingAbilityModifier = "sam";
		public double Count { get; set; }
		public string Descriptor { get; set; }
		public int Offset { get; set; }
		public int Sides { get; set; }
		public Roll()
		{

		}

		string OffsetStr
		{
			get
			{
				if (Offset > 0)
					return $"+{Offset}";
				if (Offset < 0)
					return $"{Offset}";
				return string.Empty;
			}
		}
		
		public override string ToString()
		{
			return $"{Count}d{Sides}{OffsetStr}{Descriptor}";
		}
		
		public static Roll From(string dieStr, int spellcastingAbilityModifier = int.MinValue)
		{
			int dPos = dieStr.IndexOf("d");
			if (dPos < 0)
			{
				return null;
			}
			double count = 1;
			string countStr = dieStr.Substring(0, dPos);
			if (!string.IsNullOrEmpty(countStr))
				double.TryParse(countStr, out count);

			int offset = 0;
			string offsetStr = dieStr.EverythingAfter("+");
			if (!string.IsNullOrWhiteSpace(offsetStr))
			{
				offsetStr = offsetStr.ToLower().Trim();
				if (offsetStr.StartsWith(STR_SpellcastingAbilityModifier))
					if (spellcastingAbilityModifier == int.MinValue)
						offset = 0;
					else
						offset = spellcastingAbilityModifier;
				else
					offset = offsetStr.GetFirstInt();
			}
			string sidesStr = dieStr.Substring(dPos);
			int sides = sidesStr.GetFirstInt();
			if (sides == 0)
				return null;
			string descriptor = dieStr.EverythingAfter("(");
			if (descriptor != null)
			{
				descriptor = descriptor.EverythingBefore(")").Trim();
				descriptor = "(" + descriptor + ")";
			}
			else
				descriptor = string.Empty;

			Roll roll = new Roll();
			roll.Count = count;
			roll.Sides = sides;
			roll.Offset = offset;
			roll.Descriptor = descriptor;

			return roll;
		}
	}
}
