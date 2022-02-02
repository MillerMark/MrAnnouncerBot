//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public class InString : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return textArea.IsInString();
		}
		public InString()
		{

		}
	}
}
