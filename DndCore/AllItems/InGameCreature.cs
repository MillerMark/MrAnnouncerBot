using System;
using System.Linq;
using Newtonsoft.Json;
using GoogleHelper;

namespace DndCore
{
	[SheetName("DnD")]
	[TabName("InGameCreatures")]
	public class InGameCreature
	{
		[Column]
		public string Name { get; set; }
		
		[Column]
		public string Kind { get; set; }
		
		[Column]
		public int HitPoints { get; set; }
		
		[Column]
		public int Index { get; set; }

		[JsonIgnore]
		public bool IsSelected { get; set; }


		Creature creature;
		public Creature Creature
		{
			get
			{
				if (creature == null)
				{
					Monster monster = Monster.Clone(AllMonsters.Get(Kind));
					if (monster != null)
					{
						// TODO: Fix race (or use "Kind") field and use Name for the monster's name.
						monster.Name = Name;
						monster.HitPoints = HitPoints;
						creature = monster;
					}
				}
				return creature;
			}
		}
		
		public InGameCreature()
		{

		}
	}
}

