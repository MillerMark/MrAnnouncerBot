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

namespace DHDM
{
	public enum ScrollPage
	{
		deEmphasis = 0,
		main = 1,
		skills = 2,
		equipment = 3
	}

	/// <summary>
	/// Interaction logic for CharacterSheets.xaml
	/// </summary>
	public partial class CharacterSheets : UserControl
	{
		
		public ScrollPage Page
		{
			get { return scrollPage; }
			set
			{
				scrollPage = value;
			}
		}
		

		ScrollPage scrollPage;
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

		protected virtual void OnCharacterChanged()
		{
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
			if (scrollPage == newPage)
				return;
			scrollPage = newPage;
			FocusHelper.ClearActiveStatBoxes();

			RoutedEventArgs previewEventArgs = new RoutedEventArgs(PreviewPageChangedEvent);
			RaiseEvent(previewEventArgs);
			if (previewEventArgs.Handled)
				return;
			RoutedEventArgs eventArgs = new RoutedEventArgs(PageChangedEvent);
			RaiseEvent(eventArgs);
		}

		public CharacterSheets()
		{
			InitializeComponent();
		}

		private void PageSkills_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.skills);
		}

		private void PageEquipment_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPageChanged(ScrollPage.equipment);
		}

		private void PageMain_Activated(object sender, RoutedEventArgs e)
		{
			OnPageChanged(ScrollPage.main);
		}

		private void PageMain_MouseDown(object sender, MouseButtonEventArgs e)
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

		public void SetFromCharacter(Character character)
		{
			//character.activeConditions = 
			//character.advantages = ;
			statAlignment.Text = character.alignment;
			statArmorClass.Text = character.armorClass.ToString();
			//character.blindsightRadius = 
			//character.burrowingSpeed = 
			statCharisma.Text = character.charisma.ToString();
			statCharisma2.Text = character.charisma.ToString();
			//character.climbingSpeed = 
			// character.conditionImmunities = 
			statConstitution.Text = character.constitution.ToString();
			statConstitution2.Text = character.constitution.ToString();
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
			statDexterity.Text = character.dexterity.ToString();
			//character.disadvantages = 
			//character.equipment = 
			statExperiencePoints.Text = character.experiencePoints.ToString();
			//character.flyingSpeed = 
			statGoldPieces.Text = character.goldPieces.ToString();
			statHitPoints.Text = character.hitPoints.ToString();
			statInitiative.Text = character.initiative.ToString();
			statInspiration.Text = character.inspiration.ToString();
			statIntelligence.Text = character.intelligence.ToString();
			statIntelligence2.Text = statIntelligence.Text;
			character.intelligence = statIntelligence.ToInt();
			// character.kind = 
			// character.languagesSpoken = 
			// character.languagesUnderstood = 
			statLevel.Text = character.level.ToString();
			statLoad.Text = character.load.ToString();
			//character.maxHitPoints = 
			statName.Text = character.name;
			statName2.Text = character.name;
			//character.offTurnActions = 
			//character.onTurnActions = 
			statProficiencyBonus.Text = character.proficiencyBonus.ToString();
			SetSkillProficiency(character.proficientSkills);
			statRaceClass.Text = character.raceClass;
			//character.remainingHitDice = 
			SetSavingThrowProficiency(character.savingThrowProficiency);
			//character.senses = 
			statSpeed.Text = character.speed.ToString();
			statStrength.Text = character.strength.ToString();
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
			statWisdom.Text = character.wisdom.ToString();
		}

		public string GetCharacter()
		{
			Character character = new Character();
			//character.activeConditions = 
			//character.advantages = ;
			character.alignment = statAlignment.Text;
			character.armorClass = statArmorClass.ToInt();
			//character.blindsightRadius = 
			//character.burrowingSpeed = 
			character.charisma = statCharisma.ToInt();
			//character.climbingSpeed = 
			// character.conditionImmunities = 
			character.conditionImmunities = Conditions.none;  // Allow editing of this prop?
			character.constitution = statConstitution.ToInt();
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
			character.deathSaveLife2 = statDeathSaveSkull2.IsChecked == true;
			character.deathSaveLife3 = statDeathSaveHeart3.IsChecked == true;
			character.dexterity = statDexterity.ToInt();
			//character.disadvantages = 
			//character.equipment = 
			character.experiencePoints = statExperiencePoints.ToInt();
			//character.flyingSpeed = 
			character.goldPieces = statGoldPieces.ToDouble();
			character.hitPoints = statHitPoints.ToInt();
			character.initiative = statInitiative.ToInt();
			character.inspiration = statInspiration.ToInt();
			character.intelligence = statIntelligence.ToInt();
			// character.kind = 
			character.kind = CreatureKinds.Humanoids;   // Allow editing of this prop?
			// character.languagesSpoken = 
			// character.languagesUnderstood = 
			character.languagesSpoken = Languages.Common;
			character.languagesUnderstood = Languages.Common;
			character.level = statLevel.ToInt();
			character.load = statLoad.ToInt();
			//character.maxHitPoints = 
			character.name = statName.Text;
			//character.offTurnActions = 
			//character.onTurnActions = 
			character.proficiencyBonus = statProficiencyBonus.ToInt();
			character.proficientSkills = GetProficiencySkills();
			character.raceClass = statRaceClass.Text;
			//character.remainingHitDice = 
			character.savingThrowProficiency = GetSavingThrowProficiency();
			//character.senses = 
			character.speed = statSpeed.ToDouble();
			character.strength = statStrength.ToInt();
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
			character.wisdom = statWisdom.ToInt();
			return Newtonsoft.Json.JsonConvert.SerializeObject(character);
		}
	}
}
