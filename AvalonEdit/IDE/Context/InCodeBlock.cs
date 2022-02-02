//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public class InCodeBlock : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return !textArea.IsInComment() && !textArea.IsInString();
		}
		public InCodeBlock()
		{

		}
	}
}
