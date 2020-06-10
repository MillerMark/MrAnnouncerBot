//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DHDM
{
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the
	/// completion drop down.
	public class CodeCompletionData : ICompletionData
	{
		public CodeCompletionData(string text)
		{
			this.Text = text;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}

		public string Text { get; private set; }

		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return this.Text; }
		}

		public object Description
		{
			get { return "Description for " + this.Text; }
		}

		public double Priority => 0;

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			string tokenLeftOfCaret = TextCompletionEngine.GetIdentifierLeftOf(textArea.Document, textArea.Caret.Offset);
			textArea.Document.Replace(textArea.Caret.Offset - tokenLeftOfCaret.Length, tokenLeftOfCaret.Length, this.Text);
		}
	}
}
