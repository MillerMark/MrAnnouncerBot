using Microsoft.AspNetCore.SignalR.Client;
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
using System.Threading;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<PlayerActionShortcut> actionShortcuts = new List<PlayerActionShortcut>();
		ScrollPage activePage = ScrollPage.main;
		DndTimeClock dndTimeClock;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;
		DispatcherTimer showClearButtonTimer;
		DispatcherTimer updateClearButtonTimer;
		DateTime lastUpdateTime;

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
		private void PlayerShortcutButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				PlayerActionShortcut actionShortcut = GetActionShortcut(button.Tag);
				if (actionShortcut == null)
					return;
				settingInternally = true;
				try
				{
					HighlightPlayerShortcut((int)button.Tag);
					tbxDamageDice.Text = actionShortcut.Dice;
					if (actionShortcut.Modifier > 0)
						tbxModifier.Text = "+" + actionShortcut.Modifier.ToString();
					else
						tbxModifier.Text = actionShortcut.Modifier.ToString();

					ckbUseMagic.IsChecked = actionShortcut.UsesMagic;
					NextDieRollType = actionShortcut.Type;
				}
				finally
				{
					settingInternally = false;
				}
			}
		}

		void SetActionShortcuts(int playerID)
		{

			for (int i = spShortcutsActivePlayer.Children.Count - 1; i >= 0; i--)
			{
				UIElement uIElement = spShortcutsActivePlayer.Children[i];
				if (uIElement is StackPanel)
					spShortcutsActivePlayer.Children.RemoveAt(i);
			}

			List<PlayerActionShortcut> playerActions = actionShortcuts.Where(x => x.PlayerID == playerID).ToList();

			foreach (PlayerActionShortcut playerActionShortcut in playerActions)
			{
				spShortcutsActivePlayer.Children.Add(BuildShortcutButton(playerActionShortcut));
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
				HubtasticBaseStation.PlayerDataChanged(tabPlayers.SelectedIndex, activePage, string.Empty);
			}
		}

		private void HandleCharacterChanged(object sender, RoutedEventArgs e)
		{
			if (sender is CharacterSheets characterSheets)
			{
				string character = characterSheets.GetCharacter();
				HubtasticBaseStation.PlayerDataChanged(tabPlayers.SelectedIndex, activePage, character);
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

					diceRoll.AddPlayer(checkbox.PlayerId, vantageKind, checkbox.TbxInspiration.Text);
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


			diceRoll.ThrowPower = new Random().Next() * 2;
			if (diceRoll.ThrowPower < 0.3)
				diceRoll.ThrowPower = 0.3;

			if (double.TryParse(tbxModifier.Text, out double modifierResult))
				diceRoll.Modifier = modifierResult;

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
						string playerName = GetPlayerName(playerRoll.id);
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
				btnEnterExitCombat.Background = new SolidColorBrush(Color.FromRgb(42, 42, 102));
			else
				btnEnterExitCombat.Background = new SolidColorBrush(Colors.DarkRed);
			if (dndTimeClock.InCombat)
				RollInitiative();
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
			const double timeToAutoClear = 6500;
			TimeSpan timeClearButtonHasBeenVisible = (DateTime.Now - clearButtonShowTime) - pauseTime;
			if (timeClearButtonHasBeenVisible.TotalMilliseconds > timeToAutoClear)
			{
				ClearTheDice();
				rectProgressToClear.Width = 0;
				return;
			}

			double progress = timeClearButtonHasBeenVisible.TotalMilliseconds / timeToAutoClear;
			rectProgressToClear.Width = progress * btnClearDice.Width;
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
			diceRoll.TrailingEffects.Add(new TrailingEffect()
			{
				Type = TrailingSpriteType.SmallSparks,
				LeftRightDistanceBetweenPrints = 0,
				MinForwardDistanceBetweenPrints = 33
			});

			RollTheDice(diceRoll);
		}

		private void BtnBendLuckSubtract_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.BendLuckSubtract);
			AddTrailingSparks(diceRoll);
			RollTheDice(diceRoll);
		}

		private void BtnLuckRollHigh_Click(object sender, RoutedEventArgs e)
		{
			DiceRoll diceRoll = PrepareRoll(DiceRollType.LuckRollHigh);
			AddTrailingSparks(diceRoll);
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

		private const int Player_Willy = 0;
		private const int Player_Lady = 0;
		private const int Player_Shemo = 1;
		private const int Player_Merkin = 2;
		private const int Player_Ava = 3;

		DiceRollType nextDieRollType;
		void InitializeAttackShortcuts()
		{
			highlightRectangles = null;
			actionShortcuts.Clear();
			PlayerActionShortcut.PrepareForCreation();
			//AddPlayerActionShortcutsForWilly();
			AddPlayerActionShortcutsForMerkin();
			AddPlayerActionShortcutsForAva();
			AddPlayerActionShortcutsForShemo();
			AddPlayerActionShortcutsForLady();
		}

		void AddPlayerActionShortcutsForLady()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longsword", PlayerID = Player_Lady, Dice = "1d8+3", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longsword - 2H", PlayerID = Player_Lady, Dice = "1d10+3", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Warhammer", PlayerID = Player_Lady, Dice = "1d8+3", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Warhammer - 2H", PlayerID = Player_Lady, Dice = "1d10+3", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerID = Player_Lady, Dice = "4", Modifier = 5 });

			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Longtooth Shifting Strike", PlayerID = Player_Lady, Dice = "1d6+3", Modifier = 5 });
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
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Cure Wounds", PlayerID = Player_Shemo, Dice = "1d8+3", Healing = true });
		}

		private void AddPlayerActionShortcutsForAva()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (1H)", PlayerID = Player_Ava, Dice = "1d8+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Battleaxe (2H)", PlayerID = Player_Ava, Dice = "1d10+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Javelin", PlayerID = Player_Ava, Dice = "1d6+3", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Net", PlayerID = Player_Ava, Dice = "", Modifier = 2 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Unarmed Strike", PlayerID = Player_Ava, Dice = "+4", Modifier = 5 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Cure Wounds", PlayerID = Player_Ava, Dice = "1d8+3", Healing = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Thunderous Smite", PlayerID = Player_Ava, Dice = "2d6", DC = 13, Ability = Ability.Strength });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Wrathful Smite", PlayerID = Player_Ava, Dice = "1d6", DC = 13, Ability = Ability.Wisdom });
		}

		private void AddPlayerActionShortcutsForMerkin()
		{
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Chaos Bolt", PlayerID = Player_Merkin, Dice = "2d8", Modifier = 5, UsesMagic = true, Type = DiceRollType.ChaosBolt });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Lightning Lure", PlayerID = Player_Merkin, Dice = "1d8", DC = 13, Ability = Ability.Strength, UsesMagic = true });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Crossbow, Light", PlayerID = Player_Merkin, Dice = "1d8+2", Modifier = 4 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Dagger", PlayerID = Player_Merkin, Dice = "1d4+2", Modifier = 4 });
			actionShortcuts.Add(new PlayerActionShortcut()
			{ Name = "Ray of Frost", PlayerID = Player_Merkin, Dice = "1d8", Modifier = 5, UsesMagic = true });
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

		void BuildPlayerTabs()
		{
			buildingTabs = true;

			try
			{
				tabPlayers.Items.Clear();

				foreach (Character player in players)
				{
					TabItem tabItem = new TabItem();
					tabItem.Header = player.name;
					tabPlayers.Items.Add(tabItem);

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
			kent.raceClass = "High Elf Rogue";
			kent.goldPieces = 150;
			kent.hitPoints = 35;
			kent.maxHitPoints = 35;
			kent.baseArmorClass = 15;
			kent.baseStrength = 10;
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
			kayla.raceClass = "Firbolg Druid";
			kayla.goldPieces = 170;
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
			karen.hitPoints = 36;
			karen.maxHitPoints = 36;
			karen.baseArmorClass = 16;
			karen.baseStrength = 16;
			karen.baseDexterity = 11;
			karen.baseConstitution = 14;
			karen.baseIntelligence = 8;
			karen.baseWisdom = 8;
			karen.baseCharisma = 16;
			karen.proficiencyBonus = +2;
			karen.initiative = 0;
			karen.proficientSkills = Skills.acrobatics | Skills.intimidation | Skills.performance | Skills.persuasion | Skills.survival;
			karen.savingThrowProficiency = Ability.Wisdom | Ability.Charisma;
			karen.hueShift = 210;
			karen.dieBackColor = "#04315a";
			karen.dieFontColor = "#ffffff";

			Character lara = new Character();
			lara.name = "Lady McLoveNuts";
			lara.raceClass = "Longtooth Fighter";
			lara.goldPieces = 250;
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
			lara.dieBackColor = "#a86600";
			lara.dieFontColor = "#ffffff";

			// TODO: Promote characters to field and populate tabs...
			if (this.players == null)
				this.players = new List<Character>();
			this.players.Clear();

			//players.Add(kent);
			players.Add(lara);
			players.Add(kayla);
			players.Add(mark);
			players.Add(karen);

			string playerData = JsonConvert.SerializeObject(players);
			HubtasticBaseStation.SetPlayerData(playerData);

			BuildPlayerTabs();
			BuildPlayerUI();
			InitializeAttackShortcuts();
		}

		string GetFirstName(string name)
		{
			if (name == null)
				return "No name";
			int spaceIndex = name.IndexOf(' ');
			if (spaceIndex < 0)
				return name;
			return name.Substring(0, spaceIndex);
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
			int playerId = 0;
			foreach (Character player in players)
			{
				grdPlayerRollOptions.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

				PlayerRollCheckBox checkBox = new PlayerRollCheckBox();
				checkBox.Content = GetFirstName(player.name);
				checkBox.PlayerId = playerId;
				playerId++;
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

				TextBox textBox = new TextBox();
				textBox.Text = "";
				textBox.MinWidth = 70;
				spOptions.Children.Add(textBox);
				checkBox.TbxInspiration = textBox;

				row++;
				grdPlayerRollOptions.Children.Add(checkBox);
				grdPlayerRollOptions.Children.Add(spOptions);
			}
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
			if (lastPlayerIdUnchecked == PlayerID)
				return;
			foreach (UIElement uIElement in grdPlayerRollOptions.Children)
				if (uIElement is PlayerRollCheckBox checkbox)
					checkbox.IsChecked = checkbox.PlayerId == PlayerID;
		}

		List<Character> players;
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
			if (int.TryParse(tbxHealth.Text, out int result))
			{
				// TODO: Send result via signalR.
			}
		}

		private void BtnInflictDamage_Click(object sender, RoutedEventArgs e)
		{

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
	}

	public class PlayerRollCheckBox : CheckBox
	{
		public UIElement DependantUI { get; set; }
		public int PlayerId { get; set; }
		public RadioButton RbDisadvantage { get; set; }
		public RadioButton RbAdvantage { get; set; }
		public RadioButton RbNormal { get; set; }
		public TextBox TbxInspiration { get; set; }
		public PlayerRollCheckBox()
		{

		}
	}
}
