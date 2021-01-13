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
using OBSWebsocketDotNet.Types;
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
using System.Text.RegularExpressions;
using LeapTools;
using TwitchLib.PubSub;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDungeonMasterApp
	{
		PlateManager plateManager;
		WeatherManager weatherManager;
		LeapCalibrator leapCalibrator;
		PlayerStatManager allPlayerStats;
		Dictionary<Character, List<AskUI>> askUIs = new Dictionary<Character, List<AskUI>>();
		//protected const string DungeonMasterChannel = "DragonHumpersDm";
		const string DungeonMasterChannel = "HumperBot";
		const string DragonHumpersChannel = "DragonHumpers";
		const string CodeRushedChannel = "CodeRushed";
		const string twitchIndent = "͏͏͏͏͏͏͏͏͏͏͏͏̣　　͏͏͏̣ 　　͏͏͏̣ ";  // This sequence allows indentation in Twitch chats!


		const int INT_TimeToDropDragonDice = 1800;
		const string STR_PlayerScene = "Players";
		const string STR_RepeatSpell = "[Again]";
		

		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		DungeonMasterChatBot dmChatBot = new DungeonMasterChatBot();
		TwitchClient dungeonMasterClient;
		TwitchClient dhClient;
		TwitchPubSub dhPubSub;

		public PlayerStatManager PlayerStatsManager
		{
			get
			{
				if (allPlayerStats == null)
					allPlayerStats = new PlayerStatManager();
				return allPlayerStats;
			}
		}
		
		List<PlayerActionShortcut> actionShortcuts = new List<PlayerActionShortcut>();
		CardHandManager cardHandManager = new CardHandManager();
		ScrollPage activePage = ScrollPage.main;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer sceneReturnTimer;
		DispatcherTimer delayRollTimer;
		DispatcherTimer playSceneTimer;
		DispatcherTimer reconnectToTwitchDungeonMasterTimer;
		DispatcherTimer reconnectToTwitchDragonHumpersTimer;
		DispatcherTimer showClearButtonTimer;
		DispatcherTimer stateUpdateTimer;
		DispatcherTimer cropRectUpdateTimer;
		DispatcherTimer monsterPreviewImageLoadUpdateTimer;
		DispatcherTimer reloadSpellsTimer;
		DispatcherTimer pendingShortcutsTimer;
		//DispatcherTimer wildMagicRollTimer;
		DispatcherTimer switchBackToPlayersTimer;
		DispatcherTimer updateClearButtonTimer;
		DispatcherTimer actionQueueTimer;

		DateTime lastUpdateTime;
		int keepExistingModifier = int.MaxValue;
		DndGame game = null;

		public MainWindow()
		{
			changingInternally = true;
			try
			{
				InitializeGame();
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				//`! !!!                                                                                      !!!
				//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
				//`! !!!                                                                                      !!!
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				Streamloots.Start(Twitch.Configuration["Secrets:StreamlootsId"]);
			}
			finally
			{
				changingInternally = false;
			}
		}

		void CheckActionQueue(object sender, EventArgs e)
		{
			if (askingQuestion)
				return;
			DeueueNextAction();
		}

		void PlaySceneHandler(object sender, EventArgs e)
		{
			playSceneTimer.Stop();
			if (nextSceneToPlay == null)
				return;
			PlayScene(nextSceneToPlay, nextReturnMs);
			nextSceneToPlay = null;
		}

		private void InitializeGame()
		{
			plateManager = new PlateManager(obsWebsocket);
			game = new DndGame();
			weatherManager = new WeatherManager(obsWebsocket, game);
			DndCore.Validation.ValidationFailed += Validation_ValidationFailed;
			HookGameEvents();
			realTimeAdvanceTimer = new DispatcherTimer(DispatcherPriority.Send);
			realTimeAdvanceTimer.Tick += new EventHandler(RealTimeClockHandler);
			realTimeAdvanceTimer.Interval = TimeSpan.FromMilliseconds(200);

			sceneReturnTimer = new DispatcherTimer(DispatcherPriority.Send);
			sceneReturnTimer.Tick += new EventHandler(ReturnToSceneHandler);

			delayRollTimer = new DispatcherTimer(DispatcherPriority.Send);
			delayRollTimer.Tick += new EventHandler(RollDiceNowHandler);

			playSceneTimer = new DispatcherTimer(DispatcherPriority.Send);
			playSceneTimer.Tick += new EventHandler(PlaySceneHandler);

			reconnectToTwitchDungeonMasterTimer = new DispatcherTimer(DispatcherPriority.Send);
			reconnectToTwitchDungeonMasterTimer.Tick += new EventHandler(ReconnectToTwitchDungeonMasterHandler);
			reconnectToTwitchDungeonMasterTimer.Interval = TimeSpan.FromMilliseconds(1000);

			reconnectToTwitchDragonHumpersTimer = new DispatcherTimer(DispatcherPriority.Send);
			reconnectToTwitchDragonHumpersTimer.Tick += new EventHandler(ReconnectToTwitchDragonHumpersHandler);
			reconnectToTwitchDragonHumpersTimer.Interval = TimeSpan.FromMilliseconds(1000);

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

			monsterPreviewImageLoadUpdateTimer = new DispatcherTimer();
			monsterPreviewImageLoadUpdateTimer.Tick += new EventHandler(CheckMonsterPreviewImageLoadFromTimer);
			monsterPreviewImageLoadUpdateTimer.Interval = TimeSpan.FromSeconds(0.1);

			reloadSpellsTimer = new DispatcherTimer();
			reloadSpellsTimer.Tick += new EventHandler(CheckForNewSpells);
			reloadSpellsTimer.Interval = TimeSpan.FromSeconds(1.2);

			pendingShortcutsTimer = new DispatcherTimer();
			pendingShortcutsTimer.Tick += new EventHandler(ActivatePendingShortcuts);
			pendingShortcutsTimer.Interval = TimeSpan.FromSeconds(1);

			//wildMagicRollTimer = new DispatcherTimer();
			//wildMagicRollTimer.Tick += new EventHandler(RollWildMagicHandler);
			//wildMagicRollTimer.Interval = TimeSpan.FromSeconds(9);

			switchBackToPlayersTimer = new DispatcherTimer();
			switchBackToPlayersTimer.Tick += new EventHandler(SwitchBackToPlayersHandler);
			switchBackToPlayersTimer.Interval = TimeSpan.FromSeconds(3);

			updateClearButtonTimer = new DispatcherTimer(DispatcherPriority.Send);
			updateClearButtonTimer.Tick += new EventHandler(UpdateClearButton);
			updateClearButtonTimer.Interval = TimeSpan.FromMilliseconds(80);

			actionQueueTimer = new DispatcherTimer(DispatcherPriority.Send);
			actionQueueTimer.Tick += new EventHandler(CheckActionQueue);
			actionQueueTimer.Interval = TimeSpan.FromSeconds(2);

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
			CreateDragonHumpersClient();
			CreateDhPubSub();

			dmChatBot.Initialize(this);

			dmChatBot.DungeonMasterApp = this;
			commandParsers.Add(dmChatBot);
			ConnectToObs();
			HookEvents();

			SetupSpellsChangedFileWatcher();
			LoadAvalonEditor();

			actionQueueTimer.Start();
			lbActionStack.ItemsSource = actionQueue;
			LoadEverything();
		}

		private void CreateDhPubSub()
		{
			dhPubSub = new TwitchPubSub();
			dhPubSub.OnRewardRedeemed += DhPubSub_OnRewardRedeemed;
			dhPubSub.ListenToRewards("CodeRushed");
			dhPubSub.Connect();
		}

		private void DhPubSub_OnRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
		{

		}

		// ResendExistingData

		void ReconnectToTwitchDungeonMasterHandler(object sender, EventArgs e)
		{
			CreateDungeonMasterClient();
		}

		void ReconnectToTwitchDragonHumpersHandler(object sender, EventArgs e)
		{
			CreateDragonHumpersClient();
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
			game.RequestQueueShortcut += Game_RequestQueueShortcut;
			game.RequestMessageToAll += Game_RequestMessageToAll;
			game.PlayerRequestsRoll += Game_PlayerRequestsRoll;
			game.PlayerStateChanged += Game_PlayerStateChanged;
			game.PlayerDamaged += Game_PlayerDamaged;
			game.RoundStarting += Game_RoundStarting;
			game.ActivePlayerChanged += Game_ActivePlayerChanged;
			game.ConcentratedSpellChanged += Game_ConcentratedSpellChanged;
		}

		private void Game_PlayerDamaged(object sender, CreatureDamagedEventArgs ea)
		{
			if (ea.Creature is Character player)
				if (player.concentratedSpell != null)
				{
					EnqueueBreakSpellConcentrationSavingThrow(player.playerID, ea.DamageAmount);
					// TODO: Trigger the saving throw roll to see if they break concentration with the spell.
				}
		}

		private void Game_PlayerShowState(object sender, PlayerShowStateEventArgs ea)
		{
			if (ea.DelayMs > 0)
			{
				DispatcherTimer delayFloatTextTimer = new DispatcherTimer(DispatcherPriority.Send);
				delayFloatTextTimer.Interval = TimeSpan.FromMilliseconds(ea.DelayMs);
				ea.DelayMs = 0;
				delayFloatTextTimer.Tick += new EventHandler(DelayFloatTextNow);
				delayFloatTextTimer.Tag = ea;
				delayFloatTextTimer.Start();
				return;
			}

			string outlineColor = ea.OutlineColor;
			string fillColor = ea.FillColor;
			if (outlineColor == "player")
				outlineColor = ea.Player.dieFontColor;
			if (fillColor == "player")
				fillColor = ea.Player.dieBackColor;
			HubtasticBaseStation.FloatPlayerText(ea.Player.playerID, ea.Message, fillColor, outlineColor);
		}

		void DelayFloatTextNow(object sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();

			if (!(dispatcherTimer.Tag is PlayerShowStateEventArgs ea))
				return;

			SafeInvoke(() =>
			{
				Game_PlayerShowState(this, ea);
			});
		}


		void SetObsSourceVisibiltyNow(object sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();

			if (!(dispatcherTimer.Tag is SetObsSourceVisibilityEventArgs ea))
				return;
			SetObsSourceVisibilityFunction_RequestSetObsSourceVisibility(this, ea);
		}

		private void Game_RoundStarting(object sender, DndGameEventArgs ea)
		{
			if (game.InCombat)
				clockMessage = $"Round {ea.Game.RoundIndex + 1}";
		}

		private async void Game_PickWeapon(object sender, PickWeaponEventArgs ea)
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
			int result = await AskQuestion("Target which weapon: ", weapons);
			if (result > 0)
				ea.Weapon = filteredWeapons[result - 1];
		}

		private async void Game_PickAmmunition(object sender, PickAmmunitionEventArgs ea)
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
			{
				// TODO: Show player is out of ammunition.
				return;
			}
			int result = await AskQuestion("Choose ammunition: ", ammunitionList);
			if (result > 0)
				ea.Ammunition = filteredAmmunition[result - 1];
		}

		private void HookEvents()
		{
			Expressions.ExceptionThrown += Expressions_ExceptionThrown;
			Expressions.ExecutionChanged += Expressions_ExecutionChanged;
			AskFunction.AskQuestion += AskFunction_AskQuestion;  // static event handler.
			TriggerEffect.RequestEffectTrigger += TriggerEffect_RequestEffectTrigger;
			GetRoll.GetRollRequest += GetRoll_GetRollRequest;
			Feature.FeatureDeactivated += Feature_FeatureDeactivated;
			Feature.RequestMessageToDungeonMaster += Game_RequestMessageToDungeonMaster;
			AddReminderFunction.AddReminderRequest += AddReminderFunction_AddReminderRequest;
			ActivateShortcutFunction.ActivateShortcutRequest += ActivateShortcutFunction_ActivateShortcutRequest;
			DndCharacterProperty.AskingValue += DndCharacterProperty_AskingValue;
			PlaySceneFunction.RequestPlayScene += PlaySceneFunction_RequestPlayScene;
			SetObsSourceVisibilityFunction.RequestSetObsSourceVisibility += SetObsSourceVisibilityFunction_RequestSetObsSourceVisibility;
			SelectTargetFunction.RequestSelectTarget += SelectTargetFunction_RequestSelectTarget;
			SelectMonsterFunction.RequestSelectMonster += SelectMonsterFunction_RequestSelectMonster;
			GetNumTargets.RequestTargetCount += GetNumTargets_RequestTargetCount;
			AddWindupFunction.RequestAddWindup += SendWindupFunction_SendWindup;
			ClearWindupFunction.RequestClearWindup += ClearWindup_RequestClearWindup; ;
			StreamlootsService.CardRedeemed += StreamlootsService_CardRedeemed;
			StreamlootsService.CardsPurchased += StreamlootsService_CardsPurchased;
		}

		private void TriggerEffect_RequestEffectTrigger(object sender, EffectEventArgs ea)
		{
			EffectGroup effectGroup = new EffectGroup();

			VisualEffectTarget chestTarget, bottomTarget;
			GetTargets(out chestTarget, out bottomTarget);
			AnimationEffect animationEffect = AnimationEffect.CreateEffect(ea.EffectName, bottomTarget, ea.Hue, ea.Saturation, ea.Brightness, ea.SecondaryHue, ea.SecondarySaturation, ea.SecondaryBrightness, ea.OffsetX, ea.OffsetY, ea.VelocityX, ea.VelocityY);
			animationEffect.autoRotation = ea.AutoRotation;
			animationEffect.rotation = ea.Rotation;
			animationEffect.scale = ea.Scale;
			animationEffect.timeOffsetMs = ea.TimeOffset;
			effectGroup.Add(animationEffect);

			string serializedObject = JsonConvert.SerializeObject(effectGroup);
			HubtasticBaseStation.TriggerEffect(serializedObject);
		}

		private void GetNumTargets_RequestTargetCount(object sender, TargetCountEventArgs ea)
		{
			int playerCount = 0;
			if (ea.TargetStatus.HasFlag(TargetStatus.Friendly))
				playerCount = GetNumPlayersTargeted();
			ea.Count = playerCount + AllInGameCreatures.GetTargetCount(ea.TargetStatus);
		}

		int GetNumPlayersTargeted()
		{
			return allPlayerStats.Players.Count(p => p.IsTargeted);
		}

		private void SetObsSourceVisibilityFunction_RequestSetObsSourceVisibility(object sender, SetObsSourceVisibilityEventArgs ea)
		{
			if (ea.DelaySeconds > 0)
			{
				DispatcherTimer delayFloatTextTimer = new DispatcherTimer(DispatcherPriority.Send);
				delayFloatTextTimer.Interval = TimeSpan.FromSeconds(ea.DelaySeconds);
				ea.DelaySeconds = 0;
				delayFloatTextTimer.Tick += new EventHandler(SetObsSourceVisibiltyNow);
				delayFloatTextTimer.Tag = ea;
				delayFloatTextTimer.Start();
				return;
			}

			try
			{
				obsWebsocket.SetSourceRender(ea.SourceName, ea.Visible, ea.SceneName);
			}
			catch (Exception ex)
			{

			}
		}

		private void ClearWindup_RequestClearWindup(object sender, NameEventArgs ea)
		{
			HubtasticBaseStation.ClearWindup(ea.Name);
		}

		private void SelectMonsterFunction_RequestSelectMonster(object sender, SelectMonsterEventArgs ea)
		{
			if (ActivePlayer != null && !string.IsNullOrEmpty(ActivePlayer.NextAnswer))
			{
				ea.Monster = AllMonsters.GetByKind(ActivePlayer.NextAnswer);
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
			bool foundAny = false;
			ea.Target = new Target();
			ea.Target.PlayerIds = new List<int>();
			foreach (PlayerStats playerStats in PlayerStatsManager.Players)
			{
				if (playerStats.IsTargeted)
				{
					ea.Target.PlayerIds.Add(playerStats.PlayerId);
					foundAny = true;
				}
			}
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.IsTargeted)
				{
					ea.Target.AddCreature(inGameCreature.Creature);
					foundAny = true;
				}
			}
			if (foundAny || !ea.ShowUI)
				return;

			//Old_SelectInGameTargets(ea);
			AskQuestionBlockUI("Select Target:", GetTargetAnswers(ea.TargetStatus, ea.MaxTargets), 1, ea.MaxTargets);
			if (lastRemoteAnswers != null)
				foreach (AnswerEntry answerEntry in lastRemoteAnswers)
				{
					if (answerEntry.IsSelected)
					{
						if (answerEntry.Value >= 0)
						{
							ea.Target.PlayerIds.Add(answerEntry.Value);
						}
						else
						{ // It's a in-game creature
							ea.Target.Creatures.Add(AllInGameCreatures.GetByIndex(-answerEntry.Value)?.Creature);

						}
					}
				}

		}

		List<AnswerEntry> GetTargetAnswers(TargetStatus targetStatus, int maxSelected)
		{
			int numSelected = 0;
			List<AnswerEntry> result = new List<AnswerEntry>();
			if (targetStatus.HasFlag(TargetStatus.Friendly))
				foreach (PlayerStats playerStats in allPlayerStats.Players)
				{
					AnswerEntry answer = new AnswerEntry(result.Count, playerStats.PlayerId, AllPlayers.GetFromId(playerStats.PlayerId).Name);
					result.Add(answer);
					if (result.Count < maxSelected && playerStats.IsTargeted)
					{
						numSelected++;
						answer.IsSelected = true;
					}
				}

			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (!inGameCreature.OnScreen)
					continue;

				if (targetStatus.HasFlag(TargetStatus.Friendly) && inGameCreature.IsAlly ||
					targetStatus.HasFlag(TargetStatus.Adversarial) && inGameCreature.IsEnemy ||
					targetStatus.HasFlag(TargetStatus.AllTargets))
				{
					AnswerEntry answer = new AnswerEntry(result.Count, InGameCreature.GetUniversalIndex(inGameCreature.Index), inGameCreature.Name);
					result.Add(answer);
					if (numSelected < maxSelected && inGameCreature.IsTargeted)
					{
						numSelected++;
						answer.IsSelected = true;
					}
				}
			}
			return result;
		}

		//private void Old_SelectInGameTargets(TargetEventArgs ea)
		//{
		//	FrmSelectInGameCreature frmSelectInGameCreature = new FrmSelectInGameCreature();

		//	frmSelectInGameCreature.SetDataSources(AllInGameCreatures.Creatures, game.Players);

		//	if (frmSelectInGameCreature.ShowDialog() == true)
		//	{
		//		ea.Target = new Target();
		//		foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
		//			if (inGameCreature.OnScreen)
		//				ea.Target.AddCreature(inGameCreature.Creature);
		//		foreach (Character player in game.Players)
		//			if (player.IsSelected)
		//				ea.Target.AddCreature(player);
		//	}
		//}

		// TODO: Consider stacking scene play requests.
		string nextSceneToPlay;
		int nextReturnMs;

		void PlaySceneAfter(string sceneName, int delayMs, int returnMs = -1)
		{
			playSceneTimer.Stop();
			nextSceneToPlay = sceneName;
			nextReturnMs = returnMs;
			playSceneTimer.Interval = TimeSpan.FromMilliseconds(delayMs);
			playSceneTimer.Start();
		}

		private void PlaySceneFunction_RequestPlayScene(object sender, PlaySceneEventArgs ea)
		{
			if (ea.DelayMs > 0)
				PlaySceneAfter(ea.SceneName, ea.DelayMs, ea.ReturnMs);
			else
				PlayScene(ea.SceneName, ea.ReturnMs);
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
				SafeInvoke(() =>
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
			lock (savedRolls)
				if (savedRolls.ContainsKey(ea.RollName))
					ea.Result = savedRolls[ea.RollName];
		}

		void SaveNamedResults(DiceEventArgs ea)
		{
			if (savedRolls == null)
				savedRolls = new Dictionary<string, int>();
			lock (savedRolls)
			{
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
			SafeInvoke(() =>
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
			else if (ea.IsRechargeable)
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
					UpdatePlayerScrollInGame(player);

				SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
			{
				reconnectToTwitchDungeonMasterTimer.Stop();
				Background = Brushes.White;
				btnReconnectTwitchClient.Visibility = Visibility.Hidden;
			});
			dungeonMasterClient = Twitch.CreateNewClient("DragonHumpersDM", "DragonHumpersDM", "DragonHumpersDmOAuthToken");
			HookTwitchClientDungeonMasterEvents();
		}

		private void CreateDragonHumpersClient()
		{
			SafeInvoke(() =>
			{
				reconnectToTwitchDungeonMasterTimer.Stop();
				Background = Brushes.White;
				btnReconnectTwitchClient.Visibility = Visibility.Hidden;
			});
			dhClient = Twitch.CreateNewClient("DragonHumpers", "DragonHumpers", "DragonHumpersOAuthToken");
			HookTwitchClientDragonHumpersEvents();
		}

		private void HookTwitchClientDungeonMasterEvents()
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

		private void HookTwitchClientDragonHumpersEvents()
		{
			if (dhClient == null)
				return;
			dhClient.OnMessageReceived += DragonHumpersClient_OnMessageReceived;
			dhClient.OnDisconnected += DragonHumpersClient_OnDisconnected;
		}

		private void UnhookTwitchClientDragonHumpersEvents()
		{
			if (dhClient == null)
				return;
			dhClient.OnMessageReceived -= DragonHumpersClient_OnMessageReceived;
			dhClient.OnDisconnected -= DragonHumpersClient_OnDisconnected;
		}

		private void UnhookTwitchClientDungeonMasterEvents()
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
			if (sendTwitchChannelMessagesToHistory)
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

		bool sendTwitchLogMessagesToHistory;
		bool sendMessageSendsToHistory;
		bool sendTwitchChannelMessagesToHistory;
		private void DungeonMasterClient_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
		{
			if (sendTwitchLogMessagesToHistory)
				History.Log($"BotUsername (\"{e.BotUsername}\") logged ({e.Data})");
		}

		private void DungeonMasterClient_OnLeftChannel(object sender, TwitchLib.Client.Events.OnLeftChannelArgs e)
		{
			if (sendTwitchChannelMessagesToHistory)
				History.Log($"User (\"{e.BotUsername}\") left channel ({e.Channel})");
		}

		private void DungeonMasterClient_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
		{
			if (sendTwitchChannelMessagesToHistory)
				History.Log($"User (\"{e.BotUsername}\") joined channel ({e.Channel})");
		}

		private void DungeonMasterClient_OnFailureToReceiveJoinConfirmation(object sender, TwitchLib.Client.Events.OnFailureToReceiveJoinConfirmationArgs e)
		{
			if (sendTwitchChannelMessagesToHistory)
				History.Log($"Channel (\"{e.Exception.Channel}\") FailureToReceiveJoinConfirmation ({e.Exception.Details})");
		}

		private void DungeonMasterClient_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
		}

		void SafeInvoke(Action action)
		{
			if (uiThreadSleepingWhileWaitingForAnswerToQuestion)
				return;  // Sorry kids, but we have to get out of here. Otherwise we will lock!

			Dispatcher.Invoke(() =>
			{
				action();
			});
		}

		private void DungeonMasterClient_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
		{
			SafeInvoke(() =>
			{
				Background = new SolidColorBrush(Color.FromRgb(148, 81, 81));
				btnReconnectTwitchClient.Visibility = Visibility.Visible;
				reconnectToTwitchDungeonMasterTimer.Start();
			});

			UnhookTwitchClientDungeonMasterEvents();
			dungeonMasterClient = null;
			History.Log($"DungeonMasterClient_OnDisconnected");
		}

		private void DragonHumpersClient_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
		{
			SafeInvoke(() =>
			{
				Background = new SolidColorBrush(Color.FromRgb(81, 88, 148));
				reconnectToTwitchDragonHumpersTimer.Start();
			});

			UnhookTwitchClientDragonHumpersEvents();
			dhClient = null;
			History.Log($"DragonHumpersClient_OnDisconnected");
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
					PlayScene(STR_PlayerScene);
			}
		}

		List<AnswerEntry> prebuiltAnswers;

		async Task<int> AskQuestion(string question, List<string> answers)
		{
			bool timerWasRunning = realTimeAdvanceTimer.IsEnabled;
			if (timerWasRunning)
				realTimeAdvanceTimer.Stop();
			uiThreadSleepingWhileWaitingForAnswerToQuestion = true;
			try
			{
				AnswerEntry selectedAnswer = GetNextAnswer(answers);

				if (selectedAnswer != null)
					return selectedAnswer.Value;

				return await AskQuestionAsync(question, prebuiltAnswers);
			}
			finally
			{
				uiThreadSleepingWhileWaitingForAnswerToQuestion = false;
				prebuiltAnswers = null;
				if (timerWasRunning)
					StartRealTimeTimer();
			}
		}

		private AnswerEntry GetNextAnswer(List<string> answers)
		{
			BuildAnswerMap(answers);

			if (ActivePlayer == null || string.IsNullOrEmpty(ActivePlayer.NextAnswer))
				return null;
			AnswerEntry selectedAnswer = GetNextAnswer(prebuiltAnswers);
			return selectedAnswer;
		}

		private AnswerEntry GetNextAnswer(List<AnswerEntry> answers)
		{
			AnswerEntry selectedAnswer;
			if (ActivePlayer.NextAnswer == null)
				return null;
			if (ActivePlayer.NextAnswer.EndsWith("*"))
			{
				string searchPattern = ActivePlayer.NextAnswer.EverythingBefore("*");
				selectedAnswer = answers.FirstOrDefault(x => x.AnswerText.StartsWith(searchPattern));
			}
			else
				selectedAnswer = answers.FirstOrDefault(x => x.AnswerText == ActivePlayer.NextAnswer);
			ActivePlayer.NextAnswer = null;
			return selectedAnswer;
		}

		bool askingQuestion;
		// TODO: Set this to false when we get InGameUIResponse so NewAskQuestion exits.

		int answerResponse;
		async Task<int> AskQuestionAsync(string question, List<AnswerEntry> answers, int minAnswers = 1, int maxAnswers = 1)
		//int NewAskQuestion(string question, List<AnswerEntry> answers, int minAnswers = 1, int maxAnswers = 1)
		{
			askingQuestion = true;
			HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap(question, answers, minAnswers, maxAnswers));
			while (askingQuestion)
			{
				// TODO: Check to see if we lose SignalR so we don't infinite loop here. This can happen when debugging at a breakpoint for a long time it seems.
				await Task.Delay(300);
			}
			return answerResponse;
		}

		//! It seems like we need to call AskQuestionBlockUI any time we are answering a question from a Script.
		private int AskQuestionBlockUI(string question, List<AnswerEntry> answerEntries, int minTargets = 1, int maxTargets = 1)
		{
			AnswerEntry selectedAnswer = GetNextAnswer(answerEntries);

			if (selectedAnswer != null)
				return selectedAnswer.Value;

			askingQuestion = true;
			uiThreadSleepingWhileWaitingForAnswerToQuestion = true;
			HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap(question, answerEntries, minTargets, maxTargets));
			int count = 0;
			const int SleepMs = 300;
			const int TimeoutSec = 15;
			const int maxSleeps = 1000 * TimeoutSec / SleepMs;
			while (askingQuestion && count < maxSleeps)
			{
				// TODO: Check to see if we lose SignalR so we don't infinite loop here. This can happen when debugging at a breakpoint for a long time it seems.
				Thread.Sleep(SleepMs);
				count++;
			}
			uiThreadSleepingWhileWaitingForAnswerToQuestion = false;
			if (count == maxSleeps)
			{
				// We timed out.
				HubtasticBaseStation.InGameUICommand("OK");
			}
			return answerResponse;
		}

		private void BuildAnswerMap(List<string> answers)
		{
			prebuiltAnswers = new List<AnswerEntry>();
			List<string> textAnswers = new List<string>();
			int index = 1;
			bool firstTimeIn = true;
			foreach (string answer in answers)
			{
				if (firstTimeIn && answer.ToLower().IndexOf("zero") >= 0)
					index = 0;
				firstTimeIn = false;

				AnswerEntry thisAnswer = AnswerEntry.FromAnswer(answer, index);
				prebuiltAnswers.Add(thisAnswer);
				textAnswers.Add($"{index}. " + thisAnswer.AnswerText);
				index++;
			}
		}

		private void AskFunction_AskQuestion(object sender, AskEventArgs ea)
		{
			ea.Result = AskQuestionBlockUI(ea.Question, GetAnswerEntries(ea.Answers));
		}

		List<AnswerEntry> GetAnswerEntries(List<string> answers)
		{
			int index = 0;
			List<AnswerEntry> result = new List<AnswerEntry>();

			foreach (string answer in answers)
			{
				Match match = Regex.Match(answer, "\"(\\d+):(.+)\"");
				if (match.Success)
				{
					string trimmedAnswer = match.Groups[2].Value;
					int value = int.Parse(match.Groups[1].Value);
					result.Add(new AnswerEntry(index, value, trimmedAnswer));
				}
				else
					result.Add(new AnswerEntry(index, index, answer));
				index++;
			}

			return result;

		}

		private void Expressions_ExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
		{
			MessageBox.Show(ea.Ex.Message, "Unhandled Exception");
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected)
				return;
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
			{
				tbxSaveThreshold.Text = hiddenThreshold.ToString();
			});

			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{hiddenThreshold} {twitchIndent} <-- hidden SAVE threshold");
		}

		object lockObj;

		bool IsPlayer(string userId)
		{
			const string karen = "240735151";
			const string mark = "270998178";
			const string lara = "496519211";
			const string zephyr = "238257153";
			const string brendan = "491566796";
			const string dm = "455518839";
			return userId == karen || userId == mark || userId == lara || userId == zephyr || userId == brendan || userId == dm;
		}
		private void DragonHumpersClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			if (IsPlayer(e.ChatMessage.UserId))
				CharacterSaysSomething(e.ChatMessage.Message);
		}

		private static bool CharacterSaysSomething(string message)
		{
			if (!message.StartsWith("!") || !message.Contains(":"))
				return false;
			string textColor = string.Empty;
			message = message.Substring(1, message.Length - 1);
			int colonPos = message.IndexOf(":");
			if (colonPos > 0)
			{
				string playerName = message.Substring(0, colonPos);
				int playerId = AllPlayers.GetPlayerIdFromName(playerName);
				if (playerId == 100) // DM
				{
					textColor = "(#4b107c)";
				}
				if (playerId < 0)
				{
					InGameCreature creature = AllInGameCreatures.GetActiveCreatureByFirstName(playerName);
					if (creature == null)
						return false;
					playerId = -creature.Index;
				}
				else
				{
					Character player = AllPlayers.GetFromId(playerId);
					if (player != null)
					{
						textColor = $"({player.bubbleTextColor})";
					}
				}
				message = message.Substring(colonPos + 1).Trim();
				string speechCommand = null;
				if (message.StartsWith("\""))
				{
					speechCommand = "says";
					message = message.Trim('"');
				}
				else if (message.StartsWith("("))
				{
					speechCommand = "thinks";
					message = message.TrimStart('(');
					if (!message.Contains("("))
						message = message.TrimEnd(')');
				}
				if (DateTime.Now.Hour < 16)
				{
					ProfanityFilter.ProfanityFilter profanityFilter = new ProfanityFilter.ProfanityFilter();
					message = profanityFilter.CensorString(message);
				}
				if (speechCommand != null)
				{
					HubtasticBaseStation.SpeechBubble($"{playerId}{textColor} {speechCommand}: {message}");
					return true;
				}
			}
			return false;
		}

		private void HumperBotClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			//UnhookTwitchClientDungeonMasterEvents();
			//HookTwitchClientDungeonMasterEvents();
			string message = e.ChatMessage.Message;
			if (IsPlayer(e.ChatMessage.UserId))
				if (CharacterSaysSomething(message))
					return;

			//Thread thread = Thread.CurrentThread;
			//string msg;
			//lock (lockObj)
			//{
			//	msg = String.Format("Thread ID: {0}\n", thread.ManagedThreadId) +
			//				String.Format("   Background: {0}\n", thread.IsBackground) +
			//				String.Format("   Thread Pool: {0}\n", thread.IsThreadPoolThread) +
			//				String.Format("   Thread ID: {0}\n", thread.ManagedThreadId);
			//}

			if (uiThreadSleepingWhileWaitingForAnswerToQuestion && int.TryParse(message.Trim(), out int result) && prebuiltAnswers != null)
			{
				AnswerEntry answer = prebuiltAnswers.FirstOrDefault(x => x.Index == result);
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
			SafeInvoke(() =>
			{
				History.UpdateQueuedEntries();
				logListBox.SelectedIndex = logListBox.Items.Count - 1;
				logListBox.ScrollIntoView(logListBox.SelectedItem);
			});
		}

		int activePlayerId;
		public int ActivePlayerId
		{
			get
			{
				return activePlayerId;
			}
			set
			{
				if (activePlayerId == value)
					return;

				activePlayerId = value;
				ShowTabForPlayer(activePlayerId);
			}
		}

		private void ShowTabForPlayer(int playerId)
		{
			SafeInvoke(() =>
			{
				if (tabPlayers.SelectedItem is PlayerTabItem selectedPlayerTab)
				{
					if (selectedPlayerTab.PlayerId == playerId)
						return;
				}

				foreach (object item in tabPlayers.Items)
					if (item is PlayerTabItem playerTabItem && playerTabItem.PlayerId == playerId)
					{
						tabPlayers.SelectedItem = playerTabItem;
						return;
					}
			});
		}

		public Character ActivePlayer
		{
			get
			{
				return activePlayer;
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
			SetPlayerVantageUI(ActivePlayerId, vantageMod);
		}

		public void SetPlayerVantageUI(int playerId, VantageKind vantageMod)
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

			ClearSpellWindupsInGame(ea.CastedSpell.Spell, ea.CastedSpell.SpellCaster.playerID);

			string spellToEnd = DndGame.GetSpellPlayerName(ea.CastedSpell.Spell, ea.CastedSpell.SpellCaster.playerID);
			EndSpellEffects(spellToEnd);

			if (ea.CastedSpell.Spell.RequiresConcentration && ea.CastedSpell.SpellCaster.concentratedSpell != null && ea.CastedSpell.Spell != ea.CastedSpell.SpellCaster.concentratedSpell.Spell)
			{
				ea.CastedSpell.SpellCaster.concentratedSpell = null;
				PlayerStats playerStats = PlayerStatsManager.GetPlayerStats(ea.CastedSpell.SpellCaster.playerID);
				playerStats.PercentConcentrationComplete = 100;
				playerStats.ConcentratedSpell = "";
				playerStats.ConcentratedSpellDurationSeconds = 0;
				playerStats.JustBrokeConcentration = false;
				UpdatePlayerStatsInGame();
			}

			UpdateStateUIForPlayer(ActivePlayer, true);
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

		void ClearSpellWindupsInGame(Spell spell, int playerId)
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
		string activeSpellName;
		CastedSpell castedSpellNeedingCompletion = null;
		Character spellCaster = null;

		private void ActivateShortcut(string shortcutName)
		{
			PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText == shortcutName && x.PlayerId == ActivePlayerId);
			if (shortcut == null && tbTabs.SelectedItem == tbDebug)
				shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText.StartsWith(shortcutName) && x.PlayerId == ActivePlayerId);
			if (shortcut != null)
				SafeInvoke(() =>
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
			if (actionShortcut == null)
				return;
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
			else if (diceRoll.RollScope == RollScope.ActiveInGameCreature)  // CodeBaseAlpha was right. I added this line of code.
				rbActiveNpc.IsChecked = true;  // <---- And this line of code.
			else if (diceRoll.RollScope == RollScope.TargetedInGameCreatures)  // CodeBaseAlpha was right. I added this line of code.
				rbTargetedNpcs.IsChecked = true;  // <---- And this line of code.
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

		bool settingAttackRadioButtonInternally;

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
					settingAttackRadioButtonInternally = true;
					try
					{
						rbAttack.IsChecked = true;
					}
					finally
					{
						settingAttackRadioButtonInternally = false;
					}
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
					if (actionShortcut.Spell.BonusThreshold != null && actionShortcut.Spell.BonusThreshold.StartsWith("c"))
					{
						string className = "character";
						DndCore.CharacterClass firstMatchingClass = player.FirstSpellCastingClass();
						if (firstMatchingClass != null)
						{
							className = firstMatchingClass.Name;
							TellAll($"{player.firstName} is ready to cast {actionShortcut.Spell.Name} as a level-{firstMatchingClass.Level} {className}...");
						}
						else
							TellAll($"{player.firstName} is ready to cast {actionShortcut.Spell.Name}...");
					}
					else
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

		void SetInGameDiceForShortcut(PlayerActionShortcut actionShortcut)
		{
			PlayerStatsManager.ClearReadyToRollState();

			if (actionShortcut.Type == DiceRollType.Attack || actionShortcut.Type == DiceRollType.ChaosBolt)
			{
				DieRollDetails dieRollDetails = new DieRollDetails();
				dieRollDetails.AddRoll("1d20");
				if (actionShortcut.Spell != null)
					dieRollDetails.AddRoll(actionShortcut.Spell.DieStr);
				else
				{
					dieRollDetails.AddRoll(actionShortcut.Dice);
				}
				PlayerStatsManager.SetReadyRollDice(actionShortcut.PlayerId, true, dieRollDetails);
			}
			else if (actionShortcut.Type == DiceRollType.DamageOnly)
			{
				if (actionShortcut.Spell != null)
				{
					string dieStr = actionShortcut.Spell.DieStr;
					if (!string.IsNullOrWhiteSpace(dieStr))
					{
						DieRollDetails dieRollDetails = null;
						if (actionShortcut.Spell != null)
							dieRollDetails = DieRollDetails.From(actionShortcut.Spell.DieStr);
						else
						{
							System.Diagnostics.Debugger.Break();
						}
						PlayerStatsManager.SetReadyRollDice(actionShortcut.PlayerId, true, dieRollDetails);
					}
				}
			}

			UpdatePlayerStatsInGame();
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
				SetInGameDiceForShortcut(actionShortcut);
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
			activeSpellName = null;
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

		private void SendWindupFunction_SendWindup(object sender, WindupEventArgs ea)
		{
			List<WindupDto> windups = new List<WindupDto>();
			windups.Add(ea.WindupDto);
			string serializedObject = JsonConvert.SerializeObject(windups);
			HubtasticBaseStation.AddWindup(serializedObject);
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

		private async Task<bool> ActivateSpellShortcut(PlayerActionShortcut actionShortcut, bool forceRepeat = false)
		{
			CastedSpell castedSpell = new CastedSpell(actionShortcut.Spell, ActivePlayer);

			PlayerSpellCastState playerSpellCastState = await GetPlayerSpellCastState(ActivePlayer, castedSpell);

			if (playerSpellCastState == PlayerSpellCastState.ValidationFailed)
				return false;

			if (playerSpellCastState == PlayerSpellCastState.AlreadyCasting && !forceRepeat)
				return false;

			if (playerSpellCastState == PlayerSpellCastState.Concentrating)
				return false;



			if (castedSpell.Target == null && ActivePlayer != null)
				castedSpell.Target = ActivePlayer.ActiveTarget;

			bool castingForFirstTimeNow = playerSpellCastState != PlayerSpellCastState.AlreadyCasting || !forceRepeat;

			if (castingForFirstTimeNow)
				castedSpell.PreparationStarted(game);  // Triggers onPreparing event here.

			if (castingForFirstTimeNow && actionShortcut.Spell.CastingTime > DndTimeSpan.OneAction)
			{
				CastingSpellSoon(actionShortcut.Spell.CastingTime, actionShortcut);
				return true;
			}
			Spell spell = CastSpellNow(actionShortcut);
			if (spell != null)
				tbxDamageDice.Text = spell.DieStr;

			return true;
		}

		private Spell CastSpellNow(PlayerActionShortcut actionShortcut)
		{
			Character player = GetPlayer(actionShortcut.PlayerId);
			Spell spell = actionShortcut.Spell;
			activeSpellName = spell.Name;
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
				ShowSpellCastEffectsInGame(actionShortcut.PlayerId, actionShortcut.Spell.Name);
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
			if (matchingSpell.ChargesRemaining > 0)
				matchingSpell.ChargesRemaining--;
			if (matchingSpell.ChargesRemaining == 0)
				HideShortcutUI(actionShortcut);

		}

		private void CastActionSpell(PlayerActionShortcut actionShortcut, Character player, Spell spell)
		{
			PrepareToCastSpell(spell, actionShortcut.PlayerId);

			CastedSpell castedSpell = game.Cast(player, spell);
			if (castedSpell == null)
				return;

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

		private async Task<PlayerSpellCastState> GetPlayerSpellCastState(Character player, CastedSpell castedSpell)
		{
			if (castedSpell.ValidationHasFailed(player, castedSpell.Target))
				return PlayerSpellCastState.ValidationFailed;

			PlayerSpellCastState playerSpellState = PlayerSpellCastState.FreeToCast;
			if (castedSpell.Spell.RequiresConcentration && player.concentratedSpell != null)
			{
				Spell concentratedSpell = player.concentratedSpell.Spell;

				try
				{
					if (game.ClockIsRunning())
						realTimeAdvanceTimer.Stop();
					if (concentratedSpell.Name == castedSpell.Spell.Name)
					{
						// TODO: Provide feedback that we are already casting this spell and it has game.GetSpellTimeLeft(player.playerID, concentratedSpell).
						TimeSpan remainingSpellTime = game.GetRemainingSpellTime(player.playerID, concentratedSpell);
						if (remainingSpellTime.TotalSeconds > 0)
						{
							TellDungeonMaster($"{player.firstName} is already casting {concentratedSpell.Name} ({game.GetRemainingSpellTimeStr(player.playerID, concentratedSpell)} remaining)");
							return PlayerSpellCastState.AlreadyCasting;
						}
						else  // No time remaining. No longer concentrating on this spell.
						{
							return PlayerSpellCastState.FreeToCast;
						}
					}

					// At this point, we know the player is concentrating on a spell and the player wants to cast a different spell that also requires concentration
					int result = await AskQuestion($"Break concentration with {concentratedSpell.Name} ({game.GetRemainingSpellTimeStr(player.playerID, concentratedSpell)} remaining) to cast {castedSpell.Spell.Name}?", new List<string>() { "1:Yes", "0:No" });
					if (result == 1)
						playerSpellState = PlayerSpellCastState.FreeToCast;
					else
						playerSpellState = PlayerSpellCastState.Concentrating;
				}
				finally
				{
					if (game.ClockIsRunning())
						StartRealTimeTimer();
				}
			}

			return playerSpellState;
		}

		public enum PlayerSpellCastState
		{
			ValidationFailed,
			FreeToCast,
			Concentrating,
			AlreadyCasting
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

		Character activePlayer;

		private void TabControl_PlayerChanged(object sender, SelectionChangedEventArgs e)
		{
			if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
			{
				activePlayerId = playerTabItem.PlayerId;
				activePlayer = game.GetPlayerFromId(playerTabItem.PlayerId);
			}

			btnRollDice.IsEnabled = true;
			ActivatePendingShortcuts();
			if (buildingTabs)
				return;
			if (rbActivePlayer.IsChecked == true)
			{
				CheckOnlyOnePlayer(ActivePlayerId);
			}
			game.TellPlayerTheyAreActive(ActivePlayerId);
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

		public void RollTheDice(DiceRoll diceRoll, int delayMs = 0)
		{
			if (delayMs > 0)
			{
				delayedDiceRoll = diceRoll;
				delayRollTimer.Interval = TimeSpan.FromMilliseconds(delayMs);
				delayRollTimer.Start();
				return;
			}
			CompleteCast(diceRoll);
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


			SafeInvoke(() =>
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
			if (activeSpellName != null)
				diceRoll.SpellName = activeSpellName;

			if (int.TryParse(tbxMinDamage.Text, out int result))
				diceRoll.MinDamage = result;
			else
				diceRoll.MinDamage = 0;

			if (type == DiceRollType.Initiative)
			{
				AllInGameCreatures.AddD20sForSelected(diceRoll.DiceDtos, DiceRollType.Initiative);
			}

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
				if (NextRollScope != RollScope.ActiveInGameCreature)
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

			if (NextRollScope == RollScope.ActiveInGameCreature)
			{
				diceRoll.RollScope = RollScope.ActiveInGameCreature;
				InGameCreature activeTurnCreature = game.GetActiveTurnCreature() as InGameCreature;
				if (activeTurnCreature != null)
					AllInGameCreatures.AddDiceForCreature(diceRoll.DiceDtos, NextDieStr, activeTurnCreature.Index, type);
			}
			else if (NextRollScope == RollScope.TargetedInGameCreatures)
			{
				diceRoll.RollScope = RollScope.TargetedInGameCreatures;
				AllInGameCreatures.AddDiceForTargeted(diceRoll.DiceDtos, NextDieStr);
			}
			else
			{
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

						if ((type == DiceRollType.Attack || type == DiceRollType.ChaosBolt) && checkbox.PlayerId != ActivePlayerId)
							continue;

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

		void UpdateConcentratedSpells()
		{
			List<Character> players = AllPlayers.GetActive();

			bool changeOccurred = false;
			foreach (Character player in players)
			{
				if (player.concentratedSpell != null)
				{
					PlayerStats playerStats = PlayerStatsManager.GetPlayerStats(player.playerID);
					if (playerStats != null)
					{
						// TODO: Consider adding side-effect to PercentConcentrationComplete property setter to trigger event.
						double newPercent = DndUtils.GetSpellPercentComplete(player.concentratedSpell, game);
						if (Math.Round(newPercent, 2) != Math.Round(playerStats.PercentConcentrationComplete, 2))
						{
							playerStats.PercentConcentrationComplete = newPercent;
							changeOccurred = true;
						}
					}
				}
			}

			if (changeOccurred)
				UpdateConcentratedSpellHourglassesInGame();
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
			UpdateConcentratedSpells();
		}

		string clockMessage;

		private void UpdateClock(bool bigUpdate = false, double daysSinceLastUpdate = 0)
		{
			SafeInvoke(() =>
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
					HideClock = ckShowClock.IsChecked != true,
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
			});
		}

		private void BtnAdvanceTurn_Click(object sender, RoutedEventArgs e)
		{
			game.AdvanceRound();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
			HubtasticBaseStation.AllDiceDestroyed += HubtasticBaseStation_AllDiceDestroyed;
			HubtasticBaseStation.ReceivedInGameResponse += HubtasticBaseStation_ReceivedInGameResponse;
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
			SafeInvoke(() =>
			{
				//Title = "All Dice Destroyed.";
			});
			//History.Log("All Dice Destroyed.");
			CheckForFollowUpRolls(ea.StopRollingData);
			DynamicallyThrottleFrameRate();
			DeueueNextAction();
		}

		private void DynamicallyThrottleFrameRate()
		{
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
			game.Clock.Advance(DndTimeSpan.FromDays(1), -1, Modifiers.ShiftDown);
		}

		private void BtnAddTenDay_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+10 Days";
			game.Clock.Advance(DndTimeSpan.FromDays(10), -1, Modifiers.ShiftDown);
		}

		private void BtnAddMonth_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Month";
			game.Clock.Advance(DndTimeSpan.FromDays(30), -1, Modifiers.ShiftDown);
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

		RollScope nextRollScope;

		string NextDieStr;
		public RollScope NextRollScope
		{
			get => nextRollScope;
			set
			{
				if (nextRollScope == value)
					return;

				nextRollScope = value;
				SetNextRollScopeUI();
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

		private void SetNextRollScopeUI()
		{
			if (nextRollScope == RollScope.ActiveInGameCreature)
				rbActiveNpc.IsChecked = true;
			else if (nextRollScope == RollScope.ActivePlayer)
				rbActivePlayer.IsChecked = true;
			else if (nextRollScope == RollScope.TargetedInGameCreatures)
				rbTargetedNpcs.IsChecked = true;
			else if (nextRollScope == RollScope.Individuals)
				rbIndividuals.IsChecked = true;

			btnRollDice.IsEnabled = true;
		}

		private void BtnAddHour_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Hour";
			game.Clock.Advance(DndTimeSpan.FromHours(1), -1, Modifiers.ShiftDown);
		}

		private void BtnAdd10Minutes_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+10 Minutes";
			game.Clock.Advance(DndTimeSpan.FromMinutes(10), -1, Modifiers.ShiftDown);
		}

		private void BtnAdd1Minute_Click(object sender, RoutedEventArgs e)
		{
			clockMessage = "+1 Minute";
			game.Clock.Advance(DndTimeSpan.FromMinutes(1), -1, Modifiers.ShiftDown);
		}



		void enableDiceRollButtons()
		{
			btnRollDice.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
		}

		void EnableDiceRollButtons(bool enable)
		{
			SafeInvoke(() =>
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
				case Skills.randomShit: return "Random Shit";
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

		void HandleWildMagicD20Check(int playerId, IndividualRoll individualRoll)
		{
			if (forcedWildMagicThisRoll)
				return;
			if (individualRoll.value == 1)
			{
				forcedWildMagicThisRoll = true;
				PlayScene("DH.WildMagicRoll", 2700);
				EnqueueWildMagicRoll(playerId);
				Character player = game.GetPlayerFromId(playerId);
				if (player != null)
					player.NumWildMagicChecks = 0;
				//wildMagicRollTimer.Start();
				TellAll("It's a one! Need to roll wild magic!");
			}
			else
			{
				// TellAll($"Wild Magic roll: {individualRoll.value}.");
			}
		}
		void EnqueueWildMagicRoll(int playerId)
		{
			WildMagicQueueEntry wildMagicQueueEntry = new WildMagicQueueEntry();
			wildMagicQueueEntry.PlayerId = playerId;
			EnqueueAction(wildMagicQueueEntry);
		}

		void IndividualDiceStoppedRolling(int playerId, IndividualRoll individualRoll)
		{
			switch (individualRoll.type)
			{
				case "WildMagicD20Check":
				case "Wild Magic Check":
				case "\"Wild Magic Check\"":
					History.Log("IndividualDiceStoppedRolling: " + individualRoll.type);
					HandleWildMagicD20Check(playerId, individualRoll);
					break;
			}
		}

		void IndividualDiceStoppedRolling(int playerId, List<IndividualRoll> individualRolls)
		{
			foreach (IndividualRoll individualRoll in individualRolls)
			{
				IndividualDiceStoppedRolling(playerId, individualRoll);
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

			// TODO: Trigger After Roll Effects for all players if multiple players rolled at the same time.

			if (ChaosBoltRolledDoubles(diceStoppedRollingData))
			{
				DiceRoll chaosBoltRoll = lastRoll;
				if (lastRoll.Type == DiceRollType.WildMagicD20Check)
					chaosBoltRoll = secondToLastRoll;
				if (chaosBoltRoll == null)
				{
					return;
				}
				SafeInvoke(async () =>
				{
					if (await AnswersYes("Doubles! Fire **Chaos Bolt** again?"))
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

			VisualEffectTarget chestTarget, bottomTarget;
			GetTargets(out chestTarget, out bottomTarget);

			if (spellHitType != SpellHitType.Hit)
			{
				effectGroup.Add(AnimationEffect.CreateEffect("SpellMiss", chestTarget, 0, 100, 0));
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
						AnimationEffect effectBonus1 = CreateSpellEffect(EffectGroup.GetRandomHitSpellName(), chestTarget, spell.Hue1, spell.Bright1);
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
						AnimationEffect effectBonus2 = CreateSpellEffect(EffectGroup.GetRandomHitSpellName(), chestTarget, spell.Hue2, spell.Bright2);
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

		private static void GetTargets(out VisualEffectTarget chestTarget, out VisualEffectTarget bottomTarget)
		{
			chestTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, -150));
			bottomTarget = new VisualEffectTarget(TargetType.ActivePlayer, new DndCore.Vector(0, 0), new DndCore.Vector(0, 0));
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

		private static void DequeueSpellEffects(EffectGroup effectGroup, Queue<SpellEffect> additionalSpellEffects, VisualEffectTarget chestTarget, VisualEffectTarget bottomTarget, ref double scale, double scaleIncrement, ref double autoRotation, ref int timeOffset)
		{
			while (additionalSpellEffects.Count > 0)
			{
				SpellEffect spellEffect = additionalSpellEffects.Dequeue();
				effectGroup.AddSpellEffect(chestTarget, bottomTarget, ref scale, scaleIncrement, ref autoRotation, ref timeOffset, spellEffect);
			}
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
			return AnimationEffect.CreateEffect(spriteName, target, hueShift, 100, brightness);
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
		// TODO: Delete lastDamage
		int lastDamage;
		int lastHealth;
		Dictionary<DamageType, int> latestDamage = new Dictionary<DamageType, int>();

		async Task CalculateLatestDamage(DiceEventArgs ea)
		{
			if (ea.StopRollingData == null)
				return;

			switch (ea.StopRollingData.type)
			{
				case DiceRollType.ChaosBolt:
					{
						await ProcessChaosBoltDamage(ea);
						break;
					}
				case DiceRollType.Attack:
				case DiceRollType.DamageOnly:
					latestDamage.Clear();
					lock (latestDamage)
					{
						foreach (IndividualRoll individualRoll in ea.StopRollingData.individualRolls)
						{
							if (!latestDamage.ContainsKey(individualRoll.damageType))
								latestDamage.Add(individualRoll.damageType, individualRoll.value);
							else
								latestDamage[individualRoll.damageType] += individualRoll.value;
						}
					}
					break;
			}
		}

		private async Task ProcessChaosBoltDamage(DiceEventArgs ea)
		{
			if (!ea.StopRollingData.success)
				return;
			List<DamageType> damageChoices = new List<DamageType>();
			foreach (IndividualRoll individualRoll in ea.StopRollingData.individualRolls)
			{
				if (individualRoll.numSides == 8)
				{
					DamageType damageType = DndUtils.GetChaosBoltDamage(individualRoll.value);
					if (!damageChoices.Contains(damageType))
						damageChoices.Add(damageType);
				}
			}

			DamageType chaosBoltDamageType = DamageType.None;

			if (damageChoices.Count > 1)
			{
				List<string> answers = new List<string>();
				for (int i = 0; i < damageChoices.Count; i++)
				{
					answers.Add($"{i}:{damageChoices[i]}");
				}

				int answer = await AskQuestion("Select Chaos Bolt Damage:", answers);
				if (answer >= 0 && answer < damageChoices.Count)
					chaosBoltDamageType = damageChoices[answer];
			}
			else if (damageChoices.Count > 0)
				chaosBoltDamageType = damageChoices[0];

			if (chaosBoltDamageType == DamageType.None)
				System.Diagnostics.Debugger.Break();

			ea.StopRollingData.additionalDieRollMessage = $"({chaosBoltDamageType})";
			latestDamage.Clear();
			lock (latestDamage)
			{
				foreach (IndividualRoll individualRoll in ea.StopRollingData.individualRolls)
				{
					if (!latestDamage.ContainsKey(chaosBoltDamageType))
						latestDamage.Add(chaosBoltDamageType, individualRoll.value);
					else
						latestDamage[chaosBoltDamageType] += individualRoll.value;
				}
			}
		}

		//void ApplyHalfDamageFromLastSavingThrow(List<PlayerRoll> thoseWhoSaved, string spellName)
		//{
		//	if (thoseWhoSaved.Count == 0)
		//		return;
		//	Spell spell = AllSpells.Get(spellName);
		//	if (spell != null)
		//	{
		//		Target target = new Target(thoseWhoSaved);

		//		if (string.IsNullOrWhiteSpace(spell.OnTargetFailsSave))
		//		{
		//			Expressions.Do("GiveTargetHalfDamage();", ActivePlayer, target, null, null, latestDamage);
		//		}
		//		else
		//		{
		//			// TODO: See if I need to save and pass in the CastedSpell that got us here.
		//			spell.TriggerTargetSaves(ActivePlayer, target, null, latestDamage);
		//		}
		//	}
		//	else
		//	{
		//		//System.Diagnostics.Debugger.Break();
		//	}
		//}

		void ApplyDamageFromLastSavingThrow(List<PlayerRoll> playerRolls, string damageExpression, string spellName)
		{
			if (playerRolls.Count == 0)
				return;
			Spell spell = AllSpells.Get(spellName);
			if (spell == null)
				return;

			Target target = new Target(playerRolls);

			if (string.IsNullOrWhiteSpace(spell.OnTargetFailsSave))
				Expressions.Do(damageExpression, ActivePlayer, target, null, null, latestDamage);
			else // TODO: See if I need to save and pass in the CastedSpell that got us here.
				spell.TriggerTargetFailsSave(ActivePlayer, target, null, latestDamage);
		}

		//void ApplyHalfDamageFromLastSavingThrow(List<PlayerRoll> thoseWhoSaved, string spellName)
		//{
		//	if (thoseWhoSaved.Count == 0)
		//		return;
		//	Spell spell = AllSpells.Get(spellName);
		//	if (spell != null)
		//	{
		//		Target target = new Target(thoseWhoSaved);

		//		if (string.IsNullOrWhiteSpace(spell.OnTargetFailsSave))
		//		{
		//			Expressions.Do("GiveTargetHalfDamage();", ActivePlayer, target, null, null, latestDamage);
		//		}
		//		else
		//		{
		//			// TODO: See if I need to save and pass in the CastedSpell that got us here.
		//			spell.TriggerTargetSaves(ActivePlayer, target, null, latestDamage);
		//		}
		//	}
		//	else
		//	{
		//		//System.Diagnostics.Debugger.Break();
		//	}
		//}

		void BreakSpellConcentrationFromLastSavingThrow(List<PlayerRoll> playerRolls)
		{
			if (playerRolls.Count == 0)
				return;

			foreach (PlayerRoll playerRoll in playerRolls)
			{
				if (playerRoll.data == BreakSpellConcentrationSavingThrowQueueEntry.STR_ConcentrationSave)
				{
					Character player = game.GetPlayerFromId(playerRoll.id);
					if (player != null && player.concentratedSpell != null)
					{
						string focusedSpellName = player.concentratedSpell.Spell.Name;
						TellDungeonMaster($"{player.firstName}'s save failed, breaking {player.hisHer} concentration on {focusedSpellName}.");
						game.RemoveActiveSpell(player, focusedSpellName);
						player.BreakConcentration();
					}
				}
			}
		}

		string ToDisplayList(List<PlayerRoll> playerRolls)
		{
			if (playerRolls == null || playerRolls.Count == 0)
				return string.Empty;

			string result = "";
			foreach (PlayerRoll playerRoll in playerRolls)
			{
				if (playerRoll.id > 0)
					result += GetPlayerName(playerRoll.id);
				else
					result += GetCreatureName(-playerRoll.id);

				result += ", ";
			}
			return result.TrimEnd(',', ' ');
		}

		private async void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			SaveNamedResults(ea);

			//SafeInvoke(() =>
			//{
			//	Title = "Dice Stopped Rolling (waiting for destruction)...";
			//});

			Character singlePlayer = ea.StopRollingData.GetSingleRollingPlayer();
			if (singlePlayer != null)
			{
				TriggerAfterRollEffects(ea.StopRollingData, singlePlayer);
			}
			else if (ea.StopRollingData.multiplayerSummary != null)
				foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
				{
					Character player = game.GetPlayerFromId(playerRoll.id);
					TriggerAfterRollEffects(ea.StopRollingData, player);
				}

			History.Log("Dice stopped rolling.");
			await CalculateLatestDamage(ea);
			if (dynamicThrottling)
			{
				ChangeFrameRateAndUI(Overlays.Back, 30);
				ChangeFrameRateAndUI(Overlays.Front, 30);
			}

			waitingToClearDice = true;
			if (ea.StopRollingData == null)
				return;

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
				IndividualDiceStoppedRolling(ea.StopRollingData.playerID, ea.StopRollingData.individualRolls);
			}

			NotifyPlayersRollHasStopped(ea.StopRollingData);

			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			ClearExistingWindupsInGame();
			ReportOnDieRoll(ea);
			CheckSpellHitResults(ea.StopRollingData);

			EnableDiceRollButtons(true);
			ShowClearButton(null, EventArgs.Empty);

			if (ea.StopRollingData.type == DiceRollType.Attack && ea.StopRollingData.success)
			{
				ApplyDamageToTargets(ea.StopRollingData.damage);
			}

			if (ea.StopRollingData.type == DiceRollType.DamageOnly)
			{
				if (!string.IsNullOrWhiteSpace(ea.StopRollingData.spellName))
				{
					Spell spell = AllSpells.Get(ea.StopRollingData.spellName);
					if (spell != null && spell.SpellType == SpellType.SavingThrowSpell)
					{
						EnqueueSpellSavingThrow(spell.Name, spell.SavingThrowAbility, ea.StopRollingData.playerID);
					}
				}
				// latestDamage
				//
			}

			if (ea.StopRollingData.type == DiceRollType.SavingThrow)
			{
				if (ea.StopRollingData.multiplayerSummary != null)
				{
					List<PlayerRoll> thoseWhoSaved = new List<PlayerRoll>();
					List<PlayerRoll> thoseWhoCritSaved = new List<PlayerRoll>();
					List<PlayerRoll> thoseWhoCritFailed = new List<PlayerRoll>();
					List<PlayerRoll> thoseWhoFailed = new List<PlayerRoll>();
					foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
					{
						if (playerRoll.isCompleteFail)
							thoseWhoCritFailed.Add(playerRoll);
						else if (playerRoll.isCrit)
							thoseWhoCritSaved.Add(playerRoll);
						else if (playerRoll.success)
							thoseWhoSaved.Add(playerRoll);
						else
							thoseWhoFailed.Add(playerRoll);
					}
					ApplyDamageFromLastSavingThrow(thoseWhoSaved, "GiveTargetHalfDamage();", ea.StopRollingData.spellName);
					ApplyDamageFromLastSavingThrow(thoseWhoFailed, "GiveTargetFullDamage();", ea.StopRollingData.spellName);
					ApplyDamageFromLastSavingThrow(thoseWhoCritFailed, "GiveTargetDoubleDamage();", ea.StopRollingData.spellName);
					BreakSpellConcentrationFromLastSavingThrow(thoseWhoFailed);
					BreakSpellConcentrationFromLastSavingThrow(thoseWhoCritFailed);
					if (thoseWhoCritSaved.Count > 1)
						TellDungeonMaster($"{ToDisplayList(thoseWhoCritSaved)} crit-saved, and take no damage!");
					else if (thoseWhoCritSaved.Count == 1)
						TellDungeonMaster($"{ToDisplayList(thoseWhoCritSaved)} crit-saved, and takes no damage!");
					UpdateInGameCreatures();
				}
			}
		}

		// TODO: Delete this...
		public enum SavingThrowResult
		{
			Success,
			Fail
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
				game.ClearInitiativeOrder();
				foreach (PlayerRoll playerRoll in ea.StopRollingData.multiplayerSummary)
				{
					game.AddCreatureToInitiativeOrder(playerRoll.id);
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
					if (ea.StopRollingData.damage > 0)
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
					if (ea.StopRollingData.damage > 0)
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
				SafeInvoke(() =>
				{
					Title = $"ea.StopRollingData.playerID = {ea.StopRollingData.playerID}";
				});

				singlePlayer = AllPlayers.GetFromId(ea.StopRollingData.playerID);
				string playerName = GetPlayerName(ea.StopRollingData.playerID);
				if (singlePlayer == null)
				{
					if (ea.StopRollingData.playerID < 0)
					{
						InGameCreature creature = AllInGameCreatures.GetByIndex(-ea.StopRollingData.playerID);
						if (creature != null)
							playerName = creature.Name;
					}
				}

				if (playerName != "")
					playerName = playerName + "'s ";
				else
				{

				}
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
				if (ea.StopRollingData.type == DiceRollType.WildMagicD20Check && rollValue == 0)
					message = message.Replace(": 0", ": no ones rolled (safe).");
				TellAll(message);
			}
		}

		private async Task<bool> AnswersYes(string question)
		{
			List<string> answers = new List<string>();
			answers.Add("1:Yes");
			answers.Add("2:No");
			bool yes = await AskQuestion(question, answers) == 1;
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

		string GetCreatureName(int creatureID)
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.Index == creatureID)
					return inGameCreature.Name;
			}

			return "";
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			ClearAllInGameActiveTurnIndicators();

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

		private void ClearAllInGameActiveTurnIndicators()
		{
			game.ClearInitiativeOrder();
			AllInGameCreatures.ClearAllActiveTurns();
			PlayerStatsManager.ClearAllActiveTurns();
			UpdateInGameCreatures();
			UpdatePlayerStatsInGame();
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
			SafeInvoke(() =>
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

			// TODO: need to update the in-game concentrated spell casting ui.

			UpdateStateUIForPlayer(ActivePlayer, true);
		}

		//void RollWildMagicHandler(object sender, EventArgs e)
		//{
		//	wildMagicRollTimer.Stop();
		//	SafeInvoke(() =>
		//	{
		//		ActivateShortcut("Wild Magic");
		//		btnRollDice.Content = "Roll Wild Magic";
		//		BackToPlayersIn(18);
		//	});
		//}

		void BackToPlayersIn(double seconds)
		{
			switchBackToPlayersTimer.Interval = TimeSpan.FromSeconds(seconds);
			switchBackToPlayersTimer.Start();
		}
		void SwitchBackToPlayersHandler(object sender, EventArgs e)
		{
			switchBackToPlayersTimer.Stop();
			SafeInvoke(() =>
			{
				PlayScene(STR_PlayerScene);
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
			HubtasticBaseStation.ReceivedInGameResponse -= HubtasticBaseStation_ReceivedInGameResponse;
			HubtasticBaseStation.TellDungeonMaster -= HubtasticBaseStation_TellDM;
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
					tabItem.Header = player.firstName;
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
					// TODO: Update checkboxes to match player.ShowingNameplate setting.
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
			else if (skill == Skills.randomShit)
				lowerCaseSkillName = "random shit";
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

		void RollDiceNowHandler(object sender, EventArgs e)
		{
			delayRollTimer.Stop();
			RollTheDice(delayedDiceRoll);
		}

		DiceRoll delayedDiceRoll;

		public void InvokeSkillCheck(Skills skill, List<int> playerIds, int delayMs = 0)
		{
			SafeInvoke(() =>
			{
				SelectSkill(skill);
				SetRollScopeForPlayers(playerIds);
				ckbUseMagic.IsChecked = false;
				ResetActiveFields();
				DiceRoll diceRoll = PrepareRoll(DiceRollType.SkillCheck);
				RollTheDice(diceRoll, delayMs);
			});
		}

		private void SetRollScopeForPlayers(List<int> creatureIds)
		{
			if (IncludesAllPlayers(creatureIds))
				rbEveryone.IsChecked = true;
			else if (creatureIds.Count > 1)
			{
				rbIndividuals.IsChecked = true;
				// TODO: Check each individual identified by the player id's.
			}
			else
			{
				ActivePlayerId = creatureIds[0];
				rbActivePlayer.IsChecked = true;
			}
		}

		private static bool IncludesAllPlayers(List<int> playerIds)
		{
			return playerIds == null || playerIds.Count == 0 || playerIds.First() == int.MaxValue;
		}

		public void InvokeSavingThrow(Ability ability, List<int> playerIds, int delayRollMs = 0)
		{
			SafeInvoke(() =>
			{
				SelectSavingThrowAbility(ability);
				SetRollScopeForPlayers(playerIds);
				DiceRoll diceRoll = PrepareRoll(DiceRollType.SavingThrow);
				RollTheDice(diceRoll, delayRollMs);
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			int delayRollMs = 0;
			if (playerIds == null)
			{
				playerIds = PlayerStatsManager.GetReadyToRollPlayerIds();
				SelectedPlayersAboutToRoll();
				delayRollMs = INT_TimeToDropDragonDice;
			}

			InvokeSkillCheck(skill, playerIds, delayRollMs);

			if (activePage != ScrollPage.skills)
			{
				activePage = ScrollPage.skills;
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			}

			SafeInvoke(() =>
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
			SafeInvoke(() =>
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
			int delayRollMs = 0;
			if (playerIds == null)
			{
				playerIds = PlayerStatsManager.GetReadyToRollPlayerIds();
				SelectedPlayersAboutToRoll();
				delayRollMs = INT_TimeToDropDragonDice;
			}

			InvokeSavingThrow(ability, playerIds, delayRollMs);

			if (activePage != ScrollPage.main)
			{
				activePage = ScrollPage.main;
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			}

			SafeInvoke(() =>
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

		private void SelectedPlayersAboutToRoll()
		{
			PlayerStatsManager.RollingTheDiceNow = true;
			UpdatePlayerStatsInGame();
			PlayerStatsManager.RollingTheDiceNow = false;
			PlayerStatsManager.ClearReadyToRollState();
		}

		public void InstantDiceRolledByTargets(DiceRollType diceRollType, string dieStr)
		{
			string creatures = AllInGameCreatures.GetTargetedCreatureDisplayList();
			TellAll($"Rolling {dieStr} for {creatures}...");

			SafeInvoke(() =>
			{
				SetRollTypeUI(diceRollType);
				DiceRoll diceRoll = PrepareRoll(diceRollType);
				if (diceRoll.PlayerRollOptions != null)
					diceRoll.PlayerRollOptions.Clear();
				diceRoll.RollScope = RollScope.TargetedInGameCreatures;
				AllInGameCreatures.AddDiceForTargeted(diceRoll.DiceDtos, dieStr);
				diceRoll.DamageHealthExtraDice = dieStr;
				RollTheDice(diceRoll);
			});
		}

		public void InstantDice(DiceRollType diceRollType, string dieStr, List<int> playerIds)
		{
			VantageKind vantageKind = VantageKind.Normal;
			if (dieStr.EndsWith("[adv]"))
			{
				dieStr = dieStr.EverythingBeforeLast("[");
				vantageKind = VantageKind.Advantage;
			}
			else if (dieStr.EndsWith("[disadv]"))
			{
				dieStr = dieStr.EverythingBeforeLast("[");
				vantageKind = VantageKind.Disadvantage;
			}
			NextDieStr = dieStr;
			string who;
			if (playerIds?.Count == 1 && playerIds[0] < 0)
			{
				InGameCreature creature = AllInGameCreatures.GetByIndex(-playerIds[0]);
				who = creature.Name;
				Dispatcher.Invoke(() =>
				{
					NextRollScope = RollScope.ActiveInGameCreature;
				});
			}
			else if (IncludesAllPlayers(playerIds))
				who = "all players";
			else
				who = GetPlayerName(ActivePlayerId);

			string vantageStr = "";
			if (vantageKind == VantageKind.Advantage)
				vantageStr = " (with advantage)";
			else if (vantageKind == VantageKind.Disadvantage)
				vantageStr = " (with disadvantage)";

			TellAll($"Rolling {dieStr}{vantageStr} for {who}...");

			SafeInvoke(() =>
			{
				SetRollTypeUI(diceRollType);
				SetRollScopeForPlayers(playerIds);
				DiceRoll diceRoll = PrepareRoll(diceRollType);
				// TODO: Set Modifier for this roll if it's a d20 for a creature (NextRollScope = RollScope.ActiveInGameCreature;).
				diceRoll.VantageKind = vantageKind;

				if (vantageKind != VantageKind.Normal)
				{
					if (diceRoll.DiceDtos != null)
						foreach (DiceDto diceDto in diceRoll.DiceDtos)
						{
							diceDto.Vantage = vantageKind;
							diceDto.DieCountsAs = DieCountsAs.totalScore;
						}

					foreach (PlayerRollOptions playerRollOptions in diceRoll.PlayerRollOptions)
						playerRollOptions.VantageKind = vantageKind;
				}

				diceRoll.SuppressLegacyRoll = game.InCombat && (diceRoll.DiceDtos != null && diceRoll.DiceDtos.Count > 0);
				if (diceRollType == DiceRollType.InspirationOnly)
					foreach (PlayerRollOptions playerRollOption in diceRoll.PlayerRollOptions)
						playerRollOption.Inspiration = dieStr;
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

		private void BtnReloadEverything_Click(object sender, RoutedEventArgs e)
		{
			LoadEverything();
		}

		private void LoadEverything()
		{
			List<MagicItem> magicItems = AllMagicItems.MagicItems;
			DateTime saveTime = game.Clock.Time;

			AllInGameCreatures.Invalidate();
			AllWeaponEffects.Invalidate();
			AllMagicItems.Invalidate();
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
				player.NumWildMagicChecks = 0;
				player.RebuildAllEvents();
				game.AddPlayer(player);
			}

			game.Clock.SetTime(saveTime);
			game.Start();
			SendPlayerData();
			weatherManager.Load();
			BuildPlayerTabs();
			BuildPlayerUI();
			InitializeAttackShortcuts();
			lstAllSpells.ItemsSource = AllSpells.Spells;
			spAllMonsters.DataContext = AllMonsters.Monsters;
			SetInGameCreatures();
			InitializePlayerStats();
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
				CheckPlayerUI(playerButton.PlayerId);
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
			CheckPlayerUI(ActivePlayerId);
		}

		private void CheckPlayerUI(int playerID)
		{
			if (lastPlayerIdUnchecked == playerID)
				return;
			if (grdPlayerRollOptions == null)
				return;
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				if (uIElement is PlayerRollCheckBox checkbox)
					checkbox.IsChecked = checkbox.PlayerId == playerID;
		}

		private void ChangePlayerUIRollingDice(int playerID, bool newState)
		{
			if (lastPlayerIdUnchecked == playerID)
				return;
			if (grdPlayerRollOptions == null)
				return;
			checkingInternally = true;
			try
			{
				foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				{
					if (uIElement is PlayerRollCheckBox checkbox)
						if (checkbox.PlayerId == playerID)
							checkbox.IsChecked = newState;
				}
			}
			finally
			{
				checkingInternally = false;
			}
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
			if (characterName == "*")
				return CardHandManager.IntAllPlayersId;
			return AllPlayers.GetPlayerIdFromName(game.Players, characterName);
		}

		public int GetActivePlayerId()
		{
			InGameCreature activeTurnCreature = game.GetActiveTurnCreature() as InGameCreature;
			if (activeTurnCreature != null)
				return -activeTurnCreature.Index;
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

		void UpdatePlayerScrollInGame(Character player)
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

		List<int> GetTargetedPlayerIds()
		{
			List<int> results = new List<int>();
			foreach (Character player in game.Players)
			{
				PlayerStats playerStats = PlayerStatsManager.GetPlayerStats(player.playerID);
				if (playerStats != null && playerStats.IsTargeted)
					results.Add(player.playerID);
			}
			if (results.Count == 0)
				return GetAllPlayerIds();
			return results;
		}

		void UpdatePlayerScrollUI(Character player)
		{
			Dispatcher.Invoke(() =>
			{
				CharacterSheets sheet = GetSheetForCharacter(player.playerID);
				sheet.SetFromCharacter(player);
			});
		}
		public void ApplyDamageHealthChange(DamageHealthChange damageHealthChange)
		{
			if (damageHealthChange == null)
				return;
			string playerNames = string.Empty;

			if (damageHealthChange.PlayerIds.Count == 1 && damageHealthChange.PlayerIds[0] == int.MaxValue)
				damageHealthChange.PlayerIds = GetTargetedPlayerIds();

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
				UpdatePlayerScrollInGame(player);
				UpdatePlayerScrollUI(player);
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
		bool uiThreadSleepingWhileWaitingForAnswerToQuestion;

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
			SafeInvoke(() =>
			{
				if (dataId == "Concentration")
					ReportOnConcentration();
			});
		}

		public void SetVantage(VantageKind vantageKind, int playerId)
		{
			SafeInvoke(() =>
			{
				SetPlayerVantageUI(playerId, vantageKind);
				PlayerStatsManager.ReadyRollVantage(playerId, vantageKind);
				UpdatePlayerStatsInGame();
			});
		}


		// TODO: Rename substring.
		async Task RepeatSpell(int playerId, string substring)
		{
			Character player = game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			if (player.concentratedSpell != null && player.concentratedSpell.Spell.Name == substring.Trim())
			{
				PlayerActionShortcut playerActionShortcut = PlayerActionShortcut.FromSpell(substring, player, player.concentratedSpell.SpellSlotLevel);
				await ActivateSpellShortcut(playerActionShortcut, true);
				NextDieRollType = DiceRollType.DamageOnly;
			}
		}

		public void SelectPlayerShortcut(string shortcutName, int playerId)
		{
			shortcutName = shortcutName.Trim();
			SafeInvoke(async () =>
			{
				ActivePlayerId = playerId;
				CheckOnlyOnePlayer(playerId);
				if (shortcutName.EndsWith(STR_RepeatSpell))
				{
					await RepeatSpell(playerId, shortcutName.Substring(0, shortcutName.Length - STR_RepeatSpell.Length));
				}
				else
					ActivatePlayerShortcut(shortcutName, playerId);
			});

		}

		private void ActivatePlayerShortcut(string shortcutName, int playerId)
		{
			PlayerActionShortcut shortcut = actionShortcuts.FirstOrDefault(x => x.DisplayText == shortcutName && x.PlayerId == playerId && x.Available);

			if (shortcut != null)
			{
				ActivateShortcut(shortcut);
				TellDungeonMaster($"Activated {GetPlayerName(playerId)}'s {shortcutName}.");
			}
		}

		public void SelectCharacter(int playerId)
		{
			SafeInvoke(() =>
			{
				if (tabPlayers.Items.Count > 0 && tabPlayers.Items[0] is PlayerTabItem)
					foreach (PlayerTabItem playerTabItem in tabPlayers.Items)
					{
						if (playerTabItem.PlayerId == playerId)
						{
							if (tabPlayers.SelectedItem != playerTabItem)
								tabPlayers.SelectedItem = playerTabItem;
							else
							{
								HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, "");
							}
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
			SafeInvoke(() =>
			{

			});
			// TODO: Tell DM
		}

		string GetPlural(string name, int seconds)
		{
			if (seconds == 1)
				return name;
			return name + "s";
		}

		public void AdvanceClock(int hours, int minutes, int seconds, bool resting)
		{
			if (hours == 0 && minutes == 0 && seconds == 0)
				return;
			// TODO: Calculate clockMessage based on the delta here.
			SafeInvoke(() =>
			{
				if (resting)
				{
					if (hours >= 8)
					{
						TellAll("All players have recharged after a long rest.");
						game?.RechargePlayersAfterLongRest();
					}
					else if (hours >= 2)
					{
						TellAll("All players have had a short rest.");
						game?.RechargePlayersAfterShortRest();
					}
				}

				game.Clock.Advance(DndTimeSpan.FromSeconds(seconds + minutes * 60 + hours * 3600), -1, Modifiers.ShiftDown);

				if (seconds != 0)
					if (seconds < 0)
						clockMessage = $"{seconds} {GetPlural("second", seconds)}";
					else
						clockMessage = $"+{seconds} {GetPlural("second", seconds)}";
				else if (minutes != 0)
					if (minutes < 0)
						clockMessage = $"{minutes} {GetPlural("minute", minutes)}";
					else
						clockMessage = $"+{minutes} {GetPlural("minute", minutes)}";
				else if (hours != 0)
					if (hours < 0)
						clockMessage = $"{hours} {GetPlural("hour", hours)}";
					else
						clockMessage = $"+{hours} {GetPlural("hour", hours)}";

				TellDungeonMasterTheTime();
			});


		}

		public void AdvanceDate(int days, int months, int years, bool resting)
		{
			if (days == 0 && months == 0 && years == 0)
				return;
			SafeInvoke(() =>
			{
				if (resting)
				{
					TellAll("All players have recharged after a long rest.");
					game?.RechargePlayersAfterLongRest();
				}

				game.Clock.Advance(DndTimeSpan.FromDays(days + months * 30 + years * 365), -1, Modifiers.ShiftDown);

				if (days != 0)
					if (days < 0)
						clockMessage = $"{days} {GetPlural("day", days)}";
					else
						clockMessage = $"+{days} {GetPlural("day", days)}";
				else if (months != 0)
					if (months < 0)
						clockMessage = $"{months} {GetPlural("month", months)}";
					else
						clockMessage = $"+{months} {GetPlural("month", months)}";
				else if (years != 0)
					if (years < 0)
						clockMessage = $"{years} {GetPlural("year", years)}";
					else
						clockMessage = $"+{years} {GetPlural("year", years)}";
			});
		}

		public void RollDice()
		{
			SafeInvoke(() =>
			{
				if (NextRollScope == RollScope.ActiveInGameCreature)
					NextRollScope = RollScope.Individuals;
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
			Character fred = AllPlayers.GetFromName("Fred");
			int hueShift = 0;
			if (fred != null)
			{
				object rageState = fred.GetState("_rage");
				if (rageState is Boolean inRage)
				{
					if (inRage)
					{
						hueShift = -220;
						movement += $":{hueShift}";
					}
				}
			}
			HubtasticBaseStation.MoveFred(movement);
		}

		bool IsFoundationScene(string name)
		{
			return name.SameLetters(STR_PlayerScene) || name.SameLetters("DH.Tunnels") || name.SameLetters("DH.Nebula") ||
				name.ToLower().StartsWith("DH.TheVoid.".ToLower());
		}

		string GetFoundationalScene()
		{
			try
			{
				OBSScene currentScene = obsWebsocket.GetCurrentScene();
				if (IsFoundationScene(currentScene.Name))
					return currentScene.Name;
				return STR_PlayerScene;
			}
			catch (Exception ex)
			{
				return STR_PlayerScene;
			}
		}

		string foundationalScene;

		void ReturnToSceneHandler(object sender, EventArgs e)
		{
			sceneReturnTimer.Stop();
			if (foundationalScene == null)
				return;
			PlayScene(foundationalScene);
			foundationalScene = null;
		}

		public void PlayScene(string sceneName, int returnMs = -1)
		{
			lastScenePlayed = sceneName;
			string dmMessage = $"Playing scene: {sceneName}";

			try
			{
				if (obsWebsocket.IsConnected)
				{
					if (returnMs > 0)
						foundationalScene = GetFoundationalScene();

					obsWebsocket.SetCurrentScene(sceneName);

					if (returnMs > 0)
					{
						sceneReturnTimer.Interval = TimeSpan.FromMilliseconds(returnMs);
						sceneReturnTimer.Start();
					}
				}
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
			SafeInvoke(() =>
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
			if (sendMessageSendsToHistory)
				History.Log($"Sending \"{message}\" to {channel} at {DateTime.Now.ToLongTimeString()}");
			if (JoinedChannel(channel))
				dungeonMasterClient.SendMessage(channel, message);
			else
			{
				try
				{
					if (dungeonMasterClient == null)
						CreateDungeonMasterClient();
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

		private async void UnleashTheNextRoll()
		{
			PlayerActionShortcut localSpellToCastOnRoll = spellToCastOnRoll;
			//Title = "Rolling Dice...";
			if (localSpellToCastOnRoll != null)
			{
				if (localSpellToCastOnRoll.Spell?.SpellType == SpellType.SavingThrowSpell)
				{
					nextSavingThrowAbility = localSpellToCastOnRoll.Spell.SavingThrowAbility;
					if (ActivePlayer != null)
						SetSavingThrowThreshold(ActivePlayer.SpellSaveDC);
				}
				try
				{
					if (!await ActivateSpellShortcut(localSpellToCastOnRoll))
						return;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debugger.Break();
					Console.WriteLine(ex.Message);
				}

				bool isSimpleSpell = localSpellToCastOnRoll?.Type == DiceRollType.CastSimpleSpell;
				spellToCastOnRoll = null;
				if (isSimpleSpell)
				{
					btnRollDice.IsEnabled = false;
					return;  // No need to roll the dice
				}
			}

			if (NextDieRollType != DiceRollType.None)
			{
				int delayMs = 0;
				if (PlayerStatsManager.AnyoneIsReadyToRoll)
				{
					delayMs = INT_TimeToDropDragonDice;
					SelectedPlayersAboutToRoll();
				}

				RollTheDice(PrepareRoll(NextDieRollType), delayMs);
				NextDieRollType = DiceRollType.None;
				NextRollScope = RollScope.ActivePlayer;
				NextDieStr = "";
			}
		}

		private void RbSkillCheck_Checked(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.SkillCheck;
			btnRollDice.Content = "Roll Skill Check";
		}

		private void RbSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			NextDieRollType = DiceRollType.SavingThrow;
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
			if (!settingAttackRadioButtonInternally)
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
			spell.OnValidate = latestSpell.OnValidate;
			spell.OnReceived = latestSpell.OnReceived;
			spell.OnPreparing = latestSpell.OnPreparing;
			spell.OnPreparationComplete = latestSpell.OnPreparationComplete;
			spell.OnTargetFailsSave = latestSpell.OnTargetFailsSave;
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
					SafeInvoke(() =>
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
				wealthChange.PlayerIds = GetTargetedPlayerIds();
				//wealthChange.PlayerIds = new List<int>();
				//foreach (Character player in game.Players)
				//{
				//	wealthChange.PlayerIds.Add(player.playerID);
				//}
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
			SafeInvoke(() =>
			{
				tbxAttackThreshold.Text = value.ToString();
			});
			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{value} {twitchIndent} <-- hidden ATTACK threshold");
		}
		void SetSavingThrowThreshold(int value)
		{
			SafeInvoke(() =>
			{
				tbxSaveThreshold.Text = value.ToString();
			});
			TellDungeonMaster($"{Icons.SetHiddenThreshold} {twitchIndent}{value} {twitchIndent} <-- hidden SAVE threshold");
		}

		void SetSkillCheckThreshold(int value)
		{
			SafeInvoke(() =>
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
				case "HealthToTargetedCreatures":
					ChangeTargetedCreatureHealth(intValue, InGameCreatureFilter.TargetedOnly);
					break;
				case "DamageToTargetedCreatures":
					ChangeTargetedCreatureHealth(-intValue, InGameCreatureFilter.TargetedOnly);
					break;
				case "TempHpToTargetedCreatures":
					ChangeInGameCreatureTempHp(intValue, InGameCreatureFilter.TargetedOnly);
					break;
				case "HealthToAllCreatures":
					ChangeTargetedCreatureHealth(intValue, InGameCreatureFilter.All);
					break;
				case "DamageToAllCreatures":
					ChangeTargetedCreatureHealth(-intValue, InGameCreatureFilter.All);
					break;
				case "TempHpToAllCreatures":
					ChangeInGameCreatureTempHp(intValue, InGameCreatureFilter.All);
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
					// TODO: Change lastDamage to use the newer, richer latestDamage.
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
			SafeInvoke(() =>
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
			ShowSampleMonster(monster);
			GoogleSheets.SaveChanges(monster, string.Join(",", nameof(monster.imageCropStr), nameof(monster.ImageUrl)));
		}

		private void UpdateCropInfoFromUI(Monster monster)
		{
			var scale = imgMonster.ActualWidth / imgMonster.Source.Width;
			double width = rectCrop.Width / scale;
			double x = Canvas.GetLeft(rectCrop) / scale;
			double y = Canvas.GetTop(rectCrop) / scale;

			monster.ImageCropInfo = new PictureCropInfo() { X = x, Y = y, Width = width, DpiFactor = monster.ImageCropInfo.DpiFactor };
		}

		void MoveMonsterCroppedTo(Monster monster)
		{
			if (imgMonster.Source == null || imgMonster.Source.Width == 1)
			{
				cropRectUpdateTimer.Start();
				return;
			}

			PictureCropInfo pictureCropInfo = monster.ImageCropInfo;

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
		double lastMonsterPreviewImageDpiX;
		Ability nextSavingThrowAbility;

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

					if (cropInfo.Width < PictureCropInfo.MinWidth)
						cropInfo.Width = PictureCropInfo.MinWidth;

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
				if (!string.IsNullOrWhiteSpace(monster.ImageUrl))
				{
					monsterCropPreviewBitmap = new BitmapImage(new Uri(monster.ImageUrl));
					monsterCropPreviewBitmap.DownloadCompleted += MonsterCropPreviewBitmap_DownloadCompleted;
					monsterPreviewImageLoadUpdateTimer.Start();
					lastMonsterPreviewImageDpiX = monsterCropPreviewBitmap.DpiX;
				}
			}
			else
				monsterCropPreviewBitmap = null;
		}

		void CheckMonsterPreviewImageLoadFromTimer(object sender, EventArgs e)
		{
			monsterPreviewImageLoadUpdateTimer.Stop();
			CheckMonsterImageLoaded();
		}

		private void MonsterCropPreviewBitmap_DownloadCompleted(object sender, EventArgs e)
		{
			CheckMonsterImageLoaded();
		}

		private void CheckMonsterImageLoaded()
		{
			if (lstAllMonsters.SelectedItem is Monster monster)
			{
				if (monsterCropPreviewBitmap != null)
				{
					//Title = $"{monsterCropPreviewBitmap.DpiX}, {monsterCropPreviewBitmap.DpiY}";

					double dpiFactorX = monsterCropPreviewBitmap.DpiX / 96;
					double dpiFactorY = monsterCropPreviewBitmap.DpiY / 96;
					if (dpiFactorX == dpiFactorY)
						if (monster.ImageCropInfo.DpiFactor != dpiFactorX)
						{
							monster.ImageCropInfo.DpiFactor = dpiFactorX;
							GoogleSheets.SaveChanges(monster, "imageCropStr");
						}
				}
				ShowSampleMonster(monster);
			}
		}

		private void ShowSampleMonster(Monster monster)
		{
			if (ckShowSampleMonster.IsChecked != true)
				return;

			List<InGameCreature> creatures = new List<InGameCreature>();
			InGameCreature inGameCreature = InGameCreature.FromMonster(monster);
			inGameCreature.Name = "Joe";
			creatures.Add(inGameCreature);
			HubtasticBaseStation.UpdateInGameCreatures("Set", creatures);
		}

		private void UpdateCropRectangleForSelectedMonster()
		{
			if (lstAllMonsters.SelectedItem is Monster monster)
				MoveMonsterCroppedTo(monster);
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

		private void btnTestInGameCreatures_Click(object sender, RoutedEventArgs e)
		{
			AllInGameCreatures.Invalidate();
			FrmSelectInGameCreature frmSelectInGameCreature = new FrmSelectInGameCreature();
			frmSelectInGameCreature.SetDataSources(AllInGameCreatures.Creatures, null);
			if (frmSelectInGameCreature.ShowDialog() != true)
				return;
			SetInGameCreatures();
		}

		private static void UpdateInGameCreatures()
		{
			HubtasticBaseStation.UpdateInGameCreatures("Update", AllInGameCreatures.Creatures.Where(x => x.OnScreen).ToList());
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				inGameCreature.PercentDamageJustInflicted = 0;
				inGameCreature.PercentHealthJustGiven = 0;
			}
		}
		private static void RestackInGameCreatureConditions()
		{
			HubtasticBaseStation.UpdateInGameCreatures("RestackNpcConditions", AllInGameCreatures.Creatures.Where(x => x.OnScreen).ToList());
		}

		private static void SetInGameCreatures()
		{
			HubtasticBaseStation.UpdateInGameCreatures("Set", AllInGameCreatures.Creatures.Where(x => x.OnScreen).ToList());
		}

		public void ToggleTarget(int targetNum)
		{
			InGameCreature inGameCreature = AllInGameCreatures.GetByIndex(targetNum);
			if (inGameCreature == null)
				return;
			inGameCreature.IsTargeted = !inGameCreature.IsTargeted;
			if (inGameCreature.IsTargeted)
				inGameCreature.OnScreen = true;
			UpdateInGameCreatures();
		}

		public void TogglePlayerTarget(string playerName)
		{
			// 
			int playerId = AllPlayers.GetPlayerIdFromName(playerName);
			if (playerId < 0)
				return;

			PlayerStatsManager.ToggleTarget(playerId);
			UpdatePlayerStatsInGame();
		}

		void ToggleReadyRollD20(string data)
		{
			int playerId = AllPlayers.GetPlayerIdFromName(data);
			if (playerId < 0)
				return;
			lastPlayerIdUnchecked = -1;
			PlayerStatsManager.ToggleReadyRollD20(playerId);
		}

		public void ToggleCondition(string data, string condition)
		{
			Conditions conditions = DndUtils.ToCondition(condition);

			if (data == "targets")
				ToggleTargetedCreatureConditions(conditions);
			else if (data == "players")
				ToggleTargetedPlayerConditions(conditions);
			else
				TogglePlayerCondition(data, conditions);
		}

		private void TogglePlayerCondition(string data, Conditions conditions)
		{
			int playerId = AllPlayers.GetPlayerIdFromName(data);
			if (playerId < 0)
				return;
			PlayerStatsManager.ToggleCondition(playerId, conditions);
			UpdateUIForAllPlayerStats();
			UpdatePlayerStatsInGame();
		}

		private void ClearPlayerCondition(string data)
		{
			int playerId = AllPlayers.GetPlayerIdFromName(data);
			if (playerId < 0)
				return;
			PlayerStatsManager.ClearConditions(playerId);
			UpdateUIForAllPlayerStats();
			UpdatePlayerStatsInGame();
		}

		void ToggleTargetedPlayerConditions(Conditions conditions)
		{
			bool toggledAtLeastOne = false;
			foreach (PlayerStats playerStats in PlayerStatsManager.Players)
			{
				if (playerStats.IsTargeted)
				{
					toggledAtLeastOne = true;
					PlayerStatsManager.ToggleCondition(playerStats.PlayerId, conditions);
				}
			}
			if (!toggledAtLeastOne)
			{
				// Toggle all players...
				foreach (PlayerStats playerStats in PlayerStatsManager.Players)
				{
					PlayerStatsManager.ToggleCondition(playerStats.PlayerId, conditions);
				}
			}
			UpdateUIForAllPlayerStats();
			UpdatePlayerStatsInGame();
		}

		void ClearTargetedPlayerConditions()
		{
			bool clearedAtLeastOne = false;
			foreach (PlayerStats playerStats in PlayerStatsManager.Players)
			{
				if (playerStats.IsTargeted)
				{
					clearedAtLeastOne = true;
					PlayerStatsManager.ClearConditions(playerStats.PlayerId);
				}
			}

			if (!clearedAtLeastOne)
			{
				// Clear for all players...
				foreach (PlayerStats playerStats in PlayerStatsManager.Players)
				{
					PlayerStatsManager.ClearConditions(playerStats.PlayerId);
				}
			}
			UpdateUIForAllPlayerStats();
			UpdatePlayerStatsInGame();
		}

		private void ToggleTargetedCreatureConditions(Conditions conditions)
		{
			List<InGameCreature> targetedCreatures = AllInGameCreatures.Creatures.Where(x => x.IsTargeted).ToList();
			foreach (InGameCreature creature in targetedCreatures)
				creature.ToggleCondition(conditions);
			UpdateInGameCreatures();
		}

		private void ClearTargetedCreatureConditions()
		{
			List<InGameCreature> targetedCreatures = AllInGameCreatures.Creatures.Where(x => x.IsTargeted).ToList();
			foreach (InGameCreature creature in targetedCreatures)
				creature.ClearAllConditions();
			UpdateInGameCreatures();
		}

		void ReadyRollVantage(string data, VantageKind vantage = VantageKind.Normal)
		{
			int playerId = AllPlayers.GetPlayerIdFromName(data);
			if (playerId < 0)
				return;
			PlayerStatsManager.ReadyRollVantage(playerId, vantage);
		}

		public void ChangePlayerStateCommand(string command, string data)
		{
			switch (command)
			{
				case "ToggleReadyRollDice":
					ToggleReadyRollD20(data);
					break;
				case "QuickRefresh":
					QuickRefresh();
					break;
				case "ReadyRollDice":
					PlayerStatsManager.ReadyRollDice(data);
					break;
				case "ToggleReadyRollDiceAdvantage":
					ReadyRollVantage(data, VantageKind.Advantage);
					break;
				case "ToggleReadyRollDiceDisadvantage":
					ReadyRollVantage(data, VantageKind.Disadvantage);
					break;
				default:
					return;
			}
			UpdateUIForAllPlayerStats();
			UpdatePlayerStatsInGame();
		}
		void UpdateUIForPlayerStats(PlayerStats playerStats)
		{
			ChangePlayerUIRollingDice(playerStats.PlayerId, playerStats.ReadyToRollDice);
			SetPlayerVantageUI(playerStats.PlayerId, playerStats.Vantage);
		}

		void UpdateUIForAllPlayerStats()
		{
			SafeInvoke(() =>
			{
				foreach (PlayerStats playerStats in PlayerStatsManager.Players)
				{
					UpdateUIForPlayerStats(playerStats);
				}
			});
		}

		void UpdatePlayerStatsInGame()
		{
			PlayerStatsManager.LatestCommand = "Update";
			HubtasticBaseStation.ChangePlayerStats(JsonConvert.SerializeObject(PlayerStatsManager));
		}

		void UpdateConcentratedSpellHourglassesInGame()
		{
			PlayerStatsManager.LatestCommand = "HourglassUpdate";
			HubtasticBaseStation.ChangePlayerStats(JsonConvert.SerializeObject(PlayerStatsManager));
		}

		void AddInGameCreature(InGameCreature inGameCreature)
		{
			HubtasticBaseStation.UpdateInGameCreatures("Add", new List<InGameCreature>() { inGameCreature });
		}

		void RemoveInGameCreature(InGameCreature inGameCreature)
		{
			HubtasticBaseStation.UpdateInGameCreatures("Remove", new List<InGameCreature>() { inGameCreature });
		}

		void InGameCreatureTalks(InGameCreature inGameCreature, List<InGameCreature> creaturesChanged)
		{
			HubtasticBaseStation.UpdateInGameCreatures("Talks", creaturesChanged);
		}

		public void ToggleInGameCreature(int targetNum)
		{
			InGameCreature inGameCreature = AllInGameCreatures.GetByIndex(targetNum);
			if (inGameCreature == null)
				return;
			if (inGameCreature.OnScreen)
			{
				HideInGameCreature(inGameCreature);
			}
			else
			{
				inGameCreature.OnScreen = true;
				AddInGameCreature(inGameCreature);
			}
		}

		public void TalkInGameCreature(int targetNum)
		{
			if (targetNum == 0)
				NoCreaturesTalk();
			else
				OneCreatureTalks(targetNum);
		}

		private void NoCreaturesTalk()
		{
			InGameCreatureTalks(null, AllInGameCreatures.ClearTalking());
		}

		private void OneCreatureTalks(int targetNum)
		{
			InGameCreature inGameCreature = AllInGameCreatures.GetByIndex(targetNum);
			if (inGameCreature != null && !inGameCreature.IsTalking && !inGameCreature.OnScreen)
			{
				inGameCreature.OnScreen = true;
				AddInGameCreature(inGameCreature);
			}

			List<InGameCreature> creaturesChanged = AllInGameCreatures.ToggleTalking(inGameCreature);
			InGameCreatureTalks(inGameCreature, creaturesChanged);
		}

		private void HideInGameCreature(InGameCreature inGameCreature)
		{
			if (inGameCreature == null)
				return;
			inGameCreature.OnScreen = false;
			inGameCreature.IsTalking = false;
			RemoveInGameCreature(inGameCreature);
		}

		public class TargetSaveData
		{
			public bool IsTargeted { get; set; }
			public bool IsSelected { get; set; }
			public TargetSaveData()
			{

			}
		}

		void TargetNoPlayers()
		{
			PlayerStatsManager.ClearAllTargets();
			UpdatePlayerStatsInGame();
		}
		void TargetAllPlayers()
		{
			PlayerStatsManager.TargetAll();
			UpdatePlayerStatsInGame();
		}

		public void TargetCommand(string command)
		{
			switch (command)
			{
				case "TargetShown":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.IsTargeted = inGameCreature.OnScreen;
					UpdateInGameCreatures();
					return;
				case "TargetNoPlayers":
					TargetNoPlayers();
					return;
				case "TargetAllPlayers":
					TargetAllPlayers();
					return;
				case "ClearDead":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						if (inGameCreature.Health == 0)
							inGameCreature.OnScreen = false;
					UpdateInGameCreatures();
					return;
				case "UntargetDead":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						if (inGameCreature.Health == 0)
							inGameCreature.IsTargeted = false;
					UpdateInGameCreatures();
					return;
				case "ShowNone":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.OnScreen = false;
					UpdateInGameCreatures();
					return;
				case "ShowAllTargets":
					bool allTargetsAreShown = true;
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						if (inGameCreature.IsTargeted && !inGameCreature.OnScreen)
						{
							allTargetsAreShown = false;
							break;
						}
					if (allTargetsAreShown)
					{
						// We only need to hide those that are not targeted.
						foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
							if (!inGameCreature.IsTargeted)
								inGameCreature.OnScreen = false;

						UpdateInGameCreatures();
						return;
					}
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.OnScreen = inGameCreature.IsTargeted;
					break;
				case "ShowAll":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.OnScreen = true;
					break;
				case "TargetNone":
					AllInGameCreatures.ClearAllTargets();
					UpdateInGameCreatures();
					return;
				case "TargetAll":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
					{
						inGameCreature.IsTargeted = true;
						inGameCreature.OnScreen = true;
					}
					UpdateInGameCreatures();
					return;
				case "TargetFriends":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
					{
						inGameCreature.IsTargeted = inGameCreature.IsAlly;
						if (inGameCreature.IsTargeted)
							inGameCreature.OnScreen = true;
					}
					break;
				case "TargetEnemies":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
					{
						inGameCreature.IsTargeted = inGameCreature.IsEnemy;
						if (inGameCreature.IsTargeted)
							inGameCreature.OnScreen = true;
					}
					break;
				case "ShowFriends":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.OnScreen = !inGameCreature.IsAlly;
					break;
				case "ShowEnemies":
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
						inGameCreature.OnScreen = inGameCreature.IsEnemy;
					break;

				case "ReloadAllCreatures":
					AllInGameCreatures.Invalidate();
					break;
				case "SaveCreatureHP":
					AllInGameCreatures.SaveHp();
					TellDungeonMaster("All in-game NPC/Monster hit points and temp hit points have been saved.");
					return;

				case "UpdateOnScreenCreatures":
					// Save current selection/targeting  state...
					Dictionary<int, TargetSaveData> targetSaveData = new Dictionary<int, TargetSaveData>();
					foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
					{
						TargetSaveData targetData = new TargetSaveData() { IsSelected = inGameCreature.OnScreen, IsTargeted = inGameCreature.IsTargeted };
						int index = inGameCreature.Index;
						if (targetSaveData.ContainsKey(index))
							targetSaveData[index] = targetData;
						else
							targetSaveData.Add(index, targetData);
					}

					AllInGameCreatures.Invalidate();

					// Restore selection/targeting state
					foreach (int key in targetSaveData.Keys)
					{
						InGameCreature creature = AllInGameCreatures.GetByIndex(key);
						if (creature != null)
						{
							creature.OnScreen = targetSaveData[key].IsSelected;
							creature.IsTargeted = targetSaveData[key].IsTargeted;
						}
					}

					UpdateInGameCreatures();
					return;
				//case "SavingThrow":
				//	// TODO: Roll saving throws for target monsters against last damage and last damage type.
				//	TellDungeonMaster("This Save button is not functional yet.");
				//	break;
				default:
					return;
			}
			SetInGameCreatures();
			InitializePlayerStats();
		}

		// TODO: send in a dictionary of damage types and amounts.
		void ApplyDamageToTargets(int damage)
		{
			bool changed = false;
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.IsTargeted)
				{
					// TODO: Get damage type and attack kind parameters right from the last roll!
					inGameCreature.TakeDamage(DamageType.None, AttackKind.Any, damage);

					if (inGameCreature.PercentDamageJustInflicted != 0)
						changed = true;

					TellDmCreatureHp(inGameCreature);

				}
			}
			if (changed)
				UpdateInGameCreatures();
		}

		void ChangeTargetedCreatureHealth(int amount, InGameCreatureFilter inGameCreatureFilter)
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
				if (inGameCreatureFilter == InGameCreatureFilter.All || inGameCreature.IsTargeted)
				{
					inGameCreature.ChangeHealth(amount);
					TellDmCreatureHp(inGameCreature);
				}
			UpdateInGameCreatures();
		}

		private void TellDmCreatureHp(InGameCreature inGameCreature)
		{
			string tempHpDetails = string.Empty;
			if (inGameCreature.Creature.tempHitPoints > 0)
				tempHpDetails = $" (tempHp: {inGameCreature.Creature.tempHitPoints})";
			TellDungeonMaster($"{inGameCreature.Name}'s HP: {inGameCreature.Creature.HitPoints}/{inGameCreature.Creature.maxHitPoints}{tempHpDetails}");
		}

		void ChangeInGameCreatureTempHp(int amount, InGameCreatureFilter inGameCreatureFilter)
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
				if (inGameCreatureFilter == InGameCreatureFilter.All || inGameCreature.IsTargeted)
				{
					inGameCreature.Creature.ChangeTempHP(amount);
					TellDmCreatureHp(inGameCreature);
				}
			UpdateInGameCreatures();
		}

		public enum InGameCreatureFilter
		{
			All,
			TargetedOnly
		}

		private void LogOptionsChanged(object sender, RoutedEventArgs e)
		{
			sendTwitchLogMessagesToHistory = ckLogTwitchLogMessagesToHistory.IsChecked == true;
			sendMessageSendsToHistory = ckLogTwitchSendMessagesToHistory.IsChecked == true;
			sendTwitchChannelMessagesToHistory = ckSendChannelMessagesToHistory.IsChecked == true;
		}

		private void btnInitializeOnly_Click(object sender, RoutedEventArgs e)
		{
			QuickRefresh();
		}

		private void QuickRefresh()
		{
			SendPlayerData();
			SetInGameCreatures();
		}

		private void InitializePlayerStats()
		{
			PlayerStatsManager.ClearAll();
			foreach (Character character in game.Players)
			{
				if (!character.Hidden)
					PlayerStatsManager.GetPlayerStats(character.playerID);  // Will ensure player is known.
			}
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F5 && Modifiers.NoModifiersDown)
				btnInitializeOnly_Click(null, null);
		}

		public void NextTurn()
		{
			lock (game)
				game.NextTurn();
		}

		private void Game_ActivePlayerChanged(object sender, EventArgs e)
		{
			AllInGameCreatures.ClearAllActiveTurns();
			PlayerStatsManager.ClearAllActiveTurns();

			object activeTurnCreature = game.GetActiveTurnCreature();
			if (activeTurnCreature is InGameCreature inGameCreature)
			{
				AllInGameCreatures.SetActiveTurn(inGameCreature);
				PlayerStatsManager.ActiveTurnCreatureID = InGameCreature.GetUniversalIndex(inGameCreature.Index);
			}

			if (activeTurnCreature is Character character)
			{
				PlayerStatsManager.ActiveTurnCreatureID = character.playerID;
				ActivePlayerId = character.playerID;
			}

			UpdateInGameCreatures();
			UpdatePlayerStatsInGame();
			UpdateConcentratedSpells();
		}

		Queue<ActionQueueEntry> actionQueue = new Queue<ActionQueueEntry>();
		List<AnswerEntry> lastRemoteAnswers;

		void ExecuteQueuedShortcut(ShortcutQueueEntry shortcutQueueEntry)
		{
			if (ActivePlayerId != shortcutQueueEntry.PlayerId)
				ActivePlayerId = shortcutQueueEntry.PlayerId;
			ActivateShortcut(shortcutQueueEntry.ShortcutName);
			if (shortcutQueueEntry.RollImmediately)
				UnleashTheNextRoll();
		}

		void ExecuteAction(ActionQueueEntry action)
		{
			if (action is DieRollQueueEntry dieRollQueueEntry)
				ExecuteQueuedDieRoll(dieRollQueueEntry);
			else if (action is ShortcutQueueEntry shortcutQueueEntry)
				ExecuteQueuedShortcut(shortcutQueueEntry);
		}
		void ExecuteQueuedDieRoll(DieRollQueueEntry dieRollQueueEntry)
		{
			if (dieRollQueueEntry.RollType == DiceRollType.SavingThrow)
			{
				DiceRoll diceRoll = PrepareRoll(DiceRollType.SavingThrow);
				dieRollQueueEntry.PrepareRoll(diceRoll);
				RollTheDice(diceRoll);
			}
			else if (dieRollQueueEntry.RollType == DiceRollType.WildMagic)
			{
				DiceRoll diceRoll = PrepareRoll(DiceRollType.WildMagic);
				dieRollQueueEntry.PrepareRoll(diceRoll);
				RollTheDice(diceRoll);
			}
			// TODO: roll the dice.
			// TODO: apply the damage after the dice have rolled.
		}

		void DeueueNextAction()
		{
			lock (actionQueue)
			{
				if (!actionQueue.Any())
					return;
				// TODO: why are you doing all that work on the UI thread?
				SafeInvoke(() =>
				{
					if (actionQueue.Count > 0 && !HubtasticBaseStation.DiceOnScreen || HubtasticBaseStation.SecondsSinceLastRoll > 30)
						ExecuteAction(actionQueue.Dequeue());
				});
			}
		}

		string GetTargetedCreatureList(List<DiceDto> diceDtos)
		{
			if (diceDtos == null || diceDtos.Count == 0)
				return "(no creature)";
			if (diceDtos.Count == 1)
				return diceDtos[0].Label;
			string result = "";
			for (int i = 0; i < diceDtos.Count - 1; i++)
			{
				result += diceDtos[i].Label + ", ";
			}
			if (result.EndsWith(", "))
				result = result.EverythingBeforeLast(", ");
			result += " and " + diceDtos[diceDtos.Count - 1].Label;
			return result;
		}

		void EnqueueSpellSavingThrow(string name, Ability savingThrowAbility, int playerID)
		{
			SpellSavingThrowQueueEntry dieRoll = new SpellSavingThrowQueueEntry(name, savingThrowAbility);
			bool foundAny = false;
			bool atLeastOneImmune = false;

			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				if (inGameCreature.IsTargeted)
				{
					if (inGameCreature.IsTotallyImmuneToDamage(latestDamage, AttackKind.Magical))
					{
						TellDungeonMaster($"{inGameCreature.Name} is totally immune to all {name} damage.");
						atLeastOneImmune = true;
						continue;
					}
					dieRoll.AddSavingThrowFor(savingThrowAbility, inGameCreature);
					foundAny = true;
				}
			}
			if (foundAny)
			{
				Character spellCaster = AllPlayers.GetFromId(playerID);
				if (spellCaster != null)
				{
					dieRoll.HiddenThreshold = spellCaster.SpellSaveDC;
					dieRoll.PlayerId = playerID;
					EnqueueAction(dieRoll);
					string targetedCreatureList = GetTargetedCreatureList(dieRoll.DiceDtos);
					TellDungeonMaster($"Coming up: {savingThrowAbility} saving throw for {targetedCreatureList} (as soon as the dice are cleared).");
				}
				else
				{

				}
			}
			else if (atLeastOneImmune)
				TellDungeonMaster($"No need to roll saving throw - all targeted creatures are totally immune to all {name} damage.");
		}

		private void EnqueueAction(ActionQueueEntry dieRoll)
		{
			lock (actionQueue)
			{
				if (actionQueue.Count > 0)
				{
					ActionQueueEntry lastEntry = actionQueue.Last();
					if (lastEntry.CombineWith(dieRoll))
						return;
				}
				actionQueue.Enqueue(dieRoll);
			}
		}

		void EnqueueBreakSpellConcentrationSavingThrow(int playerID, double damageTaken)
		{
			BreakSpellConcentrationSavingThrowQueueEntry futureDieRoll = new BreakSpellConcentrationSavingThrowQueueEntry();
			futureDieRoll.PlayerId = playerID;
			futureDieRoll.HiddenThreshold = Math.Max(10, DndUtils.HalveValue(damageTaken));
			History.Log($"Enqueuing future die roll ({futureDieRoll.RollType})");
			EnqueueAction(futureDieRoll);
			Character player = GetPlayer(playerID);
			if (player != null)
				TellDungeonMaster($"{player.firstName} took {damageTaken} pts of damage while concentrating on a spell ({player.concentratedSpell.Spell.Name})! Constitution saving throw (against a {futureDieRoll.HiddenThreshold}) coming up...");
		}

		private void Game_RequestQueueShortcut(object sender, QueueShortcutEventArgs ea)
		{
			ShortcutQueueEntry shortcutQueueEntry = new ShortcutQueueEntry();
			shortcutQueueEntry.PlayerId = ea.Player.playerID;
			shortcutQueueEntry.RollImmediately = ea.RollImmediately;
			shortcutQueueEntry.ShortcutName = ea.ShortcutName;
			History.Log($"Enqueuing future shortcut ({ea.ShortcutName})");
			EnqueueAction(shortcutQueueEntry);
		}

		private void Game_ConcentratedSpellChanged(object sender, SpellChangedEventArgs ea)
		{
			if (ea.Player == null)
				return;
			PlayerStats playerStats = PlayerStatsManager.GetPlayerStats(ea.Player.playerID);
			if (playerStats == null)
				return;

			switch (ea.SpellState)
			{
				case SpellState.JustCast:
					playerStats.ConcentratedSpell = ea.SpellName;
					playerStats.ConcentratedSpellDurationSeconds = (int)Math.Round(AllSpells.GetDuration(ea.SpellName).TotalSeconds);
					playerStats.PercentConcentrationComplete = 0;
					playerStats.JustBrokeConcentration = false;
					break;
				case SpellState.JustDispelled:
					playerStats.ConcentratedSpell = ea.SpellName;
					playerStats.ConcentratedSpellDurationSeconds = 0;
					playerStats.PercentConcentrationComplete = 100;
					playerStats.JustBrokeConcentration = false;
					break;
				case SpellState.BrokeConcentration:
					playerStats.ConcentratedSpell = "";
					playerStats.ConcentratedSpellDurationSeconds = 0;
					playerStats.PercentConcentrationComplete = 100;
					playerStats.JustBrokeConcentration = true;
					break;
			}

			UpdatePlayerStatsInGame();
		}

		private void Validation_ValidationFailed(object sender, ValidationEventArgs ea)
		{
			HubtasticBaseStation.ShowValidationIssue(ActivePlayerId, ea.ValidationAction, ea.FloatText);
			TellDungeonMaster(ea.DungeonMasterMessage);
		}

		public void ReStackConditions()
		{
			PlayerStatsManager.LatestCommand = "ReStackConditions";
			HubtasticBaseStation.ChangePlayerStats(JsonConvert.SerializeObject(PlayerStatsManager));
			RestackInGameCreatureConditions();
		}

		public void PrepareSkillCheck(string skillCheck)
		{
			spellToCastOnRoll = null;
			PlayScene($"DH.Skill.{skillCheck}");
			PlaySceneAfter(STR_PlayerScene, 14000);
			SafeInvoke(() =>
			{
				ckbUseMagic.IsChecked = false;
				SelectSkill(DndUtils.ToSkill(skillCheck));
				NextDieRollType = DiceRollType.SkillCheck;
			});
		}
		public void PrepareTargetSkillCheck(string skillCheck)
		{
			spellToCastOnRoll = null;
			SafeInvoke(() =>
			{
				ckbUseMagic.IsChecked = false;
				SelectSkill(DndUtils.ToSkill(skillCheck));
				NextDieRollType = DiceRollType.SkillCheck;
				NextRollScope = RollScope.TargetedInGameCreatures;
				NextDieStr = "1d20";
			});
		}

		public void PrepareSavingThrow(string savingThrow)
		{
			spellToCastOnRoll = null;
			PlayScene($"DH.Save.{savingThrow}");
			PlaySceneAfter(STR_PlayerScene, 12000);
			SafeInvoke(() =>
			{
				ckbUseMagic.IsChecked = false;
				SelectSavingThrowAbility(DndUtils.ToAbility(savingThrow));
				NextDieRollType = DiceRollType.SavingThrow;
			});
		}
		public void PrepareTargetSavingThrow(string savingThrow)
		{
			spellToCastOnRoll = null;
			SafeInvoke(() =>
			{
				ckbUseMagic.IsChecked = false;
				SelectSavingThrowAbility(DndUtils.ToAbility(savingThrow));
				NextDieRollType = DiceRollType.SavingThrow;
				NextRollScope = RollScope.TargetedInGameCreatures;
				NextDieStr = "1d20";
			});
		}

		public void ClearAllConditions(string targetName)
		{
			if (targetName == "targets")
				ClearTargetedCreatureConditions();
			else if (targetName == "players")
				ClearTargetedPlayerConditions();
			else
				ClearPlayerCondition(targetName);
		}

		public void ApplyToTargetedCreatures(string applyCommand)
		{
			List<InGameCreature> targetedCreatures = AllInGameCreatures.Creatures.Where(x => x.IsTargeted).ToList();
			if (targetedCreatures.Count == 0)
			{
				TellDungeonMaster($"͏͏͏͏͏͏͏͏͏͏͏͏̣{Icons.WarningSign} Unable to apply latest damage. No creatures targeted.");
				return;
			}
			switch (applyCommand)
			{
				case "LastDamage":
					foreach (InGameCreature inGameCreature in targetedCreatures)
					{
						inGameCreature.TakeDamage(ActivePlayer, latestDamage, AttackKind.Any);
					}
					break;
				case "LastHealth":
					foreach (InGameCreature inGameCreature in targetedCreatures)
					{
						inGameCreature.ChangeHealth(lastHealth);
					}
					break;
			}
			UpdateInGameCreatures();
		}

		public void ClearDice()
		{
			SafeInvoke(() =>
			{
				ClearTheDice();
			});
		}

		public void MoveTarget(string targetingCommand)
		{
			if (targetingCommand == null)
				return;
			if (targetingCommand.Contains("Creature"))
				MoveCreatureTarget(targetingCommand);
			else if (targetingCommand.Contains("Player"))
				MovePlayerTarget(targetingCommand);
		}

		public void MovePlayerTarget(string targetingCommand)
		{
			List<PlayerStats> targetedPlayers = PlayerStatsManager.GetTargeted();
			PlayerStats firstTargeted = targetedPlayers.FirstOrDefault();
			PlayerStats creatureToTarget;

			if (targetingCommand.Contains("Next"))
				creatureToTarget = PlayerStatsManager.Players.Next(firstTargeted);
			else
				creatureToTarget = PlayerStatsManager.Players.Previous(firstTargeted);

			if (creatureToTarget == null)
				return;

			PlayerStatsManager.ClearAllTargets();
			creatureToTarget.IsTargeted = true;
			UpdatePlayerStatsInGame();
		}

		public void MoveCreatureTarget(string targetingCommand)
		{
			List<InGameCreature> onScreenCreatures = AllInGameCreatures.GetOnScreen();
			if (!onScreenCreatures.Any())
				return;
			InGameCreature firstTargeted = onScreenCreatures.FirstOrDefault(x => x.IsTargeted);

			InGameCreature creatureToTarget;
			if (targetingCommand.Contains("Next"))
				creatureToTarget = onScreenCreatures.Next(firstTargeted);
			else
				creatureToTarget = onScreenCreatures.Previous(firstTargeted);

			if (creatureToTarget == null)
				return;

			AllInGameCreatures.ClearAllTargets();
			creatureToTarget.IsTargeted = true;
			UpdateInGameCreatures();
		}

		public void SpellScrollsToggle()
		{
			PlayerStatsManager.HideSpellScrolls = !PlayerStatsManager.HideSpellScrolls;
			UpdatePlayerStatsInGame();
		}

		private void HubtasticBaseStation_ReceivedInGameResponse(object sender, QuestionAnswerMapEventArgs ea)
		{
			// TODO: Handle multi-selected answers. answerResponse is just an int.
			int firstIndex = ea.QuestionAnswerMap.GetFirstSelectedIndex();
			if (firstIndex >= 0)
				answerResponse = ea.QuestionAnswerMap.Answers[firstIndex].Value;
			lastRemoteAnswers = ea.QuestionAnswerMap.Answers;
			askingQuestion = false;
		}

		public void InGameUICommand(string command)
		{
			if (command == "Ask1")
			{
				// Simple yes/no.
				HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap("Select chaos bolt damage:", new List<String> { "Acid", "Force" }, 1, 1));
			}
			else if (command == "Ask2")
			{
				// Simple yes/no with word-wrap.
				HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap("Break concentration with **Enlarge/Reduce** (9.2 minutes remaining) to cast **Spider Climb**:", new List<String> { "Yes", "No" }, 1, 1));
			}
			else if (command == "Ask3")
			{
				// Simple multiple choice with word-wrap.
				HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap("Select Ability to Enhance:", new List<String> { "Bear's Endurance", "Bull's Strength", "Cat's Grace", "Eagle's Splendor", "Fox's Cunning", "Owl's Wisdom" }, 1, 1));
			}
			else if (command == "Ask4")
			{
				// Simple multiple choice.
				HubtasticBaseStation.InGameUICommand(new QuestionAnswerMap("Select target for Shield spell:", new List<String> { "Fred", "Miles", "Lady", "Merkin", "L'il Cutie" }, 1, 3));

			}
			else
				HubtasticBaseStation.InGameUICommand(command);
		}

		private void btnCalibrate_Click(object sender, RoutedEventArgs e)
		{
			leapCalibrator = new LeapCalibrator();
			leapCalibrator.StartCalibration();
			SetLeapCalibrationUiVisibility(Visibility.Visible);
			lbCalibrationStatus.Items.Clear();
			lbCalibrationStatus.Items.Add("Started Leap calibration...");
			ShowCalibrationInstructions();
			CaptureMouse();
		}

		private void SetLeapCalibrationUiVisibility(Visibility visibility)
		{
			tbInstructions.Visibility = visibility;
			tbCalibrationStatus.Visibility = visibility;
			lbCalibrationStatus.Visibility = visibility;
		}

		private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			leapCalibrator?.MouseMoved(e, this);
		}

		private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (leapCalibrator == null)
				return;
			if (leapCalibrator.leapMotionCalibrationStep == LeapMotionCalibrationStep.NotCalibrating)
				return;
			LeapMotionCalibrationStep calibrationStep = leapCalibrator.leapMotionCalibrationStep;
			Point position = leapCalibrator.MouseDown(e, this);
			ShowCalibrationStepComplete(position);
			leapDevice.CalibrationPointUpdated(calibrationStep, position, leapCalibrator.FingertipPosition, leapCalibrator.ActiveScale);
			if (leapCalibrator.leapMotionCalibrationStep == LeapMotionCalibrationStep.FrontLowerRight)
			{
				ReleaseMouseCapture();
				SetLeapCalibrationUiVisibility(Visibility.Hidden);
				return;
			}
			ShowCalibrationInstructions();
		}

		private void ShowCalibrationStepComplete(Point position)
		{
			switch (leapCalibrator?.leapMotionCalibrationStep)
			{
				case LeapMotionCalibrationStep.BackUpperLeft:
					lbCalibrationStatus.Items.Add($"Back upper left position set to ({position.X}, {position.Y}).");
					break;
				case LeapMotionCalibrationStep.BackLowerRight:
					lbCalibrationStatus.Items.Add($"Back lower right position set to ({position.X}, {position.Y}).");
					break;
				case LeapMotionCalibrationStep.FrontUpperLeft:
					lbCalibrationStatus.Items.Add($"Front upper left position set to ({position.X}, {position.Y}).");
					break;
				case LeapMotionCalibrationStep.FrontLowerRight:
					lbCalibrationStatus.Items.Add($"Front lower right position set to ({position.X}, {position.Y}).");
					break;
			}
		}
		private void ShowCalibrationInstructions()
		{
			switch (leapCalibrator?.leapMotionCalibrationStep)
			{
				case LeapMotionCalibrationStep.BackUpperLeft:
					tbInstructions.Text = "Move the mouse over the back upper left point and click it!";
					break;
				case LeapMotionCalibrationStep.BackLowerRight:
					tbInstructions.Text = "Move the mouse over the back lower right point and click it!";
					break;
				case LeapMotionCalibrationStep.FrontUpperLeft:
					tbInstructions.Text = "Move the mouse over the front upper left point and click it!";
					break;
				case LeapMotionCalibrationStep.FrontLowerRight:
					tbInstructions.Text = "Move the mouse over the front lower right point and click it!";
					break;
			}
		}

		private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			leapCalibrator?.CalibrationScaleChanged(e.Delta);
		}

		LeapDevice leapDevice = new LeapDevice();
		private void ckShowLiveHandPosition_Click(object sender, RoutedEventArgs e)
		{
			leapDevice.ShowingLiveHandPosition = ckShowLiveHandPosition.IsChecked == true;
		}

		private void LeapDiagnosticsOptionsChanged(object sender, RoutedEventArgs e)

		{
			leapDevice.SetDiagnosticsOptions(chkShowBackPlane.IsChecked == true, chkShowFrontPlane.IsChecked == true, chkShowActivePlane.IsChecked == true);
		}
		public void TriggerHandFx(HandFxDto handFxDto)
		{
			leapDevice.TriggerHandFx(handFxDto);
		}

		private void ckActive_Click(object sender, RoutedEventArgs e)
		{
			leapDevice.Active = ckActive.IsChecked == true;
			ckShowLiveHandPosition.IsEnabled = leapDevice.Active;
			btnCalibrate.IsEnabled = leapDevice.Active;
		}
		public void ShowBackground(string sourceName)
		{
			plateManager.ShowBackground(sourceName);
		}
		public void ShowForeground(string sourceName)
		{
			plateManager.ShowForeground(sourceName);
		}
		public void ShowWeather(string weatherKeyword)
		{
			weatherManager.ShowWeather(weatherKeyword);
		}
		public void LaunchHandTrackingEffect(string launchCommand, string dataValue)
		{
			leapDevice.LaunchHandTrackingEffect(launchCommand, dataValue);
		}

		private void ckShowClock_CheckedChanged(object sender, RoutedEventArgs e)
		{
			UpdateClock(true);
		}

		private void StreamlootsService_CardsPurchased(object sender, CardEventArgs ea)
		{
			
		}

		private void StreamlootsService_CardRedeemed(object sender, CardEventArgs ea)
		{
			string msg = ea.CardDto.Card.message;

			int characterId = ea.CardDto.CharacterId;
			if (characterId != int.MinValue)
			{
				cardHandManager.AddCard(characterId, ea.CardDto.Card);
			}
			if (msg.IndexOf("//") >= 0)
			{
				string onlyToViewers = msg.EverythingBefore("//").Trim();
				string onlyToDm = msg.EverythingAfter("//").Trim();
				TellDungeonMaster(onlyToDm);
				TellViewers(onlyToViewers);
			}
			else
			{
				TellAll(msg);
			}
		}

		public void NpcScrollsToggle()
		{
			System.Diagnostics.Debugger.Break();
		}

		public void CardCommand(CardCommandType cardCommandType, int creatureId, string cardId = "")
		{
			switch (cardCommandType)
			{
				case CardCommandType.ToggleHandVisibility:
					cardHandManager.ToggleHandVisibility(creatureId);
					return;
				case CardCommandType.HideAllNpcCards:
					cardHandManager.HideAllNpcCards();
					return;
				case CardCommandType.SelectNextCard:
					cardHandManager.SelectNextCard(creatureId);
					return;
				case CardCommandType.SelectPreviousCard:
					cardHandManager.SelectPreviousCard(creatureId);
					return;
				case CardCommandType.PlaySelectedCard:
					cardHandManager.PlaySelectedCard(creatureId);
					return;
				case CardCommandType.RevealSecretCard:
					cardHandManager.RevealSecretCard(creatureId, cardId);
					return;
			}
			System.Diagnostics.Debugger.Break();
		}

		// TODO: Reintegrate wand/staff animations....
		/* 
			Name									Index		Effect				effectAvailableWhen		playToEndOnExpire	 hue	moveUpDown
			Melf's Minute Meteors.6				Staff.Weapon	Casting								x										30	150				
			Melf's Minute Meteors.7				Staff.Magic		Casting								x									 350	150				
		 */
	}
}