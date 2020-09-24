using System;
using System.Linq;

namespace DHDM
{
	public class QuestionAnswerMapEventArgs : EventArgs
	{
		public QuestionAnswerMap QuestionAnswerMap { get; set; }
		public QuestionAnswerMapEventArgs(QuestionAnswerMap questionAnswerMap)
		{
			QuestionAnswerMap = questionAnswerMap;
		}
		public QuestionAnswerMapEventArgs()
		{

		}
	}
}
