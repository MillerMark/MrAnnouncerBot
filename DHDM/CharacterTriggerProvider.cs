//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public abstract class CharacterTriggerProvider : CompletionProvider
	{

		public char TriggerKey { get; set; }
		public CharacterTriggerProvider()
		{

		}

		public override bool ShouldComplete(TextArea textArea, char lastKeyPressed, string expectedProviderName)
		{
			return lastKeyPressed == TriggerKey && ProviderName == expectedProviderName;
		}
	}
}
