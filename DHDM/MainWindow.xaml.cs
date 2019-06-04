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

		UIElement BuildShortcutButton(PlayerActionShortcut playerActionShortcut)
		{
			StackPanel stackPanel = new StackPanel();
			stackPanel.Margin = new Thickness(2);
			stackPanel.Tag = playerActionShortcut.Index;
			Button button = new Button();
			button.Padding = new Thickness(4,2,4,2);
			stackPanel.Children.Add(button);
			button.Content = playerActionShortcut.Name;
			button.Tag = playerActionShortcut.Index;
			button.Click += PlayerShortcutButton_Click;
			Rectangle rectangle = new Rectangle();
			rectangle.Tag = playerActionShortcut.Index;
			rectangle.Height = 3;
			rectangle.Fill = new SolidColorBrush(Colors.Red);
			return stackPanel;
		}

		PlayerActionShortcut GetActionShortcut(object tag)
		{
			if (int.TryParse(tag.ToString(), out int index))
				return actionShortcuts.FirstOrDefault(x => x.Index == index);
			return null;
		}
		private void PlayerShortcutButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				PlayerActionShortcut actionShortcut = GetActionShortcut(button.Tag);
				if (actionShortcut == null)
					return;
				tbxDamageDice.Text = actionShortcut.Dice;
				tbxModifier.Text = actionShortcut.Modifier.ToString();
				ckbUseMagic.IsChecked = actionShortcut.UsesMagic;
				NextDieRollType = actionShortcut.Type;
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
			NextDieRollType = DiceRollType.None;
			activePage = ScrollPage.main;
			FocusHelper.ClearActiveStatBoxes();
			HubtasticBaseStation.PlayerDataChanged(PlayerID, activePage, string.Empty);
			SetActionShortcuts(PlayerID);
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
			DiceRollKind diceRollKind = DiceRollKind.Normal;

			if (CanIncludeVantageDice(type))
				if (rbTestAdvantageDieRoll.IsChecked == true)
					diceRollKind = DiceRollKind.Advantage;
				else if (rbTestDisadvantageDieRoll.IsChecked == true)
					diceRollKind = DiceRollKind.Disadvantage;
			
			string damageDice = string.Empty;
			if (IsAttack(type) || type == DiceRollType.DamageOnly || type == DiceRollType.HealthOnly)
				damageDice = tbxDamageDice.Text;

			DiceRoll diceRoll = new DiceRoll(diceRollKind, damageDice);

			diceRoll.CritFailMessage = "";
			diceRoll.CritSuccessMessage = "";
			diceRoll.SuccessMessage = "";
			diceRoll.FailMessage = "";

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
					break;
			}


			diceRoll.ThrowPower = new Random().Next() * 2;
			if (diceRoll.ThrowPower < 0.3)
				diceRoll.ThrowPower = 0.3;

			if (double.TryParse(tbxModifier.Text, out double modifierResult))
				diceRoll.Modifier = modifierResult;

			if (double.TryParse(tbxHiddenThreshold.Text, out double thresholdResult))
				diceRoll.HiddenThreshold = thresholdResult;

			diceRoll.IsMagic = ckbUseMagic.IsChecked == true && IsAttack(type);
			diceRoll.Type = type;
			return diceRoll;
		}

		private void BtnFlatD20_Click(object sender, RoutedEventArgs e)
		{
			RollTheDice(PrepareRoll(DiceRollType.FlatD20));
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

		public DiceRollType NextDieRollType { get => nextDieRollType; set
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

		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.DiceRollData != null)
				History.Log("Die roll: " + ea.DiceRollData.roll);
			EnableDiceRollButtons(true);
			ShowClearButton(null, EventArgs.Empty);
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.InCombat = !dndTimeClock.InCombat;
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
		private const int Player_Shemo = 1;
		private const int Player_Merkin = 2;
		private const int Player_Ava = 3;
		DiceRollType nextDieRollType;
		void InitializeAttackShortcuts()
		{
			PlayerActionShortcut.PrepareForCreation();
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
	}
}
