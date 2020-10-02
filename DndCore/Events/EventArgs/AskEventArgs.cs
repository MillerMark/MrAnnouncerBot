using System;
using System.Collections.Generic;

namespace DndCore
{
	public class AskEventArgs : EventArgs
	{
		public int Result { get; set; }
		public string Question { get; set; }
		public List<string> Answers { get; set; }
		public List<AnswerEntry> Responses { get; set; }

		public AskEventArgs(string question, List<string> answers)
		{
			Result = 0;
			Answers = answers;
			Question = question;
		}
	}
}
