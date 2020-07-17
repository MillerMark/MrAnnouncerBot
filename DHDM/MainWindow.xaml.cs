//#define profiling
using Microsoft.AspNetCore.SignalR.Client;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using System;
using DndCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimeLineControl;
using DndUI;
using BotCore;
using System.Threading;
using OBSWebsocketDotNet;
using Newtonsoft.Json;
using ICSharpCode.AvalonEdit;
using System.IO;
using GoogleHelper;
using System.Reflection;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Controls.Primitives;
using System.Globalization;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDungeonMasterApp
	{
		Dictionary<Character, List<AskUI>> askUIs = new Dictionary<Character, List<AskUI>>();
		//protected const string DungeonMasterChannel = "DragonHumpersDm";
		const string DungeonMasterChannel = "HumperBot";
		const string DragonHumpersChannel = "DragonHumpers";
		const string CodeRushedChannel = "CodeRushed";
		const string twitchIndent = "͏͏͏͏͏͏͏͏͏͏͏͏̣　　͏͏͏̣ 　　͏͏͏̣ ";  // This sequence allows indentation in Twitch chats!

		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		DungeonMasterChatBot dmChatBot = new DungeonMasterChatBot();
		TwitchClient dungeonMasterClient;

		List<PlayerActionShortcut> actionShortcuts = new List<PlayerActionShortcut>();
		ScrollPage activePage = ScrollPage.main;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer showClearButtonTimer;
		DispatcherTimer stateUpdateTimer;
		DispatcherTimer cropRectUpdateTimer;
		DispatcherTimer reloadSpellsTimer;
		DispatcherTimer pendingShortcutsTimer;
		DispatcherTimer wildMagicRollTimer;
		DispatcherTimer switchBackToPlayersTimer;
		DispatcherTimer updateClearButtonTimer;
		DispatcherTimer needToSaveCodeTimer;
		DateTime lastUpdateTime;
		int keepExistingModifier = int.MaxValue;
		DndGame game = null;

		public MainWindow()
		{
			changingInternally = true;
			try
			{
				InitializeGame();
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void InitializeGame()
		{
			game = new DndGame();
			HookGameEvents();
			realTimeAdvanceTimer = new DispatcherTimer(DispatcherPriority.Send);
			realTimeAdvanceTimer.Tick += new EventHandler(RealTimeClockHandler);
			realTimeAdvanceTimer.Interval = TimeSpan.FromMilliseconds(200);

			showClearButtonTimer = new DispatcherTimer();
			showClearButtonTimer.Tick += new EventHandler(ShowClearButton);
			showClearButtonTimer.Interval = TimeSpan.FromSeconds(8);

			stateUpdateTimer = new DispatcherTimer();
			stateUpdateTimer.Tick += new EventHandler(UpdateStateFromTimer);
			stateUpdateTimer.Interval = TimeSpan.FromSeconds(1);
			stateUpdateTimer.Start();
			
			cropRectUpdateTimer = new DispatcherTimer();
			cropRectUpdateTimer.Tick += new EventHandler(UpdateCropRectFromTimer);
			cropRectUpdateTimer.Interval = TimeSpan.FromSeconds(0.1);

			reloadSpellsTimer = new DispatcherTimer();
			reloadSpellsTimer.Tick += new EventHandler(CheckForNewSpells);
			reloadSpellsTimer.Interval = TimeSpan.FromSeconds(1.2);

			pendingShortcutsTimer = new DispatcherTimer();
			pendingShortcutsTimer.Tick += new EventHandler(ActivatePendingShortcuts);
			pendingShortcutsTimer.Interval = TimeSpan.FromSeconds(1);

			wildMagicRollTimer = new DispatcherTimer();
			wildMagicRollTimer.Tick += new EventHandler(RollWildMagicHandler);
			wildMagicRollTimer.Interval = TimeSpan.FromSeconds(9);

			switchBackToPlayersTimer = new DispatcherTimer();
			switchBackToPlayersTimer.Tick += new EventHandler(SwitchBackToPlayersHandler);
			switchBackToPlayersTimer.Interval = TimeSpan.FromSeconds(3);

			updateClearButtonTimer = new DispatcherTimer(DispatcherPriority.Send);
			updateClearButtonTimer.Tick += new EventHandler(UpdateClearButton);
			updateClearButtonTimer.Interval = TimeSpan.FromMilliseconds(80);

			History.TimeClock = game.Clock;
			game.Clock.TimeChanged += DndTimeClock_TimeChanged;
			// TODO: Save and retrieve game time between games (or allow DM to set start time for every game).
			game.Clock.SetTime(DateTime.Now);
			InitializeComponent();
			FocusHelper.FocusedControlsChanged += FocusHelper_FocusedControlsChanged;
			groupEffectBuilder.Entries = new ObservableCollection<TimeLineEffect>();
			spTimeSegments.DataContext = game.Clock;
			logListBox.ItemsSource = History.Entries;
			History.LogUpdated += History_LogUpdated;

			//InitializeAttackShortcuts();
			//humperBotClient = Twitch.CreateNewClient("HumperBot", "HumperBot", "HumperBotOAuthToken");
			CreateDungeonMasterClient();

			dmChatBot.Initialize(this);

			dmChatBot.DungeonMasterApp = this;
			commandParsers.Add(dmChatBot);
			ConnectToObs();
			HookEvents();

			SetupSpellsChangedFileWatcher();
			LoadAvalonEditor();
		}

		void LoadTextCompletionEngine()
		{
			TextCompletionEngine = new TextCompletionEngine(tbxCode);
			TextCompletionEngine.RequestTextSave += SaveCodeChanges;
			TextCompletionEngine.ReloadShortcuts();
		}

		private void LoadAvalonEditor()
		{
			LoadAvalonSyntaxHighlighter();
			LoadTextCompletionEngine();
		}

		public TextCompletionEngine TextCompletionEngine { get; set; }

		private void HookGameEvents()
		{
			game.SpellDispelled += Game_SpellDispelled;
			game.PickWeapon += Game_PickWeapon;
			game.PickAmmunition += Game_PickAmmunition;
			game.PlayerShowState += Game_PlayerShowState;
			game.RequestMessageToDungeonMaster += Game_RequestMessageToDungeonMaster;
			game.RequestMessageToAll += Game_RequestMessageToAll;
			game.PlayerRequestsRoll += Game_PlayerRequestsRoll;
			game.PlayerStateChanged += Game_PlayerStateChanged;
			game.RoundStarting += Game_RoundStarting;
		}

		private void Game_PlayerShowState(object sender, PlayerShowStateEventArgs ea)
		{
			string outlineColor = ea.OutlineColor;
			string fillColor = ea.FillColor;
			if (outlineColor == "player")
				outlineColor = ea.Player.dieFontColor;
			if (fillColor == "player")
				fillColor = ea.Player.dieBackColor;
			HubtasticBaseStation.FloatPlayerText(ea.Player.playerID, ea.Message, fillColor, outlineColor);
		}

		private void Game_RoundStarting(object sender, DndGameEventArgs ea)
		{
			if (game.InCombat)
				clockMessage = $"Round {ea.Game.roundIndex + 1}";
		}

		private void Game_PickWeapon(object sender, PickWeaponEventArgs ea)
		{
			List<string> weapons = new List<string>();
			List<CarriedWeapon> filteredWeapons = new List<CarriedWeapon>();
			int weaponNumber = 1;
			string filterLower = null;
			if (ea.WeaponFilter != null)
				filterLower = ea.WeaponFilter.ToLower();
			for (int i = 0; i < ea.Player.CarriedWeapons.Count; i++)
			{
				CarriedWeapon carriedWeapon = ea.Player.CarriedWeapons[i];
				string weaponName = carriedWeapon.Name;
				string weaponKind = carriedWeapon.Weapon.Name;
				string weaponKindLower = weaponKind.ToLower();
				if (!string.IsNullOrEmpty(filterLower))
				{
					if (!filterLower.Contains(weaponKindLower))
						continue;
				}
				if (string.IsNullOrEmpty(weaponName))
					weaponName = weaponKind;
				weapons.Add($"{weaponNumber}: {weaponName}");
				filteredWeapons.Add(carriedWeapon);
				weaponNumber++;
			}

			if (weapons.Count <= 0)
				return;
			int result = AskQuestion("Target which weapon: ", weapons);
			if (result > 0)
				ea.Weapon = filteredWeapons[result - 1];
		}

		private void Game_PickAmmunition(object sender, PickAmmunitionEventArgs ea)
		{
			List<string> ammunitionList = new List<string>();

			List<CarriedAmmunition> filteredAmmunition = new List<CarriedAmmunition>();
			int ammunitionNumber = 1;
			string filterLower = null;
			if (ea.AmmunitionKind != null)
				filterLower = ea.AmmunitionKind.ToLower();
			for (int i = 0; i < ea.Player.CarriedAmmunition.Count; i++)
			{
				CarriedAmmunition carriedAmmunition = ea.Player.CarriedAmmunition[i];
				if (carriedAmmunition.Count <= 0)
					continue;
				string ammunitionName = carriedAmmunition.Name;
				string ammunitionKind = carriedAmmunition.Kind;
				if (!string.IsNullOrEmpty(filterLower))
				{
					if (!filterLower.Contains(ammunitionKind.ToLower()))
						continue;
				}
				if (string.IsNullOrEmpty(ammunitionName))
					ammunitionName = ammunitionKind;
				ammunitionList.Add($"{ammunitionNumber}: {ammunitionName} ({carriedAmmunition.Count})");
				filteredAmmunition.Add(carriedAmmunition);
				ammunitionNumber++;
			}

			if (ammunitionList.Count <= 0)
				return;
			int result = AskQuestion("Choose ammunition: ", ammunitionList);
			if (result > 0)
				ea.Ammunition = filteredAmmunition[result - 1];
		}

		private void HookEvents()
		{
			Expressions.ExceptionThrown += Expressions_ExceptionThrown;
			Expressions.ExecutionChanged += Expressions_ExecutionChanged;
			AskFunction.AskQuestion += AskFunction_AskQuestion;  // static event handler.
			GetRoll.GetRollRequest += GetRoll_GetRollRequest;
			Feature.FeatureDeactivated += Feature_FeatureDeactivated;
			Feature.RequestMessageToDungeonMaster += Game_RequestMessageToDungeonMaster;
			AddReminderFunction.AddReminderRequest += AddReminderFunction_AddReminderRequest;
			ActivateShortcutFunction.ActivateShortcutRequest += ActivateShortcutFunction_ActivateShortcutRequest;
			DndCharacterProperty.AskingValue += DndCharacterProperty_AskingValue;
			PlaySceneFunction.RequestPlayScene += PlaySceneFunction_RequestPlayScene;
			SelectTargetFunction.RequestSelectTarget += SelectTargetFunction_RequestSelectTarget;
			SelectMonsterFunction.RequestSelectMonster += SelectMonsterFunction_RequestSelectMonster;
		}

		private void SelectMonsterFunction_RequestSelectMonster(object sender, SelectMonsterEventArgs ea)
		{
			if (ActivePlayer != null && !string.IsNullOrEmpty(ActivePlayer.NextAnswer))
			{
				ea.Monster = AllMonsters.Get(ActivePlayer.NextAnswer);
				ActivePlayer.NextAnswer = null;
				if (ea.Monster != null)
					return;
			}
			FrmMonsterPicker frmMonsterPicker = new FrmMonsterPicker();
			frmMonsterPicker.Owner = this;
			frmMonsterPicker.spMonsters.DataContext = AllMonsters.Monsters.Where(x => x.challengeRating <= ea.MaxChallengeRating &&
			(x.kind & ea.CreatureKindFilter) == x.kind &&
			x.swimmingSpeed <= ea.MaxSwimmingSpeed && x.flyingSpeed <= ea.MaxFlyingSpeed).ToList();
			frmMonsterPicker.ShowDialog();
			ea.Monster = frmMonsterPicker.SelectedMonster;
		}

		private void SelectTargetFunction_RequestSelectTarget(TargetEventArgs ea)
		{
			// TODO: Add support for auto-select based on next answer.
			FrmSelectInGameCreature frmSelectInGameCreature = new FrmSelectInGameCreature();

			frmSelectInGameCreature.SetDataSources(AllInGameCreatures.Creatures, game.Players);

			if (frmSelectInGameCreature.ShowDialog() == true)
			{
				ea.Target = new Target();
				foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
					if (inGameCreature.IsSelected)
						ea.Target.AddCreature(inGameCreature.Creature);
				foreach (Character player in game.Players)
					if (player.IsSelected)
						ea.Target.AddCreature(player);
			}
		}

		private void PlaySceneFunction_RequestPlayScene(object sender, PlaySceneEventArgs ea)
		{
			PlayScene(ea.SceneName);
		}

		void AddBooleanAsk(AskUI askUI)
		{
			CheckBox booleanAsk = new CheckBox();
			booleanAsk.Margin = new Thickness(2, 2, 8, 2);
			booleanAsk.Tag = askUI;
			booleanAsk.Content = askUI.Caption;
			booleanAsk.Checked += BooleanAsk_CheckChanged;
			booleanAsk.Unchecked += BooleanAsk_CheckChanged;
			booleanAsk.IsChecked = askUI.GetBooleanValue();
			wpAskUI.Children.Add(booleanAsk);
		}

		private void BooleanAsk_CheckChanged(object sender, RoutedEventArgs e)
		{
			SetBooleanPropertyFromCheckbox(sender);
			SetShortcutVisibility();
		}

		private static void SetBooleanPropertyFromCheckbox(object sender)
		{
			if (!(sender is CheckBox checkBox))
				return;
			if (!(checkBox.Tag is AskUI askUI))
				return;
			askUI.SetBooleanProperty(checkBox.IsChecked);
		}

		void AddStringAsk(AskUI askUI)
		{
			System.Diagnostics.Debugger.Break();
		}

		void UpdateAskUI(Character player)
		{
			if (!askUIs.ContainsKey(player))
				return;
			foreach (var uIElement in wpAskUI.Children)
			{
				if (uIElement is CheckBox checkBox)
				{
					if (checkBox.Tag is AskUI askUI)
					{
						checkBox.IsChecked = askUI.GetBooleanValue();
					}
				}
				// TODO: Add support for text values here.
			}
		}

		void RebuildAskUI(Character player)
		{
			wpAskUI.Children.Clear();
			if (player == null)
				return;
			if (!askUIs.ContainsKey(player))
				return;
			List<AskUI> asks = askUIs[player];

			List<AskUI> dupList = asks.ToList();
			for (int i = 0; i < dupList.Count; i++)
			{
				AskUI askUI = dupList[i];
				switch (askUI.MemberTypeName)
				{
					case "Boolean":
						AddBooleanAsk(askUI);
						break;
					case "String":
						AddStringAsk(askUI);
						break;
				}
			}
		}

		private void DndCharacterProperty_AskingValue(object sender, AskValueEventArgs ea)
		{
			bool needToRebuildAskUI = false;
			if (!askUIs.ContainsKey(ea.Player))
			{
				askUIs.Add(ea.Player, new List<AskUI>());
				needToRebuildAskUI = true;
			}
			List<AskUI> asks = askUIs[ea.Player];
			AskUI foundAsk = asks.FirstOrDefault(x => x.MemberName == ea.MemberName);
			if (foundAsk == null)
			{
				foundAsk = new AskUI(ea);
				asks.Add(foundAsk);
				needToRebuildAskUI = true;
			}

			if (needToRebuildAskUI)
				RebuildAskUI(ea.Player);

			ea.Value = foundAsk.Value;
		}

		private void Expressions_ExecutionChanged(object sender, CodingSeb.ExpressionEvaluator.ExecutionPointerChangedEventArgs ea)
		{

		}

		void SetupSpellsChangedFileWatcher()
		{
			spellsChangedFileWatcher = new FileSystemWatcher
			{
				Path = Folders.CoreData,
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "DnD - Spells*.csv"
			};
			spellsChangedFileWatcher.Changed += SpellsUpdated;
			spellsChangedFileWatcher.EnableRaisingEvents = true;
		}

		string newSpellFileName;
		void ReplaceSpellsDataFile(string fullPath)
		{
			newSpellFileName = fullPath;

			reloadSpellsTimer.Stop();
			reloadSpellsTimer.Start();
		}

		void CheckForNewSpells(object sender, EventArgs e)
		{
			reloadSpellsTimer.Stop();

			try
			{
				if (!newSpellFileName.Contains('(')) // Chrome adds "(1)", "(2)", etc onto files saved to prevent name collision.
					return;  // If no parens exist, user saved with an overwrite, so no need to replace.
				string spellDataFileName = System.IO.Path.Combine(Folders.CoreData, "DnD - Spells.csv");

				if (!File.Exists(newSpellFileName))
					return;

				File.Delete(spellDataFileName);
				File.Move(newSpellFileName, spellDataFileName);
			}
			finally
			{
				Dispatcher.Invoke(() =>
				{
					SpellDto selectedItem = (SpellDto)lstAllSpells.SelectedItem;
					string spellName = selectedItem?.name;
					ReloadSpells();
					if (string.IsNullOrWhiteSpace(spellName))
						return;

					foreach (object item in lstAllSpells.Items)
					{
						if (item is SpellDto spellDto && spellDto.name == spellName)
						{
							Character player = game.GetPlayerFromId(ActivePlayerId);
							player.ClearAllCasting();

							lstAllSpells.SelectedItem = item;
							return;
						}
					}
				});
			}
		}
		private void SpellsUpdated(object sender, FileSystemEventArgs e)
		{
			ReplaceSpellsDataFile(e.FullPath);
		}

		private void AddReminderFunction_AddReminderRequest(object sender, AddReminderEventArgs ea)
		{
			if (ea.NowDuration == "1 round")
			{
				game.TellDmInRounds(1, ea.Reminder);
			}
			else if (ea.NowDuration == "end of turn")
			{
				game.TellDmInRounds(0, ea.Reminder, TurnPoint.End);
			}
		}

		Dictionary<string, int> savedRolls;
		private void GetRoll_GetRollRequest(object sender, GetRollEventArgs ea)
		{
			if (savedRolls == null)
				return;
			if (savedRolls.ContainsKey(ea.RollName))
				ea.Result = savedRolls[ea.RollName];
		}

		void SaveNamedResults(DiceEventArgs ea)
		{
			if (savedRolls == null)
				savedRolls = new Dictionary<string, int>();
			savedRolls.Clear();
			if (ea.StopRollingData.individualRolls == null)
				return;
			foreach (IndividualRoll individualRoll in ea.StopRollingData.individualRolls)
			{
				if (!string.IsNullOrWhiteSpace(individualRoll.type))
				{
					if (savedRolls.ContainsKey(individualRoll.type))
						savedRolls[individualRoll.type] += individualRoll.value;
					else
						savedRolls.Add(individualRoll.type, individualRoll.value);
				}
			}
		}


		private void Game_RequestMessageToDungeonMaster(object sender, MessageEventArgs ea)
		{
			TellDungeonMaster(ea.Message);
		}

		private void Game_RequestMessageToAll(object sender, MessageEventArgs ea)
		{
			TellAll(ea.Message);
		}

		void EnsureEverythingRemainsHookedUpAsExpected()
		{
			game.Clock.TimeChanged -= DndTimeClock_TimeChanged;
			game.Clock.TimeChanged += DndTimeClock_TimeChanged;
		}

		void SetShortcutVisibility(Panel panel)
		{
			foreach (UIElement uIElement in panel.Children)
			{
				if (uIElement is ShortcutPanel shortcutPanel)
				{
					PlayerActionShortcut shortcut = shortcutPanel.Shortcut;
					if (shortcut.Spell != null)
					{
						Character player = game.GetPlayerFromId(shortcut.PlayerId);
						KnownSpell matchingSpell = player.GetMatchingSpell(shortcut.Spell.Name);
						if (matchingSpell != null && matchingSpell.CanBeRecharged())
						{
							if (matchingSpell.HasAnyCharges())
								shortcutPanel.Visibility = Visibility.Visible;
							else
								shortcutPanel.Visibility = Visibility.Collapsed;
						}
						else if (shortcut.Spell.SpellSlotLevel > 0)
						{
							if (player.HasRemainingSpellSlotCharges(shortcut.Spell.SpellSlotLevel))
								shortcutPanel.Visibility = Visibility.Visible;
							else
								shortcutPanel.Visibility = Visibility.Collapsed;
						}
					}
					string availableWhen = shortcut.Spell?.AvailableWhen;
					if (string.IsNullOrWhiteSpace(availableWhen))
						availableWhen = shortcut.AvailableWhen;

					if (!string.IsNullOrEmpty(availableWhen))
					{
						Character player = game.GetPlayerFromId(shortcut.PlayerId);
						bool shortcutIsAvailable = Expressions.GetBool(availableWhen, player);
						if (shortcutIsAvailable != shortcut.Available)
						{
							shortcut.Available = shortcutIsAvailable;
							if (shortcut.SourceFeature != null)
								shortcut.SourceFeature.ShortcutAvailabilityChange("", player);
						}
						if (shortcutIsAvailable)
							shortcutPanel.Visibility = Visibility.Visible;
						else
							shortcutPanel.Visibility = Visibility.Collapsed;
					}
				}
			}
		}

		void SetShortcutVisibility()
		{
			Dispatcher.Invoke(() =>
			{
				SetShortcutVisibility(wpActionsActivePlayer);
				SetShortcutVisibility(spBonusActionsActivePlayer);
				SetShortcutVisibility(spReactionsActivePlayer);
				SetShortcutVisibility(spSpecialActivePlayer);
			});
		}

		void ClearAllSpellSlots()
		{
			foreach (Character player in game.Players)
			{
				CharacterSheets sheetForCharacter = GetSheetForCharacter(player.playerID);
				sheetForCharacter?.ClearAllSpellSlots();
			}
		}

		bool updatingRechargeables;
		private void Game_PlayerStateChanged(object sender, PlayerStateEventArgs ea)
		{
			if (updatingRechargeables)
				return;

			if (ea.Contains("Rechargeables"))
			{
				UpdateStateUIForPlayer(ea.Player);
			}
			else
			if (ea.IsRechargeable)
			{
				updatingRechargeables = true;
				try
				{
					int playerID = ea.Player.playerID;
					CharacterSheets sheetForCharacter = GetSheetForCharacter(playerID);
					sheetForCharacter?.UpdateRechargeableUI(ea.Key, (int)ea.NewValue);
					UpdateStateUIForPlayer(ea.Player);
				}
				finally
				{
					updatingRechargeables = false;
				}
			}

			if (ea.Player != null)
				UpdateStateUIForPlayer(ea.Player);
			SetShortcutVisibility();
		}

		bool updatingUI;
		private void UpdateStateUIForPlayer(Character player, bool fromTimer = false)
		{
			if (player == null)
				return;

			if (updatingUI)
				return;

			updatingUI = true;
			try
			{
				if (!fromTimer)
					UpdatePlayerScrollOnStream(player);
				Dispatcher.Invoke(() =>
				{
					ListBox stateList = GetStateListForCharacter(player.playerID);
					if (stateList != null)
					{
						stateList.Items.Clear();
						List<string> stateReport = player.GetStateReport();
						stateReport.Sort();
						if (player.concentratedSpell != null)
							stateReport.Add($"*Concentrating on {player.concentratedSpell.Spell.Name} with {game.GetRemainingSpellTimeStr(player.playerID, player.concentratedSpell.Spell)} remaining.");

						List<CastedSpell> activeSpells = game.GetActiveSpells(player);
						if (activeSpells != null && activeSpells.Count > 0)
							foreach (CastedSpell activeSpell in activeSpells)
								stateReport.Add($"Active spell: {activeSpell.Spell.Name} with {game.GetRemainingSpellTimeStr(player.playerID, activeSpell.Spell)} remaining.");

						foreach (string item in stateReport)
							stateList.Items.Add(item);
					}
					SetClearSpellVisibility(player);
				});
			}
			finally
			{
				updatingUI = false;
			}
		}

		Dictionary<DispatcherTimer, PlayerActionShortcut> shortcutTimers = new Dictionary<DispatcherTimer, PlayerActionShortcut>();
		private void ActivateShortcutFunction_ActivateShortcutRequest(object sender, ShortcutEventArgs ea)
		{
			Dispatcher.Invoke(() =>
			{
				if (ea.DelayMs == 0)
				{
					if (waitingToClearDice && !string.IsNullOrEmpty(ea.Shortcut.InstantDice))
						shortcutToActivateAfterClearingDice = ea.Shortcut;
					else
						ActivateShortcut(ea.Shortcut);
				}
				else
				{
					DispatcherTimer dispatcherTimer = new DispatcherTimer();
					dispatcherTimer.Interval = TimeSpan.FromMilliseconds(ea.DelayMs);
					shortcutTimers.Add(dispatcherTimer, ea.Shortcut);
					dispatcherTimer.Tick += ActivateShortcutTimer_Tick;
					dispatcherTimer.Start();
				}
			});

		}

		private void ActivateShortcutTimer_Tick(object sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();
			dispatcherTimer.Tick -= ActivateShortcutTimer_Tick;

			if (!shortcutTimers.ContainsKey(dispatcherTimer))
				return;

			PlayerActionShortcut shortcutToActivate = shortcutTimers[dispatcherTimer];
			shortcutTimers.Remove(dispatcherTimer);
			ActivateShortcut(shortcutToActivate);
			UnleashTheNextRoll();
		}

		// TODO: Spell Expired
		private void DndAlarm_AlarmFired(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Name.StartsWith(DndGame.STR_EndSpell))
			{
				string spellToEnd = ea.Alarm.Name.Substring(DndGame.STR_EndSpell.Length);
				EndSpellEffects(spellToEnd);
			}
		}

		private void CreateDungeonMasterClient()
		{
			Dispatcher.Invoke(() =>
			{
				Background = Brushes.White;
				btnReconnectTwitchClient.Visibility = Visibility.Hidden;
			});
			dungeonMasterClient = Twitch.CreateNewClient("DragonHumpersDM", "DragonHumpersDM", "DragonHumpersDmOAuthToken");
			HookTwitchClientEvents();
		}

		private void HookTwitchClientEvents()
		{
			if (dungeonMasterClient == null)
				return;
			dungeonMasterClient.OnMessageReceived += HumperBotClient_OnMessageReceived;
			dungeonMasterClient.OnConnectionError += DungeonMasterClient_OnConnectionError;
			dungeonMasterClient.OnDisconnected += DungeonMasterClient_OnDisconnected;
			dungeonMasterClient.OnError += DungeonMasterClient_OnError;
			dungeonMasterClient.OnFailureToReceiveJoinConfirmation += DungeonMasterClient_OnFailureToReceiveJoinConfirmation;
			dungeonMasterClient.OnJoinedChannel += DungeonMasterClient_OnJoinedChannel;
			dungeonMasterClient.OnLeftChannel += DungeonMasterClient_OnLeftChannel;
			dungeonMasterClient.OnLog += DungeonMasterClient_OnLog;
			dungeonMasterClient.OnMessageThrottled += DungeonMasterClient_OnMessageThrottled;
			dungeonMasterClient.OnNoPermissionError += DungeonMasterClient_OnNoPermissionError;
			dungeonMasterClient.OnChannelStateChanged += DungeonMasterClient_OnChannelStateChanged;
		}
		private void UnhookTwitchClientEvents()
		{
			if (dungeonMasterClient == null)
				return;
			dungeonMasterClient.OnMessageReceived -= HumperBotClient_OnMessageReceived;
			dungeonMasterClient.OnConnectionError -= DungeonMasterClient_OnConnectionError;
			dungeonMasterClient.OnDisconnected -= DungeonMasterClient_OnDisconnected;
			dungeonMasterClient.OnError -= DungeonMasterClient_OnError;
			dungeonMasterClient.OnFailureToReceiveJoinConfirmation -= DungeonMasterClient_OnFailureToReceiveJoinConfirmation;
			dungeonMasterClient.OnJoinedChannel -= DungeonMasterClient_OnJoinedChannel;
			dungeonMasterClient.OnLeftChannel -= DungeonMasterClient_OnLeftChannel;
			dungeonMasterClient.OnLog -= DungeonMasterClient_OnLog;
			dungeonMasterClient.OnMessageThrottled -= DungeonMasterClient_OnMessageThrottled;
			dungeonMasterClient.OnNoPermissionError -= DungeonMasterClient_OnNoPermissionError;
			dungeonMasterClient.OnChannelStateChanged -= DungeonMasterClient_OnChannelStateChanged;
		}

		private void DungeonMasterClient_OnChannelStateChanged(object sender, TwitchLib.Client.Events.OnChannelStateChangedArgs e)
		{
			History.Log($"Channel ({e.Channel}) state changed ({e.ChannelState.Channel})");
		}

		private void DungeonMasterClient_OnNoPermissionError(object sender, EventArgs e)
		{
			History.Log("DungeonMasterClient_OnNoPermissionError");
		}

		private void DungeonMasterClient_OnMessageThrottled(object sender, TwitchLib.Communication.Events.OnMessageThrottledEventArgs e)
		{
			History.Log($"Message (\"{e.Message}\") throttled ({e.SentMessageCount} messages sent in {e.Period.TotalSeconds} seconds - only {e.AllowedInPeriod} allowed)");
		}

		private void DungeonMasterClient_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
		{
			History.Log($"BotUsername (\"{e.BotUsername}\") logged ({e.Data})");
		}

		private void DungeonMasterClient_OnLeftChannel(object sender, TwitchLib.Client.Events.OnLeftChannelArgs e)
		{
			History.Log($"User (\"{e.BotUsername}\") left channel ({e.Channel})");
		}

		private void DungeonMasterClient_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
		{
			History.Log($"User (\"{e.BotUsername}\") joined channel ({e.Channel})");
		}

		private void DungeonMasterClient_OnFailureToReceiveJoinConfirmation(object sender, TwitchLib.Client.Events.OnFailureToReceiveJoinConfirmationArgs e)
		{
			History.Log($"Channel (\"{e.Exception.Channel}\") FailureToReceiveJoinConfirmation ({e.Exception.Details})");
		}

		private void DungeonMasterClient_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
		}

		private void DungeonMasterClient_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				Background = Brushes.Red;
				btnReconnectTwitchClient.Visibility = Visibility.Visible;
			});
			UnhookTwitchClientEvents();
			dungeonMasterClient = null;
			History.Log($"DungeonMasterClient_OnDisconnected");
		}

		private void DungeonMasterClient_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
		{
			History.Log($"Connection error for \"{e.BotUsername}\" with message \"{e.Error.Message}\"");
		}

		private void Feature_FeatureDeactivated(object sender, FeatureEventArgs ea)
		{
			if (ea.Feature.Name == "WildSurgeRage")
			{
				if (lastScenePlayed == "DH.WildSurge.PlantGrowth.Arrive")
				{
					PlayScene("DH.WildSurge.PlantGrowth.Leave");
					BackToPlayersIn(10);
				}
				else
					PlayScene("Players");
			}
		}

		public class AnswerMap
		{
			public int Index { get; set; }
			public int Value { get; set; }
			public string AnswerText { get; set; }
			public AnswerMap(int index, int value, string answerText)
			{
				Index = index;
				Value = value;
				AnswerText = answerText;
			}
			public static AnswerMap FromAnswer(string answerStr, int index)
			{
				AnswerMap result = new AnswerMap();
				result.Index = index;
				string workStr = answerStr.Trim(new char[] { '"', ' ' });
				int colonPos = workStr.IndexOf(':');
				if (colonPos > 0)
				{
					string valueStr = workStr.EverythingBefore(":");
					result.AnswerText = workStr.EverythingAfter(":").Trim();
					if (int.TryParse(valueStr, out int value))
						result.Value = value;
				}

				return result;
			}
			public AnswerMap()
			{

			}
		}
		List<AnswerMap> answerMap;

		int AskQuestion(string question, List<string> answers)
		{
			bool timerWasRunning = realTimeAdvanceTimer.IsEnabled;
			if (timerWasRunning)
				realTimeAdvanceTimer.Stop();
			waitingForAnswerToQuestion = true;
			try
			{
				BuildAnswerMap(answers);

				if (ActivePlayer != null && !string.IsNullOrEmpty(ActivePlayer.NextAnswer))
				{
					AnswerMap selectedAnswer;
					if (ActivePlayer.NextAnswer.EndsWith("*"))
					{
						string searchPattern = ActivePlayer.NextAnswer.EverythingBefore("*");
						selectedAnswer = answerMap.FirstOrDefault(x => x.AnswerText.StartsWith(searchPattern));
					}
					else
						selectedAnswer = answerMap.FirstOrDefault(x => x.AnswerText == ActivePlayer.NextAnswer);
					ActivePlayer.NextAnswer = null;
					if (selectedAnswer != null)
						return selectedAnswer.Index;
				}


				TellDungeonMaster($"{Icons.QuestionBlock} {twitchIndent}" + question);
				foreach (AnswerMap answer in answerMap)
				{
					TellDungeonMaster($"{twitchIndent}{twitchIndent}{twitchIndent}{twitchIndent}{twitchIndent}{twitchIndent} {answer.Index}. {answer.AnswerText}");
				}

				return FrmAsk.Ask(question, answers, this);
			}
			finally
			{
				waitingForAnswerToQuestion = false;
				answerMap = null;
				if (timerWasRunning)
					StartRealTimeTimer();
			}
		}

		private void BuildAnswerMap(List<string> answers)
		{
			answerMap = new List<AnswerMap>();
			List<string> textAnswers = new List<string>();
			int index = 1;
			bool firstTimeIn = true;
			foreach (string answer in answers)
			{
				if (firstTimeIn && answer.ToLower().IndexOf("zero") >= 0)
					index = 0;
				firstTimeIn = false;

				AnswerMap thisAnswer = AnswerMap.FromAnswer(answer, index);
				answerMap.Add(thisAnswer);
				textAnswers.Add($"{index}. " + thisAnswer.AnswerText);
				index++;
			}
		}

		private void AskFunction_AskQuestion(object sender, AskEventArgs ea)
		{
			ea.Result = AskQuestion(ea.Question, ea.Answers);
		}

		private void Expressions_ExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
		{
			MessageBox.Show(ea.Ex.Message, "Unhandled Exception");
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected) return;
			try
			{
				obsWebsocket.Connect(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);
			}
			catch (AuthFailureException)
			{
				Console.WriteLine("Authentication failed.");
			}
			catch (ErrorResponseException ex)
			{
				Console.WriteLine($"Connect failed. {ex.Message}");
			}
		}

		private void Game_PlayerRequestsRoll(object sender, PlayerRollRequestEventArgs ea)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.ExtraOnly);
			diceRoll.DamageHealthExtraDice = ea.DiceRollStr;
			RollTheDice(diceRoll);
		}

		bool JoinedChannel(string channel)
		{
			if (dungeonMasterClient == null)
				return false;
			foreach (JoinedChannel joinedChannel in dungeonMasterClient.JoinedChannels)
			{
				if (string.Compare(joinedChannel.Channel, channel, true) == 0)
					return true;
			}
			return false;
		}

		List<BaseChatBot> commandParsers = new List<BaseChatBot>();
		BaseChatBot GetCommandParser(string userId)
		{
			return commandParsers.Find(x => x.ListensTo(userId));
		}

		public string GetPlayFirstNameFromId(int playerId)
		{
			if (game == null)
				return string.Empty;
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return string.Empty;
			return player.firstName;
		}

		public void SetBoolProperty(int playerId, string propertyName, bool value)
		{
			Dispatcher.Invoke(() =>
			{
				Character player = game.GetPlayerFromId(playerId);
				if (player == null)
					return;
				Character.SetBoolProperty(player, propertyName, value);
				SetShortcutVisibility();
				UpdateAskUI(player);
			});
		}

		public bool GetBoolProperty(int playerId, string propertyName)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return false;
			return Character.GetBoolProperty(player, propertyName);
		}

		public void SetSaveHiddenThreshold(int hiddenThreshold)
		{
			Dispatcher.Invoke(() =>
			{
				tbxSaveThreshold.Text = hiddenThreshold.ToString();
			});

			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{hiddenThreshold} {twitchIndent} <-- hidden SAVE threshold");
		}

		private void HumperBotClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			if (waitingForAnswerToQuestion && int.TryParse(e.ChatMessage.Message.Trim(), out int result))
			{
				AnswerMap answer = answerMap.FirstOrDefault(x => x.Index == result);
				if (answer != null)
				{
					FrmAsk.TryAnswer(answer.Value);
					return;
				}
			}
			BaseChatBot commandParser = GetCommandParser(e.ChatMessage.UserId);
			if (commandParser == null)
				return;
			try
			{
				commandParser.HandleMessage(e.ChatMessage, dungeonMasterClient, ActivePlayer);
			}
			catch (Exception ex)
			{

			}
		}

		private void History_LogUpdated(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				History.UpdateQueuedEntries();
			});
		}

		public int ActivePlayerId
		{
			get
			{
				return Dispatcher.Invoke(() =>
				{
					if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
						return playerTabItem.PlayerId;
					return tabPlayers.SelectedIndex;
				});
			}
			set
			{
				Dispatcher.Invoke(() =>
				{
					if (tabPlayers.SelectedItem is PlayerTabItem selectedPlayerTab)
					{
						if (selectedPlayerTab.PlayerId == value)
							return;
					}

					foreach (object item in tabPlayers.Items)
						if (item is PlayerTabItem playerTabItem && playerTabItem.PlayerId == value)
						{
							tabPlayers.SelectedItem = playerTabItem;
							return;
						}
				});
			}
		}

		public Character ActivePlayer
		{
			get
			{
				return Dispatcher.Invoke(() =>
				{
					if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
						return game.GetPlayerFromId(playerTabItem.PlayerId);
					return null;
				});
			}
		}

		private void FocusHelper_FocusedControlsChanged(object sender, FocusedControlsChangedEventArgs e)
		{
			foreach (StatBox statBox in e.Active)
			{
				HubtasticBaseStation.FocusItem(ActivePlayerId, activePage, statBox.FocusItem);
			}

			foreach (StatBox statBox in e.Deactivated)
			{
				HubtasticBaseStation.UnfocusItem(ActivePlayerId, activePage, statBox.FocusItem);
			}
		}

		Dictionary<int, Rectangle> highlightRectangles;

		object GetToolTip(string description)
		{
			if (string.IsNullOrWhiteSpace(description))
				return null;
			if (description.StartsWith("$"))
			{
				description = Expressions.GetStr(description, ActivePlayer);
			}
			ToolTip toolTip = new ToolTip();
			TextBlock textBlock = new TextBlock();
			textBlock.Text = description;
			textBlock.TextWrapping = TextWrapping.Wrap;
			textBlock.Width = 200;
			toolTip.Content = textBlock;
			return toolTip;
		}
		UIElement BuildShortcutButton(PlayerActionShortcut playerActionShortcut)
		{
			if (highlightRectangles == null)
				highlightRectangles = new Dictionary<int, Rectangle>();
			ShortcutPanel shortcutPanel = new ShortcutPanel();
			shortcutPanel.Margin = new Thickness(2);
			shortcutPanel.Tag = playerActionShortcut.Index;
			shortcutPanel.Shortcut = playerActionShortcut;

			Button button = new Button();
			button.Padding = new Thickness(4, 2, 4, 2);
			shortcutPanel.Children.Add(button);
			button.Content = playerActionShortcut.DisplayText;
			button.ToolTip = GetToolTip(playerActionShortcut.Description);
			button.Tag = playerActionShortcut.Index;
			button.Click += PlayerShortcutButton_Click;
			Rectangle rectangle = new Rectangle();
			shortcutPanel.Children.Add(rectangle);
			rectangle.Tag = playerActionShortcut.Index;
			rectangle.Visibility = Visibility.Hidden;
			rectangle.Height = 3;
			rectangle.Fill = new SolidColorBrush(Colors.Red);

			highlightRectangles.Add(playerActionShortcut.Index, rectangle);
			return shortcutPanel;
		}

		PlayerActionShortcut GetActionShortcut(object tag)
		{
			//PlayerActionShortcut sneak = actionShortcuts.LastOrDefault(x => x.Name.StartsWith("Sneak"));
			if (int.TryParse(tag.ToString(), out int index))
				return actionShortcuts.FirstOrDefault(x => x.Index == index);
			return null;
		}
		void HidePlayerShortcutHighlightsUI()
		{
			if (highlightRectangles == null)
				return;
			foreach (Rectangle rectangle in highlightRectangles.Values)
			{
				rectangle.Visibility = Visibility.Hidden;
			}
		}
		void HighlightPlayerShortcutUI(int index)
		{
			HidePlayerShortcutHighlightsUI();
			if (highlightRectangles == null)
				return;
			if (highlightRectangles.ContainsKey(index))
				highlightRectangles[index].Visibility = Visibility.Visible;
		}

		void SetActivePlayerVantageUI(VantageKind vantageMod)
		{
			SetPlayerVantageUI(vantageMod, ActivePlayerId);
		}

		public void SetPlayerVantageUI(VantageKind vantageMod, int playerId)
		{
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
			{
				if (uIElement is PlayerRollCheckBox checkbox && checkbox.PlayerId == playerId)
				{
					switch (vantageMod)
					{
						case VantageKind.Normal:
							checkbox.RbNormal.IsChecked = true;
							break;
						case VantageKind.Advantage:
							checkbox.RbAdvantage.IsChecked = true;
							break;
						case VantageKind.Disadvantage:
							checkbox.RbDisadvantage.IsChecked = true;
							break;
					}
					return;
				}
			}
		}

		private void Game_SpellDispelled(object sender, CastedSpellEventArgs ea)
		{
			if (ea.CastedSpell.SpellCaster == null)
				return;

			if (!ea.CastedSpell.Active)
				return;

			Dispel(ea.CastedSpell.Spell, ea.CastedSpell.SpellCaster.playerID);

			string spellToEnd = DndGame.GetSpellPlayerName(ea.CastedSpell.Spell, ea.CastedSpell.SpellCaster.playerID);
			EndSpellEffects(spellToEnd);
		}

		private void EndSpellEffects(string spellToEnd)
		{
			if (!spellToEnd.StartsWith(PlayerActionShortcut.SpellWindupPrefix))
				spellToEnd = PlayerActionShortcut.SpellWindupPrefix + spellToEnd;
			HubtasticBaseStation.ClearWindup(spellToEnd);
			int parenPos = spellToEnd.IndexOf('(');

			string casterId = string.Empty;
			string casterEmote = string.Empty;
			if (parenPos > 0)
			{
				string playerIdStr = spellToEnd.Substring(parenPos + 1);
				char[] endChars = { ')' };
				playerIdStr = playerIdStr.Trim(endChars);
				if (int.TryParse(playerIdStr, out int playerId))
				{
					Character player = game.GetPlayerFromId(playerId);
					casterEmote = player.emoticon + " ";
					//BreakConcentration(playerId);
					casterId = $"{GetPlayerName(playerId)}'s ";
				}
				spellToEnd = spellToEnd.Substring(0, parenPos);
			}
			if (spellToEnd.StartsWith(PlayerActionShortcut.SpellWindupPrefix))
				spellToEnd = spellToEnd.Substring(PlayerActionShortcut.SpellWindupPrefix.Length);

			TellDungeonMaster($"{casterEmote}{casterId} {spellToEnd} spell ends at {game.Clock.AsFullDndDateTimeString()}.");
		}

		Dictionary<int, Spell> concentratedSpells = new Dictionary<int, Spell>();

		void Dispel(Spell spell, int playerId)
		{
			HubtasticBaseStation.ClearWindup(PlayerActionShortcut.SpellWindupPrefix + DndGame.GetSpellPlayerName(spell, playerId));
		}

		public void BreakConcentration(int playerId)
		{
			game.BreakConcentration(playerId);
		}


		void PrepareToCastSpell(Spell spell, int playerId)
		{
			spell.OwnerId = playerId;
			TellAll($"{GetPlayerName(playerId)} casts {spell.Name} at {game.Clock.AsFullDndDateTimeString()}.");
		}

		private void PlayerShortcutButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				PlayerActionShortcut actionShortcut = GetActionShortcut(button.Tag);
				if (actionShortcut == null)
					return;
				ActivateShortcut(actionShortcut);
			}
		}

		string activeTrailingEffects;
		string activeDieRollEffects;
		Spell activeIsSpell;
		CastedSpell castedSpellNeedingCompletion = null;
		Character spellCaster = null;

		private void ActivateShortcut(string shortcutName)
		{
			PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText == shortcutName && x.PlayerId == ActivePlayerId);
			if (shortcut == null && tbTabs.SelectedItem == tbDebug)
				shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText.StartsWith(shortcutName) && x.PlayerId == ActivePlayerId);
			if (shortcut != null)
				Dispatcher.Invoke(() =>
				{
					ActivateShortcut(shortcut);
				});
		}

		void HideShortcutUI(Panel panel, PlayerActionShortcut actionShortcut)
		{
			foreach (UIElement uIElement in panel.Children)
				if (uIElement is ShortcutPanel shortcutPanel && shortcutPanel.Shortcut == actionShortcut)
				{
					shortcutPanel.Visibility = Visibility.Collapsed;
					return;
				}
		}

		void HideShortcutUI(PlayerActionShortcut actionShortcut)
		{
			HideShortcutUI(wpActionsActivePlayer, actionShortcut);
			HideShortcutUI(spBonusActionsActivePlayer, actionShortcut);
			HideShortcutUI(spReactionsActivePlayer, actionShortcut);
			HideShortcutUI(spSpecialActivePlayer, actionShortcut);
		}

		void SetRollTypeUI(PlayerActionShortcut actionShortcut)
		{
			EnableRollDiceButton(actionShortcut.Type);
		}

		void SetSelectedItemFromText(ComboBox comboBox, string str)
		{

			foreach (object item in comboBox.Items)
			{
				if (item is ComboBoxItem comboBoxItem)
				{
					if (comboBoxItem.Content.ToString().ToLower() == str.ToLower())
					{
						comboBox.SelectedItem = comboBoxItem;
						return;
					}
				}
			}
			comboBox.SelectedItem = null;
		}
		void SetRollComboBoxUI(DiceRoll diceRoll)
		{
			SetSelectedItemFromText(cbDamage, diceRoll.DamageType.ToString());
			SetSelectedItemFromText(cbAbility, diceRoll.SavingThrow.ToString());
			SetSelectedItemFromText(cbSkillFilter, DndUtils.ToSkillDisplayString(diceRoll.SkillCheck));
		}

		private void EnableRollDiceButton(DiceRollType type)
		{
			btnRollDice.IsEnabled = type != DiceRollType.None;
		}


#if profiling
		long lastTimeCheck;
		long longestDifference;

		void StartProfiling()
		{
			lastTimeCheck = DateTime.Now.Ticks;
			longestDifference = 0;
		}

		void CheckTime()
		{
			long currentTime = DateTime.Now.Ticks;
			long difference = currentTime - lastTimeCheck;
			if (difference > longestDifference)
			{
				longestDifference = difference;
				if (longestDifference > 10_000_000)
				{
					
				}
			}
		}
#endif
		DiceRoll currentRoll;

		void SetRollScopeUI(DiceRoll diceRoll)
		{
			// Mark says this method will never need to change. CodeBaseAlpha says "Let's see."
			// ![](9920C0D4E763C7314FD8A6EAF74D5FB3.png)
			if (diceRoll.RollScope == RollScope.ActivePlayer)
				rbActivePlayer.IsChecked = true;
			else
				rbIndividuals.IsChecked = true;
		}

		void SetControlUIFromRoll(DiceRoll diceRoll)
		{
			settingInternally = true;
			try
			{
				// ![](CC2A109973BFCC3ADA17563004698A52.png;;968,241,1780,655;0.04000,0.04000)

				ckbUseMagic.IsChecked = diceRoll.IsMagic;
				tbxGroupInspiration.Text = diceRoll.GroupInspiration;
				tbxModifier.Text = diceRoll.Modifier.ToString();
				SetRollTypeUI(diceRoll.Type);
				tbxDamageDice.Text = diceRoll.DamageHealthExtraDice;
				tbxMinDamage.Text = diceRoll.MinDamage.ToString();
				tbxAddDiceOnHit.Text = diceRoll.AdditionalDiceOnHit;
				tbxMessageAddDiceOnHit.Text = diceRoll.AdditionalDiceOnHitMessage;
				SetRollComboBoxUI(diceRoll);
				SetRollScopeUI(diceRoll);
			}
			finally
			{
				settingInternally = false;
			}

		}

		private void SetRollTypeUI(DiceRollType type)
		{
			switch (type)
			{
				case DiceRollType.SkillCheck:
					rbSkillCheck.IsChecked = true;
					break;
				case DiceRollType.Attack:
					rbAttack.IsChecked = true;
					break;
				case DiceRollType.SavingThrow:
					rbSavingThrow.IsChecked = true;
					break;
				case DiceRollType.FlatD20:
					rbFlatD20.IsChecked = true;
					break;
				case DiceRollType.DeathSavingThrow:
					rbDeathSavingThrow.IsChecked = true;
					break;
				case DiceRollType.PercentageRoll:
					rbPercentageRoll.IsChecked = true;
					break;
				case DiceRollType.WildMagic:
					rbWildMagic.IsChecked = true;
					break;
				case DiceRollType.BendLuckAdd:
					rbBendLuckAdd.IsChecked = true;
					break;
				case DiceRollType.BendLuckSubtract:
					rbBendLuckSubtract.IsChecked = true;
					break;
				case DiceRollType.LuckRollLow:
					rbLuckRollLow.IsChecked = true;
					break;
				case DiceRollType.LuckRollHigh:
					rbLuckRollHigh.IsChecked = true;
					break;
				case DiceRollType.DamageOnly:
					rbDamageOnly.IsChecked = true;
					break;
				case DiceRollType.HealthOnly:
					rbHealth.IsChecked = true;
					break;
				case DiceRollType.ExtraOnly:
					rbExtra.IsChecked = true;
					break;
				case DiceRollType.ChaosBolt:
					rbAttack.IsChecked = true;
					break;
				case DiceRollType.Initiative:
					rbInitiative.IsChecked = true;
					break;
				case DiceRollType.WildMagicD20Check:
					rbWildMagicD20Check.IsChecked = true;
					break;
				case DiceRollType.InspirationOnly:
					rbInspirationOnly.IsChecked = true;
					break;
				case DiceRollType.AddOnDice:
					rbExtra.IsChecked = true;
					break;
				case DiceRollType.NonCombatInitiative:
					rbNonCombatInitiative.IsChecked = true;
					break;
				case DiceRollType.HPCapacity:
					rbHitPointCapacity.IsChecked = true;
					break;
				case DiceRollType.CastSimpleSpell:
					rbCastSimpleSpell.IsChecked = true;
					break;
			}
		}

		void TellDmWeAreReady(Character player, PlayerActionShortcut actionShortcut)
		{
			if (actionShortcut.Spell != null)
				if (actionShortcut.Spell.MorePowerfulWhenCastAtHigherLevels)
					TellAll($"{player.firstName} is ready to cast {actionShortcut.Spell.Name} in spell slot {actionShortcut.Spell.SpellSlotLevel}...");
				else
					TellAll($"{player.firstName} is ready to cast {actionShortcut.Spell.Name}...");
			else if (actionShortcut.CarriedWeapon != null)
			{
				string weaponName = actionShortcut.CarriedWeapon.Name;
				if (string.IsNullOrEmpty(weaponName))
					weaponName = actionShortcut.CarriedWeapon.Weapon.Name;
				TellAll($"{player.firstName} is ready to attack with {player.hisHer} {weaponName}...");
			}
			else if (actionShortcut.Type == DiceRollType.WildMagicD20Check)
			{
				TellAll($"{player.firstName} is checking wild magic...");
			}
			else if (actionShortcut.Type == DiceRollType.WildMagic)
			{
				TellAll($"{player.firstName}'s wild magic roll...");
			}
		}

		void SetPlayerPropertiesBasedOnShortcut(PlayerActionShortcut actionShortcut, Character player)
		{
			player.TwoHanded = actionShortcut.HandsOnWeapon == HandsOnWeapon.Two;
			UpdateAskUI(player);
		}
		private void ActivateShortcut(PlayerActionShortcut actionShortcut)
		{
			preparedSpell = null;
			spellToCastOnRoll = null;
			Character player = GetPlayer(actionShortcut.PlayerId);
			try
			{
				SetPlayerPropertiesBasedOnShortcut(actionShortcut, player);

				ActionType activationResult = ActivateShortcutForPlayer(actionShortcut, player);

				if (activationResult == ActionType.Abort)
					return;

				if (activationResult == ActionType.CreatesNew)
					NewShortcutActivated(actionShortcut, player);
			}
			finally
			{
				SetPlayerShortcutUI(actionShortcut, player);
				SetShortcutVisibility();
				UpdateAskUI(player);
			}
		}

		private void NewShortcutActivated(PlayerActionShortcut actionShortcut, Character player)
		{
			settingInternally = true;
			try
			{
				currentRoll = DiceRoll.GetFrom(actionShortcut, player);
				TellDmWeAreReady(player, actionShortcut);
				NextDieRollType = actionShortcut.Type;
			}
			finally
			{
				settingInternally = false;
				HighlightPlayerShortcutUI(actionShortcut.Index);
				SetControlUIFromRoll(currentRoll);
				UpdateStateUIForPlayer(player);
			}
		}

		//private void ActivateShortcut_old(PlayerActionShortcut actionShortcut)
		//{
		//	spellToCastOnRoll = null;
		//	Character player = GetPlayer(actionShortcut.PlayerId);
		//	try
		//	{
		//		if (ActivateShortcutForPlayer(actionShortcut, player) == ActionType.ModifiesExisting)
		//			return;

		//		settingInternally = true;

		//		try
		//		{
		//			HighlightPlayerShortcutUI(actionShortcut.Index);
		//			SetControlUiFromShortcut(actionShortcut);
		//			NextDieRollType = actionShortcut.Type;
		//			SetActivePlayerVantageUI(actionShortcut.VantageMod);
		//			RollInstantDiceIfNecessary(actionShortcut);
		//		}
		//		finally
		//		{
		//			settingInternally = false;
		//			UpdateStateUIForPlayer(player);
		//		}
		//	}
		//	finally
		//	{
		//		SetShortcutVisibility();
		//		UpdateAskUI(player);
		//	}
		//}

		private void RollInstantDiceIfNecessary(PlayerActionShortcut actionShortcut)
		{
			if (!actionShortcut.HasInstantDice())
				return;
			DiceRollType type = actionShortcut.Type;
			if (type == DiceRollType.None)
				type = DiceRollType.DamageOnly;
			DiceRoll diceRoll = PrepareRoll(type);
			diceRoll.SecondRollTitle = actionShortcut.AdditionalRollTitle;
			diceRoll.DamageHealthExtraDice = actionShortcut.InstantDice;
			RollTheDice(diceRoll);
		}

		public enum ActionType
		{
			Abort,
			ModifiesExisting,
			CreatesNew
		}

		private void SetPlayerShortcutUI(PlayerActionShortcut actionShortcut, Character player)
		{
			if (actionShortcut.ModifiesExistingRoll)
				ModifyExistingRollUI(actionShortcut, player);
			else
			{
				SetRollTypeUI(actionShortcut);
				SetModifierUI(actionShortcut);
			}
		}

		private ActionType ActivateShortcutForPlayer(PlayerActionShortcut actionShortcut, Character player)
		{
			ResetWildMagicChecks(actionShortcut);
			ResetActiveFields();
			AssignDieRollEffects(actionShortcut);

			if (actionShortcut.ModifiesExistingRoll)
			{
				actionShortcut.ExecuteCommands(player);
				return ActionType.ModifiesExisting;
			}

			ClearWeaponAndSpellEffects(player);

			ActionResult actionResult = PlayerTakingAction(actionShortcut, player);
			if (actionResult == ActionResult.MustAbort)
				return ActionType.Abort;

			if (actionShortcut.Spell != null)
				ShowPreparedSpellInGame(actionShortcut, player);
			else
				ShowPreparedPhysicalActionInGame(actionShortcut, player);

			if (actionShortcut.Type != DiceRollType.Attack)
				actionShortcut.ExecuteCommands(player);

			return ActionType.CreatesNew;
		}

		private void ResetWildMagicChecks(PlayerActionShortcut actionShortcut)
		{
			if (actionShortcut.Type != DiceRollType.WildMagic)
				return;
			Character activePlayer = GetPlayer(actionShortcut.PlayerId);
			if (activePlayer != null)
				activePlayer.NumWildMagicChecks = 0;
		}

		private void SetModifierUI(PlayerActionShortcut actionShortcut)
		{
			settingInternally = true;
			try
			{
				if (actionShortcut.ToHitModifier > 0)
					tbxModifier.Text = "+" + actionShortcut.ToHitModifier.ToString();
				else
					tbxModifier.Text = actionShortcut.ToHitModifier.ToString();
			}
			finally
			{
				settingInternally = false;
			}
		}

		private void ShowPreparedPhysicalActionInGame(PlayerActionShortcut actionShortcut, Character player)
		{
			if (!IsWildMagicRoll(actionShortcut))
				SwitchToMainPageInGame();

			SendShortcutWindups(actionShortcut, player);
		}

		private static bool IsWildMagicRoll(PlayerActionShortcut actionShortcut)
		{
			return actionShortcut.DisplayText.IndexOf("Wild Magic") >= 0;
		}

		private void SwitchToMainPageInGame()
		{
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, ScrollPage.main, string.Empty);
		}

		private void ShowPreparedSpellInGame(PlayerActionShortcut actionShortcut, Character player)
		{
			Spell spell = actionShortcut.Spell;
			if (spell == null)
				return;
			player.PreparingSpell();
			UpdateSpellPageUI(spell);
			spellToCastOnRoll = actionShortcut;
			SwitchToSpellPageInGame();

			ShowSpellPreparingWindups(actionShortcut, spell);
			preparedSpell = new CastedSpell(spell, player);
			preparedSpell.Prepare();
		}

		CastedSpell preparedSpell;

		private void SwitchToSpellPageInGame()
		{
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, ScrollPage.spells, string.Empty);
		}

		private void UpdateSpellPageUI(Spell spell)
		{
			if (tbTabs.SelectedItem == tbDebug)
				UpdateSpellEvents(spell);
		}

		public enum ActionResult
		{
			GoodToGo,
			MustAbort
		}

		private ActionResult PlayerTakingAction(PlayerActionShortcut actionShortcut, Character player)
		{
			player.ReadiedWeapon = null;
			if (actionShortcut.Part != TurnPart.Reaction)
				game.CreatureTakingAction(player);

			// TODO: Fix the targeting.
			if (DndUtils.IsAttack(actionShortcut.Type))
			{
				if (actionShortcut.CarriedWeapon != null && actionShortcut.CarriedWeapon.Weapon.RequiresAmmunition())
				{
					string ammunitionKind = actionShortcut.CarriedWeapon.Weapon.AmmunitionKind;
					if (!player.HasAmmunition(ammunitionKind))
					{
						TellAll($"{player.firstName} is out of {ammunitionKind}s.");
						return ActionResult.MustAbort;
					}
				}
				player.ReadiedWeapon = actionShortcut.CarriedWeapon;

				player.PrepareAttack(null, actionShortcut);

				actionShortcut.UpdatePlayerAttackingAbility(player, actionShortcut.Spell != null);

				//if (actionShortcut.Spell == null && actionShortcut.WeaponProperties != WeaponProperties.None)
				//	game.CreatureRaisingWeapon(player, actionShortcut);

			}

			return ActionResult.GoodToGo;
		}

		private static void ClearWeaponAndSpellEffects(Character player)
		{
			ClearExistingWindupsInGame();
			player.ClearAdditionalSpellEffects();
		}

		private static void ClearExistingWindupsInGame()
		{
			HubtasticBaseStation.ClearWindup("Weapon.*");
			HubtasticBaseStation.ClearWindup("Windup.*");
		}

		private void ModifyExistingRollUI(PlayerActionShortcut actionShortcut, Character player)
		{
			switch (actionShortcut.VantageMod)
			{
				case VantageKind.Advantage:
				case VantageKind.Disadvantage:
					SetActivePlayerVantageUI(actionShortcut.VantageMod);
					break;
			}
			if (!string.IsNullOrWhiteSpace(actionShortcut.AddDice))
				tbxDamageDice.Text += "," + actionShortcut.AddDice;
		}

		private void AssignDieRollEffects(PlayerActionShortcut actionShortcut)
		{
			if (!string.IsNullOrWhiteSpace(actionShortcut.TrailingEffects))
				activeTrailingEffects = actionShortcut.TrailingEffects;
			if (!string.IsNullOrWhiteSpace(actionShortcut.DieRollEffects))
				activeDieRollEffects = actionShortcut.DieRollEffects;
		}

		private void ResetActiveFields()
		{
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			activeIsSpell = null;
		}

		private void SetControlUiFromShortcut(PlayerActionShortcut actionShortcut)
		{
			if (!string.IsNullOrWhiteSpace(actionShortcut.AddDice))
				tbxDamageDice.Text += "," + actionShortcut.AddDice;
			else if (actionShortcut.Spell == null)
				tbxDamageDice.Text = actionShortcut.Dice;

			if (actionShortcut.MinDamage != keepExistingModifier)
				tbxMinDamage.Text = actionShortcut.MinDamage.ToString();

			tbxAddDiceOnHit.Text = actionShortcut.AddDiceOnHit;
			tbxMessageAddDiceOnHit.Text = actionShortcut.AddDiceOnHitMessage;

			ckbUseMagic.IsChecked = actionShortcut.UsesMagic;
		}

		private static void SendShortcutWindups(PlayerActionShortcut actionShortcut, Character player)
		{
			if (actionShortcut.Windups.Count > 0)
			{
				List<WindupDto> windups = actionShortcut.GetAvailableWindups(player);
				foreach (WindupDto windupDto in windups)
				{
					if (windupDto.EffectAvailableWhen == "usesMagicAmmunitionThisRoll == true")
					{
						if (player.ReadiedAmmunition != null)
						{
							if (!string.IsNullOrEmpty(player.ReadiedAmmunition.HueShift))
							{
								windupDto.HueStr = player.ReadiedAmmunition.HueShift;
								if (int.TryParse(player.ReadiedAmmunition.HueShift, out int result))
									windupDto.Hue = result;
							}
						}
					}
				}

				string serializedObject = JsonConvert.SerializeObject(windups);
				HubtasticBaseStation.AddWindup(serializedObject);
			}
		}

		void CastingSpellSoon(DndTimeSpan castingTime, PlayerActionShortcut actionShortcut)
		{
			string playerName = game.GetPlayerFromId(actionShortcut.PlayerId).firstName;
			string spellName = actionShortcut.Spell.Name;
			string timeSpanStr = actionShortcut.Spell.CastingTimeStr;
			TellAll($"{playerName} is preparing to cast {spellName} (takes {timeSpanStr}).");
			string alarmName = DndGame.GetSpellAlarmName(actionShortcut.Spell, actionShortcut.PlayerId);
			game.CreateAlarm(alarmName, castingTime, DndAlarm_CastSpellNow, actionShortcut, null);
		}

		private void DndAlarm_CastSpellNow(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Data is PlayerActionShortcut actionShortcut)
			{
				string playerName = game.GetPlayerFromId(actionShortcut.PlayerId).firstName;
				string spellName = actionShortcut.Spell.Name;
				TellAll($"{playerName} is now casting {spellName}!");
				CastSpellNow(actionShortcut);
			}
		}

		private void ActivateSpellShortcut(PlayerActionShortcut actionShortcut)
		{
			if (actionShortcut.Spell.CastingTime > DndTimeSpan.OneAction)
			{
				CastingSpellSoon(actionShortcut.Spell.CastingTime, actionShortcut);
				return;
			}
			Spell spell = CastSpellNow(actionShortcut);

			tbxDamageDice.Text = spell.DieStr;
		}

		private Spell CastSpellNow(PlayerActionShortcut actionShortcut)
		{
			Character player = GetPlayer(actionShortcut.PlayerId);
			Spell spell = actionShortcut.Spell;
			activeIsSpell = spell;
			KnownSpell matchingSpell = player.GetMatchingSpell(spell.Name);
			if (matchingSpell != null && matchingSpell.CanBeRecharged())
			{
				PrepareToCastSpell(spell, actionShortcut.PlayerId);
				UseRechargeableItem(actionShortcut, matchingSpell);
				CastedSpell castedSpell = new CastedSpell(spell, player);
				castedSpell.CastingWithItem();
				if (spellCaster == null)
					spellCaster = player;
				game.CompleteCast(spellCaster, castedSpell);
			}
			else
			{
				CastActionSpell(actionShortcut, player, spell);
			}
			spellCaster = player;
			return spell;
		}

		private void UseRechargeableItem(PlayerActionShortcut actionShortcut, KnownSpell matchingSpell)
		{
			matchingSpell.ChargesRemaining--;
			if (matchingSpell.ChargesRemaining == 0)
				HideShortcutUI(actionShortcut);

		}

		private void CastActionSpell(PlayerActionShortcut actionShortcut, Character player, Spell spell)
		{
			if (spell.RequiresConcentration && player.concentratedSpell != null)
			{
				Spell concentratedSpell = player.concentratedSpell.Spell;

				try
				{
					if (game.ClockIsRunning())
						realTimeAdvanceTimer.Stop();
					if (concentratedSpell.Name == spell.Name)
					{
						// TODO: Provide feedback that we are already casting this spell and it has game.GetSpellTimeLeft(player.playerID, concentratedSpell).
						TimeSpan remainingSpellTime = game.GetRemainingSpellTime(player.playerID, concentratedSpell);
						if (remainingSpellTime.TotalSeconds > 0)
						{
							TellDungeonMaster($"{player.firstName} is already casting {concentratedSpell.Name} ({game.GetRemainingSpellTimeStr(player.playerID, concentratedSpell)} remaining)");
							return;
						}
						else  // No time remaining. No longer concentrating on this spell.
							concentratedSpell = null;
					}
					if (concentratedSpell != null && AskQuestion($"Break concentration with {concentratedSpell.Name} ({game.GetRemainingSpellTimeStr(player.playerID, concentratedSpell)} remaining) to cast {spell.Name}?", new List<string>() { "1:Yes", "0:No" }) == 0)
						return;
				}
				finally
				{
					if (game.ClockIsRunning())
						StartRealTimeTimer();
				}
			}
			PrepareToCastSpell(spell, actionShortcut.PlayerId);

			CastedSpell castedSpell = game.Cast(player, spell);
			SetClearSpellVisibility(player);

			if (spell.MustRollDiceToCast())
			{
				castedSpellNeedingCompletion = castedSpell;
				actionShortcut.AttackingAbilityModifier = player.GetSpellcastingAbilityModifier();
				actionShortcut.ProficiencyBonus = (int)Math.Round(player.proficiencyBonus);
			}
			else
			{
				ShowSpellHitOrMissInGame(actionShortcut.PlayerId, SpellHitType.Hit, spell.Name);
				player.JustCastSpell(spell.Name);
			}
			ShowSpellEffects(actionShortcut, spell, "Spell");
			ShowSpellCastEffectsInGame(actionShortcut.PlayerId, actionShortcut.Spell.Name);
		}

		private static void ShowSpellPreparingWindups(PlayerActionShortcut actionShortcut, Spell spell)
		{
			ShowSpellEffects(actionShortcut, spell, "Windup");
		}

		private static void ShowSpellEffects(PlayerActionShortcut actionShortcut, Spell spell, string prefix)
		{
			CastedSpellDto spellToCastDto = new CastedSpellDto(spell, new Target() { CasterId = actionShortcut.PlayerId });

			spellToCastDto.Windups = actionShortcut.WindupsReversed.Where(x => x != null && x.Name != null && x.Name.StartsWith(prefix)).ToList();
			if (!spellToCastDto.Windups.Any())
				return;
			string serializedObject = JsonConvert.SerializeObject(spellToCastDto);
			HubtasticBaseStation.CastSpell(serializedObject);
		}

		private void SetClearSpellVisibility(Character player)
		{
			if (player.spellActivelyCasting == null && player.spellPreviouslyCasting != null)
				btnClearSpell.Visibility = Visibility.Visible;
			else
				btnClearSpell.Visibility = Visibility.Hidden;
		}

		void SetActionShortcuts(int playerID)
		{
			AddShortcutButtons(wpActionsActivePlayer, playerID, TurnPart.Action);
			AddShortcutButtons(spBonusActionsActivePlayer, playerID, TurnPart.BonusAction);
			AddShortcutButtons(spReactionsActivePlayer, playerID, TurnPart.Reaction);
			AddShortcutButtons(spSpecialActivePlayer, playerID, TurnPart.Special);
			SetShortcutVisibility();
		}

		private void AddShortcutButtons(Panel panel, int playerID, TurnPart part)
		{
			ClearExistingButtons(panel);

			List<PlayerActionShortcut> playerActions = actionShortcuts.Where(x => x.PlayerId == playerID).Where(x => x.Part == part).ToList();

			if (playerActions.Count == 0)
				panel.Visibility = Visibility.Collapsed;
			else
				panel.Visibility = Visibility.Visible;

			foreach (PlayerActionShortcut playerActionShortcut in playerActions)
			{
				panel.Children.Add(BuildShortcutButton(playerActionShortcut));
			}
		}

		private void ClearExistingButtons(Panel panel)
		{
			for (int i = panel.Children.Count - 1; i >= 0; i--)
			{
				UIElement uIElement = panel.Children[i];
				if (uIElement is StackPanel)
					panel.Children.RemoveAt(i);
			}
		}

		private void TabControl_PlayerChanged(object sender, SelectionChangedEventArgs e)
		{
			btnRollDice.IsEnabled = true;
			ActivatePendingShortcuts();
			if (buildingTabs)
				return;
			if (rbActivePlayer.IsChecked == true)
			{
				CheckOnlyOnePlayer(ActivePlayerId);
			}
			CharacterSheets sheetForCharacter = GetSheetForCharacter(ActivePlayerId);
			if (sheetForCharacter != null)
				sheetForCharacter.Page = ScrollPage.main;
			InitializeActivePlayerData();
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			ClearExistingWindupsInGame();
			SetActionShortcuts(ActivePlayerId);
			Character player = game.GetPlayerFromId(ActivePlayerId);

			if (player != null)
				player.forceShowSpell = false;
			UpdateAllUIForPlayer(player);
			TellDmActivePlayer(ActivePlayerId);
		}
		void UpdateAllUIForPlayer(Character player)
		{
			UpdateStateUIForPlayer(player);
			UpdateEventsUIForPlayer(player);
			RebuildAskUI(player);
		}

		void UpdateEventsUIForPlayer(Character player)
		{
			if (player == null)
				return;

			//lstAssignedFeatures.ItemsSource = player.GetEventGroup(typeof(Feature));
			lstKnownSpellsAndFeatures.ItemsSource = player.GetAllEventGroups();
			lstEvents.ItemsSource = null;
		}

		private void InitializeActivePlayerData()
		{
			castedSpellNeedingCompletion = null;
			spellCaster = null;
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			highlightRectangles = null;
			NextDieRollType = DiceRollType.None;
			activePage = ScrollPage.main;
			FocusHelper.ClearActiveStatBoxes();
		}

		void CheckOnlyOnePlayer(int playerID)
		{
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
			{
				if (uIElement is PlayerRollCheckBox checkbox)
					checkbox.IsChecked = checkbox.PlayerId == playerID;
			}
		}

		private void CharacterSheets_PageChanged(object sender, RoutedEventArgs ea)
		{
			if (sender is CharacterSheets characterSheets && activePage != characterSheets.Page)
			{
				activePage = characterSheets.Page;
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			}
		}

		void SetActivePlayerFromCharacter(Character character)
		{
			if (character.playerID != ActivePlayerId)
				return;
			Character player = game.GetPlayerFromId(ActivePlayerId);
			player.CopyUIChangeableAttributesFrom(character);
		}
		private void HandleCharacterSheetDataChanged(object sender, RoutedEventArgs e)
		{
			if (sender is CharacterSheets characterSheets)
			{
				Character character = characterSheets.GetCharacter();
				SetActivePlayerFromCharacter(character);
			}
		}

		private void BtnTestGroupEffect_Click(object sender, RoutedEventArgs e)
		{
			EffectGroup effectGroup = new EffectGroup();
			foreach (TimeLineEffect timeLineEffect in groupEffectBuilder.Entries)
			{
				Effect effect = null;

				if (timeLineEffect.Effect != null)
					effect = timeLineEffect.Effect.GetPrimaryEffect();

				if (effect != null)
				{
					effect.timeOffsetMs = (int)Math.Round(timeLineEffect.Start.TotalMilliseconds);
					effectGroup.Add(effect);
				}
			}

			string serializedObject = JsonConvert.SerializeObject(effectGroup);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.RemovedItems != null && e.RemovedItems.Count > 0)
				if (e.RemovedItems[0] is TabItem tabItem)
					if (tabItem == tbEffects)
						effectsList.Save();
					else if (tabItem == tbItems)
						lstItems.Save();
		}

		public class MyOtherClass
		{

			public double X { get; set; }
			public double Y { get; set; }
			public double Z { get; set; }

		}

		bool rollInspirationAfterwards;

		public void RollTheDice(DiceRoll diceRoll)
		{
			diceRoll.GroupInspiration = tbxGroupInspiration.Text;
			forcedWildMagicThisRoll = false;
			if (!string.IsNullOrWhiteSpace(diceRoll.SpellName))
			{
				Spell spell = AllSpells.Get(diceRoll.SpellName);
				if (spell != null && spell.MustRollDiceToCast())
				{
					DiceRollType diceRollType = PlayerActionShortcut.GetDiceRollType(PlayerActionShortcut.GetDiceRollTypeStr(spell));
					if (!DndUtils.IsAttack(diceRollType) && diceRoll.PlayerRollOptions.Count == 1)
						ShowSpellHitOrMissInGame(diceRoll.PlayerRollOptions[0].PlayerID, SpellHitType.Hit, spell.Name);
				}
			}

			secondToLastRoll = lastRoll;
			lastRoll = diceRoll;
			Character player = null;
			CompleteCast(diceRoll);

			if (diceRoll.PlayerRollOptions.Count == 1)
			{
				PlayerRollOptions playerRollOptions = diceRoll.PlayerRollOptions[0];
				player = game.GetPlayerFromId(playerRollOptions.PlayerID);

				if (DndUtils.IsAttack(diceRoll.Type))
				{
					// TODO: Pass the targetCreature into AttackingNow when mapping is complete and targets are known.
					player.AttackingNow(null);
				}

				if (player != null)
				{
					if (!string.IsNullOrWhiteSpace(player.additionalDiceThisRoll))
					{
						diceRoll.DamageHealthExtraDice += "," + player.additionalDiceThisRoll;
					}
					if (!string.IsNullOrWhiteSpace(player.dieRollEffectsThisRoll))
					{
						diceRoll.AddDieRollEffects(player.dieRollEffectsThisRoll);
					}
					if (!string.IsNullOrWhiteSpace(player.trailingEffectsThisRoll))
					{
						diceRoll.AddTrailingEffects(player.trailingEffectsThisRoll);
					}
				}
			}

			if (diceRoll.IsOnePlayer && player != null)
			{
				player.ReadyRollDice(diceRoll.Type, diceRoll.DamageHealthExtraDice, (int)Math.Round(diceRoll.HiddenThreshold));
			}


			Dispatcher.Invoke(() =>
			{
				showClearButtonTimer.Start();
				//rbTestNormalDieRoll.IsChecked = true;
				updateClearButtonTimer.Stop();
				EnableDiceRollButtons(false);
				btnClearDice.Visibility = Visibility.Hidden;
				PrepareForClear();
			});

			SeriouslyRollTheDice(diceRoll);

			if (diceRoll.IsOnePlayer)
				player?.ResetPlayerRollBasedState();
			else
			{
				foreach (PlayerRollOptions playerRollOption in diceRoll.PlayerRollOptions)
				{
					player = game.GetPlayerFromId(playerRollOption.PlayerID);
					player?.ResetPlayerRollBasedState();
				}
			}
		}

		DateTime lastDieRollTime;
		private void SeriouslyRollTheDice(DiceRoll diceRoll)
		{
			lastDieRollTime = DateTime.Now;
			if (dynamicThrottling)
			{
				ChangeFrameRateAndUI(Overlays.Back, 30);
				ChangeFrameRateAndUI(Overlays.Front, 30);
				ChangeFrameRateAndUI(Overlays.Dice, 30);
			}
			string serializedObject = JsonConvert.SerializeObject(diceRoll);
			HubtasticBaseStation.RollDice(serializedObject);
		}

		private void CompleteCast(DiceRoll diceRoll)
		{
			if (castedSpellNeedingCompletion != null)
			{
				game.CompleteCast(spellCaster, castedSpellNeedingCompletion);
				diceRoll.DamageHealthExtraDice = castedSpellNeedingCompletion.DieStr;
				spellCaster = null;
				castedSpellNeedingCompletion = null;
			}
		}

		void PrepareForClear()
		{
			//showClearButtonTimer.Start();
		}

		void ActivatePendingShortcuts()
		{
			if (shortcutToActivateAfterClearingDice != null)
			{
				PlayerActionShortcut shortcutToActivate = shortcutToActivateAfterClearingDice;
				shortcutToActivateAfterClearingDice = null;
				ActivateShortcut(shortcutToActivate);
			}
		}

		bool waitingToClearDice;

		void ActivatePendingShortcutsIn(int seconds)
		{
			pendingShortcutsTimer.Interval = TimeSpan.FromSeconds(seconds);
			pendingShortcutsTimer.Start();
		}
		public void ClearTheDice()
		{
			updateClearButtonTimer.Stop();
			btnClearDice.Visibility = Visibility.Hidden;
			btnRollDice.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
			HubtasticBaseStation.ClearDice();
			waitingToClearDice = false;
			ActivatePendingShortcutsIn(1);
		}

		private void BtnSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.SavingThrow));
		}

		bool CanIncludeVantageDice(DiceRollType type)
		{
			return (type == DiceRollType.Attack || type == DiceRollType.ChaosBolt || type == DiceRollType.DeathSavingThrow || type == DiceRollType.FlatD20 || type == DiceRollType.SavingThrow || type == DiceRollType.SkillCheck);
		}

		void BeforePlayerRolls(int playerId, DiceRoll diceRoll, ref VantageKind vantageKind)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			player.BeforePlayerRollsDice(diceRoll, ref vantageKind);
		}

		private DiceRoll PrepareRoll(DiceRollType type)
		{

			VantageKind diceRollKind = VantageKind.Normal;

			if (CanIncludeVantageDice(type))
			{
				//if (rbTestAdvantageDieRoll.IsChecked == true)
				//	diceRollKind = VantageKind.Advantage;
				//else if (rbTestDisadvantageDieRoll.IsChecked == true)
				//	diceRollKind = VantageKind.Disadvantage;
			}

			string damageDice = string.Empty;
			if (DndUtils.IsAttack(type) || type == DiceRollType.DamageOnly || type == DiceRollType.HealthOnly || type == DiceRollType.ExtraOnly)
				damageDice = tbxDamageDice.Text;

			DiceRoll diceRoll = new DiceRoll(type, diceRollKind, damageDice);
			diceRoll.AdditionalDiceOnHit = tbxAddDiceOnHit.Text;
			diceRoll.AdditionalDiceOnHitMessage = tbxMessageAddDiceOnHit.Text;
			if (activeIsSpell != null)
				diceRoll.SpellName = activeIsSpell.Name;

			if (int.TryParse(tbxMinDamage.Text, out int result))
				diceRoll.MinDamage = result;
			else
				diceRoll.MinDamage = 0;

			if (type == DiceRollType.SkillCheck)
			{
				ComboBoxItem selectedItem = (ComboBoxItem)cbSkillFilter.SelectedItem;
				if (selectedItem != null && selectedItem.Content != null)
				{
					string skillStr = selectedItem.Content.ToString();
					diceRoll.SkillCheck = DndUtils.ToSkill(skillStr);
				}
			}
			if (type == DiceRollType.SavingThrow)
			{
				ComboBoxItem selectedItem = (ComboBoxItem)cbAbility.SelectedItem;
				if (selectedItem != null && selectedItem.Content != null)
				{
					string abilityStr = selectedItem.Content.ToString();
					diceRoll.SavingThrow = DndUtils.ToAbility(abilityStr);
				}
			}

			diceRoll.DamageType = DamageType.None;

			if (type == DiceRollType.SkillCheck || type == DiceRollType.FlatD20 || type == DiceRollType.SavingThrow)
			{
				//if (rbActivePlayer.IsChecked == true)
				//	diceRoll.RollScope = RollScope.ActivePlayer;
				//else
				diceRoll.RollScope = RollScope.Individuals;
			}
			else
				diceRoll.RollScope = RollScope.ActivePlayer;

			diceRoll.Modifier = 0;
			if (type == DiceRollType.Attack || type == DiceRollType.ChaosBolt)
			{
				if (double.TryParse(tbxModifier.Text, out double modifierResult))
					diceRoll.Modifier = modifierResult;
			}

			if (type == DiceRollType.Attack || type == DiceRollType.DamageOnly)
			{
				ComboBoxItem selectedItem = (ComboBoxItem)cbDamage.SelectedItem;
				if (selectedItem != null && selectedItem.Content != null)
				{
					string damageStr = selectedItem.Content.ToString();
					diceRoll.DamageType = DndUtils.ToDamage(damageStr);
				}
			}

			bool foundPlayer = false;
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
			{
				if (uIElement is PlayerRollCheckBox checkbox && checkbox.IsChecked == true)
				{
					VantageKind vantageKind = VantageKind.Normal;
					if (checkbox.RbAdvantage.IsChecked == true)
						vantageKind = VantageKind.Advantage;
					else if (checkbox.RbDisadvantage.IsChecked == true)
						vantageKind = VantageKind.Disadvantage;

					checkbox.RbNormal.IsChecked = true;

					string inspirationText = rollInspirationAfterwards && type != DiceRollType.InspirationOnly ? "" : checkbox.TbxInspiration.Text;
					BeforePlayerRolls(checkbox.PlayerId, diceRoll, ref vantageKind);
					diceRoll.AddPlayer(checkbox.PlayerId, vantageKind, inspirationText);
					foundPlayer = true;
				}
			}

			if (!foundPlayer)
			{
				BeforePlayerRolls(ActivePlayerId, diceRoll, ref diceRollKind);
				diceRoll.VantageKind = diceRollKind;
			}

			diceRoll.AddCritFailMessages(type);



			diceRoll.ThrowPower = new Random().Next() * 2.8;
			if (diceRoll.ThrowPower < 0.3)
				diceRoll.ThrowPower = 0.3;

			if (type == DiceRollType.SavingThrow)
				diceRoll.SetHiddenThreshold(tbxSaveThreshold.Text);
			else if (type == DiceRollType.SkillCheck)
				diceRoll.SetHiddenThreshold(tbxSkillCheckThreshold.Text);
			else
				diceRoll.SetHiddenThreshold(tbxAttackThreshold.Text);

			diceRoll.IsMagic = ckbUseMagic.IsChecked == true || type == DiceRollType.WildMagicD20Check;

			diceRoll.AddTrailingEffects(activeTrailingEffects);
			diceRoll.AddDieRollEffects(activeDieRollEffects);

			// TODO: Make this data-driven:

			if (type == DiceRollType.WildMagicD20Check)
			{
				diceRoll.IsMagic = true;
				diceRoll.AddTrailingEffects("SmallSparks");
				if (diceRoll.PlayerRollOptions != null && diceRoll.PlayerRollOptions.Count == 1)
				{
					Character activePlayer = GetPlayer(diceRoll.PlayerRollOptions[0].PlayerID);
					activePlayer.NumWildMagicChecks++;
					diceRoll.Modifier = activePlayer.NumWildMagicChecks;
				}
			}

			if (type == DiceRollType.WildMagic)
			{
				diceRoll.Modifier = 0;
				diceRoll.HiddenThreshold = 0;
				diceRoll.IsMagic = true;
				diceRoll.OnThrowSound = "WildMagicRoll";
				diceRoll.Type = DiceRollType.WildMagic;
				diceRoll.TrailingEffects.Add(new TrailingEffect()
				{
					EffectType = "SparkTrail",
					RotationOffset = 180,
					RotationOffsetRandom = 15,
					HueShift = ActivePlayer.hueShift.ToString(),
					HueShiftRandom = 35,
					LeftRightDistanceBetweenPrints = 0,
					MinForwardDistanceBetweenPrints = 64
				});
			}

			return diceRoll;
		}

		private void BtnAddLongRest_Click(object sender, RoutedEventArgs e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))  // Ctrl + Click affects only the active player.
			{
				if (ActivePlayer == null)
					return;
				ActivePlayer.RechargeAfterLongRest();
				TellAll($"{ActivePlayer.firstName} has had a long rest.");
			}
			else
			{
				TellAll("All players have recharged after a long rest.");
				game?.RechargePlayersAfterLongRest();
				Rest(8);
				TellDungeonMasterTheTime();
			}
		}

		private void BtnAddShortRest_Click(object sender, RoutedEventArgs e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))  // Ctrl + Click affects only the active player.
			{
				if (ActivePlayer == null)
					return;
				ActivePlayer.RechargeAfterShortRest();
				TellAll($"{ActivePlayer.firstName} has had a short rest.");
			}
			else
			{
				TellAll("All players have had a short rest.");
				game?.RechargePlayersAfterShortRest();
				Rest(2);
				TellDungeonMasterTheTime();
			}
		}

		void TellDungeonMasterTheTime()
		{
			TellDungeonMaster($"The time is now {game.Clock.AsFullDndDateTimeString()}.");
		}

		private void Rest(int hours)
		{
			resting = true;
			try
			{
				clockMessage = $"+{hours} hours";
				game.Clock.Advance(DndTimeSpan.FromHours(hours));
			}
			finally
			{
				resting = false;
			}
		}

		private void DndTimeClock_TimeChanged(object sender, TimeClockEventArgs e)
		{
			bool bigUpdate = e.SpanSinceLastUpdate.TotalSeconds > 60;
			UpdateClock(bigUpdate, e.SpanSinceLastUpdate.TotalDays);

			// TODO: Update time-based curses, spells, and diseases.
			if (resting)
			{
				// TODO: Update character stats.
			}
		}

		string clockMessage;

		private void UpdateClock(bool bigUpdate = false, double daysSinceLastUpdate = 0)
		{
			if (txtTime == null)
				return;
			string timeStr = game.Clock.AsFullDndDateTimeString();

			if (game.ClockHasStopped())
				timeStr = " " + timeStr + " ";

			if (txtTime.Text == timeStr)
				return;

			txtTime.Text = timeStr;

			TimeSpan timeIntoToday = game.Clock.Time - new DateTime(game.Clock.Time.Year, game.Clock.Time.Month, game.Clock.Time.Day);
			double percentageRotation = 360 * timeIntoToday.TotalMinutes / TimeSpan.FromDays(1).TotalMinutes;

			string afterSpinMp3 = null;

			if (daysSinceLastUpdate > 0.08)  // Short rest or greater
			{
				if (timeIntoToday.TotalHours < 2 || timeIntoToday.TotalHours > 22)
				{
					afterSpinMp3 = "midnightWolf";
				}
				else if (timeIntoToday.TotalHours > 4 && timeIntoToday.TotalHours < 8)
				{
					afterSpinMp3 = "morningRooster";

				}
				else if (timeIntoToday.TotalHours > 10 && timeIntoToday.TotalHours < 14)
				{
					afterSpinMp3 = "birdsNoon";
				}
				else if (timeIntoToday.TotalHours > 16 && timeIntoToday.TotalHours < 20)
				{
					if (new Random().Next(100) > 50)
						afterSpinMp3 = "eveningCrickets";
					else
						afterSpinMp3 = "lateEveningFrogs";
				}
				if (lastAmbientSoundPlayed == afterSpinMp3)  // prevent the same sound from being played twice in a row.
					afterSpinMp3 = null;
				else
					lastAmbientSoundPlayed = afterSpinMp3;
			}

			ClockDto clockDto = new ClockDto()
			{
				Message = clockMessage,
				Time = timeStr,
				BigUpdate = bigUpdate,
				Rotation = percentageRotation,
				InCombat = game.Clock.InCombat,
				InTimeFreeze = game.Clock.InTimeFreeze,
				FullSpins = daysSinceLastUpdate,
				AfterSpinMp3 = afterSpinMp3
			};

			clockMessage = null;

			string serializedObject = JsonConvert.SerializeObject(clockDto);
			HubtasticBaseStation.UpdateClock(serializedObject);
		}

		private void BtnAdvanceTurn_Click(object sender, RoutedEventArgs e)
		{
			game.AdvanceRound();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
			HubtasticBaseStation.AllDiceDestroyed += HubtasticBaseStation_AllDiceDestroyed;
			HubtasticBaseStation.TellDungeonMaster += HubtasticBaseStation_TellDM;
			OnCombatChanged();
			UpdateClock();
			StartRealTimeTimer();
		}

		private void HubtasticBaseStation_TellDM(object sender, MessageEventArgs ea)
		{
			TellDungeonMaster(ea.Message);
		}

		private void HubtasticBaseStation_AllDiceDestroyed(object sender, DiceEventArgs ea)
		{
			CheckForFollowUpRolls(ea.StopRollingData);
			if (dynamicThrottling && DateTime.Now - lastDieRollTime > TimeSpan.FromSeconds(3))
			{
				ChangeFrameRateAndUI(Overlays.Back, 30);
				ChangeFrameRateAndUI(Overlays.Front, 30);
				ChangeFrameRateAndUI(Overlays.Dice, 1);
			}
		}

		private void BtnAddDay_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Day";
			game.Clock.Advance(DndTimeSpan.FromDays(1), Modifiers.ShiftDown);
		}

		private void BtnAddTenDay_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+10 Days";
			game.Clock.Advance(DndTimeSpan.FromDays(10), Modifiers.ShiftDown);
		}

		private void BtnAddMonth_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Month";
			game.Clock.Advance(DndTimeSpan.FromDays(30), Modifiers.ShiftDown);
		}

		public DiceRollType NextDieRollType
		{
			get => nextDieRollType;
			set
			{
				if (nextDieRollType == value)
					return;

				nextDieRollType = value;
				SetNextRollTypeUI();
			}
		}

		private void SetNextRollTypeUI()
		{
			if (nextDieRollType != DiceRollType.None)
				tbNextDieRoll.Text = $"({nextDieRollType})";
			else
				tbNextDieRoll.Text = "";
			btnRollDice.IsEnabled = true;
		}

		private void BtnAddHour_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Hour";
			game.Clock.Advance(DndTimeSpan.FromHours(1), Modifiers.ShiftDown);
		}

		private void BtnAdd10Minutes_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+10 Minutes";
			game.Clock.Advance(DndTimeSpan.FromMinutes(10), Modifiers.ShiftDown);
		}

		private void BtnAdd1Minute_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Minute";
			game.Clock.Advance(DndTimeSpan.FromMinutes(1), Modifiers.ShiftDown);
		}



		void enableDiceRollButtons()
		{
			btnRollDice.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
		}

		void EnableDiceRollButtons(bool enable)
		{
			Dispatcher.Invoke(() =>
			{
				btnRollDice.IsEnabled = enable;
				spSpecialThrows.IsEnabled = enable;
			});
		}

		string GetSkillCheckStr(Skills skillCheck)
		{
			switch (skillCheck)
			{
				case Skills.acrobatics: return "Acrobatics";
				case Skills.animalHandling: return "Animal Handling";
				case Skills.arcana: return "Arcana";
				case Skills.athletics: return "Athletics";
				case Skills.deception: return "Deception";
				case Skills.history: return "History";
				case Skills.insight: return "Insight";
				case Skills.intimidation: return "Intimidation";
				case Skills.investigation: return "Investigation";
				case Skills.medicine: return "Medicine";
				case Skills.nature: return "Nature";
				case Skills.perception: return "Perception";
				case Skills.performance: return "Performance";
				case Skills.persuasion: return "Persuasion";
				case Skills.religion: return "Religion";
				case Skills.sleightOfHand: return "Sleight of Hand";
				case Skills.stealth: return "Stealth";
				case Skills.survival: return "Survival";
				case Skills.strength: return "Strength";
				case Skills.dexterity: return "Dexterity";
				case Skills.constitution: return "Constitution";
				case Skills.intelligence: return "Intelligence";
				case Skills.wisdom: return "Wisdom";
				case Skills.charisma: return "Charisma";
			}
			return "None";
		}

		string GetAbilityStr(Ability savingThrow)
		{
			switch (savingThrow)
			{
				case Ability.charisma: return "Charisma";
				case Ability.constitution: return "Constitution";
				case Ability.dexterity: return "Dexterity";
				case Ability.intelligence: return "Intelligence";
				case Ability.strength: return "Strength";
				case Ability.wisdom: return "Wisdom";
			}
			return "None";
		}

		bool forcedWildMagicThisRoll;

		void HandleWildMagicD20Check(IndividualRoll individualRoll)
		{
			if (forcedWildMagicThisRoll)
				return;
			if (individualRoll.value == 1)
			{
				forcedWildMagicThisRoll = true;
				PlayScene("DH.WildMagicRoll");
				wildMagicRollTimer.Start();
				TellAll("It's a one! Need to roll wild magic!");
			}
			else
			{
				TellAll($"Wild Magic roll: {individualRoll.value}.");
			}
		}

		void IndividualDiceStoppedRolling(IndividualRoll individualRoll)
		{
			switch (individualRoll.type)
			{
				case "BarbarianWildSurge":
					HandleBarbarianWildSurge(individualRoll);
					break;
				case "WildMagicD20Check":
				case "Wild Magic Check":
				case "\"Wild Magic Check\"":
					History.Log("IndividualDiceStoppedRolling: " + individualRoll.type);
					HandleWildMagicD20Check(individualRoll);
					break;
			}
		}

		private void HandleBarbarianWildSurge(IndividualRoll individualRoll)
		{
			switch (individualRoll.value)
			{
				case 1:
					PlayScene("DH.WildSurge.Necrotic");
					break;
				case 2:
					PlayScene("DH.WildSurge.Teleport");
					break;
				case 3:
					PlayScene("DH.WildSurge.Flumphs");
					break;
				case 4:
					PlayScene("DH.WildSurge.ArcanaShroud");
					break;
				case 5:
					PlayScene("DH.WildSurge.PlantGrowth.Arrive");
					break;
				case 6:
					PlayScene("DH.WildSurge.Thoughts");
					break;
				case 7:
					//PlayScene("DH.WildSurge.Shadows");
					break;
				case 8:
					PlayScene("DH.WildSurge.Radiant");
					break;
			}
		}

		void IndividualDiceStoppedRolling(List<IndividualRoll> individualRolls)
		{
			foreach (IndividualRoll individualRoll in individualRolls)
			{
				IndividualDiceStoppedRolling(individualRoll);
			}
		}

		bool ChaosBoltRolledDoubles(DiceStoppedRollingData diceStoppedRollingData)
		{
			if (!diceStoppedRollingData.success)
				return false;
			bool isChaosBolt = false;
			foreach (IndividualRoll individualRoll in diceStoppedRollingData.individualRolls)
			{
				if (individualRoll.numSides == 20 && individualRoll.type == "ChaosBolt")
				{
					isChaosBolt = true;
				}
			}
			if (!isChaosBolt)
				return false;

			int firstValue = -1;
			int secondValue = -1;
			foreach (IndividualRoll individualRoll in diceStoppedRollingData.individualRolls)
			{
				if (individualRoll.numSides == 8)
					if (firstValue == -1)
						firstValue = individualRoll.value;
					else
					{
						secondValue = individualRoll.value;
						break;
					}
			}
			return firstValue == secondValue;
		}

		void CheckForFollowUpRolls(DiceStoppedRollingData diceStoppedRollingData)
		{
			if (diceStoppedRollingData == null)
				return;
			// Noticed doubling up of individual rolls.
			Character singlePlayer = diceStoppedRollingData.GetSingleRollingPlayer();
			if (singlePlayer != null)
			{
				TriggerAfterRollEffects(diceStoppedRollingData, singlePlayer);
			}
			else if (diceStoppedRollingData.multiplayerSummary != null)
				foreach (PlayerRoll playerRoll in diceStoppedRollingData.multiplayerSummary)
				{
					Character player = game.GetPlayerFromId(playerRoll.id);
					TriggerAfterRollEffects(diceStoppedRollingData, player);
				}


			// TODO: Trigger After Roll Effects for all players if multiple players rolled at the same time.


			//diceRollData.playerID
			//if (diceRollData.isSpell)

			if (ChaosBoltRolledDoubles(diceStoppedRollingData))
			{
				DiceRoll chaosBoltRoll = lastRoll;
				if (lastRoll.Type == DiceRollType.WildMagicD20Check)
					chaosBoltRoll = secondToLastRoll;
				if (chaosBoltRoll == null)
				{
					return;
				}
				Dispatcher.Invoke(() =>
				{
					if (AnswersYes("Doubles! Fire Chaos Bolt again?"))
					{
						RollTheDice(chaosBoltRoll);
					}
				});
			}
		}

		private static void TriggerAfterRollEffects(DiceStoppedRollingData diceStoppedRollingData, Character singlePlayer)
		{
			if (singlePlayer == null)
				return;

			// TODO: Add support for OnPlayerSaves event.
			singlePlayer.lastRollWasSuccessful = diceStoppedRollingData.success;

			if (!string.IsNullOrWhiteSpace(diceStoppedRollingData.spellName))
				singlePlayer.JustCastSpell(diceStoppedRollingData.spellName);
			else if (diceStoppedRollingData.type == DiceRollType.Attack)
			{
				singlePlayer.AfterPlayerSwingsWeapon();
			}

			// TODO: pass **targeted creatures** into RollIsComplete...
			singlePlayer.RollIsComplete(diceStoppedRollingData.wasCriticalHit);
		}

		//void RepeatLastRoll()
		//{
		//	RollTheDice(lastRoll);
		//}

		void NotifyPlayersRollHasStopped(DiceStoppedRollingData stopRollingData)
		{
			if (stopRollingData.multiplayerSummary != null)
				foreach (PlayerRoll playerRoll in stopRollingData.multiplayerSummary)
				{
					Character player = game.GetPlayerFromId(playerRoll.id);
					player?.RollHasStopped();
				}
			else
			{
				Character player = game.GetPlayerFromId(stopRollingData.playerID);
				player?.RollHasStopped();
			}
		}

		public enum SpellHitType
		{
			Miss,
			Hit
		}

		void ShowSpellEffectsInGame(string spellName, Queue<SpellEffect> additionalSpellEffects, Queue<SoundEffect> additionalSoundEffects, SpellHitType spellHitType = SpellHitType.Hit)
		{
			EffectGroup effectGroup = new EffectGroup();

			VisualEffectTarget chestTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, -150));
			VisualEffectTarget bottomTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, 0));

			if (spellHitType != SpellHitType.Hit)
			{
				effectGroup.Add(CreateEffect("SpellMiss", chestTarget, 0, 100, 0));
			}
			else
			{
				double scale = 1;
				double autoRotation = 140;
				const double scaleIncrement = 1.1;
				//AnimationEffect effect = CreateEffect(GetRandomHitSpellName(), chestTarget, hueShift, 100);
				//effect.autoRotation = autoRotation;
				autoRotation *= -1;
				//effectGroup.Add(effect);
				scale *= scaleIncrement;

				int timeOffset = 200;

				Spell spell = AllSpells.Get(spellName);
				if (spell != null)
				{
					if (!string.IsNullOrWhiteSpace(spell.Hue1))
					{
						AnimationEffect effectBonus1 = CreateSpellEffect(GetRandomHitSpellName(), chestTarget, spell.Hue1, spell.Bright1);
						effectBonus1.timeOffsetMs = timeOffset;
						effectBonus1.scale = scale;
						effectBonus1.autoRotation = autoRotation;
						autoRotation *= -1;
						scale *= scaleIncrement;
						effectGroup.Add(effectBonus1);
					}
					timeOffset += 200;
					if (!string.IsNullOrWhiteSpace(spell.Hue2))
					{
						AnimationEffect effectBonus2 = CreateSpellEffect(GetRandomHitSpellName(), chestTarget, spell.Hue2, spell.Bright2);
						effectBonus2.timeOffsetMs = timeOffset;
						effectBonus2.scale = scale;
						effectBonus2.autoRotation = autoRotation;
						autoRotation *= -1;
						scale *= scaleIncrement;
						effectGroup.Add(effectBonus2);
					}
				}

				DequeueSpellEffects(effectGroup, additionalSpellEffects, chestTarget, bottomTarget, ref scale, scaleIncrement, ref autoRotation, ref timeOffset);
				DequeueSoundEffects(additionalSoundEffects, effectGroup);
			}

			// TODO: Add success or fail sound effects.
			string serializedObject = JsonConvert.SerializeObject(effectGroup);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}
		void ShowSpellHitOrMissInGame(int playerId, SpellHitType spellHitType, string spellName)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			ShowSpellEffectsInGame(spellName, player.additionalSpellHitEffects, player.additionalSpellHitSoundEffects, spellHitType);
		}

		void ShowSpellCastEffectsInGame(int playerId, string spellName)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			ShowSpellEffectsInGame(spellName, player.additionalSpellCastEffects, player.additionalSpellCastSoundEffects);
		}

		void DequeueSoundEffects(Queue<SoundEffect> additionalSoundEffects, EffectGroup effectGroup)
		{
			while (additionalSoundEffects.Count > 0)
			{
				effectGroup.Add(additionalSoundEffects.Dequeue());
			}
		}

		private void DequeueSpellEffects(EffectGroup effectGroup, Queue<SpellEffect> additionalSpellEffects, VisualEffectTarget chestTarget, VisualEffectTarget bottomTarget, ref double scale, double scaleIncrement, ref double autoRotation, ref int timeOffset)
		{
			while (additionalSpellEffects.Count > 0)
			{
				SpellEffect spellHit = additionalSpellEffects.Dequeue();
				string effectName;
				bool usingSpellHits = true;
				VisualEffectTarget target;

				if (!string.IsNullOrWhiteSpace(spellHit.EffectName))
				{
					effectName = spellHit.EffectName;
					usingSpellHits = false;
					target = bottomTarget;
				}
				else
				{
					effectName = GetRandomHitSpellName();
					target = chestTarget;
				}
				AnimationEffect effectBonus = CreateEffect(effectName, target,
					spellHit.Hue, spellHit.Saturation, spellHit.Brightness,
					spellHit.SecondaryHue, spellHit.SecondarySaturation, spellHit.SecondaryBrightness);
				if (usingSpellHits)
				{
					effectBonus.timeOffsetMs = timeOffset;
					effectBonus.scale = scale;
					effectBonus.autoRotation = autoRotation;
					autoRotation *= -1;
					scale *= scaleIncrement;
					timeOffset += 200;
				}
				else
				{
					if (spellHit.TimeOffset > int.MinValue)
						effectBonus.timeOffsetMs = spellHit.TimeOffset;
					else
					{
						effectBonus.timeOffsetMs = timeOffset;
						timeOffset += 200;
					}

					effectBonus.scale = spellHit.Scale;
					effectBonus.autoRotation = spellHit.AutoRotation;
					effectBonus.rotation = spellHit.Rotation;
				}
				effectGroup.Add(effectBonus);
			}
		}

		private static string GetRandomHitSpellName()
		{
			return "SpellHit" + MathUtils.RandomBetween(1, 5).ToString();
		}

		AnimationEffect CreateSpellEffect(string spriteName, VisualEffectTarget target, string hueShiftStr, string brightnessStr = null)
		{
			int hueShift;
			if (hueShiftStr == "player")
			{
				hueShift = ActivePlayer.hueShift;
			}
			else
			{
				if (!int.TryParse(hueShiftStr, out hueShift))
					hueShift = 0;
			}
			if (!int.TryParse(brightnessStr, out int brightness))
				brightness = 100;
			return CreateEffect(spriteName, target, hueShift, 100, brightness);
		}

		AnimationEffect CreateEffect(string spriteName, VisualEffectTarget target,
			int hueShift = 0, int saturation = 100, int brightness = 100,
			int secondaryHueShift = 0, int secondarySaturation = 100, int secondaryBrightness = 100)
		{
			AnimationEffect spellEffect = new AnimationEffect();
			spellEffect.spriteName = spriteName;
			spellEffect.hueShift = hueShift;
			spellEffect.saturation = saturation;
			spellEffect.brightness = brightness;
			spellEffect.secondaryHueShift = secondaryHueShift;
			spellEffect.secondarySaturation = secondarySaturation;
			spellEffect.secondaryBrightness = secondaryBrightness;
			spellEffect.target = target;
			return spellEffect;
		}

		void CheckSpellHitResults(DiceStoppedRollingData stopRollingData)
		{
			if (!DndUtils.IsAttack(stopRollingData.type))
				return;

			if (string.IsNullOrWhiteSpace(stopRollingData.spellName))
				return;

			SpellHitType spellHitType = stopRollingData.success ? SpellHitType.Hit : SpellHitType.Miss;

			ShowSpellHitOrMissInGame(stopRollingData.playerID, spellHitType, stopRollingData.spellName);
		}
		int lastDamage;
		int lastHealth;
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (dynamicThrottling)
			{
				ChangeFrameRateAndUI(Overlays.Back, 30);
				ChangeFrameRateAndUI(Overlays.Front, 30);
			}

			waitingToClearDice = true;
			if (ea.StopRollingData.type == DiceRollType.DamageOnly)
			{
				// TODO: Store last damage type...
				lastDamage = ea.StopRollingData.damage;
			}
			else if (ea.StopRollingData.type == DiceRollType.HealthOnly)
			{
				lastHealth = ea.StopRollingData.health;
			}


			if (ea.StopRollingData.individualRolls?.Count > 0)
			{
				IndividualDiceStoppedRolling(ea.StopRollingData.individualRolls);
			}

			NotifyPlayersRollHasStopped(ea.StopRollingData);

			SaveNamedResults(ea);
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			ClearExistingWindupsInGame();
			ReportOnDieRoll(ea);
			CheckSpellHitResults(ea.StopRollingData);

			EnableDiceRollButtons(true);
			ShowClearButton(null, EventArgs.Empty);
		}

		string GetPlayerEmoticon(int playerId)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return string.Empty;
			return player.emoticon;
		}
		List<string> lastInitiativeResults = new List<string>();

		bool suppressMessagesToCodeRushedChannel;

		void ReportInitiativeResults(DiceEventArgs ea, string title)
		{
			if (ea.StopRollingData.multiplayerSummary == null)
			{
				TellDungeonMaster($"͏͏͏͏͏͏͏͏͏͏͏͏̣{Icons.WarningSign} Unexpected issue - no multiplayer results.");
				return;
			}

			suppressMessagesToCodeRushedChannel = true;

			try
			{
				TellAll(title);
				lastInitiativeResults.Clear();
				int count = 1;
				foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
				{
					string playerName = DndUtils.GetFirstName(playerRoll.name);
					string emoticon = GetPlayerEmoticon(playerRoll.id) + " ";
					if (emoticon == "Player ")
						emoticon = "";
					int rollValue = playerRoll.modifier + playerRoll.roll;
					bool success = rollValue >= ea.StopRollingData.hiddenThreshold;
					string initiativeLine = $"͏͏͏͏͏͏͏͏͏͏͏͏̣{twitchIndent}{DndUtils.GetOrdinal(count)}: {emoticon}{playerName}, rolled a {rollValue.ToString()}.";
					lastInitiativeResults.Add(initiativeLine);
					TellAll(initiativeLine);
					count++;
				}
			}
			finally
			{
				suppressMessagesToCodeRushedChannel = false;
			}
		}

		private void ReportOnDieRoll(DiceEventArgs ea)
		{
			if (ea.StopRollingData == null)
				return;

			if (ea.StopRollingData.type == DiceRollType.Initiative)
			{
				ReportInitiativeResults(ea, "Initiative: ");
				return;
			}

			if (ea.StopRollingData.type == DiceRollType.NonCombatInitiative)
			{
				ReportInitiativeResults(ea, "Non-combat Initiative: ");
				return;
			}

			int rollValue = ea.StopRollingData.roll;


			if (rollValue == 0 && ea.StopRollingData.individualRolls != null && ea.StopRollingData.individualRolls.Count > 0)
			{
				foreach (IndividualRoll individualRoll in ea.StopRollingData.individualRolls)
				{
					rollValue += individualRoll.value;
				}
			}

			string additionalMessage = ea.StopRollingData.additionalDieRollMessage;
			if (!String.IsNullOrEmpty(additionalMessage))
				additionalMessage = " " + additionalMessage;
			string rollTitle = "";
			string damageStr = "";
			string bonusStr = "";
			if (ea.StopRollingData.bonus > 0)
				bonusStr = " - bonus: " + ea.StopRollingData.bonus.ToString();
			string successStr = GetSuccessStr(ea.StopRollingData.success, ea.StopRollingData.type);
			switch (ea.StopRollingData.type)
			{
				case DiceRollType.SkillCheck:
					rollTitle = GetSkillCheckStr(ea.StopRollingData.skillCheck) + " Skill Check: ";
					break;
				case DiceRollType.Attack:
					rollTitle = "Attack: ";
					damageStr = ", Damage: " + ea.StopRollingData.damage.ToString();
					break;
				case DiceRollType.SavingThrow:
					rollTitle = GetAbilityStr(ea.StopRollingData.savingThrow) + " Saving Throw: ";
					break;
				case DiceRollType.FlatD20:
					rollTitle = "Flat D20: ";
					break;
				case DiceRollType.DeathSavingThrow:
					rollTitle = "Death Saving Throw: ";
					break;
				case DiceRollType.PercentageRoll:
					rollTitle = "Percentage Roll: ";
					break;
				case DiceRollType.WildMagic:
					rollTitle = "Wild Magic: ";
					break;
				case DiceRollType.BendLuckAdd:
					rollTitle = "Bend Luck Add: ";
					break;
				case DiceRollType.BendLuckSubtract:
					rollTitle = "Bend Luck Subtract: ";
					break;
				case DiceRollType.LuckRollLow:
					rollTitle = "Luck Roll Low: ";
					break;
				case DiceRollType.LuckRollHigh:
					rollTitle = "Luck Roll High: ";
					break;
				case DiceRollType.DamageOnly:
					rollTitle = "Damage: ";
					rollValue = ea.StopRollingData.damage;
					break;
				case DiceRollType.HealthOnly:
					rollTitle = "Health: ";
					rollValue = ea.StopRollingData.health;
					break;
				case DiceRollType.ExtraOnly:
					rollTitle = "Extra: ";
					rollValue = ea.StopRollingData.extra;
					break;
				case DiceRollType.ChaosBolt:
					rollTitle = "Chaos Bolt: ";
					damageStr = ", Damage: " + ea.StopRollingData.damage.ToString();
					break;
				case DiceRollType.Initiative:
					rollTitle = "Initiative: ";
					break;
				case DiceRollType.NonCombatInitiative:
					rollTitle = "Non-combat Initiative: ";
					break;
				case DiceRollType.WildMagicD20Check:
					rollTitle = "wild magic check: ";
					break;
			}
			if (rollTitle == "")
				rollTitle = "Dice roll: ";
			string message = string.Empty;
			Character singlePlayer = null;
			if (ea.StopRollingData.multiplayerSummary != null && ea.StopRollingData.multiplayerSummary.Count > 0)
			{
				if (ea.StopRollingData.multiplayerSummary.Count == 1)
					singlePlayer = AllPlayers.GetFromId(ea.StopRollingData.multiplayerSummary[0].id);
				foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
				{
					string playerName = DndUtils.GetFirstName(playerRoll.name);
					if (playerName != "")
						playerName = playerName + "'s ";

					rollValue = playerRoll.modifier + playerRoll.roll;
					bool success = rollValue >= ea.StopRollingData.hiddenThreshold;
					successStr = GetSuccessStr(success, ea.StopRollingData.type);
					string localDamageStr;
					if (success)
						localDamageStr = damageStr;
					else
						localDamageStr = "";
					if (!string.IsNullOrWhiteSpace(message))
						message += "; ";
					message += playerName + rollTitle + rollValue.ToString() + successStr + localDamageStr + bonusStr;
				}
			}
			else
			{
				singlePlayer = AllPlayers.GetFromId(ea.StopRollingData.playerID);
				string playerName = GetPlayerName(ea.StopRollingData.playerID);
				if (playerName != "")
					playerName = playerName + "'s ";
				if (!ea.StopRollingData.success)
					damageStr = "";

				message += playerName + rollTitle + rollValue.ToString() + successStr + damageStr + bonusStr;
			}

			if (singlePlayer != null)
				game.DieRollStopped(singlePlayer, rollValue, ea.StopRollingData);

			//DieRollStopped

			message += additionalMessage;
			if (!string.IsNullOrWhiteSpace(message))
			{
				TellAll(message);
			}
		}

		private bool AnswersYes(string question)
		{
			List<string> answers = new List<string>();
			answers.Add("1:Yes");
			answers.Add("2:No");
			bool yes = AskQuestion(question, answers) == 1;
			return yes;
		}

		private static string GetSuccessStr(bool success, DiceRollType type)
		{
			string successStr = "";
			switch (type)
			{
				case DiceRollType.FlatD20:
				case DiceRollType.SavingThrow:
				case DiceRollType.SkillCheck:
				case DiceRollType.DeathSavingThrow:
					if (success)
						successStr = " (success)";
					else
						successStr = " (fail)";
					break;
				case DiceRollType.Attack:
				case DiceRollType.ChaosBolt:
					if (success)
						successStr = " (hit)";
					else
						successStr = " (miss)";
					break;
			}

			return successStr;
		}

		string GetPlayerName(int playerID)
		{
			foreach (Character player in game.Players)
			{
				if (player.playerID == playerID)
					return DndUtils.GetFirstName(player.name);
			}

			return "";
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.InCombat = !game.Clock.InCombat;
			if (game.Clock.InCombat)
			{
				GetRandomEnterCombatClockMessage();
				ChangeThemeMusic("Battle");
				ckbUseMagic.IsChecked = false;
				game.EnteringCombat();
				btnEnterExitCombat.Background = new SolidColorBrush(Color.FromRgb(42, 42, 102));
				RollInitiative();
			}
			else
			{
				GetRandomExitCombatClockMessage();
				ChangeThemeMusic("Travel");
				game.ExitingCombat();
				btnEnterExitCombat.Background = new SolidColorBrush(Colors.DarkRed);
			}

			OnCombatChanged();
		}

		private void BtnEnterExitTimeFreeze_Click(object sender, RoutedEventArgs e)
		{
			if (game.Clock.InCombat)
			{
				if (game.Clock.InTimeFreeze)
					game.Clock.InTimeFreeze = false;
				TellDungeonMaster($"{Icons.WarningSign} -- Exit combat before rolling non-combat initiative.");
				return;
			}
			game.Clock.InTimeFreeze = !game.Clock.InTimeFreeze;
			if (game.Clock.InTimeFreeze)
			{
				GetRandomEnterTimeFreezeClockMessage();
				ChangeThemeMusic("Suspense");
				ckbUseMagic.IsChecked = false;
				game.EnteringTimeFreeze();
				btnEnterExitTimeFreeze.Background = new SolidColorBrush(Color.FromRgb(42, 42, 102));
				RollNonCombatInitiative();
			}
			else
			{
				GetRandomExitTimeFreezeClockMessage();
				ChangeThemeMusic("Travel");
				game.ExitingTimeFreeze();
				btnEnterExitTimeFreeze.Background = new SolidColorBrush(Colors.Gray);
			}

			OnTimeFreezeChanged();
		}

		private void GetRandomEnterTimeFreezeClockMessage()
		{
			switch (new Random().Next(4))
			{
				case 0:
					clockMessage = "Stopping!";
					break;
				case 1:
					clockMessage = "Freeze!";
					break;
				case 2:
					clockMessage = "Uh oh!";
					break;
				case 3:
					clockMessage = "Stop!";
					break;
			}
		}

		private void GetRandomExitTimeFreezeClockMessage()
		{
			switch (new Random().Next(3))
			{
				case 0:
					clockMessage = "Running!";
					break;
				case 1:
					clockMessage = "Go!";
					break;
				case 2:
					clockMessage = "Go go go!";
					break;
			}
		}

		private void GetRandomEnterCombatClockMessage()
		{
			DateTime dateTime = DateTime.Now;
			bool isLikelyEarlyCodeRushedShow = dateTime.Hour < 16;
			switch (new Random().Next(8))
			{
				case 0:
					clockMessage = "Combat!";
					break;
				case 1:
					clockMessage = "Fight!";
					break;
				case 2:
					clockMessage = "Battle!";
					break;
				case 3:
					if (isLikelyEarlyCodeRushedShow)
						clockMessage = "Oh cram!";
					else
						clockMessage = "Oh shit!";
					break;
				case 4:
					clockMessage = "Oh no!";
					break;
				case 5:
					if (isLikelyEarlyCodeRushedShow)
						clockMessage = "Heck no!";
					else
						clockMessage = "Fuck no!";
					break;
				case 6:
					if (isLikelyEarlyCodeRushedShow)
						clockMessage = "Cram!";
					else
						clockMessage = "Crap!";
					break;
				case 7:
					if (isLikelyEarlyCodeRushedShow)
						clockMessage = "Dang!";
					else
						clockMessage = "Damn!";
					break;
			}
		}

		private void GetRandomExitCombatClockMessage()
		{
			switch (new Random().Next(8))
			{
				case 0:
					clockMessage = "It's cool!";
					break;
				case 1:
					clockMessage = "It's over!";
					break;
				case 2:
					clockMessage = "Peace!";
					break;
				case 3:
					clockMessage = "Phew!";
					break;
				case 4:
					clockMessage = "Close one!";
					break;
				case 5:
					clockMessage = "Yes!";
					break;
				case 6:
					clockMessage = "Done!";
					break;
				case 7:
					clockMessage = "We did it!";
					break;
			}
		}


		private static void ChangeThemeMusic(string theme)
		{
			SoundCommand soundCommand = new SoundCommand(SoundPlayerFolders.Music);
			soundCommand.strData = theme;
			soundCommand.type = SoundCommandType.ChangeFolder;
			Execute(soundCommand);
		}

		private void OnCombatChanged()
		{
			if (game.Clock.InCombat)
			{
				tbEnterExitCombat.Text = "Exit Combat";
				realTimeAdvanceTimer.Stop();
				spTimeDirectModifiers.IsEnabled = false;
				btnAdvanceTurn.IsEnabled = true;
			}
			else
			{
				tbEnterExitCombat.Text = "Enter Combat";
				StartRealTimeTimer();
				spTimeDirectModifiers.IsEnabled = true;
				btnAdvanceTurn.IsEnabled = false;
			}
			UpdateClock(true);
		}

		private void OnTimeFreezeChanged()
		{
			if (game.Clock.InTimeFreeze)
			{
				tbEnterExitTimeFreeze.Text = "Start Clock";
				realTimeAdvanceTimer.Stop();
				spTimeDirectModifiers.IsEnabled = false;
				btnAdvanceTurn.IsEnabled = true;
			}
			else
			{
				tbEnterExitTimeFreeze.Text = "Freeze Time";
				StartRealTimeTimer();
				spTimeDirectModifiers.IsEnabled = true;
				btnAdvanceTurn.IsEnabled = false;
			}
			UpdateClock(true);
		}

		private void StartRealTimeTimer()
		{
			realTimeAdvanceTimer.Start();
			lastUpdateTime = DateTime.Now;
		}

		string lastAmbientSoundPlayed;
		DateTime clearButtonShowTime;
		private static readonly TimeSpan timeToAutoClear = TimeSpan.FromSeconds(2); // Delete this.
		void RealTimeClockHandler(object sender, EventArgs e)
		{
			TimeSpan timeSinceLastUpdate = DateTime.Now - lastUpdateTime;
			lastUpdateTime = DateTime.Now;
			game.Clock.Advance(timeSinceLastUpdate.TotalMilliseconds);
		}

		void PrepareUiForClearButton()
		{
			Dispatcher.Invoke(() =>
			{
				rectProgressToClear.Width = 0;
				btnClearDice.Visibility = Visibility.Visible;
			});
		}

		void ShowClearButton(object sender, EventArgs e)
		{
			pauseTime = TimeSpan.Zero;
			clearButtonShowTime = DateTime.Now;
			showClearButtonTimer.Stop();
			updateClearButtonTimer.Start();
			justClickedTheClearDiceButton = false;
			PrepareUiForClearButton();
		}

		void UpdateStateFromTimer(object sender, EventArgs e)
		{
			if (ActivePlayer == null)
				return;

			if (ActivePlayer.concentratedSpell == null)
				return;

			UpdateStateUIForPlayer(ActivePlayer, true);
		}

		void RollWildMagicHandler(object sender, EventArgs e)
		{
			wildMagicRollTimer.Stop();
			Dispatcher.Invoke(() =>
			{
				ActivateShortcut("Wild Magic");
				btnRollDice.Content = "Roll Wild Magic";
				BackToPlayersIn(18);
			});
		}
		void BackToPlayersIn(double seconds)
		{
			switchBackToPlayersTimer.Interval = TimeSpan.FromSeconds(seconds);
			switchBackToPlayersTimer.Start();
		}
		void SwitchBackToPlayersHandler(object sender, EventArgs e)
		{
			switchBackToPlayersTimer.Stop();
			Dispatcher.Invoke(() =>
			{
				PlayScene("Players");
			});
		}

		void UpdateClearButton(object sender, EventArgs e)
		{
			const double timeToAutoClear = 14000;
			TimeSpan timeClearButtonHasBeenVisible = (DateTime.Now - clearButtonShowTime) - pauseTime;
			if (timeClearButtonHasBeenVisible.TotalMilliseconds > timeToAutoClear)
			{
				ClearTheDice();
				rectProgressToClear.Width = 0;
				return;
			}

			double progress = timeClearButtonHasBeenVisible.TotalMilliseconds / timeToAutoClear;
			rectProgressToClear.Width = Math.Max(0, progress * btnClearDice.Width);
		}

		bool justClickedTheClearDiceButton;
		private void BtnClearDice_Click(object sender, RoutedEventArgs e)
		{
			justClickedTheClearDiceButton = true;
			ClearTheDice();
		}
		TimeSpan pauseTime;
		DateTime updateClearPaused;

		private void BtnClearDice_MouseEnter(object sender, MouseEventArgs e)
		{
			updateClearPaused = DateTime.Now;
			updateClearButtonTimer.Stop();
		}

		private void BtnClearDice_MouseLeave(object sender, MouseEventArgs e)
		{
			pauseTime += DateTime.Now - updateClearPaused;
			if (!justClickedTheClearDiceButton)
				updateClearButtonTimer.Start();
		}


		public static FrameworkElement FindChild(DependencyObject parent, string childName)
		{
			int childCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childCount; i++)
			{
				FrameworkElement child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
				if (child != null)
				{
					if (child.Name == childName)
						return child;
					child = FindChild(child, childName);
					if (child != null && child.Name == childName)
						return child;
				}
			}
			return null;
		}

		public enum Flavors
		{
			Vanilla,
			Chocolate,
			Strawberry,
			Mint
		}

		private void BtnWildAnimalForm_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.OnFirstContactSound = "WildForm";
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				EffectType = "PawPrint",
				Scale = 0.5,
				LeftRightDistanceBetweenPrints = 25,
				MinForwardDistanceBetweenPrints = 45,
				Lifespan = 3000
			});

			RollTheDice(diceRoll);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			if (frmCropPreview != null)
				frmCropPreview.Close();
			HubtasticBaseStation.DiceStoppedRolling -= HubtasticBaseStation_DiceStoppedRolling;
			HubtasticBaseStation.AllDiceDestroyed -= HubtasticBaseStation_AllDiceDestroyed;
		}

		private void BtnBendLuckAdd_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.BendLuckAdd);
			AddTrailingSparks(diceRoll);
			diceRoll.SecondRollTitle = "Bending Luck!";
			RollTheDice(diceRoll);
		}

		private void BtnBendLuckSubtract_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.BendLuckSubtract);
			AddTrailingSparks(diceRoll);
			diceRoll.SecondRollTitle = "Bending Luck!";
			RollTheDice(diceRoll);
		}

		private void BtnLuckRollHigh_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.LuckRollHigh);
			AddTrailingSparks(diceRoll);
			diceRoll.SecondRollTitle = "Sorcerer's Luck!";
			RollTheDice(diceRoll);
		}

		private static void AddTrailingSparks(DiceRoll diceRoll)
		{
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				EffectType = "SmallSparks",
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 33
			});
		}

		private void BtnLuckRollLow_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.LuckRollLow);
			AddTrailingSparks(diceRoll);
			diceRoll.SecondRollTitle = "Sorcerer's Luck!";
			RollTheDice(diceRoll);
		}

		void RollInitiative()
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Initiative);
			RollTheDice(diceRoll);
		}

		private void BtnModifier_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				tbxModifier.Text = button.Tag.ToString();
			}
		}

		DiceRollType nextDieRollType;
		void InitializeAttackShortcuts()
		{
			highlightRectangles = null;
			actionShortcuts = AllActionShortcuts.AllShortcuts;
		}


		bool settingInternally;

		private void TbxDamageDice_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlightsUI();
		}

		private void TbxModifier_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlightsUI();
		}

		private void CkbUseMagic_Checked(object sender, RoutedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlightsUI();
		}

		Character GetPlayer(int playerId)
		{
			foreach (Character player in game.Players)
				if (player.playerID == playerId)
					return player;
			return null;
		}

		void BuildPlayerTabs()
		{
			buildingTabs = true;

			try
			{
				tabPlayers.Items.Clear();

				foreach (Character player in game.Players)
				{
					PlayerTabItem tabItem = new PlayerTabItem();
					tabItem.Header = player.name;
					tabPlayers.Items.Add(tabItem);
					tabItem.PlayerId = player.playerID;

					StackPanel scrollControlStack = new StackPanel();
					tabItem.Content = scrollControlStack;

					ScrollViewer scrollViewer = new ScrollViewer();
					scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
					scrollControlStack.Children.Add(scrollViewer);

					Grid grid = new Grid();
					grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
					scrollViewer.Content = grid;

					CharacterSheets characterSheets = new CharacterSheets();
					characterSheets.PageChanged += CharacterSheets_PageChanged;
					characterSheets.ChargesChanged += CharacterSheets_ChargesChanged;
					characterSheets.PageBackgroundClicked += CharacterSheets_PageBackgroundClicked;
					characterSheets.CharacterChanged += HandleCharacterSheetDataChanged;
					characterSheets.SetFromCharacter(player);
					characterSheets.SkillCheckRequested += CharacterSheets_SkillCheckRequested;
					characterSheets.SavingThrowRequested += CharacterSheets_SavingThrowRequested;
					characterSheets.SavingThrowConsidered += CharacterSheets_SavingThrowConsidered;
					characterSheets.SkillCheckConsidered += CharacterSheets_SkillCheckConsidered;
					grid.Children.Add(characterSheets);
					tabItem.CharacterSheets = characterSheets;

					StackPanel playerControls = new StackPanel();
					playerControls.HorizontalAlignment = HorizontalAlignment.Center;
					playerControls.Orientation = Orientation.Horizontal;
					scrollControlStack.Children.Add(playerControls);
					Button button = new Button();
					button.Content = "Hide Scroll";
					button.Click += Button_ClearScrollClick;
					button.Width = 200;
					button.MinHeight = 45;
					playerControls.Children.Add(button);


					CheckBox chkShowPlayerNameplate = new CheckBox();
					chkShowPlayerNameplate.Margin = new Thickness(20, 10, 0, 0);
					chkShowPlayerNameplate.Content = "Show Player Nameplate";
					chkShowPlayerNameplate.Checked += ShowPlayerNameplate_CheckedChanged;
					chkShowPlayerNameplate.Unchecked += ShowPlayerNameplate_CheckedChanged;
					chkShowPlayerNameplate.MaxWidth = 200;
					chkShowPlayerNameplate.MinHeight = 45;
					chkShowPlayerNameplate.IsChecked = player.ShowingNameplate;
					chkShowPlayerNameplate.Tag = player.playerID;
					playerControls.Children.Add(chkShowPlayerNameplate);


					ListBox stateList = new ListBox();
					tabItem.StateList = stateList;
					scrollControlStack.Children.Add(stateList);
				}
			}
			finally
			{
				buildingTabs = false;
			}
		}

		private void ShowPlayerNameplate_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!(sender is CheckBox checkBox))
				return;
			if (!(checkBox.Tag is int playerID))
				return;

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
				checkBox.IsChecked = true;
				foreach (Character player in game.Players)
				{
					player.ShowingNameplate = player.playerID == playerID;
				}
			}
			else
			{
				Character singlePlayer = game.GetPlayerFromId(playerID);
				singlePlayer.ShowingNameplate = checkBox.IsChecked == true;
			}

			SendPlayerData();
		}

		private void CharacterSheets_ChargesChanged(object sender, ChargesChangedEventArgs e)
		{
			if (!(sender is CharacterSheets characterSheets))
				return;
			Character player = game.GetPlayerFromId(characterSheets.playerID);
			if (player == null)
				return;
			player.SetState(e.Key, e.NewValue);
		}

		private void CharacterSheets_SkillCheckConsidered(object sender, SkillCheckEventArgs e)
		{
			SelectSkill(e.Skill);
		}

		private void CharacterSheets_SavingThrowConsidered(object sender, AbilityEventArgs e)
		{
			SelectSavingThrowAbility(e.Ability);
		}

		private void CharacterSheets_SavingThrowRequested(object sender, AbilityEventArgs e)
		{
			SelectSavingThrowAbility(e.Ability);
			rbActivePlayer.IsChecked = true;
			SetActivePlayerVantageUI(e.VantageKind);
			RollTheDice(PrepareRoll(DiceRollType.SavingThrow));
		}

		void SelectSavingThrowAbility(Ability ability)
		{
			string lowerCaseAbility = ability.ToString().ToLower();
			for (int i = 0; i < cbAbility.Items.Count; i++)
			{
				if (cbAbility.Items[i] is ComboBoxItem item)
				{
					if (item.Content is string displayText)
					{
						string displayTextLower = displayText.ToLower();
						if (displayTextLower == lowerCaseAbility)
						{
							cbAbility.SelectedIndex = i;
							return;
						}
					}
				}
			}
		}

		void SelectSkill(Skills skill)
		{
			string lowerCaseSkillName = skill.ToString().ToLower();
			if (skill == Skills.animalHandling)
				lowerCaseSkillName = "animal handling";
			else if (skill == Skills.sleightOfHand)
				lowerCaseSkillName = "sleight of hand";
			for (int i = 0; i < cbSkillFilter.Items.Count; i++)
			{
				if (cbSkillFilter.Items[i] is ComboBoxItem item)
				{
					if (item.Content is string displayText)
					{
						string displayTextLower = displayText.ToLower();
						if (displayTextLower == lowerCaseSkillName)
						{
							cbSkillFilter.SelectedIndex = i;
							return;
						}
					}
				}
			}
			//cbSkillFilter.SelectedItem
		}

		private void CharacterSheets_SkillCheckRequested(object sender, SkillCheckEventArgs e)
		{
			SetActivePlayerVantageUI(e.VantageKind);
			List<int> playerIds = new List<int>();
			playerIds.Add(ActivePlayerId);
			InvokeSkillCheck(e.Skill, playerIds);
		}

		public void InvokeSkillCheck(Skills skill, List<int> playerIds)
		{
			Dispatcher.Invoke(() =>
			{
				SelectSkill(skill);
				SetRollScopeForPlayers(playerIds);
				ckbUseMagic.IsChecked = false;
				ResetActiveFields();
				RollTheDice(PrepareRoll(DiceRollType.SkillCheck));
			});
		}

		private void SetRollScopeForPlayers(List<int> playerIds)
		{
			if (IncludesAllPlayers(playerIds))
				rbEveryone.IsChecked = true;
			else if (playerIds.Count > 1)
			{
				rbIndividuals.IsChecked = true;
				// TODO: Check each individual identified by the player id's.
			}
			else
			{
				ActivePlayerId = playerIds[0];
				rbActivePlayer.IsChecked = true;
			}
		}

		private static bool IncludesAllPlayers(List<int> playerIds)
		{
			return playerIds == null || playerIds.Count == 0 || playerIds.First() == int.MaxValue;
		}

		public void InvokeSavingThrow(Ability ability, List<int> playerIds)
		{
			Dispatcher.Invoke(() =>
			{
				SelectSavingThrowAbility(ability);
				SetRollScopeForPlayers(playerIds);
				RollTheDice(PrepareRoll(DiceRollType.SavingThrow));
			});
		}

		void ReportLastInitiativeResults()
		{
			foreach (string initiativeResult in lastInitiativeResults)
			{
				TellDungeonMaster(initiativeResult);
			}
		}

		void EnterCombat()
		{
			Dispatcher.Invoke(() =>
			{
				if (game.Clock.InCombat)
				{
					TellDungeonMaster($"{Icons.WarningSign} Already in combat!");
					ReportLastInitiativeResults();
				}
				else
				{
					if (game.Clock.InTimeFreeze)
						game.Clock.InTimeFreeze = false;
					TellAll($"{Icons.EnteringCombat} Entering combat...");
					BtnEnterExitCombat_Click(null, null);
				}
			});
		}

		void EnterTimeFreeze()
		{
			Dispatcher.Invoke(() =>
			{
				if (game.Clock.InTimeFreeze)
				{
					TellDungeonMaster($"{Icons.WarningSign} Already in a time freeze!");
					ReportLastInitiativeResults();
				}
				else
				{
					TellAll($"Entering time freeze...");
					BtnEnterExitTimeFreeze_Click(null, null);
				}
			});
		}
		void ExitCombat()
		{
			Dispatcher.Invoke(() =>
			{
				if (!game.Clock.InCombat)
					TellDungeonMaster($"{Icons.WarningSign} Already NOT in combat!");
				else
				{
					TellAll($"{Icons.ExitCombat} Exiting combat...");
					BtnEnterExitCombat_Click(null, null);
				}
			});
		}

		void ExitTimeFreeze()
		{
			Dispatcher.Invoke(() =>
			{
				if (!game.Clock.InTimeFreeze)
					TellDungeonMaster($"{Icons.WarningSign} Already NOT in a time freeze.");
				else
				{
					TellAll($"{Icons.ExitCombat} Restarting the clock...");
					BtnEnterExitTimeFreeze_Click(null, null);
				}
			});
		}

		void RollNonCombatInitiative()
		{
			Dispatcher.Invoke(() =>
			{
				DiceRoll diceRoll = PrepareRoll(DiceRollType.NonCombatInitiative);
				RollTheDice(diceRoll);
			});
		}
		public void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand)
		{
			switch (dungeonMasterCommand)
			{
				case DungeonMasterCommand.ClearScrollEmphasis:
					HubtasticBaseStation.SendScrollLayerCommand("ClearHighlighting");
					TellDungeonMaster("Clearing emphasis...");
					break;
				case DungeonMasterCommand.NonCombatInitiative:
					RollNonCombatInitiative();
					TellDungeonMaster($"{Icons.NonCombatInitiative} Rolling non-combat initiative...");
					break;
				case DungeonMasterCommand.EnterCombat:
					EnterCombat();
					break;
				case DungeonMasterCommand.RestartClock:
					if (game.Clock.InTimeFreeze)
						ExitTimeFreeze();
					if (game.Clock.InCombat)
						ExitCombat();
					break;
				case DungeonMasterCommand.EnterTimeFreeze:
					EnterTimeFreeze();
					break;
			}
		}

		string PlusHiddenThresholdDisplayStr(TextBox textBox)
		{
			string returnMessage = string.Empty;
			Dispatcher.Invoke(() =>
			{
				returnMessage = $" (against a hidden threshold of {textBox.Text})";
			});

			return returnMessage;
		}

		CharacterSheets GetSheetForCharacter(int playerID)
		{
			foreach (TabItem tabItem in tabPlayers.Items)
			{
				if ((tabItem is PlayerTabItem playerTabItem) && (playerTabItem.PlayerId == playerID))
					return playerTabItem.CharacterSheets;
			}
			return null;
		}

		ListBox GetStateListForCharacter(int playerID)
		{
			foreach (PlayerTabItem playerTabItem in tabPlayers.Items)
			{
				if (playerTabItem.PlayerId == playerID)
					return playerTabItem.StateList;
			}
			return null;
		}

		public void RollSkillCheck(Skills skill, List<int> playerIds)
		{
			InvokeSkillCheck(skill, playerIds);

			if (activePage != ScrollPage.skills)
			{
				activePage = ScrollPage.skills;
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			}

			Dispatcher.Invoke(() =>
			{
				if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
					playerTabItem.CharacterSheets.FocusSkill(skill);
			});

			string articlePlusSkillDisplay = DndUtils.ToArticlePlusSkillDisplayString(skill);
			string who;
			string icon;
			if (IncludesAllPlayers(playerIds))
			{
				who = "all players";
				icon = Icons.SkillTest;
			}
			else
			{
				who = GetPlayerName(ActivePlayerId);
				icon = Icons.MultiplayerSkillCheck;
			}
			string firstPart = $"Rolling {articlePlusSkillDisplay} skill check for {who}";
			TellDungeonMaster($"{icon} {firstPart}{PlusHiddenThresholdDisplayStr(tbxSkillCheckThreshold)}.");
			TellViewers($"{firstPart}...");
		}

		public void RollAttack()
		{
			Dispatcher.Invoke(() =>
			{
				if (NextDieRollType == DiceRollType.None)
					NextDieRollType = DiceRollType.Attack;
				BtnRollDice_Click(null, null);
			});
			// TODO: Add report on advantage/disadvantage.
			TellDungeonMaster($"Rolling {GetPlayerName(ActivePlayerId)}'s attack with a hidden threshold of {tbxSaveThreshold.Text} and damage dice of {tbxDamageDice.Text}.");
		}

		public void RollSavingThrow(Ability ability, List<int> playerIds)
		{
			InvokeSavingThrow(ability, playerIds);

			if (activePage != ScrollPage.main)
			{
				activePage = ScrollPage.main;
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			}

			Dispatcher.Invoke(() =>
			{
				if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
					playerTabItem.CharacterSheets.FocusSavingAbility(ability);
			});

			string abilityStr = DndUtils.ToArticlePlusAbilityDisplayString(ability);

			string who;
			if (IncludesAllPlayers(playerIds))
				who = "all players";
			else
				who = GetPlayerName(ActivePlayerId);

			string firstPart = $"Rolling {abilityStr} saving throw for {who}";
			TellDungeonMaster($"{Icons.SavingThrow} {firstPart}{PlusHiddenThresholdDisplayStr(tbxSaveThreshold)}...");
			TellViewers($"{firstPart}...");
		}

		public void InstantDice(DiceRollType diceRollType, string dieStr, List<int> playerIds)
		{
			string who;
			if (IncludesAllPlayers(playerIds))
				who = "all players";
			else
				who = GetPlayerName(ActivePlayerId);

			TellAll($"Rolling {dieStr} for {who}...");

			Dispatcher.Invoke(() =>
			{
				SetRollTypeUI(diceRollType);
				SetRollScopeForPlayers(playerIds);
				DiceRoll diceRoll = PrepareRoll(diceRollType);
				if (diceRollType == DiceRollType.InspirationOnly)
					foreach (PlayerRollOptions playerRollOption in diceRoll.PlayerRollOptions)
					{
						playerRollOption.Inspiration = dieStr;
					}
				else
					diceRoll.DamageHealthExtraDice = dieStr;
				RollTheDice(diceRoll);
			});
		}

		private void CharacterSheets_PageBackgroundClicked(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.SendScrollLayerCommand("ClearHighlighting");
		}

		private void Button_ClearScrollClick(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.SendScrollLayerCommand("Close");
		}

		private void BtnInitializePlayerData_Click(object sender, RoutedEventArgs e)
		{
			List<MagicItem> magicItems = AllMagicItems.MagicItems;
			DateTime saveTime = game.Clock.Time;

			AllInGameCreatures.Invalidate();
			AllWeaponEffects.Invalidate();
			AllPlayers.Invalidate();
			AllSpells.Invalidate();
			AllSpellEffects.Invalidate();
			AllFeatures.Invalidate();
			AllDieRollEffects.Invalidate();
			AllTrailingEffects.Invalidate();
			PlayerActionShortcut.PrepareForCreation();
			AllMonsters.Invalidate();
			AllActionShortcuts.Invalidate();
			//PlayerFactory.BuildPlayers(players);

			game.Clock.TimeChanged -= DndTimeClock_TimeChanged;
			game.GetReadyToPlay();
			game.Clock.TimeChanged += DndTimeClock_TimeChanged;

			List<Character> players = AllPlayers.GetActive();

			foreach (Character player in players)
			{
				player.RebuildAllEvents();
				game.AddPlayer(player);
			}

			game.Clock.SetTime(saveTime);
			game.Start();
			SendPlayerData();

			BuildPlayerTabs();
			BuildPlayerUI();
			InitializeAttackShortcuts();
			lstAllSpells.ItemsSource = AllSpells.Spells;
			spAllMonsters.DataContext = AllMonsters.Monsters;
		}

		private void SendPlayerData()
		{
			game.PreparePlayersForSerialization();
			string playerData = JsonConvert.SerializeObject(game.Players);
			HubtasticBaseStation.SetPlayerData(playerData);
		}

		void SetGridPosition(UIElement control, int column, int row)
		{
			Grid.SetColumn(control, column);
			Grid.SetRow(control, row);
		}

		void BuildPlayerUI()
		{
			grdPlayerRollOptions.Children.Clear();
			grdPlayerRollOptions.RowDefinitions.Clear();
			int row = 0;

			foreach (Character player in game.Players)
			{
				int playerId = player.playerID;
				grdPlayerRollOptions.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

				PlayerRollCheckBox checkBox = new PlayerRollCheckBox();
				checkBox.Content = DndUtils.GetFirstName(player.name);
				checkBox.PlayerId = playerId;
				checkBox.Checked += PlayerRollCheckBox_Checked;
				checkBox.Unchecked += PlayerRollCheckBox_Unchecked;
				SetGridPosition(checkBox, 0, row);

				StackPanel spOptions = new StackPanel();
				spOptions.Visibility = Visibility.Hidden;
				checkBox.DependantUI = spOptions;
				spOptions.Orientation = Orientation.Horizontal;
				spOptions.Margin = new Thickness(14, 0, 0, 0);
				SetGridPosition(spOptions, 1, row);

				RadioButton rbNormalRoll = new RadioButton();
				rbNormalRoll.Content = "Normal";
				rbNormalRoll.IsChecked = true;
				rbNormalRoll.Tag = playerId;
				rbNormalRoll.Checked += RbNormalRoll_Checked;
				spOptions.Children.Add(rbNormalRoll);
				checkBox.RbNormal = rbNormalRoll;

				RadioButton rbAdvantageRoll = new RadioButton();
				rbAdvantageRoll.Content = "Adv.";
				rbAdvantageRoll.Margin = new Thickness(14, 0, 0, 0);
				rbAdvantageRoll.Tag = playerId;
				rbAdvantageRoll.Checked += RbAdvantageRoll_Checked;
				spOptions.Children.Add(rbAdvantageRoll);
				checkBox.RbAdvantage = rbAdvantageRoll;

				RadioButton rbDisadvantageRoll = new RadioButton();
				rbDisadvantageRoll.Content = "Disadv.";
				rbDisadvantageRoll.Margin = new Thickness(14, 0, 0, 0);
				rbDisadvantageRoll.Tag = playerId;
				rbDisadvantageRoll.Checked += RbDisadvantageRoll_Checked;
				spOptions.Children.Add(rbDisadvantageRoll);
				checkBox.RbDisadvantage = rbDisadvantageRoll;

				TextBlock textBlock = new TextBlock();
				textBlock.Margin = new Thickness(14, 0, 0, 0);
				textBlock.Text = "Inspiration: ";
				spOptions.Children.Add(textBlock);

				InspirationTextBox inspirationTextBox = new InspirationTextBox();
				inspirationTextBox.Text = "";
				inspirationTextBox.MinWidth = 70;
				inspirationTextBox.TextChanged += InspirationTextBox_TextChanged;
				spOptions.Children.Add(inspirationTextBox);
				checkBox.TbxInspiration = inspirationTextBox;

				PlayerButton btnRollInspiration = new PlayerButton();
				btnRollInspiration.Content = "Roll Inspiration";
				btnRollInspiration.IsEnabled = false;
				btnRollInspiration.Margin = new Thickness(14, 0, 0, 0);
				btnRollInspiration.Padding = new Thickness(8, 0, 8, 0);
				btnRollInspiration.Click += ButtonRollInspirationOnly_Click;
				btnRollInspiration.PlayerId = playerId;
				spOptions.Children.Add(btnRollInspiration);
				inspirationTextBox.PlayerButton = btnRollInspiration;

				row++;
				grdPlayerRollOptions.Children.Add(checkBox);
				grdPlayerRollOptions.Children.Add(spOptions);

				playerId++;
			}
		}

		private void RbNormalRoll_Checked(object sender, RoutedEventArgs e)
		{
			if (!(sender is RadioButton radioButton))
				return;
			int playerID = (int)radioButton.Tag;
			Character player = game.GetPlayerFromId(playerID);
			if (player == null)
				return;
			player.advantageDiceThisRoll = 0;
			player.disadvantageDiceThisRoll = 0;
			UpdateAskUI(player);
			SetShortcutVisibility();
		}

		private void RbDisadvantageRoll_Checked(object sender, RoutedEventArgs e)
		{
			if (!(sender is RadioButton radioButton))
				return;
			int playerID = (int)radioButton.Tag;
			Character player = game.GetPlayerFromId(playerID);
			if (player == null)
				return;
			player.advantageDiceThisRoll = 0;
			player.disadvantageDiceThisRoll = 1;
			UpdateAskUI(player);
			SetShortcutVisibility();
		}

		private void RbAdvantageRoll_Checked(object sender, RoutedEventArgs e)
		{
			if (!(sender is RadioButton radioButton))
				return;
			int playerID = (int)radioButton.Tag;
			Character player = game.GetPlayerFromId(playerID);
			if (player == null)
				return;
			player.advantageDiceThisRoll = 1;
			player.disadvantageDiceThisRoll = 0;
			UpdateAskUI(player);
			SetShortcutVisibility();
		}

		private void InspirationTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			InspirationTextBox inspirationTextBox = sender as InspirationTextBox;
			if (inspirationTextBox != null)
			{
				inspirationTextBox.PlayerButton.IsEnabled = !string.IsNullOrWhiteSpace(inspirationTextBox.Text);
			}
		}

		private void ButtonRollInspirationOnly_Click(object sender, RoutedEventArgs e)
		{
			PlayerButton playerButton = sender as PlayerButton;
			if (playerButton != null)
			{
				CheckPlayer(playerButton.PlayerId);
				RollTheDice(PrepareRoll(DiceRollType.InspirationOnly));
			}

			// TODO: Roll only for the player to the left of this button.
		}

		int lastPlayerIdUnchecked;
		private void PlayerRollCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			PlayerRollCheckBox playerRollCheckBox = sender as PlayerRollCheckBox;
			if (playerRollCheckBox == null)
				return;
			playerRollCheckBox.DependantUI.Visibility = Visibility.Hidden;

			lastPlayerIdUnchecked = playerRollCheckBox.PlayerId;

			if (checkingInternally)
				return;
			SelectRadioButtonBasedOnCheckedPlayers();
		}

		private void PlayerRollCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			PlayerRollCheckBox playerRollCheckBox = sender as PlayerRollCheckBox;
			if (playerRollCheckBox == null)
				return;
			playerRollCheckBox.DependantUI.Visibility = Visibility.Visible;
			lastPlayerIdUnchecked = -1;

			if (checkingInternally)
				return;
			SelectRadioButtonBasedOnCheckedPlayers();
		}

		bool radioingInternally;
		void SelectRadioButtonBasedOnCheckedPlayers()
		{
			radioingInternally = true;
			try
			{
				bool allCheckboxesChecked = true;
				int numRadioBoxesChecked = 0;
				int lastPlayerId = -1;
				foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				{
					if (uIElement is PlayerRollCheckBox checkbox)
						if (checkbox.IsChecked == true)
						{
							numRadioBoxesChecked++;
							lastPlayerId = checkbox.PlayerId;
						}
						else
						{
							allCheckboxesChecked = false;
						}
				}
				if (allCheckboxesChecked)
					rbEveryone.IsChecked = true;
				else if (numRadioBoxesChecked == 0)
				{
					CheckActivePlayer();
					rbActivePlayer.IsChecked = true;
				}
				else if (numRadioBoxesChecked == 1 && lastPlayerId == ActivePlayerId)
					rbActivePlayer.IsChecked = true;
				else
					rbIndividuals.IsChecked = true;
			}
			finally
			{
				radioingInternally = false;
			}
		}
		void CheckActivePlayer()
		{
			CheckPlayer(ActivePlayerId);
		}

		private void CheckPlayer(int playerID)
		{
			if (lastPlayerIdUnchecked == playerID)
				return;
			if (grdPlayerRollOptions == null)
				return;
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				if (uIElement is PlayerRollCheckBox checkbox)
					checkbox.IsChecked = checkbox.PlayerId == playerID;
		}

		bool buildingTabs;

		private void ClearHistoryLog_Click(object sender, RoutedEventArgs e)
		{
			History.Clear();
		}

		private void CbAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void CbSkillFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (rbActivePlayer.IsChecked == true)
			{
				//PlayerID;
				//tbxModifier.Text = ;
			}
		}

		private void BtnApplyHealth_Click(object sender, RoutedEventArgs e)
		{
			ChangePlayerHealth(tbxHealth, +1);
		}

		private void BtnInflictDamage_Click(object sender, RoutedEventArgs e)
		{
			ChangePlayerHealth(tbxDamage, -1);
		}

		public int GetPlayerIdFromName(string characterName)
		{
			return AllPlayers.GetPlayerIdFromName(game.Players, characterName);
		}

		public int GetActivePlayerId()
		{
			return ActivePlayerId;
		}

		private void ChangePlayerHealth(TextBox textBox, int multiplier)
		{
			DamageHealthChange damageHealthChange = GetDamageHealthChange(multiplier, textBox);

			if (damageHealthChange != null)
			{
				ApplyDamageHealthChange(damageHealthChange);
				//string serializedObject = JsonConvert.SerializeObject(damageHealthChange);
				//HubtasticBaseStation.ChangePlayerHealth(serializedObject);
			}
		}

		void UpdatePlayerScrollOnStream(Character player)
		{
			if (player == null)
				return;
			//if (ActivePlayerId == player.playerID)
			HubtasticBaseStation.PlayerDataChanged(player.playerID, player.ToJson());
		}

		List<int> GetAllPlayerIds()
		{
			List<int> results = new List<int>();
			foreach (Character player in game.Players)
			{
				results.Add(player.playerID);
			}
			return results;
		}

		public void ApplyDamageHealthChange(DamageHealthChange damageHealthChange)
		{
			if (damageHealthChange == null)
				return;
			string playerNames = string.Empty;

			if (damageHealthChange.PlayerIds.Count == 1 && damageHealthChange.PlayerIds[0] == int.MaxValue)
				damageHealthChange.PlayerIds = GetAllPlayerIds();

			int numPlayers = damageHealthChange.PlayerIds.Count;
			for (int i = 0; i < numPlayers; i++)
			{
				int playerId = damageHealthChange.PlayerIds[i];
				Character player = GetPlayer(playerId);
				if (player == null)
					continue;
				string firstName = DndUtils.GetFirstName(player.name);
				if (i < numPlayers - 2)
					playerNames += firstName + ", ";
				else if (i < numPlayers - 1)
					playerNames += firstName + ", and ";
				else
					playerNames += firstName;

				if (damageHealthChange.IsTempHitPoints)
					player.ChangeTempHP(damageHealthChange.DamageHealth);
				else
					player.ChangeHealth(damageHealthChange.DamageHealth);
				UpdatePlayerScrollOnStream(player);
			}

			HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));
			string message;
			if (damageHealthChange.DamageHealth < 0)
				message = $"{Icons.TakesDamage} {-damageHealthChange.DamageHealth} points of damage dealt to {playerNames}.";
			else
				message = $"{Icons.GainsHealth} {damageHealthChange.DamageHealth} points of healing given to {playerNames}.";

			TellDungeonMaster(message);
			TellViewers(message);
		}

		private DamageHealthChange GetDamageHealthChange(int multiplier, TextBox textBox)
		{
			if (textBox == null)
				return null;
			DamageHealthChange damageHealthChange;
			int damage;

			if (int.TryParse(textBox.Text, out damage))
			{
				damageHealthChange = new DamageHealthChange();
				damageHealthChange.DamageHealth = damage * multiplier;
				AddPlayerIds(damageHealthChange.PlayerIds);
			}
			else
				damageHealthChange = null;
			return damageHealthChange;
		}

		private void AddPlayerIds(List<int> playerIds)
		{
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				if (uIElement is PlayerRollCheckBox checkbox && checkbox.IsChecked == true)
					playerIds.Add(checkbox.PlayerId);

			if (playerIds.Count == 0)
			{
				playerIds.Add(ActivePlayerId);
			}
		}

		private void RbActivePlayer_Checked(object sender, RoutedEventArgs e)
		{
			if (radioingInternally)
				return;
			//if (spRollButtons == null)
			//	return;
			//spRollButtons.Visibility = Visibility.Visible;
			ShowHidePlayerUI(true);
			CheckActivePlayer();
		}

		private void RbEveryone_Checked(object sender, RoutedEventArgs e)
		{
			if (radioingInternally)
				return;
			//if (spRollButtons == null)
			//	return;
			//spRollButtons.Visibility = Visibility.Collapsed;
			ShowHidePlayerUI(true);
			CheckAllPlayers();
		}

		bool checkingInternally;
		string lastScenePlayed;
		PlayerActionShortcut shortcutToActivateAfterClearingDice;
		DiceRoll lastRoll;
		DiceRoll secondToLastRoll;
		DateTime lastChatMessageSent;
		bool waitingForAnswerToQuestion;
		void CheckAllPlayers()
		{
			checkingInternally = true;
			try
			{
				foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				{
					if (uIElement is PlayerRollCheckBox checkbox)
						checkbox.IsChecked = true;
				}
			}
			finally
			{
				checkingInternally = false;
			}
		}

		private void RbIndividuals_Checked(object sender, RoutedEventArgs e)
		{
			if (radioingInternally)
				return;
			//if (spRollButtons == null)
			//	return;
			//spRollButtons.Visibility = Visibility.Collapsed;
			ShowHidePlayerUI(true);
		}

		void ShowHidePlayerUI(bool showUI)
		{
			if (grdPlayerRollOptions == null)
				return;
			if (showUI)
				grdPlayerRollOptions.Visibility = Visibility.Visible;
			else
				grdPlayerRollOptions.Visibility = Visibility.Hidden;
		}

		private void CbDamage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void BtnLongtoothShiftingStrike_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				EffectType = "Fangs",
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 120,  // 120 + Random.plusMinus(30)
			});
			diceRoll.OnFirstContactSound = "Snarl";
			//diceRoll.OnFirstContactEffect = TrailingSpriteType.Fangs;
			RollTheDice(diceRoll);
		}

		public void RollWildMagicCheck()
		{
			SelectCharacter(PlayerID.Merkin);
			RollTheDice(PrepareRoll(DiceRollType.WildMagicD20Check));
			TellDungeonMaster("Rolling wild magic check.");
		}

		string GetNopeMessage()
		{
			int rand = new Random((int)game.Clock.Time.Ticks).Next(10);
			switch (rand)
			{
				case 0:
					return "Nope";
				case 1:
					return "No way";
				case 2:
					return "Sorry";
				case 3:
					return "No can do";
				case 4:
					return "Try again";
				case 5:
					return "Yeah... uh NO";
				case 6:
					return "That's a negative";
				case 7:
					return "No way padre";
				case 8:
					return "Can't do that";
				case 9:
					return "Impossible";
			}
			return "Error";
		}

		void ReportOnConcentration()
		{
			string concentrationReport = game.GetConcentrationReport();

			if (string.IsNullOrWhiteSpace(concentrationReport))
				TellDungeonMaster("No players are concentrating on any spells at this time.");
			else
				TellDungeonMaster(concentrationReport);
		}

		public void GetData(string dataId)
		{
			Dispatcher.Invoke(() =>
			{
				if (dataId == "Concentration")
					ReportOnConcentration();
			});
		}

		public void SetVantage(VantageKind vantageKind, int playerId)
		{
			Dispatcher.Invoke(() =>
			{
				SetPlayerVantageUI(vantageKind, playerId);
			});
		}

		public void SelectPlayerShortcut(string shortcutName, int playerId)
		{
			Dispatcher.Invoke(() =>
			{
				ActivePlayerId = playerId;
				PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText == shortcutName && x.PlayerId == playerId && x.Available);
				if (shortcut != null)
				{
					ActivateShortcut(shortcut);
					TellDungeonMaster($"Activated {GetPlayerName(playerId)}'s {shortcutName}.");
				}
			});

		}

		public void SelectCharacter(int playerId)
		{
			Dispatcher.Invoke(() =>
			{
				if (tabPlayers.Items.Count > 0 && tabPlayers.Items[0] is PlayerTabItem)
					foreach (PlayerTabItem playerTabItem in tabPlayers.Items)
					{
						if (playerTabItem.PlayerId == playerId)
						{
							tabPlayers.SelectedItem = playerTabItem;
							//TellDmActivePlayer(playerId);
							break;
						}
					}
			});
		}

		private void TellDmActivePlayer(int playerId)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			TellDungeonMaster($"{twitchIndent}");
			TellDungeonMaster($"{player.emoticon} {twitchIndent} ----- {GetPlayerName(playerId)} -----");
		}

		public void RollWildMagic()
		{
			DiceRoll diceRoll = new DiceRoll(DiceRollType.WildMagic);
			diceRoll.Modifier = 0;
			diceRoll.HiddenThreshold = 0;
			diceRoll.IsMagic = true;
			diceRoll.OnThrowSound = "WildMagicRoll";
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				EffectType = "SparkTrail",
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 84
			});

			RollTheDice(diceRoll);
			TellDungeonMaster("Rolling wild magic...");
		}

		private void BtnSendWindup_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnClearWindups_Click(object sender, RoutedEventArgs e)
		{

		}

		public void SetClock(int hours, int minutes, int seconds)
		{
			Dispatcher.Invoke(() =>
			{

			});
			// TODO: Tell DM
		}

		public void AdvanceClock(int hours, int minutes, int seconds)
		{
			if (hours == 0 && minutes == 0 && seconds == 0)
				return;
			// TODO: Calculate clockMessage based on the delta here.
			Dispatcher.Invoke(() =>
			{
				game.Clock.Advance(DndTimeSpan.FromSeconds(seconds + minutes * 60 + hours * 3600), Modifiers.ShiftDown);
			});
		}

		public void AdvanceDate(int days, int months, int years)
		{
			if (days == 0 && months == 0 && years == 0)
				return;
			Dispatcher.Invoke(() =>
			{
				game.Clock.Advance(DndTimeSpan.FromDays(days + months * 30 + years * 365), Modifiers.ShiftDown);
			});
		}


		public void RollDice()
		{
			Dispatcher.Invoke(() =>
			{
				UnleashTheNextRoll();
			});
		}

		public void HideScroll()
		{
			HubtasticBaseStation.SendScrollLayerCommand("Close");
			TellDungeonMaster("Closing the scroll...", true);
		}

		public void DropWindup()
		{
			HubtasticBaseStation.ClearWindup("Weapon.*");
			TellDungeonMaster("Dropping windups...");
		}

		public void MoveFred(string movement)
		{
			HubtasticBaseStation.MoveFred(movement);
		}

		public void PlayScene(string sceneName)
		{
			lastScenePlayed = sceneName;
			string dmMessage = $"Playing scene: {sceneName}";

			try
			{
				obsWebsocket.SetCurrentScene(sceneName);
			}
			catch (Exception ex)
			{
				dmMessage = $"{Icons.WarningSign} Unable to play {sceneName}: {ex.Message}";
			}
			TellDungeonMaster(dmMessage);
		}

		public void PlaySound(string soundFileName)
		{
			HubtasticBaseStation.PlaySound(soundFileName);
		}

		public void Speak(int playerId, string message)
		{
			Dispatcher.Invoke(() =>
			{
				// TODO: Implement this.
			});
			// TODO: Tell DM
		}

		public void TellDungeonMaster(string message, bool isDetail = false)
		{
			if (dungeonMasterClient == null)
				return;

			History.Log(message);
			SendMessage(message, DungeonMasterChannel);
		}

		private void SendMessage(string message, string channel)
		{
			History.Log($"Sending \"{message}\" to {channel} at {DateTime.Now.ToLongTimeString()}");
			if (JoinedChannel(channel))
				dungeonMasterClient.SendMessage(channel, message);
			else
			{
				try
				{
					dungeonMasterClient.JoinChannel(channel);
					dungeonMasterClient.SendMessage(channel, message);
				}
				catch (TwitchLib.Client.Exceptions.ClientNotConnectedException)
				{
					CreateDungeonMasterClient();
					try
					{
						if (dungeonMasterClient == null)
							return;
						dungeonMasterClient.JoinChannel(channel);
						dungeonMasterClient.SendMessage(channel, message);
					}
					catch (Exception ex)
					{

					}
				}
				catch (Exception ex)
				{

				}
			}
		}

		/* Potentially problematic, or was this my attempt to fix a disconnect issue that existed before this change?
		 
		private void SendMessage(string message, string channel)
		{
			if (JoinedChannel(channel))
				try
				{
					dungeonMasterClient.SendMessage(channel, message);
				}
				catch (Exception ex)
				{
					CreateClientJoinChannelAndSendMessage(channel, message);
				}
			else
			{
				try
				{
					dungeonMasterClient.JoinChannel(channel);
					dungeonMasterClient.SendMessage(channel, message);
				}
				catch (TwitchLib.Client.Exceptions.ClientNotConnectedException)
				{
					CreateClientJoinChannelAndSendMessage(channel, message);
				}
				catch (Exception ex)
				{

				}
			}
		}

		private void CreateClientJoinChannelAndSendMessage(string channel, string message)
		{
			CreateDungeonMasterClient();
			try
			{
				if (dungeonMasterClient != null)
				{
					dungeonMasterClient.JoinChannel(channel);
					dungeonMasterClient.SendMessage(channel, message);
				}
			}
			catch (Exception ex)
			{

			}
		}
			 
			 
			 
			 
			 
			 */

		public void TellViewers(string message)
		{
			SendMessage(message, DragonHumpersChannel);
		}

		public void TellCodeRushed(string message)
		{
			SendMessage(message, CodeRushedChannel);
		}

		public void TellAll(string message)
		{
			TellDungeonMaster(message);
			TellViewers(message);
			if (System.Diagnostics.Debugger.IsAttached && !suppressMessagesToCodeRushedChannel)
			{
				TellCodeRushed(message);
			}
		}

		void ActivatePendingShortcuts(object sender, EventArgs e)
		{
			pendingShortcutsTimer.Stop();
			ActivatePendingShortcuts();
		}

		private void BtnClearSpell_Click(object sender, RoutedEventArgs e)
		{
			ClearPlayerSpell();
		}

		private void ClearPlayerSpell()
		{
			Character player = game.GetPlayerFromId(ActivePlayerId);
			if (player == null)
				return;
			player.ClearPreviouslyCastingSpell();
			SetClearSpellVisibility(player);
		}

		int lastSpellSlotTested;
		FileSystemWatcher spellsChangedFileWatcher;
		private void ShowPlayerCasting()
		{
			Character player = game.GetPlayerFromId(ActivePlayerId);
			if (player == null)
				return;
			SpellDto selectedItem = (SpellDto)lstAllSpells.SelectedItem;
			if (selectedItem == null)
				return;
			Spell spell = AllSpells.Get(selectedItem.name, player, lastSpellSlotTested);
			player.PrepareSpell(new CastedSpell(spell, player));
		}

		private void LstAllSpells_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ShowActiveSpell();
		}

		private void ShowActiveSpell()
		{
			SpellDto selectedItem = (SpellDto)lstAllSpells.SelectedItem;
			if (selectedItem == null)
				return;
			int level;
			int.TryParse(selectedItem.level, out level);
			btnSlot1.IsEnabled = level <= 1;
			btnSlot2.IsEnabled = level <= 2;
			btnSlot3.IsEnabled = level <= 3;
			btnSlot4.IsEnabled = level <= 4;
			btnSlot5.IsEnabled = level <= 5;
			btnSlot6.IsEnabled = level <= 6;
			btnSlot7.IsEnabled = level <= 7;
			btnSlot8.IsEnabled = level <= 8;
			btnSlot9.IsEnabled = level <= 9;
			ShowPlayerCasting();
		}

		private void BtnReloadSpells_Click(object sender, RoutedEventArgs e)
		{
			ReloadSpells();
		}

		private void ReloadSpells()
		{
			AllSpells.Invalidate();
			lstAllSpells.ItemsSource = AllSpells.Spells;
		}

		private void BtnSlotTest_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				return;
			if (!int.TryParse((string)button.Tag, out lastSpellSlotTested))
				return;
			ShowPlayerCasting();
		}

		private void CkBreakTest_Checked(object sender, RoutedEventArgs e)
		{
			Expressions.Debugging = true;
		}

		private void CkBreakTest_Unchecked(object sender, RoutedEventArgs e)
		{
			Expressions.Debugging = false;
		}

		private void BtnDebugStep_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnDebugRun_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnDebugTest_Click(object sender, RoutedEventArgs e)
		{
			if (activeEventData == null)
				return;
			try
			{
				TestCastSpell(activeEventData.ParentGroup.Name);
				UnleashTheNextRoll();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debugger.Break();
			}
		}

		private void LstKnownSpellsAndFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EventData selectedItem = lstEvents.SelectedItem as EventData;
			if (e.AddedItems != null && e.AddedItems.Count > 0)
				if (e.AddedItems[0] is EventGroup eventGroup)
				{
					activeEventGroup = eventGroup;
					lstEvents.ItemsSource = eventGroup.Events;
					if (selectedItem != null)
						lstEvents.SelectedItem = eventGroup.Events.FirstOrDefault(x => x.Name == selectedItem.Name);
				}
		}

		List<DebugLine> GetDebugLines(string code)
		{
			List<DebugLine> debugLines = new List<DebugLine>();
			if (code == null)
				return debugLines;
			string[] splitLines = code.Split('\n');
			foreach (string line in splitLines)
			{
				debugLines.Add(new DebugLine(line));
			}

			return debugLines;
		}

		private static string GetPlayerEventCode(EventGroup parentGroup, string name)
		{
			string groupName = parentGroup.Name;
			object instance = null;

			if (parentGroup.Type == EventType.FeatureEvents)
				instance = AllFeatures.Get(groupName);
			else if (parentGroup.Type == EventType.SpellEvents)
				instance = AllSpells.Get(groupName);

			if (instance == null)
				return null;
			object value = instance.GetType().GetProperty(name).GetValue(instance, null);
			if (!(value is string code))
				return null;

			return code;
		}

		EventData activeEventData;
		void UpdateTheUpdateButton(EventData eventData)
		{
			btnUpdateEventHandler.Content = $"Update \"{eventData.ParentGroup.Name}\" {eventData.Name} handler";
		}

		private void LstFeatureEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems != null && e.AddedItems.Count > 0)
				if (e.AddedItems[0] is EventData eventData)
				{
					activeEventData = eventData;
					UpdateTheUpdateButton(activeEventData);
					string code = GetPlayerEventCode(eventData.ParentGroup, eventData.Name);
					SetEventCode(code);

					// TODO: Remove lstCode and related if no longer used:
					List<DebugLine> debugLines = GetDebugLines(code);
					lstCode.ItemsSource = debugLines;
				}
		}

		private void SetEventCode(string code)
		{
			changingInternally = true;
			try
			{
				HideAllCodeChangedStatusUI();
				TextCompletionEngine.SetText(code);
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void BtnRollDice_Click(object sender, RoutedEventArgs e)
		{
			UnleashTheNextRoll();
		}

		private void UnleashTheNextRoll()
		{
			if (spellToCastOnRoll != null)
			{
				ActivateSpellShortcut(spellToCastOnRoll);
				bool isSimpleSpell = spellToCastOnRoll.Type == DiceRollType.CastSimpleSpell;
				spellToCastOnRoll = null;
				if (isSimpleSpell)
				{
					btnRollDice.IsEnabled = false;
					return;  // No need to roll the dice
				}
			}

			if (NextDieRollType != DiceRollType.None)
			{
				RollTheDice(PrepareRoll(NextDieRollType));
				NextDieRollType = DiceRollType.None;
			}
		}

		private void RbSkillCheck_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.SkillCheck;
			btnRollDice.Content = "Roll Skill Check";
		}

		private void RbSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.SkillCheck;
			btnRollDice.Content = "Roll Saving Throw";
		}

		private void RbWildMagicD20Check_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.WildMagicD20Check;
			btnRollDice.Content = "Check Wild Magic";
		}

		private void RbFlatD20_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.FlatD20;
			btnRollDice.Content = "Roll d20";
		}

		private void RbAttack_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.Attack;
			btnRollDice.Content = "Roll Attack";
		}

		private void RbDeathSavingThrow_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.DeathSavingThrow;
			btnRollDice.Content = "Roll Death Save";
		}

		private void RbInspirationOnly_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.InspirationOnly;
			btnRollDice.Content = "Roll Inspiration";
		}

		private void RbHitPointCapacity_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.HPCapacity;
			btnRollDice.Content = "Roll HP Capacity";
		}

		private void RbHealth_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.HealthOnly;
			btnRollDice.Content = "Roll Health";
		}

		private void RbDamageOnly_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.DamageOnly;
			btnRollDice.Content = "Roll Damage";
		}

		private void RbPercentageRoll_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.PercentageRoll;
			btnRollDice.Content = "Roll Percentage";
		}

		private void RbWildMagic_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.WildMagic;
			btnRollDice.Content = "Roll Wild Magic";
		}

		private void RbInitiative_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.Initiative;
			btnRollDice.Content = "Roll Initiative";
		}

		private void RbNonCombatInitiative_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.NonCombatInitiative;
			btnRollDice.Content = "Roll Non-combat Initiative";
		}

		private void RbBendLuckAdd_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.BendLuckAdd;
			btnRollDice.Content = "Bend Luck Up (+)";
		}

		private void RbBendLuckSubtract_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.BendLuckSubtract;
			btnRollDice.Content = "Bend Luck Down (-)";
		}

		private void RbLuckRollHigh_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.LuckRollHigh;
			btnRollDice.Content = "Lucky Roll High";
		}

		private void RbLuckRollLow_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.LuckRollLow;
			btnRollDice.Content = "Lucky Roll Low";
		}

		private void RbExtra_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.ExtraOnly;
			btnRollDice.Content = "Roll Extra";
		}

		private void RbChaosBolt_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.ChaosBolt;
			btnRollDice.Content = "Roll Chaos Bolt";
		}

		private void DamageContextTestAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.DamageOnly);
			diceRoll.DamageHealthExtraDice = "1d12(fire),1d12(psychic),1d12(acid),1d12(cold),1d12(force),1d12(necrotic),1d12(piercing),1d12(bludgeoning),1d12(slashing),1d12(lightning),1d12(radiant),1d12(thunder)";
			RollTheDice(diceRoll);
			// 1d12(superiority),
		}

		EventGroup activeEventGroup;

		//private void LstAssignedFeatures_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		//{
		//	if (lstAssignedFeatures.SelectedItem is EventGroup eventGroup)
		//	{
		//		activeEventGroup = eventGroup;
		//		lstEvents.ItemsSource = eventGroup.Events;
		//	}
		//}

		private void LstKnownSpellsAndFeatures_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (lstKnownSpellsAndFeatures.SelectedItem is EventGroup eventGroup)
			{
				activeEventGroup = eventGroup;
				lstEvents.ItemsSource = eventGroup.Events;
				//btnRepeatLastCast.Content = $"Cast {eventGroup.Name}";
			}
		}

		private void SetEventCode(EventGroup parentGroup, string name, string code)
		{
			string groupName = parentGroup.Name;
			object instance = null;

			if (parentGroup.Type == EventType.FeatureEvents)
			{
				instance = AllFeatures.Get(groupName);
				SetValue(instance, name, code);
			}
			else if (parentGroup.Type == EventType.SpellEvents)
			{
				instance = AllSpells.GetDto(groupName);
				string lowerEventName = Char.ToLower(name[0]) + name.Substring(1); // SpellDto events start with a lower case letter.
				SetValue(instance, lowerEventName, code);

				List<Spell> matchingSpells = AllSpells.GetAll(groupName, ActivePlayer);
				if (matchingSpells.Count == 1)
					SetValue(matchingSpells[0], name, code);
			}



			//if (parentGroup.Type == EventType.FeatureEvents)
			//	
			//else if (parentGroup.Type == EventType.SpellEvents)
			//	
		}

		private static void SetValue(object instance, string name, object value)
		{
			if (instance == null)
				return;

			instance.GetType().GetProperty(name).SetValue(instance, value);
		}

		void UpdateSelectedEvent(string text)
		{
			if (activeEventData == null)
				return;
			SetEventCode(activeEventData.ParentGroup, activeEventData.Name, text);
		}

		PlayerActionShortcut spellToCastOnRoll;
		private void BtnRepeatLastCast_Click(object sender, RoutedEventArgs e)
		{
			const string CastPrefix = "Cast ";
			string buttonLabel = btnRepeatLastCast.Content as string;
			if (!buttonLabel.StartsWith(CastPrefix))
				return;
			string spellName = buttonLabel.Substring(CastPrefix.Length);
			if (string.IsNullOrWhiteSpace(spellName))
				return;
			TestCastSpell(spellName);
		}

		void TestCastSpell(string spellName)
		{
			ActivateShortcut(spellName);
		}

		void UpdateSpellEvents(Spell spell)
		{
			Spell latestSpell = AllSpells.Get(spell.Name);
			if (latestSpell == null)
				return;
			spell.OnCast = latestSpell.OnCast;
			spell.OnReceived = latestSpell.OnReceived;
			spell.OnCasting = latestSpell.OnCasting;
			spell.OnGetAttackAbility = latestSpell.OnGetAttackAbility;
			spell.OnDispel = latestSpell.OnDispel;
			spell.OnPlayerPreparesAttack = latestSpell.OnPlayerPreparesAttack;
			spell.OnDieRollStopped = latestSpell.OnDieRollStopped;
			spell.OnPlayerAttacks = latestSpell.OnPlayerAttacks;
			spell.OnPlayerHitsTarget = latestSpell.OnPlayerHitsTarget;
		}

		private void RbCastOtherSpell_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.CastSimpleSpell;
			btnRollDice.Content = "Cast Spell";
		}

		private void BtnTestShowSprinkles_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Appear Behind");
		}

		private void BtnTestHideSprinkles_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Hide Sprinkles");
		}

		private void BtnTestWalkLeft_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Walk Left");
		}

		private void BtnTestWalkRight_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Walk Right");
		}

		private void BtnSprinkleBlast_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("SprinklesBlast");
		}

		private void BtnStandIdle_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Idle");
		}

		private void BtnThreatened_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Threatened");
		}

		private void BtnCry_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("SprinklesTears");
		}

		private void BtnBabyEyes_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("BabyEyes");
		}

		private void BtnFart_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Fart");
		}

		private void BtnScoopAttack_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("ScoopAttack");
		}

		private void BtnStabAttack_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("StabAttack");
		}

		private void BtnPushUpAttack_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("PushUpAttack");
		}

		private void BtnDies_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Dies");
		}

		private void BtnFlip_Click(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.AnimateSprinkles("Flip");
		}

		private void BtnApplyTempHp_Click(object sender, RoutedEventArgs e)
		{
			DamageHealthChange damageHealthChange = GetDamageHealthChange(+1, tbxTempHp);

			if (damageHealthChange != null)
			{
				damageHealthChange.IsTempHitPoints = true;
				ApplyDamageHealthChange(damageHealthChange);
			}
		}

		private void BtnChangeWealth_Click(object sender, RoutedEventArgs e)
		{
			//game.Clock.SetTime(game.Clock.Time - TimeSpan.FromDays(365 * 900));
			WealthChange wealthChange = new WealthChange();
			wealthChange.Coins.SetFromText(tbxWealthDelta.Text);
			AddPlayerIds(wealthChange.PlayerIds);
			HubtasticBaseStation.ChangePlayerWealth(JsonConvert.SerializeObject(wealthChange));
			game.ChangeWealth(wealthChange);
		}

		int GetFrameRate(object sender)
		{
			if (sender is RadioButton radioButton)
				if (radioButton.Content != null && radioButton.Content is string rbText)
					if (int.TryParse(rbText, out int result))
						return result;
			return 30;
		}

		private void RbnFrontFpsChange_Click(object sender, RoutedEventArgs e)
		{
			if (changingFrameRateInternally)
				return;
			ChangeFps(sender, Overlays.Front);
		}

		bool changingFrameRateInternally;

		private void RbnBackFpsChange_Click(object sender, RoutedEventArgs e)
		{
			if (changingFrameRateInternally)
				return;
			ChangeFps(sender, Overlays.Back);
		}

		private void RbnDiceFpsChange_Click(object sender, RoutedEventArgs e)
		{
			if (changingFrameRateInternally)
				return;
			ChangeFps(sender, Overlays.Dice);
		}

		private void ChangeFps(object sender, string overlayName)
		{
			ChangeFrameRate(overlayName, GetFrameRate(sender));
		}

		private void ChangeFrameRate(string overlayName, int frameRate)
		{
			FrameRateChangeData frameRateChangeData = new FrameRateChangeData();
			frameRateChangeData.FrameRate = frameRate;
			frameRateChangeData.OverlayName = overlayName;
			HubtasticBaseStation.ChangeFrameRate(JsonConvert.SerializeObject(frameRateChangeData));
		}

		RadioButton GetFrameRateRadioButton(string overlayName, int frameRate)
		{
			switch (overlayName)
			{
				case Overlays.Back:
					switch (frameRate)
					{
						case 1:
							return rbnBack1;
						case 2:
							return rbnBack2;
						case 5:
							return rbnBack5;
						case 10:
							return rbnBack10;
						case 15:
							return rbnBack15;
						case 20:
							return rbnBack20;
						case 25:
							return rbnBack25;
						case 30:
							return rbnBack30;
						case 35:
							return rbnBack35;
						case 40:
							return rbnBack40;
						case 45:
							return rbnBack45;
						case 50:
							return rbnBack50;
						case 55:
							return rbnBack55;
						case 60:
							return rbnBack60;
					}
					break;
				case Overlays.Dice:
					switch (frameRate)
					{
						case 1:
							return rbnDice1;
						case 2:
							return rbnDice2;
						case 5:
							return rbnDice5;
						case 10:
							return rbnDice10;
						case 15:
							return rbnDice15;
						case 20:
							return rbnDice20;
						case 25:
							return rbnDice25;
						case 30:
							return rbnDice30;
						case 35:
							return rbnDice35;
						case 40:
							return rbnDice40;
						case 45:
							return rbnDice45;
						case 50:
							return rbnDice50;
						case 55:
							return rbnDice55;
						case 60:
							return rbnDice60;
					}
					break;
				case Overlays.Front:
					switch (frameRate)
					{
						case 1:
							return rbnFront1;
						case 2:
							return rbnFront2;
						case 5:
							return rbnFront5;
						case 10:
							return rbnFront10;
						case 15:
							return rbnFront15;
						case 20:
							return rbnFront20;
						case 25:
							return rbnFront25;
						case 30:
							return rbnFront30;
						case 35:
							return rbnFront35;
						case 40:
							return rbnFront40;
						case 45:
							return rbnFront45;
						case 50:
							return rbnFront50;
						case 55:
							return rbnFront55;
						case 60:
							return rbnFront60;
					}
					break;
			}
			return null;
		}
		private void ChangeFrameRateAndUI(string overlayName, int frameRate)
		{
			RadioButton radioButton = GetFrameRateRadioButton(overlayName, frameRate);
			if (radioButton != null)
			{
				try
				{
					changingFrameRateInternally = true;
					Dispatcher.Invoke(() =>
					{
						radioButton.IsChecked = true;
					});
				}
				finally
				{
					changingFrameRateInternally = false;
				}
			}
			ChangeFrameRate(overlayName, frameRate);
		}

		bool dynamicThrottling = false;

		private void CkDynamicThrottling_CheckedChanged(object sender, RoutedEventArgs e)
		{
			dynamicThrottling = ckDynamicThrottling.IsChecked == true;
		}

		public void GetSavingThrowStats()
		{

		}

		public void SetModifier(int modifier)
		{

		}

		public void RollSave(int diceId)
		{

		}

		// TODO: Set this when a spell is cast.
		int lastSpellSave;
		public int GetLastSpellSave()
		{
			return lastSpellSave;
		}

		public void StopPlayer(string mainFolder)
		{
			SoundCommand soundCommand = new SoundCommand(mainFolder);
			soundCommand.type = SoundCommandType.StopPlaying;
			Execute(soundCommand);
		}

		public void SetPlayerVolume(string mainFolder, int newVolume)
		{
			SoundCommand soundCommand = new SoundCommand(mainFolder);
			soundCommand.type = SoundCommandType.SetVolume;
			soundCommand.numericData = newVolume;
			Execute(soundCommand);
		}

		public void SetPlayerFolder(string mainFolder, string newTheme)
		{
			SoundCommand soundCommand = new SoundCommand(mainFolder);
			soundCommand.type = SoundCommandType.ChangeFolder;
			soundCommand.strData = newTheme;
			Execute(soundCommand);
		}

		private static void Execute(SoundCommand soundCommand)
		{
			string serializedObject = JsonConvert.SerializeObject(soundCommand);
			HubtasticBaseStation.ExecuteSoundCommand(serializedObject);
		}

		private void LstAllSpells_Drop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;

			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			ActivePlayer.ClearAllCasting();
			if (files.Length > 0)
				AssignImageToSpell(files[0]);
		}

		void AssignImageToSpell(string fileName)
		{
			SpellDto spellDto = lstAllSpells.SelectedItem as SpellDto;
			if (spellDto == null)
				return;
			AssignImageToSpell(fileName, spellDto.name);
		}

		string GetValidFileName(string fileName)
		{
			string result = fileName;

			foreach (char c in System.IO.Path.GetInvalidFileNameChars())
				result = result.Replace(c, '_');

			return result;
		}

		private void AssignImageToSpell(string fileName, string spellName)
		{
			const string targetPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Scroll\Spells\Icons";
			File.Copy(fileName, System.IO.Path.Combine(targetPath, GetValidFileName(spellName) + ".png"), true);

			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, ActivePlayer.ToJson());
			ShowActiveSpell();
		}

		private void TbDice_Drop(object sender, DragEventArgs e)
		{
			if (preparedSpell == null)
				return;

			ActivePlayer.ClearAllCasting();
			ActivePlayer.spellPrepared = ActiveSpellData.FromCastedSpell(preparedSpell);
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;

			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files.Length > 0)
				AssignImageToSpell(files[0], preparedSpell.Spell.Name);
		}

		public void ChangeWealth(List<int> playerIds, decimal deltaAmount)
		{
			WealthChange wealthChange = new WealthChange();
			wealthChange.Coins.SetFromGold(deltaAmount);
			if (playerIds.First() == int.MaxValue)
			{
				wealthChange.PlayerIds = new List<int>();
				foreach (Character player in game.Players)
				{
					wealthChange.PlayerIds.Add(player.playerID);
				}
			}
			else
				wealthChange.PlayerIds = playerIds;

			HubtasticBaseStation.ChangePlayerWealth(JsonConvert.SerializeObject(wealthChange));
			game.ChangeWealth(wealthChange);
		}

		private void BtnUpdateEventHandler_Click(object sender, RoutedEventArgs e)
		{
			SaveCodeChanges(sender, e);
		}

		private void BtnTestPlayerSave_Click(object sender, RoutedEventArgs e)
		{
			AllPlayers.SaveChanges();
		}

		DamageHealthChange NewDamageHealthChange(List<int> playerIds, int value)
		{
			DamageHealthChange damageHealthChange = new DamageHealthChange();
			damageHealthChange.PlayerIds = playerIds;
			damageHealthChange.DamageHealth = value;
			return damageHealthChange;
		}

		void IncreasePlayerHealth(List<int> playerIds, int value)
		{
			ApplyDamageHealthChange(NewDamageHealthChange(playerIds, value));
		}

		void ApplyPlayerDamage(List<int> playerIds, int value)
		{
			ApplyDamageHealthChange(NewDamageHealthChange(playerIds, -value));
		}

		void SetAttackThreshold(int value)
		{
			Dispatcher.Invoke(() =>
			{
				tbxAttackThreshold.Text = value.ToString();
			});
			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{value} {twitchIndent} <-- hidden ATTACK threshold");
		}
		void SetSavingThrowThreshold(int value)
		{
			Dispatcher.Invoke(() =>
			{
				tbxSaveThreshold.Text = value.ToString();
			});
			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{value} {twitchIndent} <-- hidden SAVE threshold");
		}

		void SetSkillCheckThreshold(int value)
		{
			Dispatcher.Invoke(() =>
			{
				tbxSkillCheckThreshold.Text = value.ToString();
			});
			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{value} {twitchIndent} <-- hidden SKILL CHECK threshold");
		}
		public void Apply(string command, decimal value, List<int> playerIds)
		{
			int intValue = (int)Math.Round(value);
			switch (command)
			{
				case "Health":
					IncreasePlayerHealth(playerIds, intValue);
					break;
				case "Damage":
					ApplyPlayerDamage(playerIds, intValue);
					break;
				case "AddCoins":
					ChangeWealth(playerIds, value);
					break;
				case "RemoveCoins":
					ChangeWealth(playerIds, -value);
					break;
				case "SaveThreshold":
					SetSavingThrowThreshold(intValue);
					break;
				case "SkillThreshold":
					SetSkillCheckThreshold(intValue);
					break;
				case "AttackThreshold":
					SetAttackThreshold(intValue);
					break;
				case "LastDamage":
					ApplyPlayerDamage(playerIds, lastDamage);
					break;
				case "LastHealth":
					IncreasePlayerHealth(playerIds, lastHealth);
					break;
				case "TempHp":
					AddTempHitPoints(playerIds, intValue);
					break;
			}
		}

		private void AddTempHitPoints(List<int> playerIds, int intValue)
		{
			DamageHealthChange damageHealthChange = NewDamageHealthChange(playerIds, intValue);
			damageHealthChange.IsTempHitPoints = true;
			ApplyDamageHealthChange(damageHealthChange);
		}

		private void BtnPickMonster_Click(object sender, RoutedEventArgs e)
		{
			FrmMonsterPicker frmMonsterPicker = new FrmMonsterPicker();
			frmMonsterPicker.Owner = this;
			frmMonsterPicker.spMonsters.DataContext = AllMonsters.Monsters;
			frmMonsterPicker.ShowDialog();
		}

		private void CkShowFPSWindow_CheckedChanged(object sender, RoutedEventArgs e)
		{
			FrameRateChangeData.GlobalShowFpsWindow = ckShowFPSWindow.IsChecked == true;
			FrameRateChangeData frameRateChangeData = new FrameRateChangeData();
			frameRateChangeData.OverlayName = Overlays.Front;
			HubtasticBaseStation.ChangeFrameRate(JsonConvert.SerializeObject(frameRateChangeData));
		}

		private void CkEnableHueShift_CheckedChanged(object sender, RoutedEventArgs e)
		{
			FrameRateChangeData.GlobalAllowColorShifting = ckEnableHueShift.IsChecked == true;
			FrameRateChangeData frameRateChangeData = new FrameRateChangeData();
			frameRateChangeData.OverlayName = Overlays.Front;
			HubtasticBaseStation.ChangeFrameRate(JsonConvert.SerializeObject(frameRateChangeData));
		}

		private void BtnReconnectTwitchClient_Click(object sender, RoutedEventArgs e)
		{
			CreateDungeonMasterClient();
		}

		private void CkEnableCanvasFilterCaching_CheckedChanged(object sender, RoutedEventArgs e)
		{
			//FrameRateChangeData.GlobalAllowCanvasFilterCaching = ckEnableCanvasFilterCaching.IsChecked == true;
			UpdateOverlayPerformanceOptions();
		}

		private static void UpdateOverlayPerformanceOptions()
		{
			HubtasticBaseStation.ChangeFrameRate(JsonConvert.SerializeObject(new FrameRateChangeData()));
		}

		private void TbxMaxFiltersOnWindup_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!int.TryParse(tbxMaxFiltersOnWindup.Text, out int num))
				return;

			FrameRateChangeData.GlobalMaxFiltersOnWindup = num;
			UpdateOverlayPerformanceOptions();
		}

		private void TbxMaxFiltersOnDieCleanup_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!int.TryParse(tbxMaxFiltersOnDieCleanup.Text, out int num))
				return;

			FrameRateChangeData.GlobalMaxFiltersOnDieCleanup = num;
			UpdateOverlayPerformanceOptions();
		}

		private void TbxMaxFiltersOnRoll_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!int.TryParse(tbxMaxFiltersOnRoll.Text, out int num))
				return;

			FrameRateChangeData.GlobalMaxFiltersOnRoll = num;
			UpdateOverlayPerformanceOptions();
		}

		private void TbxCode_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void TbxCode_SelectionChanged(object sender, RoutedEventArgs e)
		{
			// This is also called whenever the caret moves.
		}

		bool changingInternally;
		private void TbxCode_TextChanged(object sender, TextChangedEventArgs e)
		{
			CodeChanged();
		}

		private void CodeChanged()
		{
			if (changingInternally)
				return;
			changingInternally = true;
			try
			{
				UpdateSelectedEvent(tbxCode.Text);
			}
			finally
			{
				changingInternally = false;
			}
		}

		TemplateEngine templateEngine;

		public TemplateEngine TemplateEngine
		{
			get
			{
				if (templateEngine == null)
					templateEngine = new TemplateEngine();
				return templateEngine;
			}
		}

		object lastParameterTooltip;

		private Size MeasureString(string candidate)
		{
			var formattedText = new FormattedText(
					candidate,
					CultureInfo.CurrentCulture,
					FlowDirection.LeftToRight,
					new Typeface(tbxCode.FontFamily, tbxCode.FontStyle, tbxCode.FontWeight, tbxCode.FontStretch),
					tbxCode.FontSize,
					Brushes.Black,
					new NumberSubstitution(),
					1);

			return new Size(formattedText.Width, formattedText.Height);
		}

		double spaceWidth = 0;
		int lastLineShownTooltip;
		void ShowParameterTooltip(object content, int parameterStartOffset)
		{
			if (content == null)
				return;
			int thisLine = tbxCode.TextArea.Caret.Line;
			if (content is string && (string)lastParameterTooltip == (string)content)
				if (lastLineShownTooltip == thisLine)
					if (TextCompletionEngine.ParameterToolTip.IsOpen)
						return;

			lastLineShownTooltip = thisLine;
			lastParameterTooltip = content;
			if (spaceWidth == 0)
				spaceWidth = MeasureString("M").Width;
			double adjustLeft = tbxCode.TextArea.Caret.Offset - parameterStartOffset - 0.5;
			Rect caret = tbxCode.TextArea.Caret.CalculateCaretRectangle();
			TextCompletionEngine.ParameterToolTip.HorizontalOffset = Math.Round(caret.Right - adjustLeft * spaceWidth);
			TextCompletionEngine.ParameterToolTip.VerticalOffset = caret.Bottom + 9;
			TextCompletionEngine.ParameterToolTip.Content = content;
			TextCompletionEngine.ParameterToolTip.IsOpen = true;
			if (TextCompletionEngine.CompletionWindow != null && TextCompletionEngine.CompletionWindow.IsVisible)
			{
				TextCompletionEngine.CompletionWindow.Top += TextCompletionEngine.ParameterToolTip.ActualHeight;
			}
		}

		void HideParameterTooltip()
		{
			TextCompletionEngine.ParameterToolTip.IsOpen = false;
		}

		bool IsNavKey(Key key)
		{
			switch (key)
			{
				case Key.End:
				case Key.Home:
				case Key.Left:
				case Key.Up:
				case Key.Right:
				case Key.Down:
				case Key.PageUp:
				case Key.PageDown:
					return true;
			}
			return false;
		}



		int GetParameterNumber()
		{
			return tbxCode.Document.GetParameterNumberAtPosition(tbxCode.TextArea.Caret.Offset);
		}

		int GetParameterStartOffset()
		{
			return tbxCode.Document.GetParameterStartOffset(tbxCode.TextArea.Caret.Offset);
		}

		DispatcherTimer tooltipTimer;

		void FocusSelectedItem(ListBox listBox)
		{
			if (listBox.SelectedItem != null)
			{
				ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
				listBoxItem?.Focus();
			}
			else
				lstEvents.Focus();
		}
		bool CodeCompletionWindowIsUp()
		{
			if (TextCompletionEngine.CompletionWindow == null)
				return false;

			return TextCompletionEngine.CompletionWindow.Visibility == Visibility.Visible;
		}

		private void TbxCode_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && Modifiers.NoModifiersDown && !CodeCompletionWindowIsUp())
			{
				// TODO: Change this if/when we add support for markers.
				FocusSelectedItem(lstEvents);
				e.Handled = true;
			}
			else if (e.Key == Key.Space && Modifiers.NoModifiersDown)
			{
				CodeTemplate templateToExpand = TemplateEngine.GetTemplateToExpand(tbxCode);
				if (templateToExpand != null)
				{
					e.Handled = true;
					TextCompletionEngine.HideCodeCompletionWindow();
					TemplateEngine.ExpandTemplate(tbxCode, templateToExpand);
				}
			}
			else if (IsNavKey(e.Key) || e.Key == Key.OemComma && Modifiers.NoModifiersDown)
			{
				StartTooltipTimer();
			}
		}

		private void StartTooltipTimer()
		{
			if (tooltipTimer == null)
			{
				CreateToolipTimer();
			}
			tooltipTimer.Start();
		}

		private void CreateToolipTimer()
		{
			tooltipTimer = new DispatcherTimer();
			tooltipTimer.Interval = TimeSpan.FromMilliseconds(50);
			tooltipTimer.Tick += TooltipTimer_Tick;
		}

		string GetParameterTooltip(int parameterNumber)
		{
			DndFunction dndFunction = TextCompletionEngine.GetActiveDndFunction(tbxCode.TextArea);
			if (dndFunction == null)
				return null;

			IEnumerable<ParamAttribute> customAttributes = dndFunction.GetType().GetCustomAttributes(typeof(ParamAttribute)).Cast<ParamAttribute>().ToList();
			if (customAttributes == null)
				return null;

			ParamAttribute paramAttribute = customAttributes.FirstOrDefault(x => x.Index == parameterNumber);
			if (paramAttribute == null)
				return null;

			return paramAttribute.Description;
		}

		private void TooltipTimer_Tick(object sender, EventArgs e)
		{
			tooltipTimer.Stop();
			Dispatcher.Invoke(() =>
			{
				ShowParameterTooltipIfNecessary();
			});
		}

		public void ShowParameterTooltipIfNecessary()
		{
			int parameterNumber = GetParameterNumber();
			if (parameterNumber > 0)
			{
				int parameterStartOffset = GetParameterStartOffset();
				string parameterTooltip = GetParameterTooltip(parameterNumber);
				ShowParameterTooltip(parameterTooltip, parameterStartOffset);
			}
			else
				HideParameterTooltip();
		}

		void ShowStatusSavingCode()
		{
			tbStatus.Visibility = Visibility.Visible;
			iconSaving.Visibility = Visibility.Visible;
			iconSaved.Visibility = Visibility.Hidden;
		}

		void ShowStatusCodeIsSaved()
		{
			tbStatus.Visibility = Visibility.Hidden;
			iconSaving.Visibility = Visibility.Hidden;
			iconSaved.Visibility = Visibility.Visible;
		}

		void HideAllCodeChangedStatusUI()
		{
			tbStatus.Visibility = Visibility.Hidden;
			iconSaving.Visibility = Visibility.Hidden;
			iconSaved.Visibility = Visibility.Hidden;
		}

		private void TbxCode_AvalonTextChanged(object sender, EventArgs e)
		{
			if (!changingInternally)
				ShowStatusSavingCode();
			CodeChanged();
		}
		void LoadAvalonSyntaxHighlighter()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "DHDM.CSharp.xml";

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream, new System.Xml.XmlReaderSettings()))
			{
				tbxCode.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
			}
		}
		public void InvokeCodeCompletion()
		{
			TextCompletionEngine.InvokeCodeCompletion();
		}

		private void btnReloadTemplates_Click(object sender, RoutedEventArgs e)
		{
			TemplateEngine.ReloadTemplates();
			TextCompletionEngine.ReloadShortcuts();
		}

		private void TbxCode_MouseDown(object sender, MouseButtonEventArgs e)
		{
			StartTooltipTimer();
		}
		void SaveCodeChanges(object sender, EventArgs e)
		{
			if (activeEventData == null)
				return;
			// Spells only...
			string spellOrFeatureName = activeEventData.ParentGroup.Name;
			List<Spell> allSpells = AllSpells.GetAll(spellOrFeatureName);
			if (allSpells.Count == 1)
			{
				Spell spell = allSpells[0];
				GoogleSheets.SaveChanges(spell, activeEventData.Name);
			}
			else
			{
				Feature feature = AllFeatures.Get(spellOrFeatureName);
				if (feature != null)
					GoogleSheets.SaveChanges(feature, activeEventData.Name);
			}
			ShowStatusCodeIsSaved();
		}

		private void lstKnownSpellsAndFeatures_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Modifiers.NoModifiersDown)
			{
				FocusSelectedItem(lstEvents);
				e.Handled = true;
			}
		}

		private void lstEvents_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Modifiers.NoModifiersDown)
			{
				tbxCode.Focus();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape && Modifiers.NoModifiersDown)
			{
				FocusSelectedItem(lstKnownSpellsAndFeatures);
				e.Handled = true;
			}
		}

		private void btnReloadTrailingEffects_Click(object sender, RoutedEventArgs e)
		{
			AllTrailingEffects.Invalidate();
			TextCompletionEngine.RefreshCompletionProviders();
		}

		private void btnReloadMonsters_Click(object sender, RoutedEventArgs e)
		{
			AllMonsters.Invalidate();
			FilterMonsters();
		}

		CreatureKinds GetMonsterTabCreatureKindFilter()
		{
			CreatureKinds result = CreatureKinds.None;

			Array values = Enum.GetValues(typeof(CreatureKinds));
			Array names = Enum.GetNames(typeof(CreatureKinds));
			Dictionary<string, CreatureKinds> nameValueMap = new Dictionary<string, CreatureKinds>();
			for (int i = 0; i < names.Length; i++)
			{
				nameValueMap.Add(((string[])names)[i], ((CreatureKinds[])values)[i]);
			}

			// TODO: CodingGorilla says try this: var r = names.ToDictionary(n => n, v => Enum.Parse<CreatureKinds>(v));

			foreach (StackPanel sp in spMonsterFilters.Children.OfType<StackPanel>())
				foreach (CheckBox checkbox in sp.Children.OfType<CheckBox>())
					if (checkbox.IsChecked == true)
						if (checkbox.Content is string label)
							result |= nameValueMap[label];

			return result;
		}
		private void FilterMonsters()
		{
			double maxChallengeRating = double.MaxValue;
			CreatureKinds creatureKindFilter = GetMonsterTabCreatureKindFilter();
			spAllMonsters.DataContext = AllMonsters.Monsters.Where(x => x.challengeRating <= maxChallengeRating &&
			(x.kind & creatureKindFilter) == x.kind).ToList();
		}

		private void btnMonsterFilter_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!changingInternally)
				FilterMonsters();
		}

		bool isResizingCropRect;
		bool isDraggingCropRect;
		double imageCropMouseDeltaX;
		double imageCropMouseDeltaY;
		private void rectDragHandle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			GetMouseDragDeltas(e, rectDragHandle);
			ShowCropPreviewWindow();
			isResizingCropRect = true;
			UpdateMonsterCropPreview();
		}

		private void rectCrop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			GetMouseDragDeltas(e, rectCrop);
			ShowCropPreviewWindow();
			isDraggingCropRect = true;
			UpdateMonsterCropPreview();
		}

		private void rectDragHandle_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!isResizingCropRect)
				return;

			MoveRectWithMouse(e, rectDragHandle);
			PositionCropPreviewNearCropRect();
			SetCropSizeFromHandle();
		}

		void FixCropRectOutline()
		{
			Canvas.SetLeft(rectOutline, Canvas.GetLeft(rectCrop) - 1);
			Canvas.SetTop(rectOutline, Canvas.GetTop(rectCrop) - 1);
			rectOutline.Width = rectCrop.Width + 2;
			rectOutline.Height = rectCrop.Height + 2;
		}
		private void rectCrop_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!isDraggingCropRect)
				return;
			MoveRectWithMouse(e, rectCrop);
			MonsterImageCropRectChanged();
			
		}

		/// <summary>
		/// Moves the drag handle to the bottom-right of the crop box.
		/// </summary>
		void MoveDragHandle()
		{
			Canvas.SetLeft(rectDragHandle, Canvas.GetLeft(rectCrop) + rectCrop.Width - rectDragHandle.Width / 2);
			Canvas.SetTop(rectDragHandle, Canvas.GetTop(rectCrop) + rectCrop.Height - rectDragHandle.Height / 2);
		}


		private void MoveRectWithMouse(MouseEventArgs e, Rectangle rect)
		{
			double offset = 0;
			if (rect == rectDragHandle)
				offset = rectDragHandle.Width / 2;
			Point mousePosition = e.GetPosition(cvsImageOverlay);

			Canvas.SetLeft(rect, Math.Min(imgMonster.ActualWidth - rect.Width + offset, Math.Max(0, mousePosition.X - imageCropMouseDeltaX)));
			Canvas.SetTop(rect, Math.Min(imgMonster.ActualHeight - rect.Height + offset, Math.Max(0, mousePosition.Y - imageCropMouseDeltaY)));
		}

		void SetCropSizeFromHandle()
		{
			var scale = imgMonster.ActualWidth / imgMonster.Source.Width;
			double centerHandleX = Canvas.GetLeft(rectDragHandle) + rectDragHandle.Width / 2;
			double centerHandleY = Canvas.GetTop(rectDragHandle) + rectDragHandle.Height / 2;
			double newWidth = centerHandleX - Canvas.GetLeft(rectCrop);
			double newHeight = centerHandleY - Canvas.GetTop(rectCrop);
			if (newWidth < PictureCropInfo.MinWidth * scale)
				newWidth = PictureCropInfo.MinWidth * scale;

			rectCrop.Width = newWidth;
			rectCrop.Height = PictureCropInfo.GetHeightFromWidth(newWidth);
			MonsterImageCropRectChanged();
		}

		private void rectDragHandle_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (isResizingCropRect)
			{
				rectDragHandle.ReleaseMouseCapture();
				SetCropSizeFromHandle();
				UpdateSelectedMonsterImageCropData();
				HideCropPreviewWindow();
			}
			isResizingCropRect = false;
		}

		private void GetMouseDragDeltas(MouseButtonEventArgs e, Rectangle rectCrop)
		{
			Point mousePosition = e.GetPosition(cvsImageOverlay);
			double leftCropRect = Canvas.GetLeft(rectCrop);
			if (double.IsNaN(leftCropRect))
			{
				Canvas.SetLeft(rectCrop, 0);
				leftCropRect = 0;
			}

			double topCropRect = Canvas.GetTop(rectCrop);
			if (double.IsNaN(topCropRect))
			{
				Canvas.SetTop(rectCrop, 0);
				topCropRect = 0;
			}

			imageCropMouseDeltaX = mousePosition.X - leftCropRect;
			imageCropMouseDeltaY = mousePosition.Y - topCropRect;
			rectCrop.CaptureMouse();
		}

		private void rectCrop_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (isDraggingCropRect)
			{
				rectCrop.ReleaseMouseCapture();
				UpdateSelectedMonsterImageCropData();
				HideCropPreviewWindow();
			}
			isDraggingCropRect = false;
		}
		void HideCropPreviewWindow()
		{
			if (frmCropPreview == null)
				return;
			frmCropPreview.Hide();
		}

		FrmCropPreview frmCropPreview;
		void ShowCropPreviewWindow()
		{
			if (frmCropPreview == null)
				frmCropPreview = new FrmCropPreview();
			UpdateCropRectangleForSelectedMonster();
			PositionCropPreviewNearCropRect();
			frmCropPreview.Show();
		}

		void PositionCropPreviewNearCropRect()
		{
			if (frmCropPreview == null)
				return;
			if (!frmCropPreview.IsVisible)
				return;
			Point upperLeftCropRect = this.rectCrop.PointToScreen(new Point(0, 0));
			double right = upperLeftCropRect.X + rectCrop.Width;
			double bottom = upperLeftCropRect.Y + rectCrop.Height;
			const int spaceBetweenWindows = 10;
			frmCropPreview.Left = right + spaceBetweenWindows;
			frmCropPreview.Top = bottom - frmCropPreview.Height;
		}

		void UpdateSelectedMonsterImageCropData()
		{
			if (!(lstAllMonsters.SelectedItem is Monster monster))
				return;
			UpdateCropInfoFromUI(monster);
			GoogleSheets.SaveChanges(monster, string.Join(",", nameof(monster.imageCropStr), nameof(monster.ImageUrl)));
		}

		private void UpdateCropInfoFromUI(Monster monster)
		{
			var scale = imgMonster.ActualWidth / imgMonster.Source.Width;
			double width = rectCrop.Width / scale;
			double x = Canvas.GetLeft(rectCrop) / scale;
			double y = Canvas.GetTop(rectCrop) / scale;

			monster.ImageCropInfo = new PictureCropInfo() { X = x, Y = y, Width = width };
		}

		void MoveMonsterCroppedTo(PictureCropInfo pictureCropInfo)
		{
			if (imgMonster.Source == null || imgMonster.Source.Width == 1)
			{
				cropRectUpdateTimer.Start();
				return;
			}
			var scale = imgMonster.ActualWidth / imgMonster.Source.Width;
			if (pictureCropInfo == null)
			{
				Canvas.SetLeft(rectCrop, 0);
				Canvas.SetTop(rectCrop, 0);

				rectCrop.Width = PictureCropInfo.MinWidth * scale;
				rectCrop.Height = PictureCropInfo.GetHeightFromWidth(rectCrop.Width);
			}
			else
			{
				Canvas.SetLeft(rectCrop, pictureCropInfo.X * scale);
				Canvas.SetTop(rectCrop, pictureCropInfo.Y * scale);
				rectCrop.Width = pictureCropInfo.Width * scale;
				rectCrop.Height = PictureCropInfo.GetHeightFromWidth(rectCrop.Width);
			}

			MonsterImageCropRectChanged();
		}

		BitmapImage monsterCropPreviewBitmap;

		private void MonsterImageCropRectChanged()
		{
			FixCropRectOutline();
			PositionCropPreviewNearCropRect();
			MoveDragHandle();
			UpdateMonsterCropPreview();
		}

		private void UpdateMonsterCropPreview()
		{
			
			if (frmCropPreview != null && monsterCropPreviewBitmap != null)
				if (lstAllMonsters.SelectedItem is Monster monster)
				{
					if (monsterCropPreviewBitmap.PixelWidth == 1)
						return;
					double dpiFactorX = monsterCropPreviewBitmap.DpiX / 96;
					double dpiFactorY = monsterCropPreviewBitmap.DpiY / 96;
					UpdateCropInfoFromUI(monster);
					PictureCropInfo cropInfo = monster.ImageCropInfo;
					if (double.IsNaN(cropInfo.X))
					{
						cropInfo = new PictureCropInfo() { Width = 104 };
					}
					double x = cropInfo.X * dpiFactorX;
					double y = cropInfo.Y * dpiFactorY;

					if (x < 0)
						x = 0;

					if (y < 0)
						y = 0;

					double width = cropInfo.Width * dpiFactorX;
					double height = PictureCropInfo.GetHeightFromWidth(cropInfo.Width) * dpiFactorY;
					if (x + width > monsterCropPreviewBitmap.PixelWidth || y + height > monsterCropPreviewBitmap.PixelHeight)
					{
						double scaleWidth = monsterCropPreviewBitmap.PixelWidth / (x + width);
						double scaleHeight = monsterCropPreviewBitmap.PixelHeight / (y + height);
						double newScale = Math.Min(scaleWidth, scaleHeight);
						ScaleEverything(newScale, ref x, ref y, ref width, ref height);
					}
					Int32Rect int32Rect = new Int32Rect((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(width), (int)Math.Floor(height));
					frmCropPreview.imgCropPreview.Source = new CroppedBitmap(monsterCropPreviewBitmap, int32Rect);
					frmCropPreview.UpdateLayout();
				}
		}

		void ScaleEverything(double newScale, ref double x, ref double y, ref double width, ref double height)
		{
			x *= newScale;
			y *= newScale;
			width *= newScale;
			height *= newScale;
		}
			

		private void lstAllMonsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateCropRectangleForSelectedMonster();
			if (lstAllMonsters.SelectedItem is Monster monster)
			{
				monsterCropPreviewBitmap = new BitmapImage(new Uri(monster.ImageUrl));
				Title = $"{monsterCropPreviewBitmap.DpiX}, {monsterCropPreviewBitmap.DpiY}";
			}
			else
				monsterCropPreviewBitmap = null;
		}

		private void UpdateCropRectangleForSelectedMonster()
		{
			if (lstAllMonsters.SelectedItem is Monster monster)
				MoveMonsterCroppedTo(monster.ImageCropInfo);
		}

		private void lstAllMonsters_Drop(object sender, DragEventArgs e)
		{
			
			// TODO: base this code on LstAllSpells_Drop
			if (!e.Data.GetDataPresent(DataFormats.Html))
				return;

			// HACK: we are looking for the text "src=", instead of properly parsing the HTML.
			string data = (string)e.Data.GetData(DataFormats.Html);
			const string srcTag = "src=\"";
			if (!data.Contains(srcTag))
				return;

			string imgSrc = data.EverythingAfter(srcTag).EverythingBefore("\"");
			if (imgSrc.StartsWith("data:"))
			{
				System.Diagnostics.Debugger.Break();
				return;
			}
			if (lstAllMonsters.SelectedItem is Monster monster)
			{
				monster.ImageUrl = imgSrc;
				lstAllMonsters.SelectedItem = null;
				lstAllMonsters.SelectedItem = monster;
			}
		}

		void UpdateCropRectFromTimer(object sender, EventArgs e)
		{
			cropRectUpdateTimer.Stop();
			UpdateCropRectangleForSelectedMonster();
		}

		private void rectCrop_MouseEnter(object sender, MouseEventArgs e)
		{
			ShowCropPreviewWindow();
			PositionCropPreviewNearCropRect();
		}

		private void rectCrop_MouseLeave(object sender, MouseEventArgs e)
		{
			HideCropPreviewWindow();
		}
	}
	// TODO: Reintegrate wand/staff animations....
	/* 
	  Name									Index		Effect				effectAvailableWhen		playToEndOnExpire	 hue	moveUpDown
		Melf's Minute Meteors.6				Staff.Weapon	Casting								x										30	150				
		Melf's Minute Meteors.7				Staff.Magic		Casting								x									 350	150				
	 */
}
