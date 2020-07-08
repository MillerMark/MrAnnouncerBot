using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DndCore
{
	public class DndGame
	{
		List<Creature> allCreatures;
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

		public event MessageEventHandler RequestMessageToDungeonMaster;
		public event MessageEventHandler RequestMessageToAll;
		public event CastedSpellEventHandler SpellDispelled;
		public event PickWeaponEventHandler PickWeapon;
		public event PlayerShowStateEventHandler PlayerShowState;
		public event PickAmmunitionEventHandler PickAmmunition;
		public event DndGameEventHandler EnterCombat;
		public event DndGameEventHandler ExitCombat;
		public event DndGameEventHandler EnterTimeFreeze;
		public event DndGameEventHandler ExitTimeFreeze;
		public event DndGameEventHandler RoundEnded;
		public event DndGameEventHandler RoundStarting;
		public event DndCharacterEventHandler TurnEnded;
		public event DndCharacterEventHandler TurnStarting;
		public event PlayerStateChangedEventHandler PlayerStateChanged;
		public event PlayerRollRequestEventHandler PlayerRequestsRoll;
		protected virtual void OnPlayerShowState(object sender, PlayerShowStateEventArgs ea)
		{
			PlayerShowState?.Invoke(sender, ea);
		}
		protected virtual void OnPlayerRequestsRoll(object sender, PlayerRollRequestEventArgs ea)
		{
			PlayerRequestsRoll?.Invoke(sender, ea);
		}
		protected virtual void OnPickWeapon(object sender, PickWeaponEventArgs ea)
		{
			PickWeapon?.Invoke(sender, ea);
		}
		protected virtual void OnPickAmmunition(object sender, PickAmmunitionEventArgs ea)
		{
			PickAmmunition?.Invoke(sender, ea);
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
		protected virtual void OnEnterTimeFreeze(object sender, DndGameEventArgs ea)
		{
			EnterTimeFreeze?.Invoke(sender, ea);
		}
		protected virtual void OnExitCombat(object sender, DndGameEventArgs ea)
		{
			ExitCombat?.Invoke(sender, ea);
		}
		protected virtual void OnExitTimeFreeze(object sender, DndGameEventArgs ea)
		{
			ExitTimeFreeze?.Invoke(sender, ea);
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
		public bool InTimeFreeze { get; set; }
		public bool InNonCombatInitiative { get; set; }
		public Creature ActiveCreature { get => activeCreature; private set => activeCreature = value; }
		public int WaitingForRollHiddenThreshold { get; set; }

		private DndTimeClock timeClock = new DndTimeClock();
		public Creature WaitingForRollCreature { get; set; }
		public DiceRollType WaitingForRollType { get; set; }
		public Target WaitingForRollTarget { get; set; }

		public DateTime Time
		{
			get
			{
				return timeClock.Time;
			}
		}

		public Target nextTarget;

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
			HookPlayerEvents(player);
			player.AddSpellSlots();
			player.SetTimeBasedEvents();
			Players.Add(player);
			return player;
		}


		public List<Creature> KnownCreatures { get; set; } = new List<Creature>();

		public void AddCreature(Creature creature)
		{
			KnownCreatures.Add(creature);
		}


		public List<Creature> AllCreatures
		{
			get
			{
				return KnownCreatures.Union(Players.Cast<Creature>()).ToList();
			}
		}

		// AllCreatures

		private void HookPlayerEvents(Character player)
		{
			player.PickWeapon += Player_PickWeapon;
			player.PickAmmunition += Player_PickAmmunition;
			player.PlayerShowState += Player_PlayerShowState;
			player.StateChanged += Player_StateChanged;
			player.RollDiceRequest += Player_RollDiceRequest;
			player.SpellDispelled += Player_SpellDispelled;
			player.RequestMessageToDungeonMaster += Player_RequestMessageToDungeonMaster;
			player.RequestMessageToAll += Player_RequestMessageToAll;
		}

		private void Player_PlayerShowState(object sender, PlayerShowStateEventArgs ea)
		{
			OnPlayerShowState(this, ea);
		}

		private void Player_PickWeapon(object sender, PickWeaponEventArgs ea)
		{
			OnPickWeapon(this, ea);
		}
		private void Player_PickAmmunition(object sender, PickAmmunitionEventArgs ea)
		{
			OnPickAmmunition(this, ea);
		}

		private void Player_RequestMessageToDungeonMaster(object sender, MessageEventArgs ea)
		{
			TellDungeonMaster(ea.Message);
		}

		private void Player_RequestMessageToAll(object sender, MessageEventArgs ea)
		{
			TellAll(ea.Message);
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
			OnPlayerStateChanged(this, new PlayerStateEventArgs(sender as Character, ea));
		}

		Creature firstPlayer = null;
		Creature lastPlayer = null;

		public void EnteringCombat()
		{
			firstPlayer = null;
			lastPlayer = null;
			InCombat = true;
			OnEnterCombat(this, dndGameEventArgs);
			TellDungeonMaster("---");
			TellDungeonMaster("Entering combat...");
		}

		public void EnteringTimeFreeze()
		{
			firstPlayer = null;
			lastPlayer = null;
			InTimeFreeze = true;
			OnEnterTimeFreeze(this, dndGameEventArgs);
			TellDungeonMaster("---");
			TellDungeonMaster("Stopping the clock...");
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
			TellDungeonMaster("Exiting combat...");
		}
		public void ExitingTimeFreeze()
		{
			// TODO: Tell DM: "Ending {} rounds of time freeze...."
			roundIndex = 0;
			InTimeFreeze = false;
			OnExitTimeFreeze(this, dndGameEventArgs);
			TellDungeonMaster("Restarting the clock...");
		}
		Creature activeCreature;

		// TODO: As a cleanup, end the last creature's turn when 6 seconds pass after combat ends.
		public void EndingTurnFor(Creature creature)
		{
			if (activeCreature == creature)
				activeCreature = null;
			SendReminders(creature, TurnPoint.End);
		}

		public void StartingTurnFor(Creature creature)
		{
			if (activeCreature != creature && activeCreature is Character player)
				player.EndTurn();
			activeCreature = creature;
			SendReminders(creature, TurnPoint.Start);
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
			if (InCombat)
				TellDungeonMasterWhichRound();
		}

		public void AdvanceRound()
		{
			OnRoundEnded(this, dndGameEventArgs);
			roundIndex++;

			if (InCombat)
			{
				timeClock.Advance(6000);  // 6 seconds per round. Will trigger alarms.
				TellDungeonMasterWhichRound();
			}

			OnRoundStarting(this, dndGameEventArgs);
		}

		private void TellDungeonMasterWhichRound()
		{
			TellDungeonMaster($"Starting round {roundIndex + 1}...");
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

			if (lastPlayer != null && lastPlayer != player)
			{
				lastPlayer.EndTurnResetState();
				if (lastPlayer is Character character)
					character.PlayerEndsTurn();
				EndingTurnFor(player);
			}

			if (lastPlayer != player)
			{
				if (player is Character character)
					character.PlayerStartsTurn();
				player.StartTurnResetState();
				StartingTurnFor(player);
			}

			lastPlayer = player;
		}
		public CastedSpell Cast(Character player, Spell spell, Creature targetCreature)
		{
			if (targetCreature != null && player.ActiveTarget == null)
				player.ActiveTarget = new Target(AttackTargetType.Spell, targetCreature);
			return Cast(player, spell);
		}
		public CastedSpell Cast(Character player, Spell spell)
		{
			if (spell.CastingTime == DndTimeSpan.OneAction || spell.CastingTime == DndTimeSpan.OneBonusAction)
			{
				CreatureTakingAction(player);
			}

			CastedSpell castedSpell = new CastedSpell(spell, player);
			castedSpell.Casting();

			nextTarget = player.ActiveTarget;

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
			if (castedSpell.Target == null && player != null)
				castedSpell.Target = player.ActiveTarget;
			player.AboutToCompleteCast();
			player.UseSpellSlot(castedSpell.SpellSlotLevel);
			RemoveSpellFromCasting(castedSpell);
			RemoveActiveSpell(castedSpell);

			Spell spell = castedSpell.Spell;
			if (spell.Duration.HasValue())
			{
				DndAlarm dndAlarm = timeClock.CreateAlarm(spell.Duration.GetTimeSpan(), GetSpellAlarmName(spell, player.playerID), player, castedSpell);
				dndAlarm.AlarmFired += DndAlarm_SpellDurationExpired;
			}

			activeSpells.Add(castedSpell);
			castedSpell.Cast();
		}

		private void RemoveActiveSpell(CastedSpell castedSpell)
		{
			if (activeSpells.IndexOf(castedSpell) >= 0)
				activeSpells.Remove(castedSpell);
			for (int i = activeSpells.Count - 1; i >= 0; i--)  // Counting backwards because we might be removing duplicate spells.
			{
				CastedSpell x = activeSpells[i];
				if (x.Spell.Name == castedSpell.Spell.Name && x.SpellCaster == castedSpell.SpellCaster)
					x.Dispel(castedSpell.SpellCaster); // Removes the spell if active.
			}
		}

		private void RemoveSpellFromCasting(CastedSpell castedSpell)
		{
			if (castingSpells.IndexOf(castedSpell) >= 0)
				castingSpells.Remove(castedSpell);
		}

		public void Dispel(CastedSpell castedSpell)
		{
			if (activeSpells.IndexOf(castedSpell) < 0)
				return;

			activeSpells.Remove(castedSpell);
			timeClock.RemoveAlarm(GetSpellAlarmName(castedSpell.Spell, castedSpell.SpellCaster.playerID));
			OnSpellDispelled(this, new CastedSpellEventArgs(this, castedSpell));  // Triggers the event.
			castedSpell.Dispel();  // Sets its Active state to false and evaluates low-level dispel code associated with this spell.
		}

		private void DndAlarm_SpellDurationExpired(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Data is CastedSpell castedSpell)
			{
				if (castedSpell.SpellCaster?.concentratedSpell == castedSpell)
					castedSpell.SpellCaster.BreakConcentration();

				Dispel(castedSpell);
			}
		}

		public List<CastedSpell> GetActiveSpells(Character character)
		{
			return activeSpells.FindAll(x => x.SpellCaster == character);
		}

		public void CreaturePreparesAttack(Creature creature, Creature target, Attack attack, bool usesMagic)
		{
			if (!(creature is Character player))
				return;

			player.targetThisRollIsCreature = target != null;
			player.usesMagicThisRoll = usesMagic;
			List<CastedSpell> playersActiveSpells = GetActiveSpells(player);
			
			// Triggering in reverse order in case any of these spells are dispelled.
			for (int i = playersActiveSpells.Count - 1; i >= 0; i--)
			{
				CastedSpell castedSpell = playersActiveSpells[i];
				castedSpell.Spell.TriggerPlayerPreparesAttack(player, WaitingForRollTarget, castedSpell);
			}
		}

		public void DieRollStopped(Creature creature, int score, DiceStoppedRollingData diceStoppedRollingData)
		{
			OnDieRollStopped(creature, diceStoppedRollingData);
			if (WaitingForRollHiddenThreshold != int.MinValue && creature == WaitingForRollCreature)
			{
				if (DndUtils.IsAttack(WaitingForRollType) && score >= WaitingForRollHiddenThreshold)
				{
					OnCreatureHitsTarget(creature);
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

		void OnCreatureHitsTarget(Creature creature)
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
		void OnDieRollStopped(Creature creature, DiceStoppedRollingData diceStoppedRollingData)
		{
			if (WaitingForRollHiddenThreshold == int.MinValue || creature != WaitingForRollCreature)
				return;

			if (!(creature is Character player))
				return;

			List<CastedSpell> playersActiveSpells = activeSpells.FindAll(x => x.SpellCaster == player);
			for (int i = playersActiveSpells.Count - 1; i >= 0; i--)
			{
				CastedSpell castedSpell = playersActiveSpells[i];
				castedSpell.Spell.TriggerDieRollStopped(player, WaitingForRollTarget, castedSpell, diceStoppedRollingData);
			}
		}

		public void GetReadyToPlay()
		{
			dndGameEventArgs.Game = this;
			activeMap = null;
			nextTarget = null;
			roundReminders.Clear();
			maps.Clear();
			monsters.Clear();
			ActiveMap = null;
			Players.Clear();
			KnownCreatures.Clear();
			ClearAllAlarms();
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

		public DndAlarm CreateAlarm(string name, TimeSpan fromNow, DndTimeEventHandler alarmHandler = null, object data = null, Character player = null)
		{
			DndAlarm dndAlarm = timeClock.CreateAlarm(fromNow, name, player, data);
			if (alarmHandler != null)
				dndAlarm.AlarmFired += alarmHandler;
			return dndAlarm;
		}

		public DndAlarm CreateAlarm(string name, DndTimeSpan fromNow, DndTimeEventHandler alarmHandler = null, object data = null, Character player = null)
		{
			DndAlarm dndAlarm = timeClock.CreateAlarm(fromNow.GetTimeSpan(), name, player, data);
			if (fromNow.RoundSpecifier != RoundSpecifier.None)
				dndAlarm.RoundSpecifier = fromNow.RoundSpecifier;
			if (alarmHandler != null)
				dndAlarm.AlarmFired += alarmHandler;
			return dndAlarm;
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
			OnRequestMessageToDungeonMaster(this, new MessageEventArgs(message));
		}

		public void TellAll(string message)
		{
			lastMessageSentToDungeonMaster = message;
			OnRequestMessageToAll(this, new MessageEventArgs(message));
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

		public string GetRemainingSpellTimeStr(int playerId, Spell spell)
		{
			return DndUtils.GetTimeSpanStr(GetRemainingSpellTime(playerId, spell));
		}

		public TimeSpan GetRemainingSpellTime(int playerId, Spell spell)
		{
			DndAlarm alarm = Clock.GetAlarm(GetSpellAlarmName(spell, playerId));
			if (alarm == null)
				return TimeSpan.FromSeconds(0);

			return alarm.TriggerTime - Clock.Time;
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
					concentrationReport += $"{player.emoticon} {player.name} is casting {spell.Name} with {GetRemainingSpellTimeStr(player.playerID, spell)} remaining; ";
			}
			if (concentrationReport == string.Empty)
				concentrationReport = "No players are concentrating on any spells at this time.";
			return concentrationReport;
		}
		public void ClearAllAlarms()
		{
			timeClock.ClearAllAlarms();
		}
		public void RechargePlayersAfterLongRest()
		{
			foreach (Character player in Players)
			{
				player.RechargeAfterLongRest();
			}
		}
		public void RechargePlayersAfterShortRest()
		{
			foreach (Character player in Players)
			{
				player.RechargeAfterShortRest();
			}
		}

		protected virtual void OnRequestMessageToDungeonMaster(object sender, MessageEventArgs ea)
		{
			RequestMessageToDungeonMaster?.Invoke(sender, ea);
		}
		protected virtual void OnRequestMessageToAll(object sender, MessageEventArgs ea)
		{
			RequestMessageToAll?.Invoke(sender, ea);
		}
		public void Start()
		{
			foreach (Character player in Players)
			{
				player.StartGame();
			}
		}

		private void SendReminders(Creature creature, TurnPoint point)
		{
			for (int i = roundReminders.Count - 1; i >= 0; i--)
			{
				RoundReminder roundReminder = roundReminders[i];
				if (roundReminder.Creature == creature && roundReminder.TurnPoint == point && roundReminder.RoundNumber == roundIndex)
				{
					TellDungeonMaster(roundReminder.ReminderMessage);
					roundReminders.RemoveAt(i);
				}
			}
		}

		List<RoundReminder> roundReminders = new List<RoundReminder>();
		public void TellDmInRounds(int roundOffset, string reminder, TurnPoint roundPoint = TurnPoint.Start)
		{
			if (roundReminders == null)
				roundReminders = new List<RoundReminder>();
			RoundReminder item = new RoundReminder();
			item.Creature = activeCreature;
			item.RoundNumber = roundIndex + roundOffset;
			// TODO: Make this work for RoundPoint.End as well.
			item.TurnPoint = roundPoint;
			item.ReminderMessage = reminder;
			roundReminders.Add(item);
		}
		public void PreparePlayersForSerialization()
		{
			foreach (Character player in Players)
			{
				player.PrepareForSerialization();
			}
		}
		public bool PlayerIsCastingSpell(CastedSpell castedSpell, int playerId)
		{
			if (castedSpell == null)
				return false;
			DndAlarm alarm = Clock.GetAlarm(GetSpellAlarmName(castedSpell.Spell, playerId));
			return alarm != null;
		}
		void ChangeWealth(int playerId, decimal totalGold)
		{
			Character player = GetPlayerFromId(playerId);
			if (player == null)
				return;
			player.ChangeWealth(totalGold);
		}
		public void ChangeWealth(WealthChange wealthChange)
		{
			foreach (int playerId in wealthChange.PlayerIds)
			{
				ChangeWealth(playerId, wealthChange.Coins.TotalGold);
			}
		}

		public bool ClockIsRunning()
		{
			return !Clock.InCombat && !Clock.InTimeFreeze;
		}

		public bool ClockHasStopped()
		{
			return Clock.InCombat || Clock.InTimeFreeze;
		}
		public void CheckAlarmsPlayerStartsTurn(Character character)
		{
			Clock.CheckAlarmsPlayerStartsTurn(character);
		}
		public void CheckAlarmsPlayerEndsTurn(Character character)
		{
			Clock.CheckAlarmsPlayerEndsTurn(character);
		}

	}
}
