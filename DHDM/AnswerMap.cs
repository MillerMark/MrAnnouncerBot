//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class AnswerMap
	{
		public List<AnswerEntry> Answers = new List<AnswerEntry>();
		public string Question { get; set; }
		public int MinAnswers { get; set; }
		public int MaxAnswers { get; set; }
		
		public AnswerMap(string question, List<AnswerEntry> answers, int minAnswers, int maxAnswers)
		{
			Question = question;
			Answers = answers;
			MinAnswers = minAnswers;
			MaxAnswers = maxAnswers;
		}

		public AnswerMap(string question, List<string> answers, int minAnswers, int maxAnswers)
		{
			Question = question;

			for (int i = 0; i < answers.Count; i++)
				Answers.Add(new AnswerEntry(i, i, answers[i]));

			MinAnswers = minAnswers;
			MaxAnswers = maxAnswers;
		}

		public AnswerMap()
		{

		}
	}
}