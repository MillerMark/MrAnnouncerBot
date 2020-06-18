//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DHDM
{
	public interface ICompletionProvider
	{
		string ProviderName { get; set; }
		CompletionWindow Complete(TextArea textArea);
		bool ShouldComplete(TextArea textArea, char lastKeyPressed);
	}
}
