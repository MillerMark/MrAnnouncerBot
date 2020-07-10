using System;
using System.Collections.Generic;

namespace DndCore
{
	public class Magic
	{
		public event MagicEventHandler Dispel;
		
		protected virtual void OnDispel(object sender, MagicEventArgs ea)
		{
			Dispel?.Invoke(sender, ea);
		}
		// TODO: Leaning toward changing spellName from a string to a Spell or a CastedSpell, so we can hook the Spell's OnDispel event, so we can remove this Magic when the spell is dispelled!
		public Magic(Creature caster, DndGame game, string magicItemName, string spellName, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
		{
			SpellName = string.IsNullOrEmpty(spellName) ? string.Empty : spellName;
			Data1 = data1;
			Data2 = data2;
			Data3 = data3;
			Data4 = data4;
			Data5 = data5;
			Data6 = data6;
			Data7 = data7;
			Data8 = data8;
			MagicItemName = magicItemName.Trim();
			Caster = caster;
			DndTimeSpan dndTimeSpan = DndTimeSpan.FromDurationStr(MagicItem.duration);
			game.CreateAlarm($"{caster.name}.{MagicItem.Name}", dndTimeSpan, MagicExpiresHandler, this, caster as Character);
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

		void DispelMagic()
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
			AddArgument(args, Data1);
			AddArgument(args, Data2);
			AddArgument(args, Data3);
			AddArgument(args, Data4);
			AddArgument(args, Data5);
			AddArgument(args, Data6);
			AddArgument(args, Data7);
			AddArgument(args, Data8);
			return args;
		}
		public void TriggerOnReceived(Creature magicOwner)
		{
			if (string.IsNullOrWhiteSpace(MagicItem.onReceived))
				return;

			CreaturePlusModId creaturePlusModId = new CreaturePlusModId(GetModId(), magicOwner);
			List<string> args = GetArgumentList();
			string expressionToEvaluate = DndUtils.InjectParameters(MagicItem.onReceived, MagicItem.Parameters, args);

			Expressions.Do(expressionToEvaluate, null, null, null, null, creaturePlusModId);
		}

		// TODO: Consider changing the data types to Strings!
		public object Data1 { get; set; }
		public object Data2 { get; set; }
		public object Data3 { get; set; }
		public object Data4 { get; set; }
		public object Data5 { get; set; }
		public object Data6 { get; set; }
		public object Data7 { get; set; }
		public object Data8 { get; set; }
		public List<Creature> Targets { get => targets; set => targets = value; }
		public string SpellName { get; set; }
	}
}
