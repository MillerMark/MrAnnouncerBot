//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class AnswerEntry
	{
		public bool IsSelected { get; set; }
		public int Index { get; set; }
		public int Value { get; set; }
		public string AnswerText { get; set; }
		public AnswerEntry(int index, int value, string answerText)
		{
			Index = index;
			Value = value;
			AnswerText = answerText;
		}
		public static AnswerEntry FromAnswer(string answerStr, int index)
		{
			AnswerEntry result = new AnswerEntry();
			result.Index = index;
			string workStr = answerStr.Trim(new char[] { '"', ' ' });
			int colonPos = workStr.IndexOf(':');
			if (colonPos > 0)
			{
				string valueStr = workStr.EverythingBefore(":");
				result.AnswerText = workStr.EverythingAfter(":").Trim();
				if (int.TryParse(valueStr, out int value))
					result.Value = value;
			}

			return result;
		}
		public AnswerEntry()
		{

		}
	}
}