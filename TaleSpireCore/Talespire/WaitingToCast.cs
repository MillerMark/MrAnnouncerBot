using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class WaitingToCast
	{
		public float CreationTime { get; set; }
		public string EffectName { get; set; }
		public string SpellId { get; set; }
		public string CreatureId { get; set; }
		public float EnlargeTime { get; set; }
		public float LifeTime { get; set; }
		public VectorDto Position { get; set; }
		public SpellLocation Location { get; set; }
		
		bool readyToDelete { get; set; }
		public bool ShouldCreateNow => Time.time > CreationTime && !readyToDelete;
		public WaitingToCast()
		{

		}

		public WaitingToCast(SpellLocation location, float secondsDelayStart, string effectName, string spellId, string creatureId, float enlargeTime, float lifeTime = 0, VectorDto position = null)
		{
			CreationTime = Time.time + secondsDelayStart;
			EffectName = effectName;
			SpellId = spellId;
			CreatureId = creatureId;
			EnlargeTime = enlargeTime;
			LifeTime = lifeTime;
			Position = position;
			Location = location;
		}

		public void CreateNow()
		{
			readyToDelete = true;
			switch (Location)
			{
				case SpellLocation.Attached:
					Talespire.Spells.AttachEffect(EffectName, SpellId, CreatureId, EnlargeTime, 0);
					break;
				case SpellLocation.AtPosition:
					Talespire.Spells.PlayEffectAtPosition(EffectName, SpellId, Position, LifeTime, EnlargeTime);
					break;
				case SpellLocation.OverCreature:
					Talespire.Spells.PlayEffectOverCreature(EffectName, SpellId, CreatureId, LifeTime, EnlargeTime);
					break;
			}
		}
	}
}



