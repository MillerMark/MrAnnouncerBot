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
		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			TextArea textArea = Expressions.GetCustomData<TextArea>(evaluator.Variables);
			if (textArea == null)
				return null;

			return textArea.Selection.EndPosition.Line != textArea.Selection.StartPosition.Line;
		}
		public MultilineSelection()
		{

		}
	}
	public class SelectionIsCommented : BaseContext
	{
		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			TextArea textArea = Expressions.GetCustomData<TextArea>(evaluator.Variables);
			if (textArea == null)
				return null;

			textArea.GetSelectionBounds(out int startLine, out int endLine);

			for (int i = startLine; i <= endLine; i++)
			{
				ICSharpCode.AvalonEdit.Document.DocumentLine line = textArea.Document.GetLineByNumber(i);
				string lineText = textArea.Document.GetText(line.Offset, line.Length);
				if (!lineText.StartsWith("//"))
					return false;
			}

			return true;
		}
		public SelectionIsCommented()
		{

		}
	}
}
