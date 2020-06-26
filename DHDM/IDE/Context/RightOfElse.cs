//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class RightOfElse : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return TextLeftOfTemplate != null && TextLeftOfTemplate.Trim() == "else";
		}
		public RightOfElse()
		{

		}
	}
}
