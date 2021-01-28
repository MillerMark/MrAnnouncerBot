//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace AvalonEdit
{
	public class InComment : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return textArea.IsInComment();
		}
		public InComment()
		{

		}
	}
}
