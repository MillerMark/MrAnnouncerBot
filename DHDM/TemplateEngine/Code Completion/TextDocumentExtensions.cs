//#define profiling
using System;
using DndCore;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;

namespace DHDM
{
	public static class TextDocumentExtensions
	{
		public static string GetActiveMethodCall(this TextDocument document, int caretOffset)
		{
			string lineLeft = document.GetLineLeftOf(caretOffset);
			if (lineLeft.Contains("//") || lineLeft.Contains(")") || !lineLeft.Contains("("))
				return null;
			// HACK: It's a hack, kids. But solves large number of use cases with very little code.
			return lineLeft.EverythingBefore("(").Trim();
		}

		public static string GetIdentifierLeftOf(this TextDocument document, int caretOffset)
		{
			string result = string.Empty;
			string lineText = document.GetLineLeftOf(caretOffset);
			int index = lineText.Length - 1;
			while (index >= 0 && CodeUtils.IsIdentifierCharacter(lineText[index]))
			{
				result = lineText[index] + result;
				index--;
			}
			return result;
		}

		public static string GetStringLeftOf(this TextDocument document, int caretOffset)
		{
			string result = string.Empty;
			string lineText = document.GetLineLeftOf(caretOffset);
			int index = lineText.Length - 1;
			while (index >= 0 && (lineText[index] != '"' || (index > 0 && lineText[index - 1] == '\\')))
			{
				result = lineText[index] + result;
				index--;
			}
			return result;
		}

		public static int GetParameterNumberAtPosition(this TextDocument document, int caretOffset)
		{
			// TODO: Make this work with strings containing parens and commas (if you like).
			string lineLeft = document.GetLineLeftOf(caretOffset);
			// HACK: This solves 98% of use cases, but it's very hacky.
			if (!lineLeft.Contains("(") || lineLeft.Contains("//") || lineLeft.Contains(")"))
				return 0;
			string parameters = lineLeft.EverythingAfter("(");
			string[] allParamsLeft = parameters.Split(',');
			return allParamsLeft.Length;
		}

		public static int GetParameterStartOffset(this TextDocument document, int caretOffset)
		{
			string lineLeft = document.GetLineLeftOf(caretOffset);
			// HACK: This solves 98% of use cases, but it's very hacky.
			if (!lineLeft.Contains("(") || lineLeft.Contains("//") || lineLeft.Contains(")"))
				return 0;
			string parameters = lineLeft.EverythingAfter("(");
			string[] allParamsLeft = parameters.Split(',');
			return caretOffset - allParamsLeft[allParamsLeft.Length - 1].Length;
		}

		public static string GetLineLeftOf(this TextDocument document, int caretOffset)
		{
			DocumentLine line = document.GetLineByOffset(caretOffset);
			return document.GetText(line.Offset, caretOffset - line.Offset);
		}

		public static void GetSelectionBounds(this ICSharpCode.AvalonEdit.Editing.TextArea textArea, out int startLine, out int endLine)
		{
			startLine = textArea.Selection.StartPosition.Line;
			ICSharpCode.AvalonEdit.TextViewPosition endPosition = textArea.Selection.EndPosition;
			endLine = endPosition.Line;

			if (endLine < startLine)  // Swap
			{
				int saveEndLine = endLine;
				endLine = startLine;
				startLine = saveEndLine;
				endPosition = textArea.Selection.StartPosition;
			}

			TextDocument document = textArea.Document;

			string lineLeft = null;
			if (endPosition.Location.Line != 0 || endPosition.Location.Column != 0)
				lineLeft = document.GetLineLeftOf(document.GetOffset(endPosition.Location));
			if (string.IsNullOrWhiteSpace(lineLeft))
				endLine--;
		}
	}
}
