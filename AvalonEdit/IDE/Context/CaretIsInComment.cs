//#define profiling
using System;
using System.Linq;
using DndCore;
using CodingSeb.ExpressionEvaluator;
using ICSharpCode.AvalonEdit.Editing;

namespace AvalonEdit
{
	public class CaretIsInComment : BaseContext
	{
		protected override bool ContextIsSatisfied(ExpressionEvaluator evaluator, TextArea textArea)
		{
			textArea.GetSelectionBounds(out int startLine, out int endLine);

			for (int i = startLine; i <= endLine; i++)
			{
				ICSharpCode.AvalonEdit.Document.DocumentLine line = textArea.Document.GetLineByNumber(i);
				string lineText = textArea.Document.GetText(line.Offset, line.Length);
				if (!string.IsNullOrWhiteSpace(lineText) && !lineText.StartsWith("//"))
					return false;
			}

			return true;
		}

		public CaretIsInComment()
		{

		}
	}
}
