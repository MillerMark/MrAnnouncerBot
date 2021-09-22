//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using ObsControl;

namespace DHDM
{
	public class ContestManager
	{
		const string STR_SceneSkillsAbove = "DH.Contest.Skills.Above";
		const string STR_SceneSkillsBelow = "DH.Contest.Skills.Below";
		const string SCENE_Contest = "DH.Contest";

		// TODO: Delete ActiveSection.
		public ContestSection ActiveSection { get; set; } = ContestSection.Top;

		public ContestManager(DndObsManager obsManager)
		{
			dndObsManager = obsManager;
			InitializeWidths();
			AddSourceNames();
		}

		Dictionary<Skills, int> sceneWidths = new Dictionary<Skills, int>();

		void AddSceneWidth(Skills skill, int value)
		{
			sceneWidths[skill] = value;
		}

		public List<string> SourceSkillNames { get; set; }

		void AddSourceNames()
		{
			SourceSkillNames = new List<string>();
			Skills[] skills = (Skills[])Enum.GetValues(typeof(Skills));
			foreach (Skills item in skills)
				if (item != Skills.none)
					SourceSkillNames.Add(GetSourceName(item));
		}

		private static string GetSourceName(Skills item)
		{
			return $"DH.{GetSkillName(item)}";
		}

		private static string GetSkillName(Skills item)
		{
			return item.ToString().InitialCap();
		}

		void InitializeWidths()
		{
			AddSceneWidth(Skills.strength, 894);
			AddSceneWidth(Skills.wisdom, 976);
			AddSceneWidth(Skills.charisma, 1040);
			AddSceneWidth(Skills.dexterity, 1036);
			AddSceneWidth(Skills.intelligence, 1149);
			AddSceneWidth(Skills.constitution, 1166);
			AddSceneWidth(Skills.acrobatics, 1108);
			AddSceneWidth(Skills.animalHandling, 1029);
			AddSceneWidth(Skills.arcana, 940);
			AddSceneWidth(Skills.athletics, 1017);
			AddSceneWidth(Skills.deception, 1056);
			AddSceneWidth(Skills.history, 952);
			AddSceneWidth(Skills.insight, 940);
			AddSceneWidth(Skills.intimidation, 1154);
			AddSceneWidth(Skills.investigation, 1197);
			AddSceneWidth(Skills.medicine, 1028);
			AddSceneWidth(Skills.nature, 925);
			AddSceneWidth(Skills.perception, 1106);
			AddSceneWidth(Skills.performance, 1167);
			AddSceneWidth(Skills.persuasion, 1104);
			AddSceneWidth(Skills.religion, 995);
			AddSceneWidth(Skills.sleightOfHand, 953);
			AddSceneWidth(Skills.stealth, 947);
			AddSceneWidth(Skills.survival, 989);
			AddSceneWidth(Skills.randomShit, 986);
		}

		public void Invalidate()
		{

		}
		ContestDto contestDto;
		
		public ContestDto ContestDto
		{
			get
			{
				if (contestDto == null)
					contestDto = new ContestDto();
				return contestDto;
			}
			set => contestDto = value;
		}
		
		public DndObsManager dndObsManager { get; set; }

		public void AddNpc()
		{
			decimal value = DigitManager.GetValue("contest");
			DigitManager.ClearOnNextDigit("contest");
			InGameCreature inGameCreature = AllInGameCreatures.GetByIndex((int)value);
			if (inGameCreature != null)
			{
				double hueShift = CardHandManager.GetHueShift(-inGameCreature.Index);
				AddContestant(DndUtils.GetFirstName(inGameCreature.Name), -inGameCreature.Index, hueShift);
				Update();
			}
			else
				HubtasticBaseStation.ShowValidationIssue(100, ValidationAction.Stop, $"Creature #{value} not found.");
		}

		private void Update()
		{
			ContestCommand("Update");
		}

		private void ContestCommand(string command)
		{
			ContestDto.Command = command;
			HubtasticBaseStation.ContestCommand(Newtonsoft.Json.JsonConvert.SerializeObject(contestDto));
		}

		public void Backup()
		{
			if (!active)
				return;

			if (ContestDto.ActiveSection == ContestSection.Top)
				ContestDto.TopContestants.DeleteLastContestant();
			else
				ContestDto.BottomContestants.DeleteLastContestant();
			Update();
		}

		bool active;

		void HideAllSourcesBySceneName(string sceneName)
		{
			foreach (string sourceSkillName in SourceSkillNames)
			{
				ObsManager.SetSourceVisibility(sourceSkillName, sceneName, false);
			}
		}

		void HideAllSources()
		{
			HideAllSourcesBySceneName(STR_SceneSkillsAbove);
			HideAllSourcesBySceneName(STR_SceneSkillsBelow);
		}

		void CleanUpContestScene()
		{
			ObsManager.SetSourceVisibility("ContestLoop", SCENE_Contest, false);
			HideAllSources();
		}

		void ActivateIfNeeded()
		{
			if (active)
				return;
			HideAllSources();
			DndObsManager.SetSourceVisibility("DH.ContestIntro", SCENE_Contest, true, 5 * 33.3 / 1000);
			DndObsManager.SetSourceVisibility("DH.ContestIntro", SCENE_Contest, false, 10);
			DndObsManager.SetSourceVisibility("ContestLoop", SCENE_Contest, true, 4);

			//ObsManager.SetSourceVisibility("ContestLoop", SCENE_Contest, false, 15);    // For diagnostics.

			dndObsManager.PlayScene(SCENE_Contest);
			HubtasticBaseStation.PlaySound("Contest/Contest[6]");
			
			active = true;
		}

		public void SwitchToTop()
		{
			ActivateIfNeeded();
			if (ContestDto.ActiveSection != ContestSection.Top)
			{
				ContestDto.ActiveSection = ContestSection.Top;
				Update();
			}
		}

		public void SwitchToBottom()
		{
			ActivateIfNeeded();
			if (ContestDto.ActiveSection != ContestSection.Bottom)
			{
				ContestDto.ActiveSection = ContestSection.Bottom;
				Update();
			}
		}

		double GetTextWidth(Skills skill)
		{
			if (sceneWidths.ContainsKey(skill))
				return sceneWidths[skill];

			System.Diagnostics.Debugger.Break();
			return ContestGroup.INT_DefaultWidth;
		}

		public void AddSkill(Skills skill)
		{
			double delaySeconds = 0;
			if (!active)
				delaySeconds = 3.5;

			ActivateIfNeeded();

			string sceneName;
			if (ContestDto.ActiveSection == ContestSection.Top)
			{
				ContestDto.TopContestants.Skill = skill;
				ContestDto.TopContestants.Width = GetTextWidth(skill);
				sceneName = STR_SceneSkillsAbove;
			}
			else
			{
				ContestDto.BottomContestants.Skill = skill;
				ContestDto.BottomContestants.Width = GetTextWidth(skill);
				sceneName = STR_SceneSkillsBelow;
			}

			HideAllSourcesBySceneName(sceneName);
			HubtasticBaseStation.PlaySound($"Contest/{GetSkillName(skill)}[3]");
			DndObsManager.SetSourceVisibility(GetSourceName(skill), sceneName, true, delaySeconds);
			Update();
		}

		public void AddPlayer(Character player)
		{
			ActivateIfNeeded();
			AddContestant(player.firstName, player.playerID, player.hueShift);
		}

		private void AddContestant(string firstName, int creatureId, double hueShift)
		{
			Contestant contestant = new Contestant() { Name = firstName, HueShift = hueShift, CreatureId = creatureId };
			if (ContestDto.ActiveSection == ContestSection.Top)
				ContestDto.TopContestants.AddContestant(contestant);
			else
				ContestDto.BottomContestants.AddContestant(contestant);
			Update();
		}

		public void Clean()
		{
			active = false;
			ContestDto = null;
			ContestCommand("Clean");
			CleanUpContestScene();
		}
	}
}
