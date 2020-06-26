//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;

namespace DHDM
{
	public class ExpansionSegment
	{
		public string Text { get; set; }
		public string Command { get; set; }
		public TextArea TextArea { get; set; }
		public TextDocument TextDocument { get; set; }

		public ITextCommand ExpandCommand()
		{
			return TextCommandEngine.Execute(Command, TextArea, TextDocument);
		}

		public void ExpandText()
		{
			if (string.IsNullOrWhiteSpace(Text))
				return;
			int caretOffset = TextArea.Caret.Offset;
			TextDocument.Insert(caretOffset, Text);
			TextArea.Caret.Offset = caretOffset + Text.Length;
		}

		public ExpansionSegment(TextArea textArea, string segmentStr)
		{
			TextArea = textArea;
			TextDocument = textArea.Document;
			if (string.IsNullOrWhiteSpace(segmentStr))
				return;
			string[] segmentStrs = segmentStr.Split(TemplateEngine.TextCommandDelimiterEnd);
			if (segmentStrs.Length == 1)
			{
				if (segmentStr.Contains(TemplateEngine.TextCommandDelimiterEnd))
					Command = segmentStrs[0];
				else
					Text = segmentStrs[0];
			}
			else if (segmentStrs.Length == 2)
			{
				Command = segmentStrs[0];
				Text = segmentStrs[1];
			}
			else
				throw new Exception("Expecting only two segment strings.");
		}
	}
}
