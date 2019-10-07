using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DndCore
{
	public class DndGame
	{
		public const string STR_EndSpell = "EndSpell:";

		public static DndGame Instance
		{
			get
			{
				if (instance == null)
				{
					if (History.TimeClock == null)
						History.TimeClock = new DndTimeClock();
					instance = new DndGame();
				}

				return instance;
			}
		}

		public event CastedSpellEventHandler SpellDispelled;
		public event DndGameEventHandler EnterCombat;
		public event DndGameEventHandler ExitCombat;
		public event DndGameEventHandler RoundEnded;
		public event DndGameEventHandler RoundStarting;
		public event DndCharacterEventHandler TurnEnded;
		public event DndCharacterEventHandler TurnStarting;
		public event PlayerStateChangedEventHandler PlayerStateChanged;
		public event PlayerRollRequestEventHandler PlayerRequestsRoll;
		protected virtual void OnPlayerRequestsRoll(object sender, PlayerRollRequestEventArgs ea)
		{
			PlayerRequestsRoll?.Invoke(sender, ea);
		}
		protected virtual void OnSpellDispelled(object sender, CastedSpellEventArgs ea)
		{
			SpellDispelled?.Invoke(sender, ea);
		}
		protected virtual void OnPlayerStateChanged(object sender, PlayerStateEventArgs ea)
		{
			PlayerStateChanged?.Invoke(sender, ea);
		}
		protected virtual void OnEnterCombat(object sender, DndGameEventArgs ea)
		{
			EnterCombat?.Invoke(sender, ea);
		}
		protected virtual void OnExitCombat(object sender, DndGameEventArgs ea)
		{
			ExitCombat?.Invoke(sender, ea);
		}
		protected virtual void OnRoundEnded(object sender, DndGameEventArgs ea)
		{
			RoundEnded?.Invoke(sender, ea);
		}
		protected virtual void OnRoundStarting(object sender, DndGameEventArgs ea)
		{
			RoundStarting?.Invoke(sender, ea);
		}
		protected virtual void OnTurnEnded(object sender, DndCharacterEventArgs ea)
		{
			TurnEnded?.Invoke(sender, ea);
		}

		protected virtual void OnTurnStarting(object sender, DndCharacterEventArgs ea)
		{
			TurnStarting?.Invoke(sender, ea);
		}
		DndMap activeMap;

		List<DndMap> maps = new List<DndMap>();

		List<Monster> monsters = new List<Monster>();
		private static DndGame instance;
		DndGameEventArgs dndGameEventArgs = new DndGameEventArgs();

		public DndGame()
		{
			dndGameEventArgs.Game = this;
		}

		public DndMap ActiveMap
		{
			get => activeMap;
			set
			{
				ActivateMap(value);
			}
		}

		public DndRoom ActiveRoom
		{
			get
			{
				return ActiveMap?.ActiveRoom;
			}
		}

		public List<Character> Players { get; } = new List<Character>();
		public bool InCombat { get; set; }
		public Creature ActiveCreature { get => activeCreature; private set => activeCreature = value; }
		public int WaitingForRollHiddenThreshold { get; set; }

		private DndTimeClock timeClock = new DndTimeClock();
		public Creature WaitingForRollCreature { get; set; }
		public DiceRollType WaitingForRollType { get; set; }
		public Creature WaitingForRollTarget { get; set; }

		public DateTime Time
		{
			get
			{
				return timeClock.Time;
			}
		}

		public Creature nextTarget;

		public DndMap ActivateMap(DndMap map)
		{
			if (map == null)
				activeMap = null;
			else
				activeMap = maps.FirstOrDefault(x => x == map);
			return activeMap;
		}

		public DndMap ActivateMap(string mapName)
		{
			return maps.FirstOrDefault(x => x.Name == mapName);
		}

		public void ActivateRoom(DndRoom dndRoom)
		{
			if (dndRoom.Map == null)
				throw new DndException("Cannot activate a room that has not been added to a map!");
			dndRoom.Map.ActivateRoom(dndRoom);
		}
		public DndMap AddMap(DndMap dndMap)
		{
			dndMap.Game = this;
			maps.Add(dndMap);
			return dndMap;
		}

		public Monster AddMonster(Monster monster)
		{
			monster.Game = this;
			monsters.Add(monster);
			return monster;
		}

		public Character AddPlayer(Character player)
		{
			player.Game = this;
			player.StateChanged += Player_StateChanged;
			player.RollDiceRequest += Player_RollDiceRequest ;
			player.SpellDispelled += Player_SpellDispelled;
			Players.Add(player);
			return player;
		}

		private void Player_SpellDispelled(object sender, CastedSpellEventArgs ea)
		{
			OnSpellDispelled(this, ea);
		}

		private void Player_RollDiceRequest(object sender, RollDiceEventArgs ea)
		{
			OnPlayerRequestsRoll(this, new PlayerRollRequestEventArgs(sender as Character, ea.DiceRollStr));
		}

		private void Player_StateChanged(object sender, StateChangedEventArgs ea)
		{
			OnPlayerStateChanged(this, new PlayerStateEventArgs(sender as Character, ea.Key, ea.OldValue, ea.NewValue));
		}

		Creature firstPlayer = null;
		Creature lastPlayer = null;

		public void EnteringCombat()
		{
			firstPlayer = null;
			lastPlayer = null;
			InCombat = true;
			OnEnterCombat(this, dndGameEventArgs);
		}

		public void MoveAllPlayersToActiveRoom()
		{
			throw new NotImplementedException();
		}
		public void QueueAction(Creature creature, ActionAttack actionAttack)
		{
			// TODO: Implement this!!!
		}
		public void ExitingCombat()
		{
			// TODO: Tell DM: "Ending {} rounds of combat...."
			roundIndex = 0;
			InCombat = false;
			OnExitCombat(this, dndGameEventArgs);
		}
		Creature activeCreature;

		public void EndingTurnFor(Character character)
		{
			if (activeCreature == character)
				activeCreature = null;
		}

		public void StartingTurnFor(Character character)
		{
			if (activeCreature != character && activeCreature is Character player)
				player.EndTurn();
			activeCreature = character;
		}

		public void SetHiddenThreshold(Creature creature, int value, DiceRollType rollType)
		{
			WaitingForRollTarget = nextTarget;
			WaitingForRollType = rollType;
			WaitingForRollCreature = creature;
			WaitingForRollHiddenThreshold = value;
		}

		List<CastedSpell> activeSpells = new List<CastedSpell>();
		List<CastedSpell> castingSpells = new List<CastedSpell>();
		public int roundIndex;
		public string lastMessageSentToDungeonMaster;

		private void StartRound(Creature player)
		{
			roundIndex = 0;
			firstPlayer = player;
			OnRoundStarting(this, dndGameEventArgs);
		}

		void AdvanceRound()
		{
			OnRoundEnded(this, dndGameEventArgs);
			roundIndex++;

			if (InCombat)
				timeClock.Advance(6000);  // 6 seconds per round

			OnRoundStarting(this, dndGameEventArgs);
		}

		public void CreatureTakingAction(Creature player)
		{
			// TODO: Fix bug where first player dies - we need to reassign firstPlayer to the next player in the initiative line up.
			if (firstPlayer == null)
			{
				StartRound(player);
			}
			else if (lastPlayer != firstPlayer && player == firstPlayer)
			{
				// Cycled all around. We need to advance the round and the clock.
				AdvanceRound();
			}

			if (lastPlayer != null)
				lastPlayer.EndTurnResetState();

			if (lastPlayer != player)
				player.StartTurnResetState();

			lastPlayer = player;
		}

		public CastedSpell Cast(Character player, Spell spell, Creature targetCreature = null)
		{
			if (spell.CastingTime == DndTimeSpan.OneAction || spell.CastingTime == DndTimeSpan.OneBonusAction)
			{
				CreatureTakingAction(player);
			}

			CastedSpell castedSpell = new CastedSpell(spell, player, targetCreature);
			castedSpell.Casting();

			nextTarget = targetCreature;

			if (spell.MustRollDiceToCast())
			{
				castingSpells.Add(castedSpell);
				return castedSpell;
			}

			CompleteCast(player, castedSpell);
			return castedSpell;
		}

		public void CompleteCast(Character player, CastedSpell castedSpell)
		{
			if (castingSpells.IndexOf(castedSpell) >= 0)
				castingSpells.Remove(castedSpell);

			if (activeSpells.IndexOf(castedSpell) >= 0)
				activeSpells.Remove(castedSpell);

			Spell spell = castedSpell.Spell;
			if (spell.Duration.HasValue())
			{
				DndAlarm dndAlarm = timeClock.CreateAlarm(spell.Duration.GetTimeSpan(), GetSpellAlarmName(spell, player.playerID), player, castedSpell);
				dndAlarm.AlarmFired += DndAlarm_SpellDurationExpired;
			}
			
			activeSpells.Add(castedSpell);
			castedSpell.Cast();
		}

		public void Dispel(CastedSpell castedSpell)
		{
			if (activeSpells.IndexOf(castedSpell) < 0)
				return;

			castedSpell.Dispel();
			activeSpells.Remove(castedSpell);
		}

		private void DndAlarm_SpellDurationExpired(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Data is CastedSpell castedSpell)
			{
				if (castedSpell.SpellCaster?.concentratedSpell == castedSpell)
					castedSpell.SpellCaster.BreakConcentration();

				Dispel(castedSpell);
				OnSpellDispelled(this, new CastedSpellEventArgs(this, castedSpell));
			}
		}

		public List<CastedSpell> GetActiveSpells(Character character)
		{
			return activeSpells.FindAll(x => x.SpellCaster == character);
		}

		public void CreatureWillAttack(Creature creature, Creature target, Attack attack, bool usesMagic)
		{
			if (creature is Character player)
			{
				player.targetThisRollIsCreature = target != null;
				player.usesMagicThisRoll = usesMagic;
				List<CastedSpell> playersActiveSpells = GetActiveSpells(player);
				// Trigger in reverse order in case any of these spells are dispelled.
				for (int i = playersActiveSpells.Count - 1; i >= 0; i--)
				{
					CastedSpell castedSpell = playersActiveSpells[i];
					castedSpell.Spell.TriggerPlayerAttacks(player, WaitingForRollTarget, castedSpell);
				}
			}
		}

		public void OnSuccessfulRoll(Character character, int thisDieRollHiddenThreshold, int score, int damage)
		{
			throw new NotImplementedException();
		}

		public void DieRollStopped(Creature creature, int score, int damage)
		{
			if (WaitingForRollHiddenThreshold != int.MinValue && creature == WaitingForRollCreature)
			{
				if (DndUtils.IsAttack(WaitingForRollType) && score >= WaitingForRollHiddenThreshold)
				{
					OnCreatureHitsTarget(creature, damage);
				}
			}

			ClearWaitingForRollStateVars();
		}

		private void ClearWaitingForRollStateVars()
		{
			WaitingForRollHiddenThreshold = int.MinValue;
			WaitingForRollTarget = null;
			WaitingForRollCreature = null;
			WaitingForRollType = DiceRollType.None;
		}

		void OnCreatureHitsTarget(Creature creature, int damage)
		{
			if (!(creature is Character player))
				return;

			List<CastedSpell> playersActiveSpells = activeSpells.FindAll(x => x.SpellCaster == player);
			// Trigger in reverse order in case any of these spells are dispelled.
			for (int i = playersActiveSpells.Count - 1; i >= 0; i--)
			{
				CastedSpell castedSpell = playersActiveSpells[i];
				castedSpell.Spell.TriggerPlayerHitsTarget(player, WaitingForRollTarget, castedSpell);
			}
		}

		public void StartNew()
		{
			dndGameEventArgs.Game = this;
			activeMap = null;
			nextTarget = null;
			maps.Clear();
			monsters.Clear();
			ActiveMap = null;
			Players.Clear();
			InCombat = false;
			ActiveCreature = null;
			WaitingForRollHiddenThreshold = int.MinValue;
			timeClock = new DndTimeClock();
			ClearWaitingForRollStateVars();
			firstPlayer = null;
			lastPlayer = null;
			activeSpells = new List<CastedSpell>();
			castingSpells = new List<CastedSpell>();
			roundIndex = 0;
		}

		public void AdvanceClock(DndTimeSpan dndTimeSpan)
		{
			timeClock.Advance(dndTimeSpan);
		}

		public DndAlarm CreateAlarm(TimeSpan fromNow, string name, Character player = null, object data = null)
		{
			return timeClock.CreateAlarm(fromNow, name, player, data);
		}
		public int SecondsSince(DateTime startTime)
		{
			TimeSpan timeSpan = Time - startTime;
			return (int)Math.Round(timeSpan.TotalSeconds);
		}

		public DndTimeClock Clock
		{
			get
			{
				return timeClock;
			}
		}
		

		public void TellDungeonMaster(string message)
		{
			lastMessageSentToDungeonMaster = message;
			// TODO: send message to Dungeon Master
		}

		public Character GetPlayerFromId(int playerID)
		{
			return Players.FirstOrDefault(x => x.playerID == playerID);
		}

		public void BreakConcentration(int playerId)
		{
			Character player = GetPlayerFromId(playerId);
			if (player == null)
				return;
			player.BreakConcentration();
		}

		//void CreateAlarm(Spell spell, int playerId)
		//{
		//	CreateAlarm(spell.Duration.GetTimeSpan(), GetAlarmName(spell, playerId));
		//}

		public static string GetSpellPlayerName(Spell spell, int playerId)
		{
			return $"{spell.Name}({playerId})";
		}
		public static string GetSpellAlarmName(Spell spell, int playerId)
		{
			return $"{STR_EndSpell}{GetSpellPlayerName(spell, playerId)}";
		}

		//void CreateAlarm(TimeSpan timeSpan, string name)
		//{
		//	DndAlarm dndAlarm = Clock.CreateAlarm(timeSpan, name);
		//	dndAlarm.AlarmFired += DndAlarm_AlarmFired;
		//}

		//private void DndAlarm_AlarmFired(object sender, DndTimeEventArgs ea)
		//{
		//	
		//}

		string Plural(int count, string suffix)
		{
			if (count == 1)
				return $"{count} {suffix}";
			return $"{count} {suffix}s";
		}

		public string GetSpellTimeLeft(int playerId, Spell spell)
		{
			DndAlarm alarm = Clock.GetAlarm(GetSpellAlarmName(spell, playerId));
			if (alarm == null)
				return "0 seconds";
			TimeSpan time = alarm.TriggerTime - Clock.Time;
			string result;
			if (time.TotalDays >= 1)
				result = $"{Plural(time.Days, "day")}, {Plural(time.Hours, "hour")}, {Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else if (time.TotalHours >= 1)
				result = $"{Plural(time.Hours, "hour")}, {Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else if (time.TotalMinutes >= 1)
				result = $"{Plural(time.Minutes, "minute")}, {Plural(time.Seconds, "second")}";
			else
				result = Plural(time.Seconds, "second");
			return result;
		}

		public string GetConcentrationReport()
		{
			string concentrationReport = string.Empty;
			foreach (Character player in Players)
			{
				CastedSpell concentratedCastedSpell = player.concentratedSpell;
				if (concentratedCastedSpell == null)
					continue;
				Spell spell = concentratedCastedSpell.Spell;
				if (concentratedCastedSpell != null)
				concentrationReport += $"{player.name} is casting {spell.Name} with {GetSpellTimeLeft(player.playerID, spell)} remaining; ";
			}
			if (concentrationReport == string.Empty)
				concentrationReport = "No players are concentrating on any spells at this time.";
			return concentrationReport;
		}
	}
}
