//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class ChangeWeatherCardEvent : CardEvent
	{
		const string STR_WeatherEventOneMinute = "WeatherEventOneMinute";
		List<DamageModifier> damageModifier;
		public ChangeWeatherCardEvent(object[] args) : base(args)
		{
			// TODO: Figure out an elegant way to map the parameter names to the arguments.
			if (args[2] != null)
				damageModifier = DamageModifier.Create((string)args[2]);
		}

		public override void ConditionRoll(DiceRoll diceRoll)
		{
			if (damageModifier == null)
				return;
			foreach (DamageModifier modifier in damageModifier)
			{
				int creatureId = int.MinValue;
				if (diceRoll.IsOnePlayer)
					creatureId = diceRoll.SingleOwnerId;
				DamageType damage = DndUtils.ToDamage(modifier.damageType);
				if (damage != DamageType.None)
					if (diceRoll.HasDamage(damage))
						diceRoll.CardModifiers.Add(new CardModifier() { BlameName = UserName, Offset = modifier.offset, Multiplier = modifier.multiplier, CreatureId = creatureId, CardModType = modifier.CardModType });
			}
		}

		public override void Activate()
		{
			string weatherKeyword = Expressions.GetStr((string)Args[1]);
			DungeonMasterApp.ShowWeather(weatherKeyword);
			if (DungeonMasterApp.Game.InCombat)
			{
				CreateInCombatAlarm();
			}
			else
			{
				DungeonMasterApp.Game.EnterCombat += Game_EnterCombat;
				DungeonMasterApp.Game.CreateAlarm(STR_WeatherEventOneMinute, DndTimeSpan.OneMinute, OneMinuteIsDone);
			}
		}

		private void CreateInCombatAlarm()
		{
			int initiativeIndex;
			if (DungeonMasterApp.Game.InitiativeIndex == -1)  // Whoa, we haven't hit the Next Turn button!
				initiativeIndex = DungeonMasterApp.Game.PlayerCount + 1;
			else
				initiativeIndex = DungeonMasterApp.Game.InitiativeIndex;

			DungeonMasterApp.Game.CreateAlarm("WeatherEventOneRound", DndTimeSpan.OneRound, OneRoundIsDone, null, null, initiativeIndex);
		}

		void OneMinuteIsDone(object sender, DndTimeEventArgs ea)
		{
			Finish();
		}

		private void Finish()
		{
			IsDone = true;
			DungeonMasterApp.ShowWeather("None");
		}

		void OneRoundIsDone(object sender, DndTimeEventArgs ea)
		{
			DungeonMasterApp.Game.EnterCombat -= Game_EnterCombat;
			Finish();
		}

		private void Game_EnterCombat(object sender, DndGameEventArgs ea)
		{
			DungeonMasterApp.Game.RemoveAlarmByName(STR_WeatherEventOneMinute);
			CreateInCombatAlarm();
		}
	}
}
