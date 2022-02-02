//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public static class TextCommandEngine
	{
		static TextCommandEngine()
		{

		}
		public static ITextCommand Execute(string command, TextArea textArea, TextDocument document)
		{
			if (string.IsNullOrWhiteSpace(command))
				return null;
			if (!TemplateEngine.TextCommands.ContainsKey(command))
				return null;

			ITextCommand textCommand = (ITextCommand)Activator.CreateInstance(TemplateEngine.TextCommands[command]);
			if (textCommand == null)
				return null;

			textCommand.Execute(textArea, document);
			return textCommand;
		}
	}
}
