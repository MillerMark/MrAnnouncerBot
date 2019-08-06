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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDungeonMasterApp
	{
		DungeonMasterChatBot dmChatBot = new DungeonMasterChatBot();
		TwitchClient humperBotClient;
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

			InitializeAttackShortcuts();
			humperBotClient = Twitch.CreateNewClient("HumperBot", "HumperBot", "HumperBotOAuthToken");
			if (humperBotClient != null)
				humperBotClient.OnMessageReceived += HumperBotClient_OnMessageReceived;

			dmChatBot.Initialize(this);
			
			dmChatBot.DungeonMasterApp = this;
			commandParsers.Add(dmChatBot);
		}

		List<BaseChatBot> commandParsers = new List<BaseChatBot>();
		BaseChatBot GetCommandParser(string userId)
		{
			return commandParsers.Find(x => x.ListensTo(userId));
		}

		public string SetHiddenThreshold(int hiddenThreshold)
		{
			Dispatcher.Invoke(() =>
			{
				tbxHiddenThreshold.Text = hiddenThreshold.ToString();
			});

			return $"Hidden threshold successfully changed to {hiddenThreshold}.";
		}

		private void HumperBotClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			BaseChatBot commandParser = GetCommandParser(e.ChatMessage.UserId);
			if (commandParser == null)
				return;
			commandParser.HandleMessage(e.ChatMessage, humperBotClient);
		}

		private void History_LogUpdated(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				History.UpdateQueuedEntries();
			});
		}

		public int PlayerID
		{
			get
			{
				if (tabPlayers.SelectedItem is PlayerTabItem playerTabItem)
					return playerTabItem.PlayerID;
				return tabPlayers.SelectedIndex;
			}
		}

		private void FocusHelper_FocusedControlsChanged(object sender, FocusedControlsChangedEventArgs e)
		{
			foreach (StatBox statBox in e.Active)
			{
				HubtasticBaseStation.FocusItem(PlayerID, activePage, statBox.FocusItem);
			}

			foreach (StatBox statBox in e.Deactivated)
			{
				HubtasticBaseStation.UnfocusItem(PlayerID, activePage, statBox.FocusItem);
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
				if (uIElement is PlayerRollCheckBox checkbox && checkbox.PlayerId == PlayerID)
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

		private void PlayerShortcutButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				PlayerActionShortcut actionShortcut = GetActionShortcut(button.Tag);
				if (actionShortcut == null)
					return;

				if (actionShortcut.Windups.Count > 0)
				{
					List<WindupDto> windups = actionShortcut.CloneWindups();
					HubtasticBaseStation.ClearWindup("");
					string serializedObject = JsonConvert.SerializeObject(windups);
					HubtasticBaseStation.AddWindup(serializedObject);
				}

				settingInternally = true;
				try
				{
					HighlightPlayerShortcut((int)button.Tag);
					if (!string.IsNullOrWhiteSpace(actionShortcut.AddDice))
						tbxDamageDice.Text += "," + actionShortcut.AddDice;
					else
						tbxDamageDice.Text = actionShortcut.Dice;

					if (actionShortcut.MinDamage != keepExistingModifier)
						tbxMinDamage.Text = actionShortcut.MinDamage.ToString();

					if (actionShortcut.Modifier != keepExistingModifier)
						if (actionShortcut.Modifier > 0)
							tbxModifier.Text = "+" + actionShortcut.Modifier.ToString();
						else
							tbxModifier.Text = actionShortcut.Modifier.ToString();

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
		}

		void SetActionShortcuts(int playerID)
		{
			AddShortcutButtons(spActionsActivePlayer, playerID, TurnPart.Action);
			AddShortcutButtons(spBonusActionsActivePlayer, playerID, TurnPart.BonusAction);
			AddShortcutButtons(spReactionsActivePlayer, playerID, TurnPart.Reaction);
			AddShortcutButtons(spSpecialActivePlayer, playerID, TurnPart.Special);
		}

		private void AddShortcutButtons(StackPanel stackPanel, int playerID, TurnPart part)
		{
			ClearExistingButtons(stackPanel);

			List<PlayerActionShortcut> playerActions = actionShortcuts.Where(x => x.PlayerID == playerID).Where(x => x.Part == part).ToList();
			if (playerActions.Count == 0)
				stackPanel.Visibility = Visibility.Collapsed;
			else
				stackPanel.Visibility = Visibility.Visible;

			foreach (PlayerActionShortcut playerActionShortcut in playerActions)
			{
				stackPanel.Children.Add(BuildShortcutButton(playerActionShortcut));
			}
		}

		private void ClearExistingButtons(StackPanel stackPanel)
		{
			for (int i = stackPanel.Children.Count - 1; i >= 0; i--)
			{
				UIElement uIElement = stackPanel.Children[i];
				if (uIElement is StackPanel)
					stackPanel.Children.RemoveAt(i);
			}
		}

		private void TabControl_PlayerChanged(object sender, SelectionChangedEventArgs e)
		{
			if (buildingTabs)
				return;
			if (rbActivePlayer.IsChecked == true)
			{
				CheckOnlyOnePlayer(PlayerID);
			}
			highlightRectangles = null;
			NextDieRollType = DiceRollType.None;
			activePage = ScrollPage.main;
			FocusHelper.ClearActiveStatBoxes();
			HubtasticBaseStation.PlayerDataChanged(PlayerID, activePage, string.Empty);
			HubtasticBaseStation.ClearWindup("*");
			SetActionShortcuts(PlayerID);
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
				HubtasticBaseStation.PlayerDataChanged(PlayerID, activePage, string.Empty);
			}
		}

		private void HandleCharacterChanged(object sender, RoutedEventArgs e)
		{
			if (sender is CharacterSheets characterSheets)
			{
				string character = characterSheets.GetCharacter();
				HubtasticBaseStation.PlayerDataChanged(PlayerID, activePage, character);
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
			showClearButtonTimer.Start();
			rbTestNormalDieRoll.IsChecked = true;
			updateClearButtonTimer.Stop();
			EnableDiceRollButtons(false);
			btnClearDice.Visibility = Visibility.Hidden;
			PrepareForClear();
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
			diceRoll.CritFailMessage = "";
			diceRoll.CritSuccessMessage = "";
			diceRoll.SuccessMessage = "";
			diceRoll.FailMessage = "";
			diceRoll.SkillCheck = Skills.none;
			diceRoll.SavingThrow = Ability.None;
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
			else if (type == DiceRollType.Initiative)
				diceRoll.HiddenThreshold = -100;
			else if (double.TryParse(tbxHiddenThreshold.Text, out double thresholdResult))
				diceRoll.HiddenThreshold = thresholdResult;

			diceRoll.IsMagic = (ckbUseMagic.IsChecked == true && IsAttack(type)) || type == DiceRollType.WildMagicD20Check;
			diceRoll.Type = type;
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
			string timeStr = dndTimeClock.Time.ToString("H:mm:ss") + ", " + dndTimeClock.AsDndDateString();

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
			}
			return "None";
		}

		string GetAbilityStr(Ability savingThrow)
		{
			switch (savingThrow)
			{
				case Ability.Charisma: return "Charisma";
				case Ability.Constitution: return "Constitution";
				case Ability.Dexterity: return "Dexterity";
				case Ability.Intelligence: return "Intelligence";
				case Ability.Strength: return "Strength";
				case Ability.Wisdom: return "Wisdom";
			}
			return "None";
		}
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.DiceRollData != null)
			{
				int rollValue = ea.DiceRollData.roll;
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
				}
				if (rollTitle == "")
					rollTitle = "Dice roll: ";
				if (ea.DiceRollData.multiplayerSummary != null && ea.DiceRollData.multiplayerSummary.Count > 0)
				{
					foreach (PlayerRoll playerRoll in ea.DiceRollData.multiplayerSummary)
					{
						string playerName = playerRoll.name;
						if (playerName != "")
							playerName = playerName + " ";

						rollValue = playerRoll.modifier + playerRoll.roll;
						bool success = rollValue >= ea.DiceRollData.hiddenThreshold;
						successStr = GetSuccessStr(success, ea.DiceRollData.type);
						string localDamageStr;
						if (success)
							localDamageStr = damageStr;
						else
							localDamageStr = "";
						History.Log(playerName + rollTitle + rollValue.ToString() + successStr + localDamageStr + bonusStr);
					}
				}
				else
				{
					string playerName = GetPlayerName(ea.DiceRollData.playerID);
					if (playerName != "")
						playerName = playerName + " ";
					if (!ea.DiceRollData.success)
						damageStr = "";
					History.Log(playerName + rollTitle + rollValue.ToString() + successStr + damageStr + bonusStr);
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

		// TODO: Remove this:
		string GetPlayerName(int playerID)
		{
			switch (playerID)
			{
				case 0:
					return "Willy";
				case 1:
					return "Shemo";
				case 2:
					return "Merkin";
				case 3:
					return "Ava";
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
			diceRoll.IsPaladinSmiteAttack = true;
			diceRoll.NumHalos = 3;
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Raven,
				LeftRightDistanceBetweenPrints = 15,
				MinForwardDistanceBetweenPrints = 5,
				OnPrintPlaySound = "Flap",
				MinSoundInterval = 600,
				PlusMinusSoundInterval = 300,
				NumRandomSounds = 6
			});
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Spiral,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 150,
				OnPrintPlaySound = "Crow",
				MinSoundInterval = 500,
				PlusMinusSoundInterval = 300,
				NumRandomSounds = 3
			});
			RollTheDice(diceRoll);
		}

		private void BtnSneakAttack_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.Attack);
			diceRoll.IsSneakAttack = true;
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.Smoke,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 120,  // 120 + Random.plusMinus(30)
			});
			diceRoll.OnFirstContactSound = "SneakAttack";
			diceRoll.OnFirstContactEffect = TrailingSpriteType.SmokeExplosion;
			RollTheDice(diceRoll);
		}

		private void BtnWildMagic_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = new DiceRoll();
			diceRoll.Modifier = 0;
			diceRoll.HiddenThreshold = 0;
			diceRoll.IsMagic = true;
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
			diceRoll.IsWildAnimalAttack = true;
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

		private const int Player_Lady = 0;
		private const int Player_Shemo = 1;
		private const int Player_Merkin = 2;
		private const int Player_Ava = 3;
		private const int Player_Fred = 4;
		private const int Player_Willy = 5;

		DiceRollType nextDieRollType;
		void InitializeAttackShortcuts()
		{
			highlightRectangles = null;
			actionShortcuts.Clear();
			PlayerActionShortcut.PrepareForCreation();
			AddPlayerActionShortcutsForWilly();
			AddPlayerActionShortcutsForMerkin();
			AddPlayerActionShortcutsForAva();
			AddPlayerActionShortcutsForShemo();
			AddPlayerActionShortcutsForLady();
			AddPlayerActionShortcutsForFred();
		}

		void AddPlayerActionShortcutsForFred()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (1H)", PlayerID = Player_Fred, Dice = "1d8+5(slashing)", Modifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (2H)", PlayerID = Player_Fred, Dice = "1d10+3(slashing)", Modifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Handaxe", PlayerID = Player_Fred, Dice = "1d6+5(slashing)", Modifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longbow", PlayerID = Player_Fred, Dice = "1d8(piercing)", Modifier = +2 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerID = Player_Fred, Dice = "4(bludgeoning)", Modifier = +5, Description = "You can punch, kick, head-butt, or use a similar forceful blow and deal bludgeoning damage equal to 1 + STR modifier." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bite", PlayerID = Player_Fred, Dice = "1d6+3(piercing)", Modifier = +5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Feinting Attack",
				PlayerID = Player_Fred,
				Modifier = keepExistingModifier,
				AddDice = "1d8(superiority)",
				VantageMod = VantageKind.Advantage,
				Part = TurnPart.BonusAction,
				Description = "You can expend one superiority die and use a bonus action on your turn to add the total to the damage roll and to gain advantage on your next attack roll against a chosen creature within 5 ft. this turn."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Goading Attack", PlayerID = Player_Fred, Type = DiceRollType.AddOnDice, InstantDice = "1d8(superiority:damage)", AdditionalRollTitle = "Goading Attack", Part = TurnPart.Special, Description = "When you hit with a weapon attack, you can expend one superiority die to add the total to the damage roll and the target must make a WIS saving throw (DC 13). On failure, the target has disadvantage on all attack rolls against targets other than you until the end of your next turn." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Trip Attack", PlayerID = Player_Fred, Type = DiceRollType.AddOnDice, InstantDice = "1d8(superiority:damage)", AdditionalRollTitle = "Trip Attack", Part = TurnPart.Special, Description = "When you hit with a weapon attack, you can expend one superiority die to add the total to the damage roll, and if the target is Large or smaller, it must make a STR saving throw (DC 13). On failure, you knock the target prone." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Action Surge", PlayerID = Player_Fred, Part = TurnPart.Special, Description = "You can take one additional action on your turn. This can be used 1 times per short rest." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Second Wind", PlayerID = Player_Fred, Dice = "1d10+4", Type = DiceRollType.HealthOnly, Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest, you can use a bonus action to regain 1d10 + {$Level} HP." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Hungry Jaws", PlayerID = Player_Fred, Modifier = 5, Dice = "1d6+3", Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest as a bonus action, you can make a bite attack. If it hits, you gain {$ConstitutionModifier} temporary HP." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Great Weapon Master Attack", PlayerID = Player_Fred, Part = TurnPart.BonusAction, Description = "On your turn, when you score a critical hit with a melee weapon or reduce a creature to 0 HP with one, you can make one melee weapon attack as a bonus action." });
		}

		void AddPlayerActionShortcutsForLady()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longsword", PlayerID = Player_Lady, Dice = "1d8+3(slashing)", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longsword - 2H", PlayerID = Player_Lady, Dice = "1d10+3(slashing)", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Warhammer", PlayerID = Player_Lady, Dice = "1d8+3(bludgeoning)", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Warhammer - 2H", PlayerID = Player_Lady, Dice = "1d10+3(bludgeoning)", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerID = Player_Lady, Dice = "4(bludgeoning)", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Second Wind", PlayerID = Player_Lady, Dice = "1d10+4", Type = DiceRollType.HealthOnly, Part = TurnPart.BonusAction, LimitCount = 1, LimitSpan = DndTimeSpan.ShortRest, Description = "Once per short rest, you can use a bonus action to regain 1d10 + {$Level} HP." });


			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Shifting",
				PlayerID = Player_Lady,
				Type = DiceRollType.None,
				Part = TurnPart.BonusAction,
				LimitCount = 1,
				LimitSpan = DndTimeSpan.ShortRest,
				Description = "Once per short rest as a bonus action, you can assume a more bestial appearance. This transformation lasts for 1 minute, until you die, or until you revert to your normal appearance as a bonus action. When you shift, you gain +5 temp HP and additional benefits that depend on your shifter subrace."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Action Surge", PlayerID = Player_Lady, Part = TurnPart.Special, Description = "You can take one additional action on your turn. This can be used 1 times per short rest." });

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Longtooth Shifting Strike",
				PlayerID = Player_Lady,
				Part = TurnPart.BonusAction,
				Dice = "1d6+3(piercing)",
				Modifier = 5,
				Description = "While shifted, you can use your fangs to make an unarmed strike as a bonus action. If you hit, you can deal 1d6 + 3 piercing damage, instead of the bludgeoning damage normal for an unarmed strike."
			});
		}

		private void AddPlayerActionShortcutsForShemo()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Scimitar", PlayerID = Player_Shemo, Dice = "1d6+1", Modifier = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Scimitar (Shillelagh)", PlayerID = Player_Shemo, Dice = "1d8+1", Modifier = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Poison Spray", PlayerID = Player_Shemo, Dice = "1d12", DC = 13, Ability = Ability.Constitution, UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shillelagh", PlayerID = Player_Shemo, Dice = "1d8+3", Modifier = 5, UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Thunderwave", PlayerID = Player_Shemo, Dice = "2d8", DC = 13, Ability = Ability.Constitution, UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Healing Word", PlayerID = Player_Shemo, Dice = "1d4+3", Healing = true });
			AddCureWounds(Player_Shemo, "1d8+3");
		}

		void AddCureWounds(int playerID, string diceStr)
		{
			Character player = this.GetPlayer(playerID);
			if (player == null)
				return;
			PlayerActionShortcut cureWounds = new PlayerActionShortcut()
			{
				Name = "Cure Wounds",
				PlayerID = playerID,
				Dice = diceStr,
				Healing = true,
				Description = "A creature you touch regains a number of hit points equal to 1d8 + your spellcasting ability modifier. This spell has no effect on undead or constructs.\n\nAt Higher Levels: When you cast this spell using a spell slot of 2nd level or higher, the healing increases by 1d8 for each slot level above 1st."
			};

			cureWounds.Windups.Add(new WindupDto()
			{
				Effect = "Fairy",
				Hue = player.hueShift,
				Scale = 0.8,
				SoundFileName = "CureWounds"
			}.Float());
			actionShortcuts.Add(cureWounds);
		}

		void AddDivineSense(int playerId)
		{
			PlayerActionShortcut divineSense = new PlayerActionShortcut()
			{
				Name = "Divine Sense",
				PlayerID = playerId,
				Part = TurnPart.Action,
				Description = "As an action, you can detect good and evil. Until the end of your next turn, you can sense anything affected by the hallow spell or know the location of any celestial, fiend, undead within 60 ft. that is not behind total cover. You can use this feature 5 times per long rest."
			};
			const string effectName = "Orb";
			divineSense.Windups.Add(new WindupDto()
			{
				Effect = effectName,
				Hue = 220,
				SoundFileName = "DetectGoodEvil"
			}.Float());
			divineSense.Windups.Add(new WindupDto()
			{
				Effect = effectName,
				DegreesOffset = 180,
				Hue = 0
			}.Float());
			actionShortcuts.Add(divineSense);
		}

		void AddChillTouch(int playerId, string damageDiceStr)
		{
			PlayerActionShortcut chillTouch = new PlayerActionShortcut()
			{ Name = "Chill Touch", PlayerID = playerId, Dice = damageDiceStr, Modifier = 5, UsesMagic = true, Type = DiceRollType.Attack };

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
			actionShortcuts.Add(chillTouch);
		}

		void AddShieldOfFaith(int playerId)
		{
			PlayerActionShortcut shieldOfFaith = new PlayerActionShortcut()
			{
				Name = "Shield of Faith",
				PlayerID = playerId,
				Part = TurnPart.BonusAction,
				Description = "A shimmering field appears and surrounds a creature of your choice within range, granting it a +2 bonus to AC for the duration."
			};

			const string effectName = "Plasma";
			shieldOfFaith.Windups.Add(new WindupDto() { Effect = effectName, Hue = -30 }.MoveUpDown(160).Fade());
			actionShortcuts.Add(shieldOfFaith);
		}

		void AddSanctuary(int playerId)
		{
			PlayerActionShortcut sanctuary = new PlayerActionShortcut()
			{
				Name = "Sanctuary",
				PlayerID = playerId,
				Part = TurnPart.BonusAction,
				Description = "You ward a creature within range against attack. Until the spell ends, any creature who targets the warded creature with an attack or a harmful spell must first make a Wisdom saving throw. On a failed save, the creature must choose a new target or lose the attack or spell. This spell doesn't protect the warded creature from area effects, such as the explosion of a fireball.\n\nIf the warded creature makes an attack, casts a spell that affects an enemy, or deals damage to another creature, this spell ends."
			};
			sanctuary.Windups.Add(new WindupDto() { Effect = "Wide", Hue = 170 }.Fade());
			sanctuary.Windups.Add(new WindupDto() { Effect = "Wide", DegreesOffset = -60, Hue = 200, Rotation = 45 }.Fade());
			sanctuary.Windups.Add(new WindupDto() { Effect = "Wide", DegreesOffset = 60, Hue = 230, Rotation = -45 }.Fade());
			actionShortcuts.Add(sanctuary);
		}
		void AddThunderousSmite(int playerId, string additionalDice)
		{
			PlayerActionShortcut thunderousSmite = new PlayerActionShortcut()
			{
				Name = "Thunderous Smite",
				PlayerID = playerId,
				Part = TurnPart.BonusAction,
				AddDice = additionalDice,
				DC = 13,
				Ability = Ability.Strength,
				MinDamage = keepExistingModifier,
				Modifier = keepExistingModifier,
				Description = "The first time you hit with a melee weapon attack during this spell’s duration, your weapon rings with thunder that is audible within 300 feet of you, and the attack deals an extra 2d6 thunder damage to the target. Additionally, if the target is a creature, it must succeed on a Strength saving throw or be pushed 10 feet away from you and knocked prone."
			};
			AddLightning(thunderousSmite);
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = 45 }.Fade());
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Wide", FlipHorizontal = true, Hue = 45 }.Fade().MoveUpDown(40));
			actionShortcuts.Add(thunderousSmite);
		}

		private static void AddLightning(PlayerActionShortcut thunderousSmite, int hueAdjust = 0)
		{
			thunderousSmite.Windups.Add(new WindupDto() { Effect = "Lightning", Hue = hueAdjust, Scale = 2, SoundFileName = "Lightning" }.MoveUpDown(146));
		}

		void AddWrathfulSmite(int playerId, string additionalDice)
		{
			PlayerActionShortcut wrathfulSmite = new PlayerActionShortcut()
			{
				Name = "Wrathful Smite",
				PlayerID = playerId,
				Part = TurnPart.BonusAction,
				AddDice = additionalDice,
				DC = 13,
				Ability = Ability.Wisdom,
				MinDamage = keepExistingModifier,
				Modifier = keepExistingModifier,
				Description = "The next time you hit with a melee weapon attack during this spell’s duration, your attack deals an extra 1d6 psychic damage. Additionally, if the target is a creature, it must make a Wisdom saving throw or be frightened of you until the spell ends. As an action, the creature can make a Wisdom check against your spell save DC to steel its resolve and end this spell."
			};

			AddLightning(wrathfulSmite, -1);
			wrathfulSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = -1, SoundFileName = "Psychic1" }.Fade());
			wrathfulSmite.Windups.Add(new WindupDto() { Effect = "Wide", Hue = -1, FlipHorizontal = true, SoundFileName = "Psychic2" }.Fade().MoveUpDown(40));
			
			actionShortcuts.Add(wrathfulSmite);
			
		}
		private void AddPlayerActionShortcutsForAva()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (1H)", PlayerID = Player_Ava, Dice = "1d8+3(slashing)", Modifier = 5, MinDamage = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (2H)", PlayerID = Player_Ava, Dice = "1d10+3(slashing)", Modifier = 5, MinDamage = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Greatsword, +1", PlayerID = Player_Ava, Dice = "2d6+4(slashing)", Modifier = +6, MinDamage = 3 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Javelin", PlayerID = Player_Ava, Dice = "1d6+3(piercing)", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Net", PlayerID = Player_Ava, Dice = "", Modifier = 2 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerID = Player_Ava, Dice = "+4(bludgeoning)", Modifier = 5 });

			AddCureWounds(Player_Ava, "1d8+3");

			AddThunderousSmite(Player_Ava, "2d6(thunder)");

			AddWrathfulSmite(Player_Ava, "1d6(psychic)");

			AddShieldOfFaith(Player_Ava);
			AddSanctuary(Player_Ava);


			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Divine Smite",
				PlayerID = Player_Ava,
				Type = DiceRollType.AddOnDice,
				Part = TurnPart.Special,
				AdditionalRollTitle = "Divine Smite",
				InstantDice = "2d8(radiant:damage)",
				Description = "When you hit with a melee weapon attack, you can expend one spell slot to deal 2d8 extra radiant damage to the target plus 1d8 for each spell level higher than 1st (max 5d8) and plus 1d8 against undead or fiends."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Channel Divinity: Rebuke the Violent",
				PlayerID = Player_Ava,
				Type = DiceRollType.None,
				Part = TurnPart.Reaction,
				DC = 14,
				Ability = Ability.Wisdom,
				Description = "Immediately after an attacker within 30 ft. deals damage with an attack against a creature other than you, you can use your reaction to force the attacker to make a WIS saving throw (DC 14). On failure, the attacker takes radiant damage equal to the damage it just dealt, or half damage on success."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Channel Divinity: Emissary of Peace",
				PlayerID = Player_Ava,
				Part = TurnPart.BonusAction,
				Description = "As a bonus action, you grant yourself a +5 bonus to Charisma (Persuasion) checks for the next 10 minutes."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Great Weapon Master Attack",
				PlayerID = Player_Ava,
				Part = TurnPart.BonusAction,
				Description = "On your turn, when you score a critical hit with a melee weapon or reduce a creature to 0 HP with one, you can make one melee weapon attack as a bonus action."
			});

			AddDivineSense(Player_Ava);

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Lay on Hands Pool",
				PlayerID = Player_Ava,
				Part = TurnPart.Action,
				Description = "You have a pool of healing power that can restore 20 HP per long rest. As an action, you can touch a creature to restore any number of HP remaining in the pool, or 5 HP to either cure a disease or neutralize a poison affecting the creature."
			});

			actionShortcuts.Add(new PlayerActionShortcut()
			{
				Name = "Divine Smite (undead)",
				PlayerID = Player_Ava,
				Type = DiceRollType.AddOnDice,
				Part = TurnPart.Special,
				AdditionalRollTitle = "Divine Smite",
				InstantDice = "3d8(radiant:damage)",
				Description = "When you hit with a melee weapon attack, you can expend one spell slot to deal 2d8 extra radiant damage to the target plus 1d8 for each spell level higher than 1st (max 5d8) and plus 1d8 against undead or fiends."
			});
		}

		private void AddPlayerActionShortcutsForMerkin()
		{
			// TODO: Remove WindupName
			AddChaosBolt();
			AddLightningLure();
			AddMelfsMinuteMeteors();
			AddRayOfFrost();


			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Crossbow, Light", PlayerID = Player_Merkin, Dice = "1d8+2(piercing)", Modifier = 4 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger", PlayerID = Player_Merkin, Dice = "1d4+2(piercing)", Modifier = 4 });

			AddChillTouch(Player_Merkin, "1d8(necrotic)");
		}
		void AddRayOfFrost()
		{
			PlayerActionShortcut rayOfFrost = new PlayerActionShortcut()
			{ Name = "Ray of Frost", PlayerID = Player_Merkin, Dice = "1d8(cold)", Modifier = 5, UsesMagic = true };

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
			actionShortcuts.Add(rayOfFrost);
		}

		void AddMelfsMinuteMeteors()
		{
			double scale = 0.6;
			DndCore.Vector offset = new DndCore.Vector(0, -140);
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = "Melf's Minute Meteors", PlayerID = Player_Merkin, Dice = "1d6(fire)", DC = 13, Ability = Ability.Strength, UsesMagic = true };
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

			actionShortcuts.Add(shortcut);
		}

		private void AddLightningLure()
		{
			DndCore.Vector offset = new DndCore.Vector(0, 70);
			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = "Lightning Lure", PlayerID = Player_Merkin, Dice = "1d8(lightning)", DC = 13, Ability = Ability.Strength, UsesMagic = true };

			shortcut.Windups.Add(new WindupDto() { Hue = 45, Scale = 1.1, Effect = "LiquidSparks", Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Hue = 220, Scale = 1.1, DegreesOffset = 180, Effect = "LiquidSparks", Offset = offset });

			actionShortcuts.Add(shortcut);
		}

		private void AddChaosBolt()
		{
			DndCore.Vector offset = new DndCore.Vector(0, 140);

			PlayerActionShortcut shortcut = new PlayerActionShortcut()
			{ Name = "Chaos Bolt", PlayerID = Player_Merkin, WindupName = "Wide", Dice = "2d8", Modifier = 5, UsesMagic = true, Type = DiceRollType.ChaosBolt };
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 300, Rotation = 45, FlipHorizontal = true, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 220, Rotation = -45, Offset = offset });
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 300 + 30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 220 + 30, Rotation = -45, Offset = offset, DegreesOffset = 120 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 300 - 30, Rotation = 45, FlipHorizontal = true, Offset = offset, DegreesOffset = 240 });
			shortcut.Windups.Add(new WindupDto() { Effect = "Trails", Hue = 220 - 30, Rotation = -45, Offset = offset, DegreesOffset = 240 });

			actionShortcuts.Add(shortcut);
		}

		private void AddPlayerActionShortcutsForWilly()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger of Warning", PlayerID = Player_Willy, Dice = "1d4+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortbow", PlayerID = Player_Willy, Dice = "1d6+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortsword", PlayerID = Player_Willy, Dice = "1d6+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Fire Bolt", PlayerID = Player_Willy, Dice = "1d10", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bonus Dagger", PlayerID = Player_Willy, Dice = "1d4", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger of Warning (Sneak)", PlayerID = Player_Willy, Dice = "2d6,1d4+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortbow (Sneak)", PlayerID = Player_Willy, Dice = "3d6+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Shortsword (Sneak)", PlayerID = Player_Willy, Dice = "3d6+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Bonus Dagger (Sneak)", PlayerID = Player_Willy, Dice = "2d6,1d4", Modifier = 5 });
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
					tabItem.PlayerID = player.playerID;

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
			RollSkillCheck(e.Skill);
		}
		
		public void RollSkillCheck(Skills skill)
		{
			SelectSkill(skill);
			rbActivePlayer.IsChecked = true;
			RollTheDice(PrepareRoll(DiceRollType.SkillCheck));
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
			Character kent = new Character();
			kent.name = "Willy Shaker";
			kent.playerID = Player_Willy;
			kent.raceClass = "High Elf Rogue";
			kent.goldPieces = 150;
			kent.hitPoints = 35;
			kent.maxHitPoints = 35;
			kent.baseArmorClass = 15;
			kent.baseStrength = 10;
			kent.headshotIndex = 4;
			kent.baseDexterity = 17;
			kent.baseConstitution = 16;
			kent.baseIntelligence = 9;
			kent.baseWisdom = 8;
			kent.baseCharisma = 14;
			kent.proficiencyBonus = 2;
			kent.proficientSkills = Skills.insight | Skills.perception | Skills.performance | Skills.slightOfHand | Skills.stealth;
			kent.savingThrowProficiency = Ability.Dexterity | Ability.Intelligence;
			kent.doubleProficiency = Skills.deception | Skills.persuasion;
			kent.initiative = +3;
			kent.hueShift = 0;
			kent.dieBackColor = "#710138";
			kent.dieFontColor = "#ffffff";
			kent.rollInitiative = VantageKind.Advantage;

			Character kayla = new Character();
			kayla.name = "Shemo Globin";
			kayla.playerID = Player_Shemo;
			kayla.raceClass = "Firbolg Druid";
			kayla.goldPieces = 170;
			kayla.headshotIndex = 1;
			kayla.hitPoints = 31;
			kayla.maxHitPoints = 31;
			kayla.baseArmorClass = 15;
			kayla.baseStrength = 10;
			kayla.baseDexterity = 12;
			kayla.baseConstitution = 15;
			kayla.baseIntelligence = 8;
			kayla.baseCharisma = 13;
			kayla.baseWisdom = 17;
			kayla.proficiencyBonus = 2;
			kayla.initiative = +1;
			kayla.proficientSkills = Skills.animalHandling | Skills.arcana | Skills.history | Skills.nature;
			kayla.savingThrowProficiency = Ability.Intelligence | Ability.Wisdom;
			kayla.hueShift = 138;
			kayla.dieBackColor = "#00641d";
			kayla.dieFontColor = "#ffffff";

			Character mark = new Character();
			mark.name = "Merkin Bushwacker";
			mark.raceClass = "Half-Elf Sorcerer";
			mark.goldPieces = 128;
			mark.headshotIndex = 2;
			mark.playerID = Player_Merkin;
			mark.hitPoints = 26;
			mark.maxHitPoints = 26;
			mark.baseArmorClass = 12;
			mark.baseStrength = 8;
			mark.baseDexterity = 14;
			mark.baseConstitution = 14;
			mark.baseIntelligence = 12;
			mark.baseCharisma = 17;
			mark.baseWisdom = 12;
			mark.proficiencyBonus = 2;
			mark.initiative = +2;
			mark.proficientSkills = Skills.acrobatics | Skills.deception | Skills.intimidation | Skills.perception | Skills.performance | Skills.persuasion;
			mark.savingThrowProficiency = Ability.Constitution | Ability.Charisma;
			mark.hueShift = 260;
			mark.dieBackColor = "#401260";
			mark.dieFontColor = "#ffffff";

			Character karen = new Character();
			karen.name = "Ava Wolfhard";
			karen.raceClass = "Human Paladin";
			karen.goldPieces = 150;
			karen.playerID = Player_Ava;
			karen.headshotIndex = 3;
			karen.hitPoints = 36;
			karen.maxHitPoints = 36;
			karen.baseArmorClass = 16;
			karen.baseStrength = 16;
			karen.baseDexterity = 11;
			karen.baseConstitution = 14;
			karen.baseIntelligence = 8;
			karen.baseWisdom = 8;
			karen.baseCharisma = 18;
			karen.proficiencyBonus = +2;
			karen.initiative = 0;
			karen.proficientSkills = Skills.acrobatics | Skills.intimidation | Skills.performance | Skills.persuasion | Skills.survival;
			karen.savingThrowProficiency = Ability.Wisdom | Ability.Charisma;
			karen.hueShift = 210;
			karen.dieBackColor = "#04315a";
			karen.dieFontColor = "#ffffff";

			Character fred = new Character();
			fred.name = "Fred";
			fred.raceClass = "Lizardfolk Fighter";
			fred.goldPieces = 10;
			fred.playerID = Player_Fred;
			fred.headshotIndex = 5;
			fred.hitPoints = 40;
			fred.maxHitPoints = 40;
			fred.baseArmorClass = 16;
			fred.baseStrength = 16;
			fred.baseDexterity = 16;
			fred.baseConstitution = 16;
			fred.baseIntelligence = 8;
			fred.baseWisdom = 9;
			fred.baseCharisma = 10;
			fred.proficiencyBonus = +2;
			fred.initiative = +3;
			fred.proficientSkills = Skills.acrobatics | Skills.athletics | Skills.nature | Skills.perception | Skills.survival | Skills.stealth;
			fred.savingThrowProficiency = Ability.Strength | Ability.Constitution;
			fred.hueShift = 206;
			fred.dieBackColor = "#136399";
			fred.dieFontColor = "#ffffff";

			Character lara = new Character();
			lara.name = "Lady McLoveNuts";
			lara.raceClass = "Longtooth Fighter";
			lara.goldPieces = 250;
			lara.playerID = Player_Lady;
			lara.headshotIndex = 0;
			lara.hitPoints = 32;
			lara.maxHitPoints = 32;
			lara.baseArmorClass = 16;
			lara.baseStrength = 16;
			lara.baseDexterity = 14;
			lara.baseConstitution = 12;
			lara.baseIntelligence = 13;
			lara.baseWisdom = 12;
			lara.baseCharisma = 12;
			lara.proficiencyBonus = +2;
			lara.initiative = +2;
			lara.proficientSkills = Skills.animalHandling | Skills.arcana | Skills.intimidation | Skills.investigation | Skills.perception | Skills.survival;
			lara.savingThrowProficiency = Ability.Strength | Ability.Constitution;
			lara.hueShift = 37;
			//lara.dieBackColor = "#a86600";
			//lara.dieFontColor = "#ffffff";
			lara.dieBackColor = "#fea424";
			lara.dieFontColor = "#000000";

			players.Clear();

			//players.Add(kent);
			players.Add(fred);
			players.Add(lara);
			//players.Add(kayla);
			players.Add(mark);
			players.Add(karen);


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
				else if (numRadioBoxesChecked == 1 && lastPlayerId == PlayerID)
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
			CheckPlayer(PlayerID);
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
			string lowerName = characterName.ToLower();
			foreach (Character character in this.players)
			{
				if (character.name.ToLower().StartsWith(lowerName))
				{
					return character.playerID;
				}
			}
			return -1;
		}

		private void ChangePlayerHealth(TextBox textBox, int multiplier)
		{
			DamageHealthChange damageHealthChange = GetDamageHealthChange(multiplier, textBox);

			ApplyDamageHealthChange(damageHealthChange);

			if (damageHealthChange != null)
			{
				string serializedObject = JsonConvert.SerializeObject(damageHealthChange);
				HubtasticBaseStation.ChangePlayerHealth(serializedObject);
			}
		}

		public void ApplyDamageHealthChange(DamageHealthChange damageHealthChange)
		{
			foreach (int playerId in damageHealthChange.PlayerIds)
			{
				Character player = GetPlayer(playerId);
				if (player != null)
				{
					player.ChangeHealth(damageHealthChange.DamageHealth);
					HubtasticBaseStation.PlayerDataChanged(playerId, player.ToJson());
				}
			}
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
			SelectCharacter(Player_Merkin);
			RollTheDice(PrepareRoll(DiceRollType.WildMagicD20Check));
		}

		public void SelectCharacter(int playerId)
		{
			Dispatcher.Invoke(() =>
			{
				foreach (PlayerTabItem playerTabItem in tabPlayers.Items)
				{
					if (playerTabItem.PlayerID == playerId)
					{
						tabPlayers.SelectedItem = playerTabItem;
						return;
					}
				}
			});
		}


		public void RollWildMagic()
		{
			BtnWildMagic_Click(null, null);
		}

		private void BtnSendWindup_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnClearWindups_Click(object sender, RoutedEventArgs e)
		{

		}

		public void SetClock(int hours, int minutes, int seconds)
		{

		}
		public void AdvanceClock(int hours, int minutes, int seconds)
		{

		}

		public void RollDice(string diceStr, DiceRollType diceRollType)
		{
			
		}

		public void HideScroll()
		{
			
		}

		public void Speak(int playerId, string message)
		{
			
		}
	}
}
