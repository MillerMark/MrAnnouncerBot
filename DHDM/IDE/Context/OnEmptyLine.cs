//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class OnEmptyLine : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return string.IsNullOrWhiteSpace(TextLeftOfTemplate) && string.IsNullOrWhiteSpace(TextRightOfTemplate);
		}
		public OnEmptyLine()
		{

		}
	}
}
