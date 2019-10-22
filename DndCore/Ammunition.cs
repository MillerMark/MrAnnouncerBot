using System;
using System.Linq;

namespace DndCore
{
	public class Ammunition : ItemViewModel
	{
		public static Ammunition buildBlowgunNeedlePack()
		{
			Ammunition blowDart = new Ammunition();
			blowDart.StandardName = "Blow Needle";
			blowDart.costValue = 1;
			blowDart.weight = 1;
			blowDart.count = 50;
			return blowDart;
		}
	}
}
