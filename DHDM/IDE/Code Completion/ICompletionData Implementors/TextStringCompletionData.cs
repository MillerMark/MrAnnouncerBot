//#define profiling
using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DHDM
{
	public class TextStringCompletionData : ICompletionData
	{
		public string ToolTip { get; set; }
		public TextStringCompletionData(string text)
		{
			Text = text;
			ToolTip = Text;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}


		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return Text; }
		}

		public object Description
		{
			get { return ToolTip; }
		}

		public double Priority => 0;
		public string Text { get; set; }

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			string tokenLeftOfCaret = textArea.Document.GetIdentifierLeftOf(textArea.Caret.Offset);
			textArea.Document.Replace(textArea.Caret.Offset - tokenLeftOfCaret.Length, tokenLeftOfCaret.Length, "");
			TemplateEngine.Expand(textArea, $"{Text}\"");
		}
	}
}
