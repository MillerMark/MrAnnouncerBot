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
using System.IO;

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


		const string STR_FrontOverlay = "Front";
		const string STR_BackOverlay = "Back";
		const string STR_DiceOverlay = "Dice";

		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		DungeonMasterChatBot dmChatBot = new DungeonMasterChatBot();
		TwitchClient dungeonMasterClient;

		List<PlayerActionShortcut> actionShortcuts = new List<PlayerActionShortcut>();
		ScrollPage activePage = ScrollPage.main;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer showClearButtonTimer;
		DispatcherTimer stateUpdateTimer;
		DispatcherTimer reloadSpellsTimer;
		DispatcherTimer pendingShortcutsTimer;
		DispatcherTimer wildMagicRollTimer;
		DispatcherTimer switchBackToPlayersTimer;
		DispatcherTimer updateClearButtonTimer;
		DateTime lastUpdateTime;
		int keepExistingModifier = int.MaxValue;
		DndGame game = null;

		public MainWindow()
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
		}

		private void HookGameEvents()
		{
			game.SpellDispelled += Game_SpellDispelled;
			game.PickWeapon += Game_PickWeapon;
			game.RequestMessageToDungeonMaster += Game_RequestMessageToDungeonMaster;
			game.RequestMessageToAll += Game_RequestMessageToAll;
			game.PlayerRequestsRoll += Game_PlayerRequestsRoll;
			game.PlayerStateChanged += Game_PlayerStateChanged;
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
				game.TellDmInRounds(0, ea.Reminder, RoundPoint.End);
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
			dungeonMasterClient = Twitch.CreateNewClient("DragonHumpersDM", "DragonHumpersDM", "DragonHumpersDmOAuthToken");

			if (dungeonMasterClient != null)
				dungeonMasterClient.OnMessageReceived += HumperBotClient_OnMessageReceived;
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
					result.AnswerText = workStr.EverythingAfter(":");
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

		public void SetHiddenThreshold(int hiddenThreshold)
		{
			Dispatcher.Invoke(() =>
			{
				tbxHiddenThreshold.Text = hiddenThreshold.ToString();
			});

			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{hiddenThreshold} {twitchIndent} <-- hidden threshold");
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
			commandParser.HandleMessage(e.ChatMessage, dungeonMasterClient);
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
			button.Content = playerActionShortcut.Name;
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
			PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.Name == shortcutName && x.PlayerId == ActivePlayerId);
			if (shortcut == null && tbTabs.SelectedItem == tbDebug)
				shortcut = actionShortcuts.FirstOrDefault(x => x.Name.StartsWith(shortcutName) && x.PlayerId == ActivePlayerId);
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

				if (ActivateShortcutForPlayer(actionShortcut, player) == ActionType.ModifiesExisting)
					return;

				settingInternally = true;
				try
				{
					if (actionShortcut.ModifiesExistingRoll)
					{
						currentRoll.Modify(actionShortcut);
						actionShortcut.ExecuteCommands(player);
					}
					else
					{
						currentRoll = DiceRoll.GetFrom(actionShortcut, player);
						TellDmWeAreReady(player, actionShortcut);
						NextDieRollType = actionShortcut.Type;
					}
				}
				finally
				{
					settingInternally = false;
					HighlightPlayerShortcutUI(actionShortcut.Index);
					SetControlUIFromRoll(currentRoll);
					UpdateStateUIForPlayer(player);
				}
			}
			finally
			{
				SetPlayerShortcutUI(actionShortcut, player);
				SetShortcutVisibility();
				UpdateAskUI(player);
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

			PlayerTakingAction(actionShortcut, player);

			if (actionShortcut.Spell != null)
				PrepareSpell(actionShortcut, player);
			else
				AboutToTakePhysicalAction(actionShortcut, player);

			player.Use(actionShortcut);
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

		private void AboutToTakePhysicalAction(PlayerActionShortcut actionShortcut, Character player)
		{
			if (!IsWildMagicRoll(actionShortcut))
				SwitchToMainPageInGame();

			SendShortcutWindups(actionShortcut, player);
		}

		private static bool IsWildMagicRoll(PlayerActionShortcut actionShortcut)
		{
			return actionShortcut.Name.IndexOf("Wild Magic") >= 0;
		}

		private void SwitchToMainPageInGame()
		{
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, ScrollPage.main, string.Empty);
		}

		private void PrepareSpell(PlayerActionShortcut actionShortcut, Character player)
		{
			Spell spell = actionShortcut.Spell;
			if (spell == null)
				return;
			player.PreparingSpell();
			UpdateSpellPageUI(spell);
			spellToCastOnRoll = actionShortcut;
			SwitchToSpellPageInGame();

			ShowSpellPreparingWindups(actionShortcut, spell);
			preparedSpell = new CastedSpell(spell, player, null);
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

		private void PlayerTakingAction(PlayerActionShortcut actionShortcut, Character player)
		{
			player.ReadiedWeapon = null;
			if (actionShortcut.Part != TurnPart.Reaction)
				game.CreatureTakingAction(player);

			// TODO: Fix the targeting.
			if (DndUtils.IsAttack(actionShortcut.Type))
			{
				player.ReadiedWeapon = actionShortcut.CarriedWeapon;
				actionShortcut.UpdatePlayerAttackingAbility(player);


				if (actionShortcut.Spell == null && actionShortcut.WeaponProperties != WeaponProperties.None)
					game.CreatureRaisingWeapon(player, actionShortcut);
				player.PrepareAttack(null, actionShortcut);
			}
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

				string serializedObject = JsonConvert.SerializeObject(windups);
				HubtasticBaseStation.AddWindup(serializedObject);
			}
		}

		void CastingSpell(DndTimeSpan castingTime, PlayerActionShortcut actionShortcut)
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
				CastingSpell(actionShortcut.Spell.CastingTime, actionShortcut);
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
				CastedSpell castedSpell = new CastedSpell(spell, player, null);
				castedSpell.CastingWithItem();
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
					if (!game.Clock.InCombat)
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
					if (!game.Clock.InCombat)
						StartRealTimeTimer();
				}
			}
			PrepareToCastSpell(spell, actionShortcut.PlayerId);

			// TODO: Fix the targeting.
			//ShowCastingEffects(actionShortcut, spell);

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
				ShowSpellHitInGame(actionShortcut.PlayerId, SpellHitType.Hit, spell.Name);
				player.JustCastSpell(spell.Name);
			}
			ShowSpellEffects(actionShortcut, spell, "Spell");
		}

		private static void ShowSpellPreparingWindups(PlayerActionShortcut actionShortcut, Spell spell)
		{
			ShowSpellEffects(actionShortcut, spell, "Windup");
		}

		private static void ShowSpellEffects(PlayerActionShortcut actionShortcut, Spell spell, string prefix)
		{
			CastedSpellDto spellToCastDto = new CastedSpellDto(spell, new SpellTarget() { Target = SpellTargetType.Player, PlayerId = actionShortcut.PlayerId });

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

			lstAssignedFeatures.ItemsSource = player.GetEventGroup(typeof(Feature));
			lstKnownSpells.ItemsSource = player.GetEventGroup(typeof(Spell));
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
						ShowSpellHitInGame(diceRoll.PlayerRollOptions[0].PlayerID, SpellHitType.Hit, spell.Name);
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

			SeriouslyRollTheDice(diceRoll);
		}

		private void SeriouslyRollTheDice(DiceRoll diceRoll)
		{
			if (dynamicThrottling)
			{
				ChangeFrameRateAndUI(STR_BackOverlay, 5);
				ChangeFrameRateAndUI(STR_FrontOverlay, 10);
				ChangeFrameRateAndUI(STR_DiceOverlay, 15);
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

		void BeforePlayerRolls(int playerId)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			player.BeforePlayerRollsDice();
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
					diceRoll.AddPlayer(checkbox.PlayerId, vantageKind, inspirationText);
					BeforePlayerRolls(checkbox.PlayerId);
					foundPlayer = true;
				}
			}

			if (!foundPlayer)
				BeforePlayerRolls(ActivePlayerId);

			diceRoll.AddCritFailMessages(type);



			diceRoll.ThrowPower = new Random().Next() * 2.8;
			if (diceRoll.ThrowPower < 0.3)
				diceRoll.ThrowPower = 0.3;

			diceRoll.SetHiddenThreshold(tbxHiddenThreshold.Text);

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

		private void UpdateClock(bool bigUpdate = false, double daysSinceLastUpdate = 0)
		{
			if (txtTime == null)
				return;
			string timeStr = game.Clock.AsFullDndDateTimeString();

			if (game.Clock.InCombat)
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
				Time = timeStr,
				BigUpdate = bigUpdate,
				Rotation = percentageRotation,
				InCombat = game.Clock.InCombat,
				FullSpins = daysSinceLastUpdate,
				AfterSpinMp3 = afterSpinMp3
			};

			string serializedObject = JsonConvert.SerializeObject(clockDto);
			HubtasticBaseStation.UpdateClock(serializedObject);
		}

		private void BtnAdvanceTurn_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromSeconds(6));
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
		}

		private void BtnAddDay_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromDays(1), ShiftKeyDown);
		}

		private void BtnAddTenDay_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromDays(10), ShiftKeyDown);
		}

		private void BtnAddMonth_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromDays(30), ShiftKeyDown);
		}

		public bool ShiftKeyDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			}
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
			game.Clock.Advance(DndTimeSpan.FromHours(1), ShiftKeyDown);
		}

		private void BtnAdd10Minutes_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromMinutes(10), ShiftKeyDown);
		}

		private void BtnAdd1Minute_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.Advance(DndTimeSpan.FromMinutes(1), ShiftKeyDown);
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
					Character player = game.GetPlayerFromId(playerRoll.playerId);
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
					Character player = game.GetPlayerFromId(playerRoll.playerId);
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

		void ShowSpellHitInGame(int playerId, SpellHitType spellHitType, string spellName)
		{
			Spell spell = AllSpells.Get(spellName);
			if (spell == null)
				return;
			int hueShift = DndUtils.GetHueShift(spell.SchoolOfMagic);
			EffectGroup effectGroup = new EffectGroup();

			VisualEffectTarget chestTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, -150));
			VisualEffectTarget bottomTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, 0));

			if (spellHitType == SpellHitType.Miss)
			{
				effectGroup.Add(CreateEffect("SpellMiss", chestTarget, hueShift, 20));
			}
			else
			{
				double scale = 1;
				double autoRotation = 140;
				const double scaleIncrement = 1.1;
				AnimationEffect effect = CreateEffect(GetRandomHitSpellName(), chestTarget, hueShift);
				effect.autoRotation = autoRotation;
				autoRotation *= -1;
				effectGroup.Add(effect);
				scale *= scaleIncrement;

				int timeOffset = 200;
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
				Character player = game.GetPlayerFromId(playerId);

				if (player != null)
				{
					DequeueAnimationEffects(effectGroup, chestTarget, bottomTarget, ref scale, scaleIncrement, ref autoRotation, ref timeOffset, player);
					DequeueSoundEffects(player, effectGroup);
				}
			}
			// TODO: Add success or fail sound effects.
			string serializedObject = JsonConvert.SerializeObject(effectGroup);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}

		void DequeueSoundEffects(Character player, EffectGroup effectGroup)
		{
			while (player.additionalSoundEffects.Count > 0)
			{
				effectGroup.Add(player.additionalSoundEffects.Dequeue());
			}
		}

		private void DequeueAnimationEffects(EffectGroup effectGroup, VisualEffectTarget chestTarget, VisualEffectTarget bottomTarget, ref double scale, double scaleIncrement, ref double autoRotation, ref int timeOffset, Character player)
		{
			while (player.additionalSpellEffects.Count > 0)
			{
				SpellHit spellHit = player.additionalSpellEffects.Dequeue();
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
				AnimationEffect effectBonus = CreateEffect(effectName, target, spellHit.Hue, spellHit.Brightness);
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
			return CreateEffect(spriteName, target, hueShift, brightness);
		}

		AnimationEffect CreateEffect(string spriteName, VisualEffectTarget target, int hueShift, int brightness = 100)
		{
			AnimationEffect spellEffect = new AnimationEffect();
			spellEffect.spriteName = spriteName;
			spellEffect.hueShift = hueShift;
			spellEffect.brightness = brightness;
			spellEffect.target = target;
			return spellEffect;
		}

		void CheckSpellHitResults(DiceStoppedRollingData stopRollingData)
		{
			if (!DndUtils.IsAttack(stopRollingData.type))
				return;

			if (string.IsNullOrWhiteSpace(stopRollingData.spellName))
				return;
			if (stopRollingData.success)
			{
				ShowSpellHitInGame(stopRollingData.playerID, SpellHitType.Hit, stopRollingData.spellName);
			}
			else
			{
				ShowSpellHitInGame(stopRollingData.playerID, SpellHitType.Miss, stopRollingData.spellName);
			}
		}
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (dynamicThrottling)
			{
				ChangeFrameRateAndUI(STR_BackOverlay, 25);
				ChangeFrameRateAndUI(STR_FrontOverlay, 25);
				ChangeFrameRateAndUI(STR_DiceOverlay, 15);
			}

			waitingToClearDice = true;
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
		void ReportInitiativeResults(DiceEventArgs ea)
		{
			if (ea.StopRollingData.multiplayerSummary == null)
			{
				TellDungeonMaster($"͏͏͏͏͏͏͏͏͏͏͏͏̣{Icons.WarningSign} Unexpected issue - no multiplayer results.");
				return;
			}

			TellAll("Initiative: ");
			int count = 1;
			foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
			{
				string playerName = DndUtils.GetFirstName(playerRoll.name);
				string emoticon = GetPlayerEmoticon(playerRoll.playerId);
				int rollValue = playerRoll.modifier + playerRoll.roll;
				bool success = rollValue >= ea.StopRollingData.hiddenThreshold;
				TellAll($"͏͏͏͏͏͏͏͏͏͏͏͏̣{twitchIndent}{DndUtils.GetOrdinal(count)}: {emoticon} {playerName}, rolled a {rollValue.ToString()}.");
				count++;
			}
		}

		private void ReportOnDieRoll(DiceEventArgs ea)
		{
			if (ea.StopRollingData == null)
				return;

			if (ea.StopRollingData.type == DiceRollType.Initiative)
			{
				ReportInitiativeResults(ea);
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
					singlePlayer = AllPlayers.GetFromId(ea.StopRollingData.multiplayerSummary[0].playerId);
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
				game.DieRollStopped(singlePlayer, rollValue, ea.StopRollingData.damage);

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
				ckbUseMagic.IsChecked = false;
				game.EnteringCombat();
				btnEnterExitCombat.Background = new SolidColorBrush(Color.FromRgb(42, 42, 102));
				RollInitiative();
			}
			else
			{
				game.ExitingCombat();
				btnEnterExitCombat.Background = new SolidColorBrush(Colors.DarkRed);
			}

			OnCombatChanged();
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

		void EnterCombat()
		{
			Dispatcher.Invoke(() =>
			{
				if (game.Clock.InCombat)
					TellDungeonMaster($"{Icons.WarningSign} Already in combat!");
				else
				{
					TellAll($"{Icons.EnteringCombat} Entering combat...");
					BtnEnterExitCombat_Click(null, null);
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
				case DungeonMasterCommand.ExitCombat:
					ExitCombat();
					break;
			}
		}

		string PlusHiddenThresholdDisplayStr()
		{
			string returnMessage = string.Empty;
			Dispatcher.Invoke(() =>
			{
				returnMessage = $" (against a hidden threshold of {tbxHiddenThreshold.Text})";
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
			TellDungeonMaster($"{icon} {firstPart}{PlusHiddenThresholdDisplayStr()}.");
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
			TellDungeonMaster($"Rolling {GetPlayerName(ActivePlayerId)}'s attack with a hidden threshold of {tbxHiddenThreshold.Text} and damage dice of {tbxDamageDice.Text}.");
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
			TellDungeonMaster($"{Icons.SavingThrow} {firstPart}{PlusHiddenThresholdDisplayStr()}...");
			TellViewers($"{firstPart}...");
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
			DateTime saveTime = game.Clock.Time;

			AllWeaponEffects.Invalidate();
			AllPlayers.Invalidate();
			AllSpells.Invalidate();
			AllSpellEffects.Invalidate();
			AllFeatures.Invalidate();
			AllDieRollEffects.Invalidate();
			AllTrailingEffects.Invalidate();
			PlayerActionShortcut.PrepareForCreation();
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

		public void ApplyDamageHealthChange(DamageHealthChange damageHealthChange)
		{
			if (damageHealthChange == null)
				return;
			string playerNames = string.Empty;

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
				PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.Name == shortcutName && x.PlayerId == playerId);
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
			Dispatcher.Invoke(() =>
			{
				game.Clock.Advance(DndTimeSpan.FromSeconds(seconds + minutes * 60 + hours * 3600), ShiftKeyDown);
			});
		}

		public void AdvanceDate(int days, int months, int years)
		{
			if (days == 0 && months == 0 && years == 0)
				return;
			Dispatcher.Invoke(() =>
			{
				game.Clock.Advance(DndTimeSpan.FromDays(days + months * 30 + years * 365), ShiftKeyDown);
			});
		}


		public void RollDice()
		{
			Dispatcher.Invoke(() =>
			{
				UnleashTheNextRoll();
				TellDungeonMaster("Rolling the dice...");
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
			if (System.Diagnostics.Debugger.IsAttached)
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
			//ckBreakTest.IsChecked = true;
			Expressions.Debugging = true;
			const string script =
@"if (Get(_justEnteredWildSurgeRage) == true)
{
	Set(_rageDeactivationMessage, """");
	Set(_justEnteredWildSurgeRage, false);
	result = GetRoll(BarbarianWildSurge);
	if (result == 1)
		TellDm(WildSurgeNecrotic);
	else if (result == 2)
	{
		Set(_rageDeactivationMessage, $""{firstName} can no longer teleport."");
		TellDm(WildSurgeTeleport);
	}
	else if (result == 3)
	{
		AddReminder(""All flumph - like spirits explode and each creature within 5 feet of one or more of them must succeed on a * *Dexterity * *saving throw or take 2d8 force damage."", ""end of turn"");
		TellDm(WildSurgeFlumphs);
	}
	else if (result == 4)
	{
		Set(_rageDeactivationMessage, $""{firstName}'s AC is back to normal."");
		TellDm(WildSurgeArcaneShroud);
	}
	else if (result == 5)
	{
		Set(_rageDeactivationMessage, $""{firstName}'s weapons are back to normal."");
		TellDm(WildSurgePlantGrowth);
	}
	else if (result == 6)
	{
		TellDm(WildSurgeReadThoughts);
		AddReminder($""Creatures whose thoughts were detected by {firstName} no longer have disadvantage against {firstName}."", ""1 round"");
	}
	else if (result == 7)
	{
		Set(_rageDeactivationMessage, $""{firstName}'s weapons are back to normal."");
		TellDm(WildSurgeShadowWeapon);
	}
	else if (result == 8)
	{
		TellDm(WildSurgeRadiantLight);
		AddReminder($""Blinded creatures from {firstName}'s Wild Surge Radiance can now see"", ""1 round"");
	}
}";
			Expressions.Do(script, ActivePlayer);
		}

		private void LstEventCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems != null && e.AddedItems.Count > 0)
				if (e.AddedItems[0] is EventGroup eventGroup)
				{
					activeEventGroup = eventGroup;
					lstEvents.ItemsSource = eventGroup.Events;
				}
		}

		List<DebugLine> GetDebugLines(string code)
		{
			List<DebugLine> debugLines = new List<DebugLine>();
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
		private void LstFeatureEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems != null && e.AddedItems.Count > 0)
				if (e.AddedItems[0] is EventData eventData)
				{
					activeEventData = eventData;
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
				tbxCode.Text = code;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void BtnRollDice_Click(object sender, RoutedEventArgs e)
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

			UnleashTheNextRoll();
		}

		private void UnleashTheNextRoll()
		{
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

		private void LstAssignedFeatures_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (lstAssignedFeatures.SelectedItem is EventGroup eventGroup)
			{
				activeEventGroup = eventGroup;
				lstEvents.ItemsSource = eventGroup.Events;
			}
		}

		private void LstKnownSpells_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (lstKnownSpells.SelectedItem is EventGroup eventGroup)
			{
				activeEventGroup = eventGroup;
				lstEvents.ItemsSource = eventGroup.Events;
				btnRepeatLastCast.Content = $"Cast {eventGroup.Name}";
			}
		}

		private void SetEventCode(EventGroup parentGroup, string name, string code)
		{
			string groupName = parentGroup.Name;
			object instance = null;

			if (parentGroup.Type == EventType.FeatureEvents)
				instance = AllFeatures.Get(groupName);
			else if (parentGroup.Type == EventType.SpellEvents)
			{
				instance = AllSpells.GetDto(groupName);
				name = Char.ToLower(name[0]) + name.Substring(1);  // SpellDto events start with a lower case letter.
			}

			if (instance == null)
				return;

			instance.GetType().GetProperty(name).SetValue(instance, code);

			//if (parentGroup.Type == EventType.FeatureEvents)
			//	
			//else if (parentGroup.Type == EventType.SpellEvents)
			//	
		}

		void UpdateSelectedEvent(string text)
		{
			if (activeEventData == null)
				return;
			SetEventCode(activeEventData.ParentGroup, activeEventData.Name, text);
		}

		bool changingInternally;
		PlayerActionShortcut spellToCastOnRoll;
		private void TbxCode_TextChanged(object sender, TextChangedEventArgs e)
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
			ChangeFps(sender, STR_FrontOverlay);
		}

		bool changingFrameRateInternally;

		private void RbnBackFpsChange_Click(object sender, RoutedEventArgs e)
		{
			if (changingFrameRateInternally)
				return;
			ChangeFps(sender, STR_BackOverlay);
		}

		private void RbnDiceFpsChange_Click(object sender, RoutedEventArgs e)
		{
			if (changingFrameRateInternally)
				return;
			ChangeFps(sender, STR_DiceOverlay);
		}

		private void ChangeFps(object sender, string overlayName)
		{
			ChangeFrameRate(overlayName, GetFrameRate(sender));
		}

		private static void ChangeFrameRate(string overlayName, int frameRate)
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
				case STR_BackOverlay:
					switch (frameRate)
					{
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
					}
					break;
				case STR_DiceOverlay:
					switch (frameRate)
					{
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
					}
					break;
				case STR_FrontOverlay:
					switch (frameRate)
					{
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

		bool dynamicThrottling = true;

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

		public void SetThemeVolume(int newVolume)
		{
			SoundCommand soundCommand = new SoundCommand();
			soundCommand.type = SoundCommandType.SetVolume;
			soundCommand.numericData = newVolume;
			Execute(soundCommand);
		}

		public void SetTheme(string newTheme)
		{
			SoundCommand soundCommand = new SoundCommand();
			soundCommand.type = SoundCommandType.ChangeTheme;
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
	}
	// TODO: Reintegrate wand/staff animations....
	/* 
	  Name									Index		Effect				effectAvailableWhen		playToEndOnExpire	 hue	moveUpDown
		Melf's Minute Meteors.6				Staff.Weapon	Casting								x										30	150				
		Melf's Minute Meteors.7				Staff.Magic		Casting								x									 350	150				
	 */
}
