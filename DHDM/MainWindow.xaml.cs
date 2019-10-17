using Microsoft.AspNetCore.SignalR.Client;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using Newtonsoft.Json;
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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDungeonMasterApp
	{
		//protected const string DungeonMasterChannel = "DragonHumpersDm";
		const string DungeonMasterChannel = "HumperBot";
		const string DragonHumpersChannel = "DragonHumpers";

		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		DungeonMasterChatBot dmChatBot = new DungeonMasterChatBot();
		TwitchClient dungeonMasterClient;

		List<PlayerActionShortcut> actionShortcuts = new List<PlayerActionShortcut>();
		ScrollPage activePage = ScrollPage.main;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer showClearButtonTimer;
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
			game.SpellDispelled += Game_SpellDispelled;
			game.PlayerRequestsRoll += Game_PlayerRequestsRoll;
			game.PlayerStateChanged += Game_PlayerStateChanged;
			realTimeAdvanceTimer = new DispatcherTimer(DispatcherPriority.Send);
			realTimeAdvanceTimer.Tick += new EventHandler(RealTimeClockHandler);
			realTimeAdvanceTimer.Interval = TimeSpan.FromMilliseconds(200);

			showClearButtonTimer = new DispatcherTimer();
			showClearButtonTimer.Tick += new EventHandler(ShowClearButton);
			showClearButtonTimer.Interval = TimeSpan.FromSeconds(8);

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
			// TODO: Save and retrieve game time.
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

			Expressions.ExceptionThrown += Expressions_ExceptionThrown;
			AskFunction.AskQuestion += AskFunction_AskQuestion;  // static event handler.
			Feature.FeatureDeactivated += Feature_FeatureDeactivated;
			ActivateShortcutFunction.ActivateShortcutRequest += ActivateShortcutFunction_ActivateShortcutRequest;
		}

		void SetShortcutVisibility(Panel panel)
		{
			foreach (UIElement uIElement in panel.Children)
			{
				if (uIElement is ShortcutPanel shortcutPanel)
				{
					PlayerActionShortcut shortcut = shortcutPanel.Shortcut;
					string availableWhen = shortcut.Spell?.AvailableWhen;
					if (!string.IsNullOrEmpty(availableWhen))
					{
						Character player = AllPlayers.GetFromId(shortcut.PlayerId);
						if (Expressions.GetBool(availableWhen, player))
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
		private void Game_PlayerStateChanged(object sender, PlayerStateEventArgs ea)
		{
			SetShortcutVisibility();
		}

		private void ActivateShortcutFunction_ActivateShortcutRequest(object sender, ShortcutEventArgs ea)
		{
			Dispatcher.Invoke(() =>
			{
				if (waitingToClearDice && !string.IsNullOrEmpty(ea.Shortcut.InstantDice))
					shortcutToActivateAfterClearingDice = ea.Shortcut;
				else
					ActivateShortcut(ea.Shortcut);
			});
			
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

		private void AskFunction_AskQuestion(object sender, AskEventArgs ea)
		{
			ea.Result = FrmAsk.Ask(ea.Question, ea.Answers, this);
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
			diceRoll.DamageDice = ea.DiceRollStr;
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

		public void SetHiddenThreshold(int hiddenThreshold)
		{
			Dispatcher.Invoke(() =>
			{
				tbxHiddenThreshold.Text = hiddenThreshold.ToString();
			});

			TellDungeonMaster($"Hidden threshold successfully changed to {hiddenThreshold}.");
		}

		private void HumperBotClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
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
			if (int.TryParse(tag.ToString(), out int index))
				return actionShortcuts.FirstOrDefault(x => x.Index == index);
			return null;
		}
		void HidePlayerShortcutHighlights()
		{
			if (highlightRectangles == null)
				return;
			foreach (Rectangle rectangle in highlightRectangles.Values)
			{
				rectangle.Visibility = Visibility.Hidden;
			}
		}
		void HighlightPlayerShortcut(int index)
		{
			HidePlayerShortcutHighlights();
			if (highlightRectangles == null)
				return;
			if (highlightRectangles.ContainsKey(index))
				highlightRectangles[index].Visibility = Visibility.Visible;
		}

		void SetVantageForActivePlayer(VantageKind vantageMod)
		{
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
			{
				if (uIElement is PlayerRollCheckBox checkbox && checkbox.PlayerId == ActivePlayerId)
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
			if (parenPos > 0)
			{
				string playerIdStr = spellToEnd.Substring(parenPos + 1);
				char[] endChars = { ')' };
				playerIdStr = playerIdStr.Trim(endChars);
				if (int.TryParse(playerIdStr, out int playerId))
				{
					//BreakConcentration(playerId);
					casterId = $"{GetPlayerName(playerId)}'s ";
				}
				spellToEnd = spellToEnd.Substring(0, parenPos);
			}
			TellDungeonMaster($"{casterId} {spellToEnd} spell ends at {game.Clock.AsFullDndDateTimeString()}.");
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
			TellDungeonMaster($"{GetPlayerName(playerId)} casts {spell.Name} at {game.Clock.AsFullDndDateTimeString()}.");
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
			if (shortcut != null)
				Dispatcher.Invoke(() =>
				{
					ActivateShortcut(shortcut);
				});
		}
		private void ActivateShortcut(PlayerActionShortcut actionShortcut)
		{
			ActivatePendingShortcuts();
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			activeIsSpell = null;
			if (!string.IsNullOrWhiteSpace(actionShortcut.TrailingEffects))
				activeTrailingEffects = actionShortcut.TrailingEffects;
			if (!string.IsNullOrWhiteSpace(actionShortcut.DieRollEffects))
				activeDieRollEffects = actionShortcut.DieRollEffects;
			//diceRoll.TrailingEffects
			if (actionShortcut.ModifiesExistingRoll)
			{
				switch (actionShortcut.VantageMod)
				{
					case VantageKind.Advantage:
					case VantageKind.Disadvantage:
						SetVantageForActivePlayer(actionShortcut.VantageMod);
						break;
				}
				if (!string.IsNullOrWhiteSpace(actionShortcut.AddDice))
					tbxDamageDice.Text += "," + actionShortcut.AddDice;
				return;
			}

			// TODO: Clear the weapons, but not the spells...
			HubtasticBaseStation.ClearWindup("Weapon.*");
			HubtasticBaseStation.ClearWindup("Windup.Spell.*");


			Character player = GetPlayer(actionShortcut.PlayerId);

			// TODO: Fix the targeting.
			if (DndUtils.IsAttack(actionShortcut.Type))
				player.WillAttack(null, actionShortcut);

			Spell spell = actionShortcut.Spell;
			if (spell != null)
			{
				activeIsSpell = spell;
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
							return;
						}
						if (FrmAsk.Ask($"Break concentration with {concentratedSpell.Name} ({game.GetSpellTimeLeft(player.playerID, concentratedSpell)} remaining) to cast {spell.Name}?", new List<string>() { "1:Yes", "0:No" }, this) == 0)
							return;
					}
					finally
					{
						if (!game.Clock.InCombat)
						{
							realTimeAdvanceTimer.Start();
							lastUpdateTime = DateTime.Now;
						}
					}
				}
				PrepareToCastSpell(spell, actionShortcut.PlayerId);

				// TODO: Fix the targeting.
				CastedSpellDto spellToCastDto = new CastedSpellDto(spell, new SpellTarget() { Target = SpellTargetType.Player, PlayerId = actionShortcut.PlayerId });

				spellToCastDto.Windups = actionShortcut.WindupsReversed;
				string serializedObject = JsonConvert.SerializeObject(spellToCastDto);
				HubtasticBaseStation.CastSpell(serializedObject);

				CastedSpell castedSpell = game.Cast(player, spell);

				if (spell.RequiresConcentration)
					player.CastingSpell(castedSpell);

				if (spell.MustRollDiceToCast())
				{
					castedSpellNeedingCompletion = castedSpell;
					actionShortcut.AttackingAbilityModifier = player.GetSpellcastingAbilityModifier();
					actionShortcut.ProficiencyBonus = (int)Math.Round(player.proficiencyBonus);
				}

				tbxDamageDice.Text = spell.DieStr;
				spellCaster = player;
			}
			else
			{
				if (actionShortcut.Windups.Count > 0)
				{
					List<WindupDto> windups = actionShortcut.CloneWindups();

					string serializedObject = JsonConvert.SerializeObject(windups);
					HubtasticBaseStation.AddWindup(serializedObject);
				}
			}

			actionShortcut.ExecuteCommands(player);
			player.Use(actionShortcut);

			// TODO: keepExistingModifier????
			// if (actionShortcut.PlusModifier != keepExistingModifier)
			if (actionShortcut.ToHitModifier > 0)
				tbxModifier.Text = "+" + actionShortcut.ToHitModifier.ToString();
			else
				tbxModifier.Text = actionShortcut.ToHitModifier.ToString();

			settingInternally = true;
			try
			{
				HighlightPlayerShortcut(actionShortcut.Index);
				if (!string.IsNullOrWhiteSpace(actionShortcut.AddDice))
					tbxDamageDice.Text += "," + actionShortcut.AddDice;
				else if (spell == null)
					tbxDamageDice.Text = actionShortcut.Dice;

				if (actionShortcut.MinDamage != keepExistingModifier)
					tbxMinDamage.Text = actionShortcut.MinDamage.ToString();

				tbxAddDiceOnHit.Text = actionShortcut.AddDiceOnHit;
				tbxMessageAddDiceOnHit.Text = actionShortcut.AddDiceOnHitMessage;

				ckbUseMagic.IsChecked = actionShortcut.UsesMagic;
				NextDieRollType = actionShortcut.Type;

				if (actionShortcut.VantageMod != VantageKind.Normal)
					SetVantageForActivePlayer(actionShortcut.VantageMod);

				if (!string.IsNullOrWhiteSpace(actionShortcut.InstantDice))
				{
					DiceRollType type = actionShortcut.Type;
					if (type == DiceRollType.None)
						type = DiceRollType.DamageOnly;
					DiceRoll diceRoll = PrepareRoll(type);
					diceRoll.SecondRollTitle = actionShortcut.AdditionalRollTitle;
					diceRoll.DamageDice = actionShortcut.InstantDice;
					RollTheDice(diceRoll);
				}

			}
			finally
			{
				settingInternally = false;
			}
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
			ActivatePendingShortcuts();
			if (buildingTabs)
				return;
			if (rbActivePlayer.IsChecked == true)
			{
				CheckOnlyOnePlayer(ActivePlayerId);
			}
			InitializeActivePlayerData();
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			HubtasticBaseStation.ClearWindup("Weapon.*");
			HubtasticBaseStation.ClearWindup("Windup.*");
			SetActionShortcuts(ActivePlayerId);
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

		private void HandleCharacterChanged(object sender, RoutedEventArgs e)
		{
			if (sender is CharacterSheets characterSheets)
			{
				string character = characterSheets.GetCharacter();
				HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, character);
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
			Character player = null;
			if (castedSpellNeedingCompletion != null)
			{
				game.CompleteCast(spellCaster, castedSpellNeedingCompletion);
				diceRoll.DamageDice = castedSpellNeedingCompletion.DieStr;
				spellCaster = null;
				castedSpellNeedingCompletion = null;
			}

			if (diceRoll.PlayerRollOptions.Count == 1)
			{
				PlayerRollOptions playerRollOptions = diceRoll.PlayerRollOptions[0];
				player = game.GetPlayerFromId(playerRollOptions.PlayerID);
				if (player != null)
				{
					if (!string.IsNullOrWhiteSpace(player.additionalDiceThisRoll))
					{
						diceRoll.DamageDice += "," + player.additionalDiceThisRoll;
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
				player.ReadyRollDice(diceRoll.Type, diceRoll.DamageDice, (int)Math.Round(diceRoll.HiddenThreshold));


			Dispatcher.Invoke(() =>
			{
				showClearButtonTimer.Start();
				rbTestNormalDieRoll.IsChecked = true;
				updateClearButtonTimer.Stop();
				EnableDiceRollButtons(false);
				btnClearDice.Visibility = Visibility.Hidden;
				PrepareForClear();
			});
			string serializedObject = JsonConvert.SerializeObject(diceRoll);
			HubtasticBaseStation.RollDice(serializedObject);

			if (player != null)
				player.ResetPlayerRollBasedState();
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
			spRollNowButtons.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
			HubtasticBaseStation.ClearDice();
			waitingToClearDice = false;
			ActivatePendingShortcutsIn(1);
		}

		private void BtnSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.SkillCheck));
		}

		private void BtnSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.SavingThrow));
		}

		private void BtnDeathSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.DeathSavingThrow));
		}

		private void BtnAttack_Click(object sender, RoutedEventArgs e)
		{
			DiceRollType rollType;
			if (NextDieRollType == DiceRollType.None)
				rollType = DiceRollType.Attack;
			else
				rollType = NextDieRollType;
			RollTheDice(PrepareRoll(rollType));
		}

		bool CanIncludeVantageDice(DiceRollType type)
		{
			return (type == DiceRollType.Attack || type == DiceRollType.ChaosBolt || type == DiceRollType.DeathSavingThrow || type == DiceRollType.FlatD20 || type == DiceRollType.SavingThrow || type == DiceRollType.SkillCheck);
		}

		private DiceRoll PrepareRoll(DiceRollType type)
		{
			VantageKind diceRollKind = VantageKind.Normal;

			if (CanIncludeVantageDice(type))
				if (rbTestAdvantageDieRoll.IsChecked == true)
					diceRollKind = VantageKind.Advantage;
				else if (rbTestDisadvantageDieRoll.IsChecked == true)
					diceRollKind = VantageKind.Disadvantage;

			string damageDice = string.Empty;
			if (DndUtils.IsAttack(type) || type == DiceRollType.DamageOnly || type == DiceRollType.HealthOnly || type == DiceRollType.ExtraOnly)
				damageDice = tbxDamageDice.Text;

			DiceRoll diceRoll = new DiceRoll(diceRollKind, damageDice);
			diceRoll.GroupInspiration = tbxInspiration.Text;
			diceRoll.AdditionalDiceOnHit = tbxAddDiceOnHit.Text;
			diceRoll.AdditionalDiceOnHitMessage = tbxMessageAddDiceOnHit.Text;
			diceRoll.CritFailMessage = "";
			diceRoll.CritSuccessMessage = "";
			diceRoll.SuccessMessage = "";
			diceRoll.FailMessage = "";
			diceRoll.SkillCheck = Skills.none;
			diceRoll.SavingThrow = Ability.none;
			diceRoll.SpellName = "";
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
				}
			}

			switch (type)
			{
				case DiceRollType.SkillCheck:
					diceRoll.CritFailMessage = "COMPLETE FAILURE!";
					diceRoll.CritSuccessMessage = "Nat 20!";
					diceRoll.SuccessMessage = "Success!";
					diceRoll.FailMessage = "Fail!";
					break;
				case DiceRollType.Attack:
				case DiceRollType.ChaosBolt:
					diceRoll.CritFailMessage = "SPECTACULAR MISS!";
					diceRoll.CritSuccessMessage = "Critical Hit!";
					diceRoll.SuccessMessage = "Hit!";
					diceRoll.FailMessage = "Miss!";
					break;
				case DiceRollType.SavingThrow:
					diceRoll.CritFailMessage = "COMPLETE FAILURE!";
					diceRoll.CritSuccessMessage = "Critical Success!";
					diceRoll.SuccessMessage = "Success!";
					diceRoll.FailMessage = "Fail!";
					break;
				case DiceRollType.DeathSavingThrow:
					diceRoll.CritFailMessage = "COMPLETE FAILURE!";
					diceRoll.CritSuccessMessage = "Critical Success!";
					diceRoll.SuccessMessage = "Success!";
					diceRoll.FailMessage = "Fail!";
					diceRoll.HiddenThreshold = 10;
					break;
			}


			diceRoll.ThrowPower = new Random().Next() * 2.8;
			if (diceRoll.ThrowPower < 0.3)
				diceRoll.ThrowPower = 0.3;

			if (type == DiceRollType.DeathSavingThrow)
				diceRoll.HiddenThreshold = 10;
			else if (type == DiceRollType.Initiative || type == DiceRollType.NonCombatInitiative)
				diceRoll.HiddenThreshold = -100;
			else if (double.TryParse(tbxHiddenThreshold.Text, out double thresholdResult))
				diceRoll.HiddenThreshold = thresholdResult;

			diceRoll.IsMagic = (ckbUseMagic.IsChecked == true && DndUtils.IsAttack(type)) || type == DiceRollType.WildMagicD20Check;
			diceRoll.Type = type;

			diceRoll.AddTrailingEffects(activeTrailingEffects);
			diceRoll.AddDieRollEffects(activeDieRollEffects);

			return diceRoll;
		}

		private void BtnFlatD20_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.FlatD20));
		}

		private void BtnWildMagicD20Check_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.WildMagicD20Check);
			diceRoll.NumHalos = 3;
			diceRoll.AddTrailingEffects("SmallSparks");
			RollTheDice(diceRoll);
		}

		private void BtnAddLongRest_Click(object sender, RoutedEventArgs e)
		{
			Rest(8);
		}

		private void BtnAddShortRest_Click(object sender, RoutedEventArgs e)
		{
			Rest(2);
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
			OnCombatChanged();
			UpdateClock();
			StartRealTimeTimer();
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
			get => nextDieRollType; set
			{
				if (nextDieRollType == value)
					return;

				nextDieRollType = value;
				if (nextDieRollType != DiceRollType.None)
					tbNextDieRoll.Text = $"({nextDieRollType})";
				else
					tbNextDieRoll.Text = "";
			}
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
			spRollNowButtons.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
		}

		void EnableDiceRollButtons(bool enable)
		{
			Dispatcher.Invoke(() =>
			{
				spRollNowButtons.IsEnabled = enable;
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
				case Skills.slightOfHand: return "Slight of Hand";
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
		void HandleWildMagicD20Check(IndividualRoll individualRoll)
		{
			if (individualRoll.value == 1)
			{
				PlayScene("DH.WildMagicRoll");
				wildMagicRollTimer.Start();
				TellDungeonMaster("It's a one! Need to roll wild magic!");
			}
			else
			{
				TellDungeonMaster($"Wild Magic roll: {individualRoll.value}.");
			}
		}
		// TODO: Delete this.
		void NeedToRollWildMagic(IndividualRoll individualRoll)
		{
			
		}
		void IndividualDiceStoppedRolling(IndividualRoll individualRoll)
		{
			switch (individualRoll.type)
			{
				case "BarbarianWildSurge":
					HandleBarbarianWildSurge(individualRoll);
					break;
				case "WildMagicD20Check":
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
		void CheckForFollowUpRolls(DiceStoppedRollingData diceStoppedRollindData)
		{
			if (diceStoppedRollindData == null)
				return;
			// Noticed doubling up of individual rolls.
			Character singlePlayer = diceStoppedRollindData.GetSingleRollingPlayer();
			if (singlePlayer == null)
				return;

			if (!string.IsNullOrWhiteSpace(diceStoppedRollindData.spellName))
				singlePlayer.JustCastSpell(diceStoppedRollindData.spellName);
			else if (diceStoppedRollindData.type == DiceRollType.Attack)
				singlePlayer.JustSwungWeapon();

			// TODO: pass **targeted creatures** into RollIsComplete...
			singlePlayer.RollIsComplete(diceStoppedRollindData.wasCriticalHit);

			//diceRollData.playerID
			//if (diceRollData.isSpell)
		}
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			waitingToClearDice = true;
			if (ea.StopRollingData.individualRolls?.Count > 0)
			{
				IndividualDiceStoppedRolling(ea.StopRollingData.individualRolls);
			}
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
			HubtasticBaseStation.ClearWindup("Windup.*");
			HubtasticBaseStation.ClearWindup("Weapon.*");
			if (ea.StopRollingData != null)
			{
				int rollValue = ea.StopRollingData.roll;
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
						rollTitle = "Damage Only: ";
						rollValue = ea.StopRollingData.damage;
						break;
					case DiceRollType.HealthOnly:
						rollTitle = "Health Only: ";
						rollValue = ea.StopRollingData.health;
						break;
					case DiceRollType.ExtraOnly:
						rollTitle = "Extra Only: ";
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
						string playerName = StrUtils.GetFirstName(playerRoll.name);
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
					TellDungeonMaster(message);
					TellViewers(message);
				}
			}


			EnableDiceRollButtons(true);
			ShowClearButton(null, EventArgs.Empty);
			CheckForFollowUpRolls(ea.StopRollingData);
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
					return StrUtils.GetFirstName(player.name);
			}

			return "";
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			game.Clock.InCombat = !game.Clock.InCombat;
			if (game.Clock.InCombat)
			{
				btnEnterExitCombat.Background = new SolidColorBrush(Color.FromRgb(42, 42, 102));
				RollInitiative();
			}
			else
				btnEnterExitCombat.Background = new SolidColorBrush(Colors.DarkRed);

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

		void RollWildMagicHandler(object sender, EventArgs e)
		{
			wildMagicRollTimer.Stop();
			Dispatcher.Invoke(() =>
			{
				ActivateShortcut("Wild Magic");
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
			const double timeToAutoClear = 9000;
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

		private void BtnPaladinSmite_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.OnThrowSound = "PaladinThunder";
			diceRoll.NumHalos = 3;
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Raven,
				LeftRightDistanceBetweenPrints = 15,
				MinForwardDistanceBetweenPrints = 5,
				OnPrintPlaySound = "Flap[6]",
				MedianSoundInterval = 600,
				PlusMinusSoundInterval = 300
			});
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Spiral,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 150,
				OnPrintPlaySound = "Crow[3]",
				MedianSoundInterval = 500,
				PlusMinusSoundInterval = 300
			});
			RollTheDice(diceRoll);
		}

		private void BtnSneakAttack_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.OnThrowSound = "SneakAttackWhoosh";
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Smoke,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 120,  // 120 + Random.plusMinus(30)
			});
			diceRoll.OnFirstContactSound = "SneakAttack";
			diceRoll.OnFirstContactEffect = "SmokeExplosion";
			RollTheDice(diceRoll);
		}

		private void BtnWildMagic_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = new DiceRoll();
			diceRoll.Modifier = 0;
			diceRoll.HiddenThreshold = 0;
			diceRoll.IsMagic = true;
			diceRoll.OnThrowSound = "WildMagicRoll";
			diceRoll.Type = DiceRollType.WildMagic;
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.SparkTrail,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 84
			});

			RollTheDice(diceRoll);
		}

		private void BtnWildAnimalForm_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.OnFirstContactSound = "WildForm";
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.PawPrint,
				LeftRightDistanceBetweenPrints = 50,
				MinForwardDistanceBetweenPrints = 90,
			});

			RollTheDice(diceRoll);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			HubtasticBaseStation.DiceStoppedRolling -= HubtasticBaseStation_DiceStoppedRolling;
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
				Type = TrailingSpriteType.SmallSparks,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 33
			});
		}

		private void BtnRollHealthOnly_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.HealthOnly);
			diceRoll.HiddenThreshold = 0;
			RollTheDice(diceRoll);
		}

		private void BtnRollDamageOnly_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.DamageOnly);
			diceRoll.HiddenThreshold = 0;
			RollTheDice(diceRoll);
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

		private void BtnHiddenThreshold_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
				tbxHiddenThreshold.Text = button.Tag.ToString();
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
			actionShortcuts.Clear();
			PlayerActionShortcut.PrepareForCreation();
			AllActionShortcuts.LoadData();
			actionShortcuts = AllActionShortcuts.AllShortcuts;
		}


		bool settingInternally;

		private void TbxDamageDice_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlights();
		}

		private void TbxModifier_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlights();
		}

		private void CkbUseMagic_Checked(object sender, RoutedEventArgs e)
		{
			if (settingInternally)
				return;
			HidePlayerShortcutHighlights();
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

					StackPanel stackPanel = new StackPanel();
					tabItem.Content = stackPanel;

					Grid grid = new Grid();
					grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
					stackPanel.Children.Add(grid);

					CharacterSheets characterSheets = new CharacterSheets();
					characterSheets.PageChanged += CharacterSheets_PageChanged;
					characterSheets.PageBackgroundClicked += CharacterSheets_PageBackgroundClicked;
					characterSheets.CharacterChanged += HandleCharacterChanged;
					characterSheets.SetFromCharacter(player);
					characterSheets.SkillCheckRequested += CharacterSheets_SkillCheckRequested;
					characterSheets.SavingThrowRequested += CharacterSheets_SavingThrowRequested;
					grid.Children.Add(characterSheets);
					tabItem.CharacterSheets = characterSheets;

					Button button = new Button();
					button.Content = "Hide Scroll";
					button.Click += Button_ClearScrollClick;
					button.MaxWidth = 200;
					button.MinHeight = 45;
					stackPanel.Children.Add(button);
				}
			}
			finally
			{
				buildingTabs = false;
			}
		}

		private void CharacterSheets_SavingThrowRequested(object sender, AbilityEventArgs e)
		{
			SelectSavingThrowAbility(e.Ability);
			rbActivePlayer.IsChecked = true;
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
			else if (skill == Skills.slightOfHand)
				lowerCaseSkillName = "slight of hand";
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
			InvokeSkillCheck(e.Skill);
		}

		public void InvokeSkillCheck(Skills skill, bool allPlayers = false)
		{
			Dispatcher.Invoke(() =>
			{
				SelectSkill(skill);
				if (allPlayers)
					rbEveryone.IsChecked = true;
				else
					rbActivePlayer.IsChecked = true;
				RollTheDice(PrepareRoll(DiceRollType.SkillCheck));
			});
		}

		public void InvokeSavingThrow(Ability ability, bool allPlayers)
		{
			Dispatcher.Invoke(() =>
			{
				SelectSavingThrowAbility(ability);
				if (allPlayers)
					rbEveryone.IsChecked = true;
				else
					rbActivePlayer.IsChecked = true;
				RollTheDice(PrepareRoll(DiceRollType.SavingThrow));
			});
		}

		void EnterCombat()
		{
			Dispatcher.Invoke(() =>
			{
				if (game.Clock.InCombat)
					TellDungeonMaster("Already in combat!");
				else
				{
					TellDungeonMaster("Entering combat...");
					BtnEnterExitCombat_Click(null, null);
				}
			});
		}
		void ExitCombat()
		{
			Dispatcher.Invoke(() =>
			{
				if (!game.Clock.InCombat)
					TellDungeonMaster("Already NOT in combat!");
				else
				{
					TellDungeonMaster("Exiting combat...");
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
					TellDungeonMaster("Rolling non-combat initiative...");
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

		public void RollSkillCheck(Skills skill, bool allPlayers = false)
		{
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

			InvokeSkillCheck(skill, allPlayers);

			string articlePlusSkillDislay = DndUtils.ToArticlePlusSkillDisplayString(skill);
			string who;
			if (allPlayers)
				who = "all players";
			else
				who = GetPlayerName(ActivePlayerId);
			string firstPart = $"Rolling {articlePlusSkillDislay} skill check for {who}";
			TellDungeonMaster($"{firstPart}{PlusHiddenThresholdDisplayStr()}.");
			TellViewers($"{firstPart}...");
		}

		public void RollAttack()
		{
			Dispatcher.Invoke(() =>
			{
				BtnAttack_Click(null, null);
			});
			// TODO: Add report on advantage/disadvantage.
			TellDungeonMaster($"Rolling {GetPlayerName(ActivePlayerId)}'s attack with a hidden threshold of {tbxHiddenThreshold.Text} and damage dice of {tbxDamageDice.Text}.");
		}

		public void RollSavingThrow(Ability ability, bool allPlayers = false)
		{
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

			InvokeSavingThrow(ability, allPlayers);
			string abilityStr = DndUtils.ToArticlePlusAbilityDisplayString(ability);

			string who;
			if (allPlayers)
				who = "all players";
			else
				who = GetPlayerName(ActivePlayerId);

			string firstPart = $"Rolling {abilityStr} saving throw for {who}";
			TellDungeonMaster($"{firstPart}{PlusHiddenThresholdDisplayStr()}...");
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
			game.ClearAllAlarms();
			//PlayerFactory.BuildPlayers(players);
			game.Players.Clear();
			AllPlayers.LoadData();
			AllSpells.LoadData();
			AllFeatures.LoadData();
			AllDieRollEffects.LoadData();
			AllTrailingEffects.LoadData();

			List<Character> players = AllPlayers.GetActive();

			string playerData = JsonConvert.SerializeObject(players);
			HubtasticBaseStation.SetPlayerData(playerData);

			foreach (Character player in players)
			{
				game.AddPlayer(player);
			}

			BuildPlayerTabs();
			BuildPlayerUI();
			InitializeAttackShortcuts();
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
				checkBox.Content = StrUtils.GetFirstName(player.name);
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
				spOptions.Children.Add(rbNormalRoll);
				checkBox.RbNormal = rbNormalRoll;

				RadioButton rbAdvantageRoll = new RadioButton();
				rbAdvantageRoll.Content = "Adv.";
				rbAdvantageRoll.Margin = new Thickness(14, 0, 0, 0);
				spOptions.Children.Add(rbAdvantageRoll);
				checkBox.RbAdvantage = rbAdvantageRoll;

				RadioButton rbDisadvantageRoll = new RadioButton();
				rbDisadvantageRoll.Content = "Disadv.";
				rbDisadvantageRoll.Margin = new Thickness(14, 0, 0, 0);
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
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				if (uIElement is PlayerRollCheckBox checkbox)
					checkbox.IsChecked = checkbox.PlayerId == playerID;
		}

		bool buildingTabs;

		private void BtnRollExtraOnly_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.ExtraOnly);
			diceRoll.HiddenThreshold = 0;
			RollTheDice(diceRoll);
		}

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

		public int GetPlayerIdFromNameStart(string characterName)
		{
			return AllPlayers.GetPlayerIdFromNameStart(game.Players, characterName);
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

		public void ApplyDamageHealthChange(DamageHealthChange damageHealthChange)
		{
			if (damageHealthChange == null)
				return;
			string playerNames = string.Empty;
			if (damageHealthChange.PlayerIds.Count == 0)
			{
				damageHealthChange.PlayerIds.Add(ActivePlayerId);
			}
			int numPlayers = damageHealthChange.PlayerIds.Count;
			for (int i = 0; i < numPlayers; i++)
			{
				int playerId = damageHealthChange.PlayerIds[i];
				Character player = GetPlayer(playerId);
				if (player == null)
					continue;
				string firstName = StrUtils.GetFirstName(player.name);
				if (i < numPlayers - 2)
					playerNames += firstName + ", ";
				else if (i < numPlayers - 1)
					playerNames += firstName + ", and ";
				else
					playerNames += firstName;

				player.ChangeHealth(damageHealthChange.DamageHealth);
				HubtasticBaseStation.PlayerDataChanged(playerId, player.ToJson());
			}

			HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));
			string message;
			if (damageHealthChange.DamageHealth < 0)
				message = $"{-damageHealthChange.DamageHealth} points of damage dealt to {playerNames}.";
			else
				message = $"{damageHealthChange.DamageHealth} points of healing given to {playerNames}.";

			TellDungeonMaster(message);
			TellViewers(message);
		}

		private DamageHealthChange GetDamageHealthChange(int multiplier, TextBox textBox)
		{
			DamageHealthChange damageHealthChange;
			int damage;

			if (int.TryParse(textBox.Text, out damage))
			{
				damageHealthChange = new DamageHealthChange();
				damageHealthChange.DamageHealth = damage * multiplier;
				foreach (UIElement uIElement in grdPlayerRollOptions.Children)
					if (uIElement is PlayerRollCheckBox checkbox && checkbox.IsChecked == true)
						damageHealthChange.PlayerIds.Add(checkbox.PlayerId);
			}
			else
				damageHealthChange = null;
			return damageHealthChange;
		}


		private void RbActivePlayer_Checked(object sender, RoutedEventArgs e)
		{
			if (radioingInternally)
				return;
			if (spRollButtons == null)
				return;
			spRollButtons.Visibility = Visibility.Visible;
			ShowHidePlayerUI(true);
			CheckActivePlayer();
		}

		private void RbEveryone_Checked(object sender, RoutedEventArgs e)
		{
			if (radioingInternally)
				return;
			if (spRollButtons == null)
				return;
			spRollButtons.Visibility = Visibility.Collapsed;
			ShowHidePlayerUI(true);
			CheckAllPlayers();
		}

		bool checkingInternally;
		string lastScenePlayed;
		PlayerActionShortcut shortcutToActivateAfterClearingDice;
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
			if (spRollButtons == null)
				return;
			spRollButtons.Visibility = Visibility.Collapsed;
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
				Type = TrailingSpriteType.Fangs,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 120,  // 120 + Random.plusMinus(30)
			});
			diceRoll.OnFirstContactSound = "Snarl";
			//diceRoll.OnFirstContactEffect = TrailingSpriteType.Fangs;
			RollTheDice(diceRoll);
		}

		private void BtnInspirationOnly_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.InspirationOnly));
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

		public void SelectPlayerShortcut(string shortcutName, int playerId)
		{
			Dispatcher.Invoke(() =>
			{
				if (playerId != ActivePlayerId)
				{
					TellDungeonMaster($"===================> {GetNopeMessage()}. {GetPlayerName(playerId)} is not the active player!");
					return;
				}
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
							TellDungeonMaster($"----- {GetPlayerName(playerId)} -----");
							break;
						}
					}
			});
		}
		public void SetSpellSlotLevel(int level)
		{
			Dispatcher.Invoke(() =>
			{
				tbxSpellSlot.Text = level.ToString();
			});
		}


		public void RollWildMagic()
		{
			BtnWildMagic_Click(null, null);
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
			Dispatcher.Invoke(() =>
			{

			});
		}

		public void RollDice(string diceStr, DiceRollType diceRollType)
		{
			Dispatcher.Invoke(() =>
			{

			});
			// TODO: Tell DM
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
				dmMessage = $"Unable to play {sceneName}: {ex.Message}";
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
			History.Log(message);
			if (dungeonMasterClient == null)
				return;
			if (!JoinedChannel(DungeonMasterChannel))
			{
				try
				{
					dungeonMasterClient.JoinChannel(DungeonMasterChannel);
					// TODO: determine whether we are showing detail messages or not and suppress this message if we are not showing detail messages and isDetail is true.
					dungeonMasterClient.SendMessage(DungeonMasterChannel, message);
				}
				catch (TwitchLib.Client.Exceptions.ClientNotConnectedException)
				{
					CreateDungeonMasterClient();
					try
					{
						dungeonMasterClient.JoinChannel(DungeonMasterChannel);
						// TODO: determine whether we are showing detail messages or not and suppress this message if we are not showing detail messages and isDetail is true.
						dungeonMasterClient.SendMessage(DungeonMasterChannel, message);
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
			if (!JoinedChannel(DragonHumpersChannel))
			{
				try
				{
					dungeonMasterClient.JoinChannel(DragonHumpersChannel);
					dungeonMasterClient.SendMessage(DragonHumpersChannel, message);
				}
				catch (Exception ex)
				{

				}
			}
		}

		private void BtnSpellSlot_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
				tbxSpellSlot.Text = button.Tag.ToString();
		}

		void ActivatePendingShortcuts(object sender, EventArgs e)
		{
			pendingShortcutsTimer.Stop();
			ActivatePendingShortcuts();
		}
	}

	/* 
	  Name									Index		Effect				effectAvailableWhen		playToEndOnExpire	 hue	moveUpDown
		Melf's Minute Meteors.6				Staff.Weapon	Casting								x										30	150				
		Melf's Minute Meteors.7				Staff.Magic		Casting								x									 350	150				
	 */
}
