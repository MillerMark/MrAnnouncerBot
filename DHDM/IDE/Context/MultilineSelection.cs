//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using DndCore;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class MultilineSelection : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			return textArea.Selection.EndPosition.Line != textArea.Selection.StartPosition.Line;
		}
		
		public MultilineSelection()
		{

		}
	}
}
