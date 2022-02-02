//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public interface ITextCommand
	{
		void Execute(TextArea textArea, TextDocument document);
		void ExpansionComplete(TextArea textArea);
		void AllExpansionsComplete(TextArea textArea);
	}
}
