//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace DHDM
{
	public abstract class CompletionProvider : ICompletionProvider
	{
		public string ProviderName { get; set; }
		public abstract bool ShouldComplete(TextArea textArea, char lastKeyPressed, string requestedProviderName);
		public abstract CompletionWindow Complete(TextArea textArea);
	}
}
