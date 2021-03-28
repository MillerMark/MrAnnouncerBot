﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Asks the specified multiple choice question.")]
	[Param(1, typeof(string), "question", "The question to ask.", ParameterIs.Required)]
	[Param(2, typeof(string), "answer1", "The first answer to the question. Answers should start with the value to return when that answer is selected, a colon, and then the answer (e.g., \"1: Yes\"). Return values need not be sequential.", ParameterIs.Required)]
	[Param(3, typeof(string), "answer2", "The second answer to the question.", ParameterIs.Required)]
	[Param(4, typeof(string), "answer3", "The third answer to the question.", ParameterIs.Optional)]
	[Param(5, typeof(string), "answer4", "The fourth answer to the question.", ParameterIs.Optional)]
	[Param(6, typeof(string), "answer5", "The fifth answer to the question.", ParameterIs.Optional)]
	[Param(7, typeof(string), "answer6", "The sixth answer to the question.", ParameterIs.Optional)]
	[Param(8, typeof(string), "answer7", "The seventh answer to the question.", ParameterIs.Optional)]
	[Param(9, typeof(string), "answer8", "The eighth answer to the question.", ParameterIs.Optional)]
	[Param(10, typeof(string), "answer9", "The ninth answer to the question.", ParameterIs.Optional)]
	public class AskFunction : DndFunction
	{
		public static event AskEventHandler AskQuestion;
		public static void OnAskQuestion(object sender, AskEventArgs ea)
		{
			AskQuestion?.Invoke(sender, ea);
		}
		//public async Task OnAskQuestion(object sender, AskEventArgs ea)
		//{
		//	AskEventHandler handler = AskQuestion;

		//	if (handler == null)
		//		return;

		//	Delegate[] invocationList = handler.GetInvocationList();
		//	Task[] handlerTasks = new Task[invocationList.Length];

		//	for (int i = 0; i < invocationList.Length; i++)
		//	{
		//		handlerTasks[i] = ((AskEventHandler)invocationList[i])(sender, ea);
		//	}

		//	await Task.WhenAll(handlerTasks);
		//}


		public override string Name { get; set; } = "Ask";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			string question = Expressions.GetStr(args[0], player, target, spell);
			List<string> answers = args.Skip(1).ToList();
			AskEventArgs ea = new AskEventArgs(question, answers);

			OnAskQuestion(player, ea);
			return ea.Result;
		}
	}
}
