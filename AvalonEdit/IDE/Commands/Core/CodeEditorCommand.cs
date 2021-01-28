//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;

namespace AvalonEdit
{
	public class CodeEditorCommand
	{
		public virtual void Execute(TextCompletionEngine textCompletionEngine, TextArea textArea)
		{

		}
		public CodeEditorCommand()
		{

		}
	}
}
