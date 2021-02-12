using System;
using System.Collections.Generic;

namespace DndCore
{
	public class Magic
	{
		private const int NumArgs = 8;
		public event MagicEventHandler Dispel;
		
		protected virtual void OnDispel(object sender, MagicEventArgs ea)
		{
			Dispel?.Invoke(sender, ea);
		}
		
		public Magic(Creature caster, DndGame game, string magicItemName, CastedSpell castedSpell, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
		{
			CastedSpellId = castedSpell?.ID;
			Game = game;
			SpellName = castedSpell?.Spell?.Name;
			if (string.IsNullOrWhiteSpace(SpellName))
				SpellName = "{No Spell Name}";
			Args[0] = data1;
			Args[1] = data2;
			Args[2] = data3;
			Args[3] = data4;
			Args[4] = data5;
			Args[5] = data6;
			Args[6] = data7;
			Args[7] = data8;
			MagicItemName = magicItemName.Trim();
			Caster = caster;

			// TODO: Should we ever set the Magic's duration to the spell's duration?
			if (string.IsNullOrWhiteSpace(MagicItem.duration))
			{
				for (int i = 0; i < MagicItem.Parameters.Count; i++)
				{
					if (MagicItem.Parameters[i] == "$Duration")
					{
						MagicItem.duration = (string)Args[i];
						break;
					}
				}
			}

			DndTimeSpan dndTimeSpan = DndTimeSpan.FromDurationStr(MagicItem.duration);
			if (dndTimeSpan.HasValue())
				game.CreateAlarm($"{caster.name}.{MagicItem.Name}", dndTimeSpan, MagicExpiresHandler, this, caster as Character);

			Spell spell = castedSpell?.Spell;
			if (spell != null && spell.RequiresConcentration)
			{
				castedSpell.OnDispel += CastedSpell_OnDispel;
			}
		}

		private void CastedSpell_OnDispel(object sender, EventArgs e)
		{
			foreach (Creature creature in Targets)
			{
				TriggerEvent(creature, MagicItem.onExpire);
				creature.RemoveMagic(this);
			}
		}

		public Creature Caster { get; set; }
		
		List<Creature> targets = new List<Creature>();
		public void AddTarget(Creature target)
		{
			targets.Add(target);
		}
		public string MagicItemName { get; set; }
		MagicItem magicItem;
		public MagicItem MagicItem
		{
			get
			{
				if (magicItem == null)
					magicItem = AllMagicItems.Get(MagicItemName);
				return magicItem;
			}
		}

		// TODO: Call this before we trigger any Magic events.
		public void SetMagicVariables()
		{
			
		}

		public void DispelMagic()
		{
			MagicItem.TriggerDispel(this);
			OnDispel(this, new MagicEventArgs(this));
		}

		void MagicExpiresHandler(object sender, DndTimeEventArgs ea)
		{
			DispelMagic();
		}

		public static string ActiveModId;
		string GetModId()
		{
			return $"{Caster.name}.{SpellName}.{MagicItemName}";
		}


		void AddArgument(List<string> args, object data)
		{
			args.Add(data == null ? null : data.ToString());
		}

		List<string> GetArgumentList()
		{
			List<string> args = new List<string>();
			for (int i = 0; i < NumArgs; i++)
				AddArgument(args, Args[i]);
			return args;
		}

		/// <summary>
		/// Triggers the specified event, ensuring a CreaturePlusModId is passed in as custom data to the expressions engine.
		/// </summary>
		void TriggerEvent(Creature magicOwner, string eventCode)
		{
			if (string.IsNullOrWhiteSpace(eventCode))
				return;

			SystemVariables.Creature = magicOwner;

			CreaturePlusModId creaturePlusModId = new CreaturePlusModId(GetModId(), magicOwner);
			creaturePlusModId.Magic = this;
			List<string> args = GetArgumentList();
			string expressionToEvaluate = DndUtils.InjectParameters(eventCode, MagicItem.Parameters, args);

			Expressions.Do(expressionToEvaluate, magicOwner, null, null, null, creaturePlusModId);
		}

		public void TriggerOnReceived(Creature magicOwner)
		{
			TriggerEvent(magicOwner, MagicItem.onReceived);
		}

		public void TriggerRecipientSaves(Creature magicOwner)
		{
			TriggerEvent(magicOwner, MagicItem.onRecipientSaves);
		}

		public void TriggerRecipientChecksSkill(Creature magicOwner)
		{
			TriggerEvent(magicOwner, MagicItem.onRecipientChecksSkill);
		}

		public void TriggerRecipientAttacks(Creature magicOwner)
		{
			TriggerEvent(magicOwner, MagicItem.onRecipientAttacks);
		}

		// TODO: Consider changing the Args elements data types to Strings!
		public object[] Args { get; set; } = new object[NumArgs];

		public List<Creature> Targets { get => targets; set => targets = value; }
		public string SpellName { get; set; }
		public DndGame Game { get; set; }
		public string CastedSpellId { get; set; }
		public CastedSpell CastedSpell => Game.GetActiveSpellById(Caster as Character, CastedSpellId);
	}
}
