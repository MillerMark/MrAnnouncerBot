﻿using System;
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
		public float ShrinkTime { get; set; }
		public float RotationDegrees { get; set; }
		public float LifeTime { get; set; }
		public Vector3 Position { get; set; }
		public SpellLocation Location { get; set; }
		public bool IsMoveable { get; set; }
		bool readyToDelete { get; set; }
		public bool ShouldCreateNow => Time.time > CreationTime && !readyToDelete;
		public Action<GameObject> Conditioning { get; set; }
		public WaitingToCast()
		{

		}

		public WaitingToCast(SpellLocation location, float secondsDelayStart, string effectName, string spellId, string creatureId, float enlargeTime, float lifeTime = 0, Vector3? position = null, float shrinkTime = 0, float rotation = 0, bool isMoveable = false, Action<GameObject> conditioning = null)
		{
			Conditioning = conditioning;
			CreationTime = Time.time + secondsDelayStart;
			EffectName = effectName;
			SpellId = spellId;
			CreatureId = creatureId;
			EnlargeTime = enlargeTime;
			ShrinkTime = shrinkTime;
			RotationDegrees = rotation;
			LifeTime = lifeTime;
			if (position.HasValue)
				Position = position.Value;
			Location = location;
			IsMoveable = isMoveable;
		}

		public void CreateNow()
		{
			readyToDelete = true;
			switch (Location)
			{
				case SpellLocation.Attached:
					Talespire.Spells.AttachEffect(EffectName, SpellId, CreatureId, 0, EnlargeTime, LifeTime, ShrinkTime, RotationDegrees);
					break;
				case SpellLocation.AtPosition:
					Talespire.Spells.PlayEffectAtPosition(EffectName, SpellId, Position, LifeTime, EnlargeTime, 0, ShrinkTime, RotationDegrees, IsMoveable, Conditioning);
					break;
				case SpellLocation.AtCreatureBase:
					Talespire.Spells.PlayEffectAtCreatureBase(EffectName, SpellId, CreatureId, LifeTime, EnlargeTime, 0, ShrinkTime, RotationDegrees);
					break;
				case SpellLocation.CreatureCastSpell:
					Talespire.Spells.CreatureCastSpell(EffectName, SpellId, CreatureId, LifeTime, EnlargeTime, 0, ShrinkTime, RotationDegrees, IsMoveable);
					break;
			}
		}
	}
}



