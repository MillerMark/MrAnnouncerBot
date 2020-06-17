//#define profiling
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using DndCore;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DHDM
{
	public class TextCompletionEngine
	{

		public TextCompletionEngine(TextEditor tbxCode)
		{
			TbxCode = tbxCode;
		}

		CompletionWindow completionWindow;


		bool IsIdentifierKey(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;
			return IsIdentifierCharacter(text[0]); ;
		}

		public static bool IsIdentifierCharacter(char character)
		{
			return char.IsLetterOrDigit(character) || character == '_';
		}

		public static string GetIdentifierLeftOf(TextDocument document, int caretOffset)
		{
			string result = string.Empty;
			string lineText = GetLineLeftOf(document, caretOffset);
			int index = lineText.Length - 1;
			while (index >= 0 && IsIdentifierCharacter(lineText[index]))
			{
				result = lineText[index] + result;
				index--;
			}
			return result;
		}

		public static string GetStringLeftOf(TextDocument document, int caretOffset)
		{
			string result = string.Empty;
			string lineText = GetLineLeftOf(document, caretOffset);
			int index = lineText.Length - 1;
			while (index >= 0 && (lineText[index] != '"' || (index > 0 && lineText[index - 1] == '\\')))
			{
				result = lineText[index] + result;
				index--;
			}
			return result;
		}

		public int GetParameterNumberAtPosition(TextDocument document, int caretOffset)
		{
			// TODO: Make this work with strings containing parens and commas (if you like).
			string lineLeft = GetLineLeftOf(document, caretOffset);
			// HACK: This solves 98% of use cases, but it's very hacky.
			if (!lineLeft.Contains("(") || lineLeft.Contains("//") || lineLeft.Contains(")"))
				return 0;
			string parameters = lineLeft.EverythingAfter("(");
			string[] allParamsLeft = parameters.Split(',');
			return allParamsLeft.Length;
		}

		public int GetParameterStartOffset(TextDocument document, int caretOffset)
		{
			string lineLeft = GetLineLeftOf(document, caretOffset);
			// HACK: This solves 98% of use cases, but it's very hacky.
			if (!lineLeft.Contains("(") || lineLeft.Contains("//") || lineLeft.Contains(")"))
				return 0;
			string parameters = lineLeft.EverythingAfter("(");
			string[] allParamsLeft = parameters.Split(',');
			return caretOffset - allParamsLeft[allParamsLeft.Length - 1].Length;
		}

		static string GetLineLeftOf(TextDocument document, int caretOffset)
		{
			DocumentLine line = document.GetLineByOffset(caretOffset);
			return document.GetText(line.Offset, caretOffset - line.Offset);
		}

		bool ExpectingSoundPath()
		{
			int offset = TbxCode.TextArea.Caret.Offset;
			string lineLeftOfCaret = GetLineLeftOf(TbxCode.Document, offset);
			if (lineLeftOfCaret.Contains("AddSound"))
				return true;

			return false;
		}

		public void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			//Expressions.functions
			if (completionWindow != null)
				return;

			InvokeCodeCompletionIfNecessary(e.Text);
		}

		void InvokeCodeCompletionIfNecessary(string lastKeyPressed)
		{
			if (lastKeyPressed == null)
				return;
			if (IsIdentifierKey(lastKeyPressed))
				CompleteIdentifier();
			else if (lastKeyPressed == "\"" && ExpectingSoundPath())
				CompleteSoundPath();
			else if (lastKeyPressed == "/" && ExpectingSoundPath())
				CompleteSoundPath();
		}


		void CompleteSoundPath()
		{
			int offset = TbxCode.TextArea.Caret.Offset;

			string filter = GetStringLeftOf(TbxCode.Document, offset);

			const string soundFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\SoundEffects\";
			string fullPattern = $"{soundFolder}{filter}*.mp3";
			string filePattern = System.IO.Path.GetFileName(fullPattern);
			string folderName = System.IO.Path.GetDirectoryName(fullPattern);
			string[] files = Directory.GetFiles(folderName, filePattern);
			string[] directories = Directory.GetDirectories(folderName);
			if ((files == null || files.Length == 0) && (directories == null || directories.Length == 0))
				return;

			completionWindow = new CompletionWindow(TbxCode.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			if (files != null)
				foreach (string fileName in files)
				{
					data.Add(new FileCompletionData(fileName));
				}
			if (directories != null)
				foreach (string directoryName in directories)
				{
					data.Add(new FolderCompletionData(directoryName));
				}
			completionWindow.Show();
			completionWindow.Closed += delegate
			{
				completionWindow = null;
			};
		}

		private void CompleteIdentifier()
		{
			int offset = TbxCode.TextArea.Caret.Offset;

			string tokenLeftOfCaret = GetIdentifierLeftOf(TbxCode.Document, offset);

			if (string.IsNullOrWhiteSpace(tokenLeftOfCaret))
				return;

			List<DndFunction> dndFunctions = Expressions.GetFunctionsStartingWith(tokenLeftOfCaret);
			if (dndFunctions == null || dndFunctions.Count == 0)
				return;

			completionWindow = new CompletionWindow(TbxCode.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			foreach (DndFunction dndFunction in dndFunctions)
			{
				if (!string.IsNullOrWhiteSpace(dndFunction.Name))
					data.Add(new FunctionCompletionData(dndFunction));
			}
			completionWindow.Show();
			completionWindow.Closed += delegate
			{
				completionWindow = null;
			};
		}

		public void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Length > 0 && completionWindow != null)
			{
				if (!char.IsLetterOrDigit(e.Text[0]))
				{
					// Whenever a non-letter is typed while the completion window is open,
					// insert the currently selected element.
					completionWindow.CompletionList.RequestInsertion(e);
				}
			}
			// Do not set e.Handled=true.
			// We still want to insert the character that was typed.
		}

		public string CharacterLeft
		{
			get
			{
				if (TbxCode.CaretOffset <= 1)
					return null;
				return TbxCode.Document.GetText(TbxCode.CaretOffset - 1, 1);
			}
		}
		

		public void InvokeCodeCompletion()
		{
			InvokeCodeCompletionIfNecessary(CharacterLeft);
		}
		public string GetActiveMethodCall(TextDocument document, int caretOffset)
		{
			string lineLeft = GetLineLeftOf(document, caretOffset);
			if (lineLeft.Contains("//") || lineLeft.Contains(")") || !lineLeft.Contains("("))
				return null;
			// HACK: It's a hack, kids. But solves large number of use cases with very little code.
			return lineLeft.EverythingBefore("(").Trim();
		}
		public TextEditor TbxCode { get; set; }
	}
}
