using System;
using DndCore;
using System.Collections.Generic;
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

namespace DndUI
{
	/// <summary>
	/// Interaction logic for CharacterSheets.xaml
	/// </summary>
	public partial class CharacterSheets : UserControl
	{
		public delegate void SkillCheckEventHandler(object sender, SkillCheckEventArgs e);
		public delegate void AbilityEventHandler(object sender, AbilityEventArgs e);

		public static readonly RoutedEvent SavingThrowRequestedEvent = EventManager.RegisterRoutedEvent("SavingThrowRequested", RoutingStrategy.Bubble, typeof(AbilityEventHandler), typeof(CharacterSheets));

		public event AbilityEventHandler SavingThrowRequested
		{
			add { AddHandler(SavingThrowRequestedEvent, value); }
			remove { RemoveHandler(SavingThrowRequestedEvent, value); }
		}

		protected virtual void OnSavingThrowRequested(Ability ability)
		{
			AbilityEventArgs eventArgs = new AbilityEventArgs(SavingThrowRequestedEvent, ability);
			RaiseEvent(eventArgs);
		}
		

		public static readonly RoutedEvent SkillCheckRequestedEvent = EventManager.RegisterRoutedEvent("SkillCheckRequested", RoutingStrategy.Bubble, typeof(SkillCheckEventHandler), typeof(CharacterSheets));

		public event SkillCheckEventHandler SkillCheckRequested
		{
			add { AddHandler(SkillCheckRequestedEvent, value); }
			remove { RemoveHandler(SkillCheckRequestedEvent, value); }
		}

		protected virtual void OnSkillCheckRequested(Skills skill)
		{
			SkillCheckEventArgs eventArgs = new SkillCheckEventArgs(SkillCheckRequestedEvent, skill);
			RaiseEvent(eventArgs);
		}
		
		public ScrollPage Page { get; set; }

		public static readonly RoutedEvent PageBackgroundClickedEvent = EventManager.RegisterRoutedEvent("PageBackgroundClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CharacterSheets));

		public event RoutedEventHandler PageBackgroundClicked
		{
			add { AddHandler(PageBackgroundClickedEvent, value); }
			remove { RemoveHandler(PageBackgroundClickedEvent, value); }
		}

		protected virtual void OnPageBackgroundClicked()
		{
			RoutedEventArgs eventArgs = new RoutedEventArgs(PageBackgroundClickedEvent);
			RaiseEvent(eventArgs);
		}

		public static readonly RoutedEvent CharacterChangedEvent = EventManager.RegisterRoutedEvent("CharacterChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CharacterSheets));
		public static readonly RoutedEvent PreviewCharacterChangedEvent = EventManager.RegisterRoutedEvent("PreviewCharacterChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CharacterSheets));

		public event RoutedEventHandler CharacterChanged
		{
			add { AddHandler(CharacterChangedEvent, value); }
			remove { RemoveHandler(CharacterChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewCharacterChanged
		{
			add { AddHandler(PreviewCharacterChangedEvent, value); }
			remove { RemoveHandler(PreviewCharacterChangedEvent, value); }
		}
		bool changingInternally;
		protected virtual void OnCharacterChanged()
		{
			if (changingInternally)
				return;
			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewCharacterChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(CharacterChangedEvent);
			eventArgs.Source = this;
			RaiseEvent(eventArgs);
		}
		
		public static readonly RoutedEvent PageChangedEvent = 
			EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble, 
				typeof(RoutedEventHandler), typeof(CharacterSheets));

		public static readonly RoutedEvent PreviewPageChangedEvent = EventManager.RegisterRoutedEvent("PreviewPageChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CharacterSheets));
		int playerID;
		int headshotIndex;

		public event RoutedEventHandler PageChanged
		{
			add { AddHandler(PageChangedEvent, value); }
			remove { RemoveHandler(PageChangedEvent, value); }
		}
		public event RoutedEventHandler PreviewPageChanged
		{
			add { AddHandler(PreviewPageChangedEvent, value); }
			remove { RemoveHandler(PreviewPageChangedEvent, value); }
		}

		protected virtual void OnPageChanged(ScrollPage newPage)
		{
			if (Page == newPage)
			{
				return;
			}
			Page = newPage;
			FocusHelper.ClearActiveStatBoxes();

			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewPageChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(PageChangedEvent);
			RaiseEvent(eventArgs);
		}

		public void FocusSkill(Skills skill)
		{
			FocusHelper.ClearActiveStatBoxes();
			switch (skill)
			{
				case Skills.acrobatics: FocusHelper.Add(statSkillAcrobatics); break;
				case Skills.animalHandling: FocusHelper.Add(statSkillAnimalHandling); break;
				case Skills.arcana: FocusHelper.Add(statSkillArcana); break;
				case Skills.athletics: FocusHelper.Add(statSkillAthletics); break;
				case Skills.deception: FocusHelper.Add(statSkillDeception); break;
				case Skills.history: FocusHelper.Add(statSkillHistory); break;
				case Skills.insight: FocusHelper.Add(statSkillInsight); break;
				case Skills.intimidation: FocusHelper.Add(statSkillIntimidation); break;
				case Skills.investigation: FocusHelper.Add(statSkillInvestigation); break;
				case Skills.medicine: FocusHelper.Add(statSkillMedicine); break;
				case Skills.nature: FocusHelper.Add(statSkillNature); break;
				case Skills.perception: FocusHelper.Add(statSkillPerception); break;
				case Skills.performance: FocusHelper.Add(statSkillPerformance); break;
				case Skills.persuasion: FocusHelper.Add(statSkillPersuasion); break;
				case Skills.religion: FocusHelper.Add(statSkillReligion); break;
				case Skills.slightOfHand: FocusHelper.Add(statSkillSlightOfHand); break;
				case Skills.stealth: FocusHelper.Add(statSkillStealth); break;
				case Skills.survival: FocusHelper.Add(statSkillSurvival); break;
				case Skills.strength: FocusHelper.Add(statStrength2); break;
				case Skills.dexterity: FocusHelper.Add(statDexterity2); break;
				case Skills.constitution: FocusHelper.Add(statConstitution2); break;
				case Skills.intelligence: FocusHelper.Add(statIntelligence2); break;
				case Skills.wisdom: FocusHelper.Add(statWisdom2); break;
				case Skills.charisma: FocusHelper.Add(statCharisma2); break;
			}
		}
		
		public void FocusSavingAbility(Ability ability)
		{
			FocusHelper.ClearActiveStatBoxes();
			switch (ability)
			{
				case Ability.charisma: FocusHelper.Add(statSavingCharisma); break;
				case Ability.constitution: FocusHelper.Add(statSavingConstitution); break;
				case Ability.dexterity: FocusHelper.Add(statSavingDexterity); break;
				case Ability.intelligence: FocusHelper.Add(statSavingIntelligence); break;
				case Ability.strength: FocusHelper.Add(statSavingStrength); break;
				case Ability.wisdom: FocusHelper.Add(statSavingWisdom); break;
			}
		}

		public CharacterSheets()
		{
			InitializeComponent();
		}

		private void PageSkills_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.skills);
		}

		private void PageEquipment_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.equipment);
		}

		private void PageMain_Activated(object sender, RoutedEventArgs e)
		{
			OnPageChanged(ScrollPage.main);
		}

		private void PageMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.main);
		}

		private void PageSkills_Activated(object sender, RoutedEventArgs e)
		{
			OnPageChanged(ScrollPage.skills);
		}

		private void PageEquipment_Activated(object sender, RoutedEventArgs e)
		{
			OnPageChanged(ScrollPage.equipment);
		}

		private void AnyStatChanged(object sender, RoutedEventArgs e)
		{
			OnCharacterChanged();
		}

		Skills GetProficiencySkills()
		{
			Skills result = Skills.none;
			if (statSkillAcrobaticsProficient.IsChecked == true)
				result |= Skills.acrobatics;
			if (statSkillAthleticsProficient.IsChecked == true)
				result |= Skills.athletics;
			if (statSkillDeceptionProficient.IsChecked == true)
				result |= Skills.deception;
			if (statSkillHistoryProficient.IsChecked == true)
				result |= Skills.history;
			if (statSkillInsightProficient.IsChecked == true)
				result |= Skills.insight;
			if (statSkillIntimidationProficient.IsChecked == true)
				result |= Skills.intimidation;
			if (statSkillInvestigationProficient.IsChecked == true)
				result |= Skills.investigation;
			if (statSkillMedicineProficient.IsChecked == true)
				result |= Skills.medicine;
			if (statSkillNatureProficient.IsChecked == true)
				result |= Skills.nature;
			if (statSkillPerceptionProficient.IsChecked == true)
				result |= Skills.perception;
			if (statSkillPerformanceProficient.IsChecked == true)
				result |= Skills.performance;
			if (statSkillPersuasionProficient.IsChecked == true)
				result |= Skills.persuasion;
			if (statSkillReligionProficient.IsChecked == true)
				result |= Skills.religion;
			if (statSkillSlightOfHandProficient.IsChecked == true)
				result |= Skills.slightOfHand;
			if (statSkillStealthProficient.IsChecked == true)
				result |= Skills.stealth;
			if (statSkillSurvivalProficient.IsChecked == true)
				result |= Skills.survival;
			return result;
		}
		Ability GetSavingThrowProficiency()
		{
			Ability result = Ability.none;
			if (statSavingCharismaProficient.IsChecked == true)
				result |= Ability.charisma;
			if (statSavingConstitutionProficient.IsChecked == true)
				result |= Ability.constitution;
			if (statSavingIntelligenceProficient.IsChecked == true)
				result |= Ability.intelligence;
			if (statSavingWisdomProficient.IsChecked == true)
				result |= Ability.wisdom;
			if (statSavingDexterityProficient.IsChecked == true)
				result |= Ability.dexterity;
			if (statSavingStrengthProficient.IsChecked == true)
				result |= Ability.strength;
			return result;
		}
		
		Ability GetSpellCastingAbility()
		{
			Ability result = Ability.none;
			// TODO: Implement this.
			return result;
		}

		void SetSkillProficiency(Skills skills)
		{
			statSkillAcrobaticsProficient.IsChecked = (skills & Skills.acrobatics) == Skills.acrobatics;
			statSkillAnimalHandlingProficient.IsChecked = (skills & Skills.animalHandling) == Skills.animalHandling;
			statSkillAthleticsProficient.IsChecked = (skills & Skills.athletics) == Skills.athletics;
			statSkillDeceptionProficient.IsChecked = (skills & Skills.deception) == Skills.deception;
			statSkillHistoryProficient.IsChecked = (skills & Skills.history) == Skills.history;
			statSkillInsightProficient.IsChecked = (skills & Skills.insight) == Skills.insight;
			statSkillIntimidationProficient.IsChecked = (skills & Skills.intimidation) == Skills.intimidation;
			statSkillInvestigationProficient.IsChecked = (skills & Skills.investigation) == Skills.investigation;
			statSkillMedicineProficient.IsChecked = (skills & Skills.medicine) == Skills.medicine;
			statSkillNatureProficient.IsChecked = (skills & Skills.nature) == Skills.nature;
			statSkillPerceptionProficient.IsChecked = (skills & Skills.perception) == Skills.perception;
			statSkillPerformanceProficient.IsChecked = (skills & Skills.performance) == Skills.performance;
			statSkillPersuasionProficient.IsChecked = (skills & Skills.persuasion) == Skills.persuasion;
			statSkillReligionProficient.IsChecked = (skills & Skills.religion) == Skills.religion;
			statSkillSlightOfHandProficient.IsChecked = (skills & Skills.slightOfHand) == Skills.slightOfHand;
			statSkillStealthProficient.IsChecked = (skills & Skills.stealth) == Skills.stealth;
			statSkillSurvivalProficient.IsChecked = (skills & Skills.survival) == Skills.survival;
		}

		void SetSavingThrowProficiency(Ability ability)
		{
			statSavingCharismaProficient.IsChecked = (ability & Ability.charisma) == Ability.charisma;
			statSavingConstitutionProficient.IsChecked = (ability & Ability.constitution) == Ability.constitution;
			statSavingDexterityProficient.IsChecked = (ability & Ability.dexterity) == Ability.dexterity;
			statSavingIntelligenceProficient.IsChecked = (ability & Ability.intelligence) == Ability.intelligence;
			statSavingStrengthProficient.IsChecked = (ability & Ability.strength) == Ability.strength;
			statSavingWisdomProficient.IsChecked = (ability & Ability.wisdom) == Ability.wisdom;
		}

		string PlusModifier(double value)
		{
			if (value > 0)
				return "+";
			return "";
		}

		void SetSkillText(StatBox statBox, double value)
		{
			string statStr = SetStatForeground(statBox, value);
			statBox.Text = statStr;
		}

		static SolidColorBrush GetTextBrush(double value)
		{
			// TODO: Turn into read-only fields and reference them here:
			if (value > 0)
				return new SolidColorBrush(Color.FromRgb(0, 115, 192));
			else if (value < 0)
				return new SolidColorBrush(Color.FromRgb(192, 0, 0));
			else
				return new SolidColorBrush(Color.FromRgb(173, 133, 87));
		}

		private static string SetStatForeground(Control control, double value)
		{
			string statStr = value.ToString();
			if (value > 0)
				statStr = '+' + statStr;
			control.Foreground = GetTextBrush(value);
			return statStr;
		}

		private static string SetTextBlockForeground(TextBlock textBlock, double value)
		{
			string statStr = value.ToString();
			if (value > 0)
				statStr = '+' + statStr;
			textBlock.Foreground = GetTextBrush(value);
			return statStr;
		}

		void SetCalculatedFields(Character character)
		{
			SetSkillText(statSkillAcrobatics, character.skillModAcrobatics);
			SetSkillText(statSkillAnimalHandling, character.skillModAnimalHandling);
			SetSkillText(statSkillArcana, character.skillModArcana);
			SetSkillText(statSkillAthletics, character.skillModAthletics);
			SetSkillText(statSkillDeception, character.skillModDeception);
			SetSkillText(statSkillHistory, character.skillModHistory);
			SetSkillText(statSkillInsight, character.skillModInsight);
			SetSkillText(statSkillIntimidation, character.skillModIntimidation);
			SetSkillText(statSkillInvestigation, character.skillModInvestigation);
			SetSkillText(statSkillMedicine, character.skillModMedicine);
			SetSkillText(statSkillNature, character.skillModNature);
			SetSkillText(statSkillPerception, character.skillModPerception);
			SetSkillText(statSkillPerformance, character.skillModPerformance);
			SetSkillText(statSkillPersuasion, character.skillModPersuasion);
			SetSkillText(statSkillReligion, character.skillModReligion);
			SetSkillText(statSkillSlightOfHand, character.skillModSlightOfHand);
			SetSkillText(statSkillStealth, character.skillModStealth);
			SetSkillText(statSkillSurvival, character.skillModSurvival);
			SetTextBlockText(calcStatStrengthModifier2, character.strengthMod);
			SetTextBlockText(calcStatDexterityModifier2, character.dexterityMod);
			SetTextBlockText(calcStatConstitutionModifier2, character.constitutionMod);
			SetTextBlockText(calcStatIntelligenceModifier2, character.intelligenceMod);
			SetTextBlockText(calcStatWisdomModifier2, character.wisdomMod);
			SetTextBlockText(calcStatCharismaModifier2, character.charismaMod);
			SetTextBlockText(calcStatStrengthModifier, character.strengthMod);
			SetTextBlockText(calcStatDexterityModifier, character.dexterityMod);
			SetTextBlockText(calcStatConstitutionModifier, character.constitutionMod);
			SetTextBlockText(calcStatIntelligenceModifier, character.intelligenceMod);
			SetTextBlockText(calcStatWisdomModifier, character.wisdomMod);
			SetTextBlockText(calcStatCharismaModifier, character.charismaMod);
			//SetSkillText(stat, character.skillModSurvival);
		}
		void SetTextBlockText(TextBlock textBlock, double value)
		{
			textBlock.Text = SetTextBlockForeground(textBlock, value);
		}

		void SetName(StatBox statBox, string name)
		{
			statBox.Text = StrUtils.GetFirstName(name);
			statBox.IsEnabled = false;
		}
		
		void SetSpellCastingAbility(Ability ability)
		{
			// TODO: Implement this.
		}

		public void SetFromCharacter(Character character)
		{
			changingInternally = true;
			try
			{
				playerID = character.playerID;
				headshotIndex = character.headshotIndex;
				statGoldPieces.Text = character.goldPieces.ToString();
				statGoldPieces3.Text = statGoldPieces.Text;
				//character.activeConditions = 
				//character.advantages = ;
				statAlignment.Text = character.alignment;
				statArmorClass.Text = character.baseArmorClass.ToString();
				//character.blindsightRadius = 
				//character.burrowingSpeed = 
				statCharisma.Text = character.Charisma.ToString();
				statCharisma2.Text = character.Charisma.ToString();
				//character.climbingSpeed = 
				// character.conditionImmunities = 
				statConstitution.Text = character.Constitution.ToString();
				statConstitution2.Text = character.Constitution.ToString();
				//character.creatureSize = 
				//character.cursesAndBlessings = 
				//character.damageImmunities = 
				//character.damageResistance = 
				//character.damageVulnerability = 
				//character.darkvisionRadius = 
				statDeathSaveSkull1.IsChecked = character.deathSaveDeath1;
				statDeathSaveSkull2.IsChecked = character.deathSaveDeath2;
				statDeathSaveSkull3.IsChecked = character.deathSaveDeath3;
				statDeathSaveHeart1.IsChecked = character.deathSaveLife1;
				statDeathSaveHeart2.IsChecked = character.deathSaveLife2;
				statDeathSaveHeart3.IsChecked = character.deathSaveLife3;
				statDexterity.Text = character.Dexterity.ToString();
				statDexterity2.Text = statDexterity.Text;
				//character.disadvantages = 
				//character.equipment = 
				statExperiencePoints.Text = character.experiencePoints.ToString();
				//character.flyingSpeed = 
				statGoldPieces.Text = character.goldPieces.ToString();
				statHitPoints.Text = character.hitPoints.ToString();
				statInitiative.Text = PlusModifier(character.initiative) + character.initiative.ToString();
				statInspiration.Text = character.inspiration.ToString();
				statIntelligence.Text = character.Intelligence.ToString();
				statIntelligence2.Text = statIntelligence.Text;
				// character.baseIntelligence = statIntelligence.ToInt();
				// character.kind = 
				// character.languagesSpoken = 
				// character.languagesUnderstood = 

				//statLevel.Text = character.level.ToString();  read-only

				statLoad.Text = character.load.ToString();
				//character.maxHitPoints = 
				SetName(statName, character.name);
				SetName(statName2, character.name);
				SetName(statName3, character.name);

				//character.offTurnActions = 
				//character.onTurnActions = 
				statProficiencyBonus.Text = PlusModifier(character.proficiencyBonus) + character.proficiencyBonus.ToString();
				SetSkillProficiency(character.proficientSkills | character.doubleProficiency);
				statRaceClass.Text = character.raceClass;
				//character.remainingHitDice = 
				SetSavingThrowProficiency(character.savingThrowProficiency);
				SetSpellCastingAbility(character.spellCastingAbility);
				//character.senses = 
				statSpeed.Text = character.baseWalkingSpeed.ToString();
				statStrength.Text = character.Strength.ToString();
				statStrength2.Text = statStrength.Text;
				//character.swimmingSpeed = 
				//character.telepathyRadius = 
				//character.tempAcrobaticsMod = 
				//character.tempAnimalHandlingMod = 
				//character.tempArcanaMod = 
				//character.tempAthleticsMod = 
				//character.tempDeceptionMod = 
				//character.tempHistoryMod =
				statTempHitPoints.Text = character.tempHitPoints.ToString();
				//character.tempInsightMod = 
				//character.tempIntimidationMod = 
				//character.tempInvestigationMod =
				//character.tempMedicineMod = 
				//character.tempNatureMod = 
				//character.tempPerceptionMod =
				//character.tempPerformanceMod = 
				//character.tempPersuasionMod = 
				//character.tempReligionMod = 
				//character.tempSavingThrowModCharisma = 
				//character.tempSavingThrowModConstitution = 
				//character.tempSavingThrowModDexterity = 
				//character.tempSavingThrowModIntelligence = 
				//character.tempSavingThrowModStrength = 
				//character.tempSavingThrowModWisdom = 
				//character.tempSlightOfHandMod = 
				//character.tempStealthMod = 
				//character.tempSurvivalMod = 
				statHitDice.Text = character.totalHitDice;
				//character.tremorSenseRadius =
				//character.truesightRadius = 
				statWeight.Text = character.weight.ToString();
				statWisdom.Text = character.Wisdom.ToString();
				statWisdom2.Text = statWisdom.Text;
				SetCalculatedFields(character);
			}
			finally
			{
				changingInternally = false;
			}
		}
		

		public string GetCharacter()
		{
			Character character = new Character();
			character.playerID = playerID;
			character.headshotIndex = headshotIndex;
			//character.activeConditions = 
			//character.advantages = ;
			character.alignment = statAlignment.Text;
			character.baseArmorClass = statArmorClass.ToInt();
			//character.blindsightRadius = 
			//character.burrowingSpeed = 
			character.baseCharisma = statCharisma.ToInt();
			//character.climbingSpeed = 
			// character.conditionImmunities = 
			character.conditionImmunities = Conditions.None;  // Allow editing of this prop?
			character.baseConstitution = statConstitution.ToInt();
			//character.creatureSize = 
			//character.cursesAndBlessings = 
			//character.damageImmunities = 
			//character.damageResistance = 
			//character.damageVulnerability = 
			//character.darkvisionRadius = 
			character.deathSaveDeath1 = statDeathSaveSkull1.IsChecked == true;
			character.deathSaveDeath2 = statDeathSaveSkull2.IsChecked == true;
			character.deathSaveDeath3 = statDeathSaveSkull3.IsChecked == true;
			character.deathSaveLife1 = statDeathSaveHeart1.IsChecked == true;
			character.deathSaveLife2 = statDeathSaveHeart2.IsChecked == true;
			character.deathSaveLife3 = statDeathSaveHeart3.IsChecked == true;
			character.baseDexterity = statDexterity.ToInt();
			//character.disadvantages = 
			//character.equipment = 
			character.experiencePoints = statExperiencePoints.ToInt();
			//character.flyingSpeed = 
			character.goldPieces = statGoldPieces.ToDouble();
			character.hitPoints = statHitPoints.ToInt();
			character.initiative = statInitiative.ToInt();
			character.inspiration = statInspiration.Text;
			character.baseIntelligence = statIntelligence.ToInt();
			// character.kind = 
			character.kind = CreatureKinds.Humanoids;   // Allow editing of this prop?
			// character.languagesSpoken = 
			// character.languagesUnderstood = 
			character.languagesSpoken = Languages.Common;
			character.languagesUnderstood = Languages.Common;
			//character.level = statLevel.ToInt();  // read only
			character.load = statLoad.ToInt();
			//character.maxHitPoints = 
			character.name = statName.Text;
			//character.offTurnActions = 
			//character.onTurnActions = 
			character.proficiencyBonus = statProficiencyBonus.ToInt();
			character.proficientSkills = GetProficiencySkills();
			//character.raceClass = statRaceClass.Text;  // read only
			//character.remainingHitDice = 
			character.savingThrowProficiency = GetSavingThrowProficiency();
			character.spellCastingAbility = GetSpellCastingAbility();
			//character.senses = 
			character.baseWalkingSpeed = statSpeed.ToDouble();
			character.baseStrength = statStrength.ToInt();
			//character.swimmingSpeed = 
			//character.telepathyRadius = 
			//character.tempAcrobaticsMod = 
			//character.tempAnimalHandlingMod = 
			//character.tempArcanaMod = 
			//character.tempAthleticsMod = 
			//character.tempDeceptionMod = 
			//character.tempHistoryMod =
			character.tempHitPoints = statTempHitPoints.ToDouble();
			//character.tempInsightMod = 
			//character.tempIntimidationMod = 
			//character.tempInvestigationMod =
			//character.tempMedicineMod = 
			//character.tempNatureMod = 
			//character.tempPerceptionMod =
			//character.tempPerformanceMod = 
			//character.tempPersuasionMod = 
			//character.tempReligionMod = 
			//character.tempSavingThrowModCharisma = 
			//character.tempSavingThrowModConstitution = 
			//character.tempSavingThrowModDexterity = 
			//character.tempSavingThrowModIntelligence = 
			//character.tempSavingThrowModStrength = 
			//character.tempSavingThrowModWisdom = 
			//character.tempSlightOfHandMod = 
			//character.tempStealthMod = 
			//character.tempSurvivalMod = 
			character.totalHitDice = statHitDice.Text;
			//character.tremorSenseRadius =
			//character.truesightRadius = 
			character.weight = statWeight.ToDouble();
			character.baseWisdom = statWisdom.ToInt();
			return character.ToJson();
		}

		void HookChangedEvents(Visual visual)
		{
			int childCount = VisualTreeHelper.GetChildrenCount(visual);

			for (int i = 0; i <= childCount - 1; i++)
			{
				Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);

				switch (child)
				{
					case StatBox statBox:
						statBox.StatChanged += AnyStatChanged;
						statBox.MouseDown += StatBox_MouseDown;
						break;
					case CheckBox checkBox:
						checkBox.Checked += AnyStatChanged;
						checkBox.Unchecked += AnyStatChanged;
						break;
				}

				if (VisualTreeHelper.GetChildrenCount(child) > 0)
					HookChangedEvents(child);
			}
		}

		private void StatBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			HookChangedEvents(this);
		}

		private void PageSkillsBack_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageBackgroundClicked();
		}

		private void PageEquipment_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageBackgroundClicked();
		}

		private void PageMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageBackgroundClicked();
		}

		private void AcrobaticsSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.acrobatics);
		}
		private void AnimalHandlingSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.animalHandling);
		}

		private void ArcanaSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.arcana);
		}
		private void AthleticsSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.athletics);

		}
		private void DeceptionSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.deception);
		}

		private void HistorySkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.history);
		}
		private void InsightSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.insight);
		}
		private void IntimidationSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.intimidation);
		}
		private void InvestigationSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.investigation);
		}
		private void MedicineSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.medicine);
		}
		private void NatureSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.nature);
		}
		private void PerceptionSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.perception);
		}
		private void PerformanceSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.performance);
		}
		private void PersuasionSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.persuasion);
		}
		private void ReligionSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.religion);
		}
		private void SlightOfHandSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.slightOfHand);
		}
		private void StealthSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.stealth);
		}
		private void SurvivalSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.survival);
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DexteritySavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.dexterity);
		}
		private void StrengthSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.strength);
		}
		private void ConstitutionSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.constitution);
		}
		private void IntelligenceSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.intelligence);
		}
		private void WisdomSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.wisdom);
		}
		private void CharismaSavingThrow_Click(object sender, RoutedEventArgs e)
		{
			OnSavingThrowRequested(Ability.charisma);
		}

		private void StrengthSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.strength);
		}

		private void ConstitutionSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.constitution);
		}

		private void WisdomSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.wisdom);
		}

		private void DexteritySkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.dexterity);
		}

		private void IntelligenceSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.intelligence);
		}

		private void CharismaSkillCheck_Click(object sender, RoutedEventArgs e)
		{
			OnSkillCheckRequested(Skills.charisma);
		}

		private void SkillCheckContextMenu_Drop(object sender, DragEventArgs e)
		{

		}

		private void AbilityCheckContextMenu_Drop(object sender, DragEventArgs e)
		{

		}

		private void SavingThrowContextMenu_Drop(object sender, DragEventArgs e)
		{

		}
	}
}
