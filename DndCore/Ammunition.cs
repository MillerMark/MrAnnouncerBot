using System;
using System.Linq;
using DndCore.ViewModels;

namespace DndCore
{
	public class Ammunition : ItemViewModel
	{
		public static Ammunition buildBlowgunNeedlePack()
		{
			Ammunition blowDart = new Ammunition();
			blowDart.Name = "Blow Needle";
			blowDart.costValue = 1;
			blowDart.weight = 1;
			blowDart.count = 50;
			return blowDart;
		}
	}
}
