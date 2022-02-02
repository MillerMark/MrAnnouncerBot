﻿using System;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Linq;
using System.Collections.Generic;

namespace SuperAvalonEdit
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
