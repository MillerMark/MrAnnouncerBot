//#define profiling
using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace SuperAvalonEdit
{
	public class FileCompletionData : ICompletionData
	{
		public string ToolTip { get; set; }
		public FileCompletionData(string path)
		{
			Path = path;
			Text = System.IO.Path.GetFileName(Path);
			ToolTip = Path;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}

		public string Text { get; private set; }

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
		public string Path { get; set; }

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			string tokenLeftOfCaret = textArea.Document.GetIdentifierLeftOf(textArea.Caret.Offset);
			textArea.Document.Replace(textArea.Caret.Offset - tokenLeftOfCaret.Length, tokenLeftOfCaret.Length, "");
			TemplateEngine.Expand(textArea, $"{System.IO.Path.GetFileNameWithoutExtension(Text)}\"");
		}
	}
}
