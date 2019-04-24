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
		ScrollPage activePage = ScrollPage.main;
		DndTimeClock dndTimeClock;
		bool resting = false;
		DispatcherTimer realTimeAdvanceTimer;

		public MainWindow()
		{
			realTimeAdvanceTimer = new DispatcherTimer();
			realTimeAdvanceTimer.Tick += new EventHandler(realTimeClockHandler);
			realTimeAdvanceTimer.Interval = new TimeSpan(0, 0, 1);

			dndTimeClock = new DndTimeClock();
			dndTimeClock.TimeChanged += DndTimeClock_TimeChanged;
			// TODO: Save and retrieve time.
			dndTimeClock.SetTime(DateTime.Now);
			InitializeComponent();
			FocusHelper.FocusedControlsChanged += FocusHelper_FocusedControlsChanged;
			groupEffectBuilder.Entries = new ObservableCollection<TimeLineEffect>();
			spTimeSegments.DataContext = dndTimeClock;
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

		private void TabControl_PlayerChanged(object sender, SelectionChangedEventArgs e)
		{
			activePage = ScrollPage.main;
			FocusHelper.ClearActiveStatBoxes();
			HubtasticBaseStation.PlayerDataChanged(PlayerID, activePage, string.Empty);
		}

		void ConnectToHub()
		{

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

		public void TestRollDice(DiceRoll diceRoll)
		{
			string serializedObject = JsonConvert.SerializeObject(diceRoll);
			HubtasticBaseStation.RollDice(serializedObject);
		}

		private void BtnSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			TestRollDice(PrepareRoll(DiceRollType.SkillCheck));
		}

		private void BtnSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			TestRollDice(PrepareRoll(DiceRollType.SavingThrow));
		}

		private void BtnDeathSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			TestRollDice(PrepareRoll(DiceRollType.DeathSavingThrow));
		}

		private void BtnAttack_Click(object sender, RoutedEventArgs e)
		{
			TestRollDice(PrepareRoll(DiceRollType.Attack));
		}

		private DiceRoll PrepareRoll(DiceRollType type)
		{
			DiceRollKind diceRollKind;
			if (rbTestAdvantageDieRoll.IsChecked == true)
				diceRollKind = DiceRollKind.Advantage;
			else if (rbTestDisadvantageDieRoll.IsChecked == true)
				diceRollKind = DiceRollKind.Disadvantage;
			else
				diceRollKind = DiceRollKind.Normal;
			DiceRoll diceRoll = new DiceRoll(diceRollKind, tbxDamageDice.Text);

			if (double.TryParse(tbxModifier.Text, out double modifierResult))
				diceRoll.Modifier = modifierResult;

			if (double.TryParse(tbxHiddenThreshold.Text, out double thresholdResult))
				diceRoll.HiddenThreshold = thresholdResult;

			diceRoll.IsMagic = ckbUseMagic.IsChecked == true;
			diceRoll.Type = type;
			return diceRoll;
		}

		private void BtnFlatD20_Click(object sender, RoutedEventArgs e)
		{

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

		private void DndTimeClock_TimeChanged(object sender, EventArgs e)
		{
			UpdateClock();

			// TODO: Update time-based curses, spells, and diseases.
			if (resting)
			{
				// TODO: Update character stats.
			}
		}

		private void UpdateClock()
		{
			if (txtTime == null)
				return;
			txtTime.Text = dndTimeClock.Time.ToString("H:mm:ss") + ", " + dndTimeClock.AsDndDateString();
		}

		private void BtnAdvanceTurn_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromSeconds(6));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			OnCombatChanged();
			UpdateClock();
		}

		private void BtnAddDay_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromDays(1));
		}

		private void BtnAddTenDay_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromDays(10));
		}

		private void BtnAddMonth_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromDays(30));
		}

		private void BtnAddHour_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromHours(1));
		}

		private void BtnAdd10Minutes_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromMinutes(10));
		}

		private void BtnEnterExitCombat_Click(object sender, RoutedEventArgs e)
		{
			dndTimeClock.InCombat = !dndTimeClock.InCombat;
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
				realTimeAdvanceTimer.Start();
				spTimeDirectModifiers.IsEnabled = true;
				btnAdvanceTurn.IsEnabled = false;
			}
		}

		void realTimeClockHandler(object sender, EventArgs e)
		{
			dndTimeClock.Advance(DndTimeSpan.FromSeconds(1));
		}
	}
}
