//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	[TextCommandName("Caret")]
	public class CaretCommand : PositionSaverCommand
	{
		public CaretCommand()
		{

		}

		public override void ExpansionComplete(TextArea textArea)
		{
			textArea.Caret.Offset = afterCaretOffset;
		}
	}
}
