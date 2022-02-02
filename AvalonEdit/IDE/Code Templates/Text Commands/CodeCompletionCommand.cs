//#define profiling
using System;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	[TextCommandName("CodeCompletion")]
	public class CodeCompletionCommand : PositionSaverCommand
	{
		public static event TextAreaEventHandler ExpansionCompleted;
		public static void OnExpansionCompleted(object sender, TextAreaEventArgs e)
		{
			ExpansionCompleted?.Invoke(sender, e);
		}
		public CodeCompletionCommand()
		{

		}

		public override void ExpansionComplete(TextArea textArea)
		{
			textArea.Caret.Offset = afterCaretOffset;
			OnExpansionCompleted(this, new TextAreaEventArgs(textArea));
		}
	}
}
