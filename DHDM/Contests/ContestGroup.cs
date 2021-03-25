//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class ContestGroup
	{
		public const int INT_DefaultWidth = 1000;
		Skills skill;
		public Skills Skill
		{
			get => skill; 
			set
			{
				if (skill == value)
					return;
				skill = value;
				UpdateContestantMods();
			}
		}
		public double Width { get; set; } = INT_DefaultWidth;
		public List<Contestant> Contestants { get; set; }

		public ContestGroup()
		{
			Contestants = new List<Contestant>();
		}

		void UpdateContestantMods()
		{
			foreach (Contestant contestant in Contestants)
				UpdateMod(contestant);
		}

		public void AddContestant(Contestant contestant)
		{
			Contestant existingContestant = Contestants.FirstOrDefault(x => x.Name == contestant.Name);
			if (existingContestant != null)
				return;
			UpdateMod(contestant);

			Contestants.Add(contestant);
		}

		private void UpdateMod(Contestant contestant)
		{
			Creature creature = DndUtils.GetCreatureById(contestant.CreatureId);
			if (creature != null)
				contestant.Mod = creature.GetSkillCheckMod(Skill);
		}

		public void DeleteLastContestant()
		{
			if (Contestants.Count == 0)
				return;
			Contestants.RemoveAt(Contestants.Count - 1);
		}
	}
}
