using System;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class TextListCompletionProvider : CharacterTriggerProvider
	{
		public TextListCompletionProvider()
		{
		}
		public override CompletionWindow Complete(TextArea textArea)
		{
			CompletionWindow completionWindow = new CompletionWindow(textArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			if (entries != null)
				foreach (string entry in entries)
				{
					data.Add(new TextStringCompletionData(entry));
				}

			return completionWindow;
		}
		public override bool ShouldComplete(TextArea textArea, char lastKeyPressed)
		{
			return base.ShouldComplete(textArea, lastKeyPressed);
		}

		string list;
		List<string> entries;
		public string List
		{
			get => list;
			set
			{
				if (list == value)
					return;
				list = value;
				entries = list.Split(';').Select(x => x.Trim()).ToList();
				entries.Sort();
			}
		}
	}
}
