//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace AvalonEdit
{
	public abstract class PositionSaverCommand : ITextCommand
	{
		protected int beforeCaretOffset;
		protected int afterCaretOffset;
		
		public PositionSaverCommand()
		{
		}

		public void Execute(TextArea textArea, TextDocument document)
		{
			BeforeExecute(textArea, document);
			DoExecute(textArea, document);
			AfterExecute(textArea, document);
		}

		protected virtual void DoExecute(TextArea textArea, TextDocument document)
		{
		}

		protected virtual void BeforeExecute(TextArea textArea, TextDocument document)
		{
			beforeCaretOffset = textArea.Caret.Offset;
		}

		protected virtual void AfterExecute(TextArea textArea, TextDocument document)
		{
			afterCaretOffset = textArea.Caret.Offset;
		}

		public virtual void ExpansionComplete(TextArea textArea)
		{

		}

		public virtual void AllExpansionsComplete(TextArea textArea)
		{

		}
	}
}
