using System;
using System.Linq;

namespace DndCore
{
	public class CarriedAmmunition
	{
		public string Name { get; set; } = string.Empty;
		public string DamageBonusStr { get; set; } = string.Empty;
		public string HueShift { get; set; } = string.Empty;
		public string Kind { get; set; }
		public int Count { get; set; }
		public CarriedAmmunition()
		{

		}
		public void SetAmmunitionParameters(string ammoParameters, Character player)
		{
			Name = "";
			DamageBonusStr = "";
			HueShift = "";
			if (string.IsNullOrEmpty(ammoParameters))
				return;
			string[] parameters = ammoParameters.Split(',');
			if (parameters.Length == 0)
				return;
			Name = Expressions.GetStr(parameters[0], player);
			if (parameters.Length > 1)
			{
				DamageBonusStr = parameters[1].Trim();
				if (parameters.Length > 2)
					HueShift = parameters[2].Trim();
			}
		}
	}
}