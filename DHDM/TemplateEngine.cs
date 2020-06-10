//#define profiling
using System;
using System.Linq;
using System.Windows.Controls;
using DndCore;
using ICSharpCode.AvalonEdit.Document;

namespace DHDM
{
	public class TemplateEngine
	{

		public TemplateEngine()
		{

		}

		bool IsLegalWordCharacter(char value)
		{
			return Char.IsLetterOrDigit(value) || value == '/';
		}
		/// <summary>
		/// Gets the word to the left of the specified line and offset.
		/// </summary>
		/// <param name="lineText">The text of the line to examine.</param>
		/// <param name="caretPosition">The offset to start looking at on that line.</param>
		/// <returns></returns>
		string GetWordLeft(string lineText, int caretPosition)
		{

			//` ![Any text you want](0F27055329C09621F45E659C0C8691C2.png;;144,12,1185,565;0.01525,0.01418)

			string result = string.Empty;
			int characterIndex = caretPosition - 1;
			while (characterIndex >= 0 && IsLegalWordCharacter(lineText[characterIndex]))
			{
				result = lineText[characterIndex] + result;
				characterIndex--;
			}

			return result;
		}
		private const string Caret = "«Caret»";
		string GetTemplateExpansion(string wordToLeftOfCaret)
		{
			if (wordToLeftOfCaret == "if")
				return $"if ({Caret})";
			if (wordToLeftOfCaret == "/m")
				return "// Mark was here!!!";
			return null;
		}
		/// <summary>
		/// Expands the template to the left of the caret
		/// </summary>
		/// <param name="textBox"></param>
		/// <returns>Returns true if the template was found and expanded.</returns>
		public bool ExpandTemplate(TextBox textBox)
		{
			int caretIndex = textBox.CaretIndex;
			int selectionLength = textBox.SelectionLength;
			if (selectionLength != 0)
				return false;
			//int selectionStart = textBox.SelectionStart;
			int lineIndex = textBox.GetLineIndexFromCharacterIndex(caretIndex);
			string lineText = textBox.GetLineText(lineIndex);
			int lineStartIndex = textBox.GetCharacterIndexFromLineIndex(lineIndex);  // Gets first character on the line.
			int lineOffset = caretIndex - lineStartIndex;
			string template = GetWordLeft(lineText, lineOffset);
			if (string.IsNullOrWhiteSpace(template))
				return false;
			string expansion = GetTemplateExpansion(template);
			if (string.IsNullOrWhiteSpace(expansion))
				return false;

			int finalInsertionPointOffset = 0;
			if (expansion.Contains(Caret))
			{
				string textAfterCaret = expansion.EverythingAfter(Caret);
				finalInsertionPointOffset = -textAfterCaret.Length;
				expansion = expansion.Replace(Caret, string.Empty);
			}

			int insertionPoint = textBox.CaretIndex - template.Length;

			textBox.Text = textBox.Text.Remove(insertionPoint, template.Length).Insert(insertionPoint, expansion);
			// TODO: Look for <<Caret>> or <<Cursor>>
			textBox.CaretIndex = insertionPoint + expansion.Length + finalInsertionPointOffset;
			return true;
			// Can only do selections from start to end, (but not end to start).
			//
		}

		/// <summary>
		/// Expands the template to the left of the caret
		/// </summary>
		/// <param name="textBox"></param>
		/// <returns>Returns true if the template was found and expanded.</returns>
		public bool ExpandTemplate(ICSharpCode.AvalonEdit.TextEditor textBox)
		{
			int caretIndex = textBox.TextArea.Caret.Offset;
			int selectionLength = textBox.SelectionLength;
			if (selectionLength != 0)
				return false;
			//int selectionStart = textBox.SelectionStart;
			DocumentLine line = textBox.Document.GetLineByOffset(caretIndex);

			//string lineText = line.GetText(textBox.Document);
			string lineText = textBox.Document.GetText(line.Offset, line.Length);
			int lineStartIndex = line.Offset;
			int lineOffset = caretIndex - lineStartIndex;
			string template = GetWordLeft(lineText, lineOffset);
			if (string.IsNullOrWhiteSpace(template))
				return false;
			string expansion = GetTemplateExpansion(template);
			if (string.IsNullOrWhiteSpace(expansion))
				return false;

			int finalInsertionPointOffset = 0;
			if (expansion.Contains(Caret))
			{
				string textAfterCaret = expansion.EverythingAfter(Caret);
				finalInsertionPointOffset = -textAfterCaret.Length;
				expansion = expansion.Replace(Caret, string.Empty);
			}

			int insertionPoint = caretIndex - template.Length;

			textBox.Text = textBox.Text.Remove(insertionPoint, template.Length).Insert(insertionPoint, expansion);
			// TODO: Look for <<Caret>> or <<Cursor>>
			textBox.TextArea.Caret.Offset = insertionPoint + expansion.Length + finalInsertionPointOffset;
			return true;
			// Can only do selections from start to end, (but not end to start).
			//
		}
	}
}
