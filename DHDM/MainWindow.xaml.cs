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
		DndTimeClock dndTimeClock;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer showClearButtonTimer;
		DispatcherTimer updateClearButtonTimer;
		DateTime lastUpdateTime;
		int keepExistingModifier = int.MaxValue;

		public MainWindow()
		{
			realTimeAdvanceTimer = new DispatcherTimer(DispatcherPriority.Send);
			realTimeAdvanceTimer.Tick += new EventHandler(RealTimeClockHandler);
			realTimeAdvanceTimer.Interval = TimeSpan.FromMilliseconds(200);

			showClearButtonTimer = new DispatcherTimer();
			showClearButtonTimer.Tick += new EventHandler(ShowClearButton);
			showClearButtonTimer.Interval = TimeSpan.FromSeconds(8);

			updateClearButtonTimer = new DispatcherTimer(DispatcherPriority.Send);
			updateClearButtonTimer.Tick += new EventHandler(UpdateClearButton);
			updateClearButtonTimer.Interval = TimeSpan.FromMilliseconds(80);

			dndTimeClock = new DndTimeClock();
			History.TimeClock = dndTimeClock;
			dndTimeClock.TimeChanged += DndTimeClock_TimeChanged;
			// TODO: Save and retrieve game time.
			dndTimeClock.SetTime(DateTime.Now);
			InitializeComponent();
			FocusHelper.FocusedControlsChanged += FocusHelper_FocusedControlsChanged;
			groupEffectBuilder.Entries = new ObservableCollection<TimeLineEffect>();
			spTimeSegments.DataContext = dndTimeClock;
			logListBox.ItemsSource = History.Entries;
			History.LogUpdated += History_LogUpdated;

			//InitializeAttackShortcuts();
			//humperBotClient = Twitch.CreateNewClient("HumperBot", "HumperBot", "HumperBotOAuthToken");
			dungeonMasterClient = Twitch.CreateNewClient("DragonHumpersDM", "DragonHumpersDM", "DragonHumpersDmOAuthToken");

			if (dungeonMasterClient != null)
				dungeonMasterClient.OnMessageReceived += HumperBotClient_OnMessageReceived;

			dmChatBot.Initialize(this);

			dmChatBot.DungeonMasterApp = this;
			commandParsers.Add(dmChatBot);
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
			StackPanel stackPanel = new StackPanel();
			stackPanel.Margin = new Thickness(2);
			stackPanel.Tag = playerActionShortcut.Index;
			Button button = new Button();
			button.Padding = new Thickness(4, 2, 4, 2);
			stackPanel.Children.Add(button);
			button.Content = playerActionShortcut.Name;
			button.ToolTip = GetToolTip(playerActionShortcut.Description);
			button.Tag = playerActionShortcut.Index;
			button.Click += PlayerShortcutButton_Click;
			Rectangle rectangle = new Rectangle();
			stackPanel.Children.Add(rectangle);
			rectangle.Tag = playerActionShortcut.Index;
			rectangle.Visibility = Visibility.Hidden;
			rectangle.Height = 3;
			rectangle.Fill = new SolidColorBrush(Colors.Red);

			highlightRectangles.Add(playerActionShortcut.Index, rectangle);
			return stackPanel;
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

		void CreateAlarm(Spell spell, int playerId)
		{
			CreateAlarm(spell.Duration.GetTimeSpan(), GetAlarmName(spell, playerId));
		}

		private static string GetAlarmName(Spell spell, int playerId)
		{
			return $"{STR_EndSpell}{spell.Name}({playerId})";
		}

		void CreateAlarm(TimeSpan timeSpan, string name)
		{
			DndAlarm dndAlarm = dndTimeClock.CreateAlarm(timeSpan, name);
			dndAlarm.AlarmFired += DndAlarm_AlarmFired;
		}

		private void DndAlarm_AlarmFired(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Name.StartsWith(STR_EndSpell))
			{
				string spellToEnd = ea.Alarm.Name.Substring(STR_EndSpell.Length);
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
						BreakConcentration(playerId);
						casterId = $"{GetPlayerName(playerId)}'s ";
					}
					spellToEnd = spellToEnd.Substring(0, parenPos);
				}
				TellDungeonMaster($"{casterId} {spellToEnd} spell ends at {dndTimeClock.AsFullDndDateTimeString()}.");
			}
		}

		Dictionary<int, Spell> concentratedSpells = new Dictionary<int, Spell>();

		void Dispel(Spell spell, int playerId)
		{
			HubtasticBaseStation.ClearWindup($"{spell.Name}({playerId})");
		}

		public void BreakConcentration(int playerId)
		{
			// TODO: Add support for Twinning spells.
			if (concentratedSpells.ContainsKey(playerId))
			{
				Dispel(concentratedSpells[playerId], playerId);
				concentratedSpells.Remove(playerId);
			}
		}

		void PlayerIsNowConcentrating(int playerId, Spell spell)
		{
			BreakConcentration(playerId);
			concentratedSpells.Add(playerId, spell);
		}


		void PrepareToCastSpell(Spell spell, int playerId)
		{
			spell.OwnerId = playerId;
			TellDungeonMaster($"{GetPlayerName(playerId)} casts {spell.Name} at {dndTimeClock.AsFullDndDateTimeString()}.");
			if (spell.Duration.HasValue())
				CreateAlarm(spell, playerId);
			if (spell.RequiresConcentration)
				PlayerIsNowConcentrating(playerId, spell);
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

		private void ActivateShortcut(PlayerActionShortcut actionShortcut)
		{
			activeTrailingEffects = string.Empty;
			activeDieRollEffects = string.Empty;
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

			HubtasticBaseStation.ClearWindup("*");

			Character player = GetPlayer(actionShortcut.PlayerId);
			actionShortcut.ExecuteCommands(player);
			Spell spell = actionShortcut.Spell;
			if (spell != null)
			{
				PrepareToCastSpell(spell, actionShortcut.PlayerId);

				CastedSpell spellToCast = new CastedSpell(spell, new SpellTarget() { Target = SpellTargetType.Player, PlayerId = PlayerID.Merkin });
				spellToCast.Windups = actionShortcut.WindupsReversed;
				string serializedObject = JsonConvert.SerializeObject(spellToCast);
				HubtasticBaseStation.CastSpell(serializedObject);
				tbxDamageDice.Text = spell.DieStr;
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
			if (buildingTabs)
				return;
			if (rbActivePlayer.IsChecked == true)
			{
				CheckOnlyOnePlayer(ActivePlayerId);
			}
			InitializeActivePlayerData();
			HubtasticBaseStation.PlayerDataChanged(ActivePlayerId, activePage, string.Empty);
			HubtasticBaseStation.ClearWindup("*");
			SetActionShortcuts(ActivePlayerId);
		}

		private void InitializeActivePlayerData()
		{
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
		}

		void PrepareForClear()
		{
			//showClearButtonTimer.Start();
		}

		public void ClearTheDice()
		{
			updateClearButtonTimer.Stop();
			btnClearDice.Visibility = Visibility.Hidden;
			spRollNowButtons.IsEnabled = true;
			spSpecialThrows.IsEnabled = true;
			HubtasticBaseStation.ClearDice();
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

		bool IsAttack(DiceRollType type)
		{
			return type == DiceRollType.Attack || type == DiceRollType.ChaosBolt;
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
			if (IsAttack(type) || type == DiceRollType.DamageOnly || type == DiceRollType.HealthOnly || type == DiceRollType.ExtraOnly)
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

			diceRoll.IsMagic = (ckbUseMagic.IsChecked == true && IsAttack(type)) || type == DiceRollType.WildMagicD20Check;
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
			RollTheDice(PrepareRoll(DiceRollType.WildMagicD20Check));
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
				dndTimeClock.Advance(DndTimeSpan.FromHours(hours));
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
			string timeStr = dndTimeClock.AsFullDndDateTimeString();

			if (dndTimeClock.InCombat)
				timeStr = " " + timeStr + " ";

			if (txtTime.Text == timeStr)
				return;

			txtTime.Text = timeStr;

			TimeSpan timeIntoToday = dndTimeClock.Time - new DateTime(dndTimeClock.Time.Year, dndTimeClock.Time.Month, dndTimeClock.Time.Day);
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
				InCombat = dndTimeClock.InCombat,
				FullSpins = daysSinceLastUpdate,
				AfterSpinMp3 = afterSpinMp3
			};

			string serializedObject = JsonConvert.SerializeObject(clockDto);
			HubtasticBaseStation.UpdateClock(serializedObject);
		}

		private void BtnAdvanceTurn_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromSeconds(6));
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
			dndTimeClock.Advance(DndTimeSpan.FromDays(1), ShiftKeyDown);
		}

		private void BtnAddTenDay_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromDays(10), ShiftKeyDown);
		}

		private void BtnAddMonth_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromDays(30), ShiftKeyDown);
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
			dndTimeClock.Advance(DndTimeSpan.FromHours(1), ShiftKeyDown);
		}

		private void BtnAdd10Minutes_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromMinutes(10), ShiftKeyDown);
		}

		private void BtnAdd1Minute_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromMinutes(1), ShiftKeyDown);
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
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			// TODO: Clear the actual windup by name.
			HubtasticBaseStation.ClearWindup("*");
			if (ea.DiceRollData != null)
			{
				int rollValue = ea.DiceRollData.roll;
				string additionalMessage = ea.DiceRollData.additionalDieRollMessage;
				if (!String.IsNullOrEmpty(additionalMessage))
					additionalMessage = " " + additionalMessage;
				string rollTitle = "";
				string damageStr = "";
				string bonusStr = "";
				if (ea.DiceRollData.bonus > 0)
					bonusStr = " - bonus: " + ea.DiceRollData.bonus.ToString();
				string successStr = GetSuccessStr(ea.DiceRollData.success, ea.DiceRollData.type);
				switch (ea.DiceRollData.type)
				{
					case DiceRollType.SkillCheck:
						rollTitle = GetSkillCheckStr(ea.DiceRollData.skillCheck) + " Skill Check: ";
						break;
					case DiceRollType.Attack:
						rollTitle = "Attack: ";
						damageStr = ", Damage: " + ea.DiceRollData.damage.ToString();
						break;
					case DiceRollType.SavingThrow:
						rollTitle = GetAbilityStr(ea.DiceRollData.savingThrow) + " Saving Throw: ";
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
						rollValue = ea.DiceRollData.damage;
						break;
					case DiceRollType.HealthOnly:
						rollTitle = "Health Only: ";
						rollValue = ea.DiceRollData.health;
						break;
					case DiceRollType.ExtraOnly:
						rollTitle = "Extra Only: ";
						rollValue = ea.DiceRollData.extra;
						break;
					case DiceRollType.ChaosBolt:
						rollTitle = "Chaos Bolt: ";
						damageStr = ", Damage: " + ea.DiceRollData.damage.ToString();
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
				if (ea.DiceRollData.multiplayerSummary != null && ea.DiceRollData.multiplayerSummary.Count > 0)
				{
					foreach (PlayerRoll playerRoll in ea.DiceRollData.multiplayerSummary)
					{
						string playerName = StrUtils.GetFirstName(playerRoll.name);
						if (playerName != "")
							playerName = playerName + "'s ";

						rollValue = playerRoll.modifier + playerRoll.roll;
						bool success = rollValue >= ea.DiceRollData.hiddenThreshold;
						successStr = GetSuccessStr(success, ea.DiceRollData.type);
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
					string playerName = GetPlayerName(ea.DiceRollData.playerID);
					if (playerName != "")
						playerName = playerName + "'s ";
					if (!ea.DiceRollData.success)
						damageStr = "";

					message += playerName + rollTitle + rollValue.ToString() + successStr + damageStr + bonusStr;
				}

				message += additionalMessage;
				if (!string.IsNullOrWhiteSpace(message))
				{
					TellDungeonMaster(message);
					TellViewers(message);
					History.Log(message);
				}
			}


			EnableDiceRollButtons(true);
			ShowClearButton(null, EventArgs.Empty);
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
			foreach (Character player in players)
			{
				if (player.playerID == playerID)
					return StrUtils.GetFirstName(player.name);
			}

			return "";
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.InCombat = !dndTimeClock.InCombat;
			if (dndTimeClock.InCombat)
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
			if (dndTimeClock.InCombat)
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
			dndTimeClock.Advance(timeSinceLastUpdate.TotalMilliseconds);
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

		const string STR_EndSpell = "EndSpell:";

		DiceRollType nextDieRollType;
		void InitializeAttackShortcuts()
		{
			highlightRectangles = null;
			actionShortcuts.Clear();
			PlayerActionShortcut.PrepareForCreation();
			AllActionShortcuts.LoadData();
			actionShortcuts = AllActionShortcuts.AllShortcuts;
			//AddPlayerActionShortcutsForWilly();
			//AddPlayerActionShortcutsForMerkin();
			//AddPlayerActionShortcutsForAva();
			//AddPlayerActionShortcutsForShemo();
			//AddPlayerActionShortcutsForLady();
			//AddPlayerActionShortcutsForFred();
		}

		void AddPlayerActionShortcutsForFred()
		{
			AddBattleAxe("Battleaxe (1H)", PlayerID.Fred, "1d8+5(slashing)", 5);
			AddBattleAxe("Battleaxe (2H)", PlayerID.Fred, "1d10+3(slashing)", 5);

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Handaxe", PlayerId = PlayerID.Fred, Dice = "1d6+5(slashing)", PlusModifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longbow", PlayerId = PlayerID.Fred, Dice = "1d8(piercing)", PlusModifier = +2 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerId = PlayerID.Fred, Dice = "4(bludgeoning)", PlusModifier = +5, Description = "You can punch, kick, head-butt, or use a similar forceful blow and deal bludgeoning damage equal to 1 + STR modifier." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bite", PlayerId = PlayerID.Fred, Dice = "1d6+3(piercing)", PlusModifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Feinting Attack",
				PlayerId = PlayerID.Fred,
				PlusModifier = keepExistingModifier,
				AddDice = "1d8(superiority)",
				VantageMod = VantageKind.Advantage,
				Part = TurnPart.BonusAction,
				Description = "You can expend one superiority die and use a bonus action on your turn to add the total to the damage roll and to gain advantage on your next attack roll against a chosen creature within 5 ft. this turn."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Goading Attack", PlayerId = PlayerID.Fred, Type = DiceRollType.AddOnDice, InstantDice = "1d8(superiority:damage)", AdditionalRollTitle = "Goading Attack", Part = TurnPart.Special, Description = "When you hit with a weapon attack, you can expend one superiority die to add the total to the damage roll and the target must make a WIS saving throw (DC 13). On failure, the target has disadvantage on all attack rolls against targets other than you until the end of your next turn." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Trip Attack", PlayerId = PlayerID.Fred, Type = DiceRollType.AddOnDice, InstantDice = "1d8(superiority:damage)", AdditionalRollTitle = "Trip Attack", Part = TurnPart.Special, Description = "When you hit with a weapon attack, you can expend one superiority die to add the total to the damage roll, and if the target is Large or smaller, it must make a STR saving throw (DC 13). On failure, you knock the target prone." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Action Surge", PlayerId = PlayerID.Fred, Part = TurnPart.Special, Description = "You can take one additional action on your turn. This can be used 1 times per short rest." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Second Wind", PlayerId = PlayerID.Fred, Dice = "1d10+4", Type = DiceRollType.HealthOnly, Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest, you can use a bonus action to regain 1d10 + {$Level} HP." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Hungry Jaws", PlayerId = PlayerID.Fred, PlusModifier = 5, Dice = "1d6+4", Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest as a bonus action, you can make a bite attack. If it hits, you gain {$ConstitutionModifier} temporary HP." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Great Weapon Master Attack", PlayerId = PlayerID.Fred, Part = TurnPart.BonusAction, Description = "On your turn, when you score a critical hit with a melee weapon or reduce a creature to 0 HP with one, you can make one melee weapon attack as a bonus action." });
		}

		private void AddBattleAxe(string name, int playerId, string damage, int modifier, int minDamage = 0, int hueShiftMagicA = int.MinValue, int hueShiftMagicB = int.MinValue, double scale = 1.0)
		{
			PlayerActionShortcut battleAxe = new PlayerActionShortcut()
			{ Name = name, PlayerId = playerId, Dice = damage, PlusModifier = modifier, MinDamage = minDamage };

			const int deltaY = 80;
			battleAxe.Windups.Add(new WindupDto()
			{
				Effect = "BattleAxe.Weapon",
				Scale = scale,
				FadeIn = 0,
				PlayToEndOnExpire = true
			}.MoveUpDown(deltaY));

			if (hueShiftMagicA != int.MinValue)
				battleAxe.Windups.Add(new WindupDto()
				{
					Effect = "BattleAxe.MagicA",
					Scale = scale,
					FadeIn = 0,
					PlayToEndOnExpire = true,
					Hue = hueShiftMagicA
				}.MoveUpDown(deltaY));

			if (hueShiftMagicB != int.MinValue)
				battleAxe.Windups.Add(new WindupDto()
				{
					Effect = "BattleAxe.MagicB",
					Scale = scale,
					FadeIn = 0,
					PlayToEndOnExpire = true,
					Hue = hueShiftMagicB
				}.MoveUpDown(deltaY));

			actionShortcuts.Add(battleAxe);
		}

		void AddPlayerActionShortcutsForLady()
		{
			Character lady = GetPlayer(PlayerID.Lady);

			AddLongSword("Longsword", PlayerID.Lady, "1d8+3(slashing)", 5, lady.hueShift);
			AddLongSword("Longsword - 2H", PlayerID.Lady, "1d10+3(slashing)", 5, lady.hueShift, lady.hueShift);

			AddWarhammer("Warhammer", PlayerID.Lady, "1d8+3(bludgeoning)", 5, lady.hueShift);
			AddWarhammer("Warhammer - 2H", PlayerID.Lady, "1d10+3(bludgeoning)", 5, 220, int.MinValue, 0, 220);

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerId = PlayerID.Lady, Dice = "4(bludgeoning)", PlusModifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Second Wind", PlayerId = PlayerID.Lady, Dice = "1d10+4", Type = DiceRollType.HealthOnly, Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest, you can use a bonus action to regain 1d10 + {$Level} HP." });


			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Shifting",
				PlayerId = PlayerID.Lady,
				Type = DiceRollType.None,
				Part = TurnPart.BonusAction,
				LimitCount = 1,
				LimitSpan = DndTimeSpan.ShortRest,
				Description = "Once per short rest as a bonus action, you can assume a more bestial appearance. This transformation lasts for 1 minute, until you die, or until you revert to your normal appearance as a bonus action. When you shift, you gain +5 temp HP and additional benefits that depend on your shifter subrace."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Action Surge", PlayerId = PlayerID.Lady, Part = TurnPart.Special, Description = "You can take one additional action on your turn. This can be used 1 times per short rest." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Longtooth Shifting Strike",
				PlayerId = PlayerID.Lady,
				Part = TurnPart.BonusAction,
				Dice = "1d6+3(piercing)",
				PlusModifier = 5,
				Description = "While shifted, you can use your fangs to make an unarmed strike as a bonus action. If you hit, you can deal 1d6 + 3 piercing damage, instead of the bludgeoning damage normal for an unarmed strike."
			});
		}
		void AddLongSword(string name, int playerId, string damage, int modifier, int hueLongsword, int hueMagic = int.MinValue)
		{
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = name, PlayerId = playerId, Dice = damage, PlusModifier = modifier };

			actionShortcuts.Add(shortcut);

			const int yAdjust = 150;

			WindupDto longsword = CreateWeapon("LongSword.Weapon", hueLongsword);
			longsword.MoveUpDown(yAdjust);
			shortcut.Windups.Add(longsword);

			if (hueMagic != int.MinValue)
			{
				WindupDto magic = CreateWeapon("LongSword.Magic", hueMagic);
				magic.MoveUpDown(yAdjust);
				shortcut.Windups.Add(magic);
			}
		}
		void AddWarhammer(string name, int playerId, string damage, int modifier,
			int hueHammer = int.MinValue, int hueMagicHandle = int.MinValue,
			int hueMagicHeadFront = int.MinValue, int hueMagicHeadBack = int.MinValue)
		{
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = name, PlayerId = playerId, Dice = damage, PlusModifier = modifier };
			WindupDto hammer = CreateWeapon("WarHammer.Weapon", hueHammer);

			const int yAdjust = 150;
			hammer.MoveUpDown(yAdjust);
			shortcut.Windups.Add(hammer);

			if (hueMagicHandle != int.MinValue)
			{
				WindupDto handle = CreateWeapon("WarHammer.MagicHandle", hueMagicHandle);
				handle.MoveUpDown(yAdjust);
				shortcut.Windups.Add(handle);
			}

			if (hueMagicHeadFront != int.MinValue)
			{
				WindupDto headFront = CreateWeapon("WarHammer.MagicFrontHead", hueMagicHeadFront);
				headFront.MoveUpDown(yAdjust);
				shortcut.Windups.Add(headFront);
			}
			if (hueMagicHeadBack != int.MinValue)
			{
				WindupDto headBack = CreateWeapon("WarHammer.MagicBackHead", hueMagicHeadBack);
				headBack.MoveUpDown(yAdjust);
				shortcut.Windups.Add(headBack);
			}

			actionShortcuts.Add(shortcut);
		}

		private void AddPlayerActionShortcutsForShemo()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Scimitar", PlayerId = PlayerID.Shemo, Dice = "1d6+1", PlusModifier = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Scimitar (Shillelagh)", PlayerId = PlayerID.Shemo, Dice = "1d8+1", PlusModifier = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Poison Spray", PlayerId = PlayerID.Shemo, Dice = "1d12", UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shillelagh", PlayerId = PlayerID.Shemo, Dice = "1d8+3", PlusModifier = 5, UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Thunderwave", PlayerId = PlayerID.Shemo, Dice = "2d8", UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Healing Word", PlayerId = PlayerID.Shemo, Dice = "1d4+3(healing)" });
			AddCureWounds(PlayerID.Shemo, "1d8+3");
		}

		void AddCureWounds(int playerID, string diceStr)
		{
			Character player = GetPlayer(playerID);
			if (player == null)
				return;
			PlayerActionShortcut cureWounds = new PlayerActionShortcut()
			{
				Name = "Cure Wounds",
				PlayerId = playerID,
				Dice = diceStr,
				Description = "A creature you touch regains a number of hit points equal to 1d8 + your spellcasting ability modifier. This spell has no effect on undead or constructs.\n\nAt Higher Levels: When you cast this spell using a spell slot of 2nd level or higher, the healing increases by 1d8 for each slot level above 1st."
			};
			cureWounds.Spell = AllSpells.Get("Cure Wounds", player, 1);

			cureWounds.Windups.Add(new WindupDto()
			{
				Effect = "Fairy",
				Hue = player.hueShift,
				Scale = 0.8,
				StartSound = "CureWounds"
			}.Float());
			actionShortcuts.Add(cureWounds);
		}

		void AddDivineSense(int playerId)
		{
			PlayerActionShortcut divineSense = new PlayerActionShortcut()
			{
				Name = "Divine Sense",
				PlayerId = playerId,
				Part = TurnPart.Action,
				Description = "As an action, you can detect good and evil. Until the end of your next turn, you can sense anything affected by the hallow spell or know the location of any celestial, fiend, undead within 60 ft. that is not behind total cover. You can use this feature 5 times per long rest."
			};
			const string effectName = "Orb";
			divineSense.Windups.Add(new WindupDto()
			{
				Effect = effectName,
				Hue = 220,
				StartSound = "DetectGoodEvil"
			}.Float());
			divineSense.Windups.Add(new WindupDto()
			{
				Effect = effectName,
				DegreesOffset = 180,
				Hue = 0
			}.Float());
			actionShortcuts.Add(divineSense);
		}

		void AddChillTouch(int playerId, string damageDiceStr, int spellSlotLevel)
		{
			PlayerActionShortcut chillTouch = new PlayerActionShortcut()
			{ Name = SpellNames.ChillTouch, PlayerId = playerId, Dice = damageDiceStr, PlusModifier = 5, UsesMagic = true, Type = DiceRollType.Attack };

			chillTouch.Spell = AllSpells.Get(SpellNames.ChillTouch, GetPlayer(playerId), spellSlotLevel);

			const double shoulderDistance = 140;
			double yPos = 140;
			DndCore.Vector leftArm = new DndCore.Vector(-shoulderDistance, yPos);
			DndCore.Vector rightArm = new DndCore.Vector(shoulderDistance, yPos);

			chillTouch.Windups.Add(new WindupDto()
			{
				Effect = "Ghost",
				Rotation = 110,
				Scale = 0.8,
				Offset = leftArm,
				FadeIn = 500
			}.Necrotic());

			chillTouch.Windups.Add(new WindupDto()
			{
				Effect = "Smoke",
				Rotation = 110,
				Scale = 0.8,
				Offset = leftArm,
				FadeIn = 500,
				DegreesOffset = -40
			}.Necrotic().SetBright(30));

			chillTouch.Windups.Add(new WindupDto()
			{
				Effect = "Ghost",
				Rotation = 80,
				Scale = 0.8,
				Offset = rightArm,
				FlipHorizontal = true,
				FadeIn = 500
			}.Necrotic());

			chillTouch.Windups.Add(new WindupDto()
			{
				Effect = "Smoke",
				Rotation = 80,
				Scale = 0.8,
				Offset = rightArm,
				FlipHorizontal = true,
				FadeIn = 500,
				DegreesOffset = -40
			}.Necrotic().SetBright(20));
			WindupDto staffWeapon = CreateWeapon("Staff.Weapon", 30);
			staffWeapon.MoveUpDown(150);
			chillTouch.Windups.Add(staffWeapon);
			WindupDto staffMagic = CreateWeapon("Staff.Magic", 30);
			staffMagic.Brightness = 0;
			staffMagic.MoveUpDown(150);
			chillTouch.Windups.Add(staffMagic);

			actionShortcuts.Add(chillTouch);
		}

		private static WindupDto CreateWeapon(string effectName, int hueShift = int.MinValue)
		{
			WindupDto weapon = new WindupDto();
			weapon.Effect = effectName;
			weapon.FadeIn = 0;
			if (hueShift != int.MinValue)
				weapon.Hue = hueShift;
			weapon.PlayToEndOnExpire = true;
			return weapon;
		}

		void AddShieldOfFaith(int playerId, int spellSlotLevel)
		{
			PlayerActionShortcut shieldOfFaith = new PlayerActionShortcut()
			{
				Name = SpellNames.ShieldOfFaith,
				PlayerId = playerId,
				Part = TurnPart.BonusAction,
				Description = "A shimmering field appears and surrounds a creature of your choice within range, granting it a +2 bonus to AC for the duration."
			};

			shieldOfFaith.Spell = AllSpells.Get(SpellNames.ShieldOfFaith, GetPlayer(playerId), spellSlotLevel);
			//shieldOfFaith.Spell = new Spell()
			//{
			//	Name = "Shield of Faith", OwnerId = playerId,
			//	CastingTime = DndTimeSpan.OneBonusAction,
			//	Range = 60,
			//	Components = SpellComponents.All,
			//	Material = "A small parchment with a bit of holy text written on it.",
			//	Duration = DndTimeSpan.FromMinutes(10),
			//	RequiresConcentration = true
			//};

			const string effectName = "Plasma";
			shieldOfFaith.Windups.Add(new WindupDto() { Effect = effectName, Hue = -30 }.MoveUpDown(160).Fade());
			actionShortcuts.Add(shieldOfFaith);
		}

		void AddSleep(int playerId, int spellSlotLevel)
		{
			PlayerActionShortcut sleep = new PlayerActionShortcut()
			{
				Name = "Sleep",
				PlayerId = playerId,
				Part = TurnPart.Action,
			};

			sleep.Spell = AllSpells.Get(SpellNames.Sleep, GetPlayer(playerId), spellSlotLevel);

			//sleep.Spell = new Spell()
			//{
			//	Name = "Sleep",
			//	OwnerId = playerId,
			//	Duration = DndTimeSpan.OneMinute,
			//	CastingTime = DndTimeSpan.OneBonusAction,
			//	Material = "a pinch of fine sand, rose petals, or a cricket",
			//	Range = 90
			//	//,
			//	//Subrange = 20,
			//	//SubrangeType = AreaOfEffect.Sphere
			//};
		}

		void AddSanctuary(int playerId, int spellSlotLevel)
		{
			PlayerActionShortcut sanctuary = new PlayerActionShortcut()
			{
				Name = SpellNames.Sanctuary,
				PlayerId = playerId,
				Part = TurnPart.BonusAction,
				Description = "You ward a creature within range against attack. Until the spell ends, any creature who targets the warded creature with an attack or a harmful spell must first make a Wisdom saving throw. On a failed save, the creature must choose a new target or lose the attack or spell. This spell doesn't protect the warded creature from area effects, such as the explosion of a fireball.\n\nIf the warded creature makes an attack, casts a spell that affects an enemy, or deals damage to another creature, this spell ends."
			};

			sanctuary.Spell = AllSpells.Get(SpellNames.Sanctuary, GetPlayer(playerId), spellSlotLevel);
			//sanctuary.Spell = new Spell()
			//{
			//	Name = "Sanctuary", OwnerId = playerId,
			//	Duration = DndTimeSpan.OneMinute,
			//	CastingTime = DndTimeSpan.OneBonusAction,
			//	Material = "a small silver mirror",
			//	Range = 30
			//};

			sanctuary.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 170 }.Fade());
			sanctuary.Windups.Add(new WindupDto() { Effect = "Trails", DegreesOffset = -60, Hue = 200, Rotation = 45 }.Fade());
			sanctuary.Windups.Add(new WindupDto() { Effect = "Trails", DegreesOffset = 60, Hue = 230, Rotation = -45 }.Fade());
			actionShortcuts.Add(sanctuary);
		}
		void AddThunderousSmite(int playerId, string additionalDice)
		{
			PlayerActionShortcut thunderousSmite = new PlayerActionShortcut()
			{
				Name = "Thunderous Smite",
				PlayerId = playerId,
				Part = TurnPart.BonusAction,
				AddDiceOnHit = additionalDice,
				AddDiceOnHitMessage = Name,
				MinDamage = keepExistingModifier,
				PlusModifier = keepExistingModifier,
				Description = "The first time you hit with a melee weapon attack during this spell’s duration, your weapon rings with thunder that is audible within 300 feet of you, and the attack deals an extra 2d6 thunder damage to the target. Additionally, if the target is a creature, it must succeed on a Strength saving throw or be pushed 10 feet away from you and knocked prone."
			};
			thunderousSmite.Spell = new Spell()
			{
				Name = thunderousSmite.Name,
				OwnerId = playerId,
				Duration = DndTimeSpan.OneMinute,
				RequiresConcentration = true,
				CastingTime = DndTimeSpan.OneBonusAction,
				Components = SpellComponents.Verbal,
				Range = 30
				// TODO: Clear Thunderous Smite visual effects on a hit.
			};
			AddLightning(thunderousSmite);
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = 45 }.Fade());
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Wide", FlipHorizontal = true, Hue = 45 }.Fade().MoveUpDown(40));
			actionShortcuts.Add(thunderousSmite);
		}

		private static void AddLightning(PlayerActionShortcut thunderousSmite, int hueAdjust = 0)
		{
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Lightning", Hue = hueAdjust, Scale = 2, StartSound = "Lightning" }.MoveUpDown(146));
		}

		void AddWrathfulSmite(int playerId, string additionalDice)
		{
			PlayerActionShortcut wrathfulSmite = new PlayerActionShortcut()
			{
				Name = "Wrathful Smite",
				PlayerId = playerId,
				Part = TurnPart.BonusAction,
				AddDiceOnHit = additionalDice,
				AddDiceOnHitMessage = Name,
				MinDamage = keepExistingModifier,
				PlusModifier = keepExistingModifier,
				Description = "The next time you hit with a melee weapon attack during this spell’s duration, your attack deals an extra 1d6 psychic damage. Additionally, if the target is a creature, it must make a Wisdom saving throw or be frightened of you until the spell ends. As an action, the creature can make a Wisdom check against your spell save DC to steel its resolve and end this spell."
			};

			wrathfulSmite.Spell = new Spell()
			{
				Name = wrathfulSmite.Name,
				OwnerId = playerId,
				Duration = DndTimeSpan.OneMinute,
				RequiresConcentration = true,
				CastingTime = DndTimeSpan.OneBonusAction,
				Components = SpellComponents.Verbal,
				Range = 30
			};

			AddLightning(wrathfulSmite, -1);
			wrathfulSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = -1, StartSound = "Psychic1" }.Fade());
			wrathfulSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = -1, FlipHorizontal = true, StartSound = "Psychic2" }.Fade().MoveUpDown(40));

			actionShortcuts.Add(wrathfulSmite);

		}
		private void AddPlayerActionShortcutsForAva()
		{
			AddBattleAxe("Battleaxe (1H)", PlayerID.Ava, "1d8+3(slashing)", 5, 3, int.MinValue, int.MinValue, 1.3);
			AddBattleAxe("Battleaxe (2H)", PlayerID.Ava, "1d10+3(slashing)", 5, 3, 220, 280, 1.3);

			AddGreatSword(PlayerID.Ava, "2d6+4(slashing)");
			AddJavelin("Javelin", PlayerID.Ava, "1d6+3(piercing)", 5, 220);

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Net", PlayerId = PlayerID.Ava, Dice = "", PlusModifier = 2 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerId = PlayerID.Ava, Dice = "+4(bludgeoning)", PlusModifier = 5 });

			AddCureWounds(PlayerID.Ava, "1d8+3");

			AddThunderousSmite(PlayerID.Ava, "2d6(thunder)");

			AddWrathfulSmite(PlayerID.Ava, "1d6(psychic)");

			AddShieldOfFaith(PlayerID.Ava, 1);
			AddSanctuary(PlayerID.Ava, 1);
			AddSleep(PlayerID.Ava, 1);

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Divine Smite",
				PlayerId = PlayerID.Ava,
				Type = DiceRollType.AddOnDice,
				Part = TurnPart.Special,
				AdditionalRollTitle = "Divine Smite",
				InstantDice = "2d8(radiant:damage)",
				Description = "When you hit with a melee weapon attack, you can expend one spell slot to deal 2d8 " +
											"extra radiant damage to the target plus 1d8 for each spell level higher than 1st " +
											"(max 5d8) and plus 1d8 against undead or fiends."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Channel Divinity: Rebuke the Violent",
				PlayerId = PlayerID.Ava,
				Type = DiceRollType.None,
				Part = TurnPart.Reaction,
				Description = "Immediately after an attacker within 30 ft. deals damage with an attack against a creature other than you, you can use your reaction to force the attacker to make a WIS saving throw (DC 14). On failure, the attacker takes radiant damage equal to the damage it just dealt, or half damage on success."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Channel Divinity: Emissary of Peace",
				PlayerId = PlayerID.Ava,
				Part = TurnPart.BonusAction,
				Description = "As a bonus action, you grant yourself a +5 bonus to Charisma (Persuasion) checks for the next 10 minutes."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Great Weapon Master Attack",
				PlayerId = PlayerID.Ava,
				Part = TurnPart.BonusAction,
				Description = "On your turn, when you score a critical hit with a melee weapon or reduce a creature to 0 HP with one, you can make one melee weapon attack as a bonus action."
			});

			AddDivineSense(PlayerID.Ava);

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Lay on Hands Pool",
				PlayerId = PlayerID.Ava,
				Part = TurnPart.Action,
				Description = "You have a pool of healing power that can restore 20 HP per long rest. As an action, you can touch a creature to restore any number of HP remaining in the pool, or 5 HP to either cure a disease or neutralize a poison affecting the creature."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Divine Smite (undead)",
				PlayerId = PlayerID.Ava,
				Type = DiceRollType.AddOnDice,
				Part = TurnPart.Special,
				AdditionalRollTitle = "Divine Smite",
				InstantDice = "3d8(radiant:damage)",
				Description = "When you hit with a melee weapon attack, you can expend one spell slot to deal 2d8 extra radiant damage to the target plus 1d8 for each spell level higher than 1st (max 5d8) and plus 1d8 against undead or fiends."
			});
		}
		void AddJavelin(string name, int playerId, string damage, int modifier, int magicHue = int.MinValue)
		{
			PlayerActionShortcut javelin = new PlayerActionShortcut()
			{ Name = name, PlayerId = playerId, Dice = damage, PlusModifier = modifier };

			WindupDto javelin3D = new WindupDto();
			javelin3D.Effect = "Javelin.Weapon";
			javelin3D.FadeIn = 0;
			javelin3D.PlayToEndOnExpire = true;
			javelin3D.MoveUpDown(150);
			javelin.Windups.Add(javelin3D);

			if (magicHue != int.MinValue)
			{
				WindupDto javelinMagic = new WindupDto();
				javelinMagic.Effect = "Javelin.Magic";
				javelinMagic.FadeIn = 0;
				javelinMagic.PlayToEndOnExpire = true;
				javelinMagic.MoveUpDown(150);
				javelinMagic.Hue = magicHue;
				javelin.Windups.Add(javelinMagic);
			}

			actionShortcuts.Add(javelin);
		}

		private void AddGreatSword(int playerId, string damage)
		{
			PlayerActionShortcut greatSword = new PlayerActionShortcut()
			{ Name = "Greatsword, +1", PlayerId = playerId, Dice = damage, PlusModifier = +6, MinDamage = 3 };
			WindupDto greatSword3d = new WindupDto();
			greatSword3d.Effect = "GreatSword.Weapon";
			greatSword3d.FadeIn = 0;
			greatSword3d.PlayToEndOnExpire = true;
			greatSword3d.MoveUpDown(150);
			greatSword.Windups.Add(greatSword3d);
			WindupDto greatSwordMagic = new WindupDto();
			greatSwordMagic.Effect = "GreatSword.Magic";
			greatSwordMagic.FadeIn = 0;
			greatSwordMagic.Hue = 270;
			greatSwordMagic.PlayToEndOnExpire = true;
			greatSwordMagic.MoveUpDown(150);
			greatSword.Windups.Add(greatSwordMagic);
			actionShortcuts.Add(greatSword);
		}

		private void AddPlayerActionShortcutsForMerkin()
		{
			// TODO: Remove WindupName
			AddChaosBolt(PlayerID.Merkin, 1);
			AddChaosBolt(PlayerID.Merkin, 2);
			AddChaosBolt(PlayerID.Merkin, 3);
			AddFireBolt(PlayerID.Merkin, 0);
			AddLightningLure(PlayerID.Merkin, 0);
			AddMelfsMinuteMeteors();
			AddRayOfFrost();


			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Crossbow, Light", PlayerId = PlayerID.Merkin, Dice = "1d8+2(piercing)", PlusModifier = 4 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger", PlayerId = PlayerID.Merkin, Dice = "1d4+2(piercing)", PlusModifier = 4 });

			AddChillTouch(PlayerID.Merkin, "1d8(necrotic)", 0);
		}
		void AddRayOfFrost()
		{
			PlayerActionShortcut rayOfFrost = new PlayerActionShortcut()
			{ Name = "Ray of Frost", PlayerId = PlayerID.Merkin, Dice = "1d8(cold)", PlusModifier = 5, UsesMagic = true };

			rayOfFrost.Windups.Add(new WindupDto()
			{
				Effect = "Ghost",
				Hue = 230,
				Scale = 1.2,
				Brightness = 120,
				FlipHorizontal = true
			}.MoveUpDown(60));
			rayOfFrost.Windups.Add(new WindupDto()
			{
				Effect = "Smoke",
				Saturation = 0,
				Scale = 1.1,
				Brightness = 170,
				FlipHorizontal = true,
				DegreesOffset = 0
			}.MoveUpDown(60));

			AddStaff(rayOfFrost, 230, 210);
			actionShortcuts.Add(rayOfFrost);
		}

		void AddMelfsMinuteMeteors()
		{
			double scale = 1;
			DndCore.Vector offset = new DndCore.Vector(0, 30);
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = "Melf's Minute Meteors", PlayerId = PlayerID.Merkin, Dice = "1d6(fire)", UsesMagic = true };
			const int numMeteors = 3;
			int degreeSpan = 360 / numMeteors;
			int degreeOffset = 6;
			const int hueSpan = 30;
			int hueOffset = -hueSpan;
			for (int i = 0; i < numMeteors; i++)
			{
				shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Scale = scale, Hue = hueOffset, DegreesOffset = degreeOffset, Offset = offset });
				shortcut.Windups.Add(new WindupDto() { Effect = "Smoke", Scale = scale, Hue = hueOffset, DegreesOffset = degreeOffset - 20, Offset = offset, Brightness = 20 });
				degreeOffset += degreeSpan;
				hueOffset += hueSpan;
			}
			AddStaff(shortcut, 30, 350);
			actionShortcuts.Add(shortcut);
		}

		private void AddLightningLure(int playerId, int spellSlotLevel)
		{
			DndCore.Vector offset = new DndCore.Vector(0, 70);
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = "Lightning Lure", PlayerId = PlayerID.Merkin, Dice = "1d8(lightning)", UsesMagic = true };

			shortcut.Windups.Add(new WindupDto() { Hue = 45, Scale = 1.1, Effect = "LiquidSparks", Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Hue = 220, Scale = 1.1, DegreesOffset = 180, Effect = "LiquidSparks", Offset = offset });
			shortcut.Spell = AllSpells.Get(SpellNames.LightningLure, GetPlayer(playerId), spellSlotLevel);
			AddStaff(shortcut, 220, 35);
			actionShortcuts.Add(shortcut);
		}

		private void AddChaosBolt(int playerId, int spellSlotLevel)
		{
			DndCore.Vector offset = new DndCore.Vector(0, 140);

			string effectName;
			if (spellSlotLevel <= 1)
				effectName = "Narrow";
			else if (spellSlotLevel == 2)
				effectName = "Wide";
			else
				effectName = "Trails";
			PlayerActionShortcut shortcut = new PlayerActionShortcut()

			{ Name = $"{SpellNames.ChaosBolt} ({spellSlotLevel})", PlayerId = playerId, Dice = "2d8", PlusModifier = 5, UsesMagic = true, Type = DiceRollType.ChaosBolt };
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 300, Rotation = 45, FlipHorizontal = true, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 220, Rotation = -45, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 300 + 30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 220 + 30, Rotation = -45, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 300 - 30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 240 });
			shortcut.Windups.Add(new WindupDto() { Effect = effectName, Hue = 220 - 30, Rotation = -45, Offset = offset, DegreesOffset = 240 });
			AddStaff(shortcut, 220, 270);
			shortcut.Spell = AllSpells.Get(SpellNames.ChaosBolt, GetPlayer(playerId), spellSlotLevel);
			actionShortcuts.Add(shortcut);
		}

		private void AddFireBolt(int playerId, int spellSlotLevel)
		{
			DndCore.Vector offset = new DndCore.Vector(0, 140);

			PlayerActionShortcut shortcut = new PlayerActionShortcut()

			{ Name = $"{SpellNames.FireBolt} ({spellSlotLevel})", PlayerId = playerId, Dice = "2d8", PlusModifier = 5, UsesMagic = true, Type = DiceRollType.Attack };
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = 0, Rotation = 45, FlipHorizontal = true, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = 330, Rotation = -45, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = 30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = 330 + 30, Rotation = -45, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = -30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 240 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Fire", Hue = 330 - 30, Rotation = -45, Offset = offset, DegreesOffset = 240 });
			AddStaff(shortcut, 0, 20);
			shortcut.Spell = AllSpells.Get(SpellNames.FireBolt, GetPlayer(playerId), spellSlotLevel);
			actionShortcuts.Add(shortcut);
		}

		private static void AddStaff(PlayerActionShortcut shortcut, int staffHue, int headHue)
		{
			WindupDto staffWeapon = new WindupDto();
			staffWeapon.Effect = "Staff.Weapon";
			staffWeapon.FadeIn = 0;
			staffWeapon.Hue = staffHue;
			staffWeapon.PlayToEndOnExpire = true;
			staffWeapon.MoveUpDown(150);
			shortcut.Windups.Add(staffWeapon);
			WindupDto staffMagic = new WindupDto();
			staffMagic.Effect = "Staff.Magic";
			staffMagic.FadeIn = 0;
			staffMagic.Hue = headHue;
			staffMagic.PlayToEndOnExpire = true;
			staffMagic.MoveUpDown(150);
			shortcut.Windups.Add(staffMagic);
		}

		private void AddPlayerActionShortcutsForWilly()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger of Warning", PlayerId = PlayerID.Willy, Dice = "1d4+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortbow", PlayerId = PlayerID.Willy, Dice = "1d6+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortsword", PlayerId = PlayerID.Willy, Dice = "1d6+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Fire Bolt", PlayerId = PlayerID.Willy, Dice = "1d10", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bonus Dagger", PlayerId = PlayerID.Willy, Dice = "1d4", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger of Warning (Sneak)", PlayerId = PlayerID.Willy, Dice = "2d6,1d4+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortbow (Sneak)", PlayerId = PlayerID.Willy, Dice = "3d6+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortsword (Sneak)", PlayerId = PlayerID.Willy, Dice = "3d6+3", PlusModifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bonus Dagger (Sneak)", PlayerId = PlayerID.Willy, Dice = "2d6,1d4", PlusModifier = 5 });
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
			foreach (Character player in players)
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

				foreach (Character player in players)
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
				if (dndTimeClock.InCombat)
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
				if (!dndTimeClock.InCombat)
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
			//PlayerFactory.BuildPlayers(players);
			AllPlayers.LoadData();
			AllDieRollEffects.LoadData();
			AllTrailingEffects.LoadData();

			players = AllPlayers.GetActive();
			string playerData = JsonConvert.SerializeObject(players);
			HubtasticBaseStation.SetPlayerData(playerData);

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

			foreach (Character player in players)
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

		List<Character> players = new List<Character>();
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
			return AllPlayers.GetPlayerIdFromNameStart(players, characterName);
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
			int rand = new Random((int)dndTimeClock.Time.Ticks).Next(10);
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

		string Plural(int count, string suffix)
		{
			if (count == 1)
				return $"{count} {suffix}";
			return $"{count} {suffix}s";
		}

		string GetTimeLeft(int playerId, Spell spell)
		{
			DndAlarm alarm = dndTimeClock.GetAlarm(GetAlarmName(spell, playerId));
			if (alarm == null)
				return "0 seconds";
			TimeSpan time = alarm.TriggerTime - dndTimeClock.Time;
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

		void ReportOnConcentration()
		{
			string concentrationReport = string.Empty;
			foreach (int key in concentratedSpells.Keys)
			{
				concentrationReport += $"{GetPlayerName(key)} is casting {concentratedSpells[key].Name} with {GetTimeLeft(key, concentratedSpells[key])} remaining; ";
			}

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
			HubtasticBaseStation.ClearWindup("*");
			TellDungeonMaster("Dropping windups...");
		}

		public void MoveFred(string movement)
		{
			HubtasticBaseStation.MoveFred(movement);
		}

		public void PlayScene(string sceneName)
		{
			string dmMessage = $"Playing scene: {sceneName}";

			try
			{
				obsWebsocket.SetCurrentScene(sceneName);
			}
			catch (Exception ex)
			{
				dmMessage = $"kill -name chrome -Force" +
					$"Unable to play {sceneName}: {ex.Message}";
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
			if (!JoinedChannel(DungeonMasterChannel))
			{
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
	}
}
