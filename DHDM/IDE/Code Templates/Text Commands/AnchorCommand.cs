//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	[TextCommandName("Anchor")]
	public class AnchorCommand : PositionSaverCommand
	{
		public AnchorCommand()
		{

		}

		public override void AllExpansionsComplete(TextArea textArea)
		{
			textArea.Selection = Selection.Create(textArea, textArea.Caret.Offset, afterCaretOffset);
		}
	}
}
