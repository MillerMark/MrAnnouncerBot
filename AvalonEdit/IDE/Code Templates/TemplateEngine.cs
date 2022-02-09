//#define profiling
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;
using DndCore;
using ICSharpCode.AvalonEdit.Document;
using SheetsPersist;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public class TemplateEngine
	{
		// TODO: figure out what I want to do with these private constants...
		private const string Caret = "«Caret»";
		private const string CodeCompletion = "«CodeCompletion»";

		public const char TextCommandDelimiterBegin = '«';
		public const char TextCommandDelimiterEnd = '»';

		public TemplateEngine()
		{
		}

		static TemplateEngine()
		{
			LoadTextCommands();
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

		CodeTemplate GetCodeTemplate(string wordToLeftOfCaret, TextArea textArea)
		{
			// TODO: Add support for context and multiple templates.
			return templates.FirstOrDefault(x => x.Template == wordToLeftOfCaret &&
			(string.IsNullOrWhiteSpace(x.Context) || 
			Expressions.GetBool(x.Context, null, null, null, null, textArea)));
		}

		List<ExpansionSegment> expansionSegments = new List<ExpansionSegment>();
		void AddSegments(TextArea textArea, string[] split)
		{
			expansionSegments.Clear();
			foreach (string segmentStr in split)
				expansionSegments.Add(new ExpansionSegment(textArea, segmentStr));
		}

		List<ITextCommand> commandsExpanded = new List<ITextCommand>();

		void ExpandSegments()
		{
			commandsExpanded.Clear();
			foreach (ExpansionSegment expansionSegment in expansionSegments)
			{
				ITextCommand expandedCommand = expansionSegment.ExpandCommand();
				if (expandedCommand != null)
					commandsExpanded.Add(expandedCommand);
				expansionSegment.ExpandText();
			}
		}

		void ExpansionComplete(TextArea textArea)
		{
			foreach (ITextCommand textCommand in commandsExpanded)
			{
				textCommand.ExpansionComplete(textArea);
			}
		}
		void AllExpansionsComplete(TextArea textArea)
		{
			foreach (ITextCommand textCommand in commandsExpanded)
			{
				textCommand.AllExpansionsComplete(textArea);
			}
		}

		void ExpandTemplate(TextArea textArea, string expansion)
		{
			AddSegments(textArea, expansion.Split(TextCommandDelimiterBegin));
			ExpandSegments();
			ExpansionComplete(textArea);
			AllExpansionsComplete(textArea);
			commandsExpanded.Clear();

		}

		/// <summary>
		/// Gets the template, if it exists, that is to the left of the caret.
		/// </summary>
		public CodeTemplate GetTemplateToExpand(TextEditor textBox)
		{
			int caretIndex = textBox.TextArea.Caret.Offset;
			int selectionLength = textBox.SelectionLength;
			if (selectionLength != 0)
				return null;

			DocumentLine line = textBox.Document.GetLineByOffset(caretIndex);
			string lineText = textBox.Document.GetText(line.Offset, line.Length);
			int lineStartIndex = line.Offset;
			int lineOffset = caretIndex - lineStartIndex;
			string template = GetWordLeft(lineText, lineOffset);
			int templateStartIndex = lineText.LastIndexOf(template);

			BaseContext.TextLeftOfTemplate = lineText.Remove(lineText.Length - template.Length, template.Length);
			BaseContext.TextRightOfTemplate = lineText.Remove(0, templateStartIndex + template.Length);

			if (string.IsNullOrWhiteSpace(template))
				return null;
			return GetCodeTemplate(template, textBox.TextArea);
		}

		/// <summary>
		/// Expands the specified template at the caret position.
		/// </summary>
		public void ExpandTemplate(TextEditor textBox, CodeTemplate templateToExpand)
		{
			DeleteTemplate(textBox.TextArea, templateToExpand.Template);
			ExpandTemplate(textBox.TextArea, templateToExpand.Expansion);
		}

		void DeleteTemplate(TextArea textArea, string template)
		{
			int caretIndex = textArea.Caret.Offset;
			int insertionPoint = caretIndex - template.Length;
			textArea.Document.Text = textArea.Document.Text.Remove(insertionPoint, template.Length);
			textArea.Caret.Offset = insertionPoint;
		}

		public static Dictionary<string, Type> TextCommands = new Dictionary<string, Type>();
		List<CodeTemplate> templates = LoadTemplates();
		static void AddTextCommand(string name, Type type)
		{
			TextCommands.Add(name, type);
		}

		static void LoadTextCommands()
		{
			Assembly dndCore = typeof(TemplateEngine).Assembly;

			Type textCommandType = typeof(ITextCommand);

			foreach (Type type in dndCore.GetTypes())
			{
				if (!type.IsClass || type.IsAbstract)
					continue;

				if (textCommandType.IsAssignableFrom(type))
				{
					TextCommandNameAttribute customAttribute = type.GetCustomAttribute<TextCommandNameAttribute>();
					if (customAttribute == null)
						throw new Exception($"ITextCommand implementer ({type.Name}) must include a TextCommandNameAttribute.");
					AddTextCommand(customAttribute.Name, type);
				}
			}
		}
		public static string GetCommand(string str)
		{
			return $"{TextCommandDelimiterBegin}{str}{TextCommandDelimiterEnd}";
		}

		public static void Expand(TextArea textArea, string str)
		{
			TemplateEngine templateEngine = new TemplateEngine();
			templateEngine.ExpandTemplate(textArea, str);
		}

		public static List<CodeTemplate> LoadTemplates()
		{
			return GoogleSheets.Get<CodeTemplate>();
		}

		public void ReloadTemplates()
		{
			templates = LoadTemplates();

		}
	}
}
