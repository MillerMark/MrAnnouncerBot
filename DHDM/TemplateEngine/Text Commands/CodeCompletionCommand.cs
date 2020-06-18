//#define profiling
using System;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	[TextCommandName("CodeCompletion")]
	public class CodeCompletionCommand : PositionSaverCommand
	{
		public CodeCompletionCommand()
		{

		}

		public override void ExpansionComplete(TextArea textArea)
		{
			textArea.Caret.Offset = afterCaretOffset;
			if (Application.Current.MainWindow is MainWindow mainWindow)
				mainWindow.InvokeCodeCompletion();
		}
	}
}
