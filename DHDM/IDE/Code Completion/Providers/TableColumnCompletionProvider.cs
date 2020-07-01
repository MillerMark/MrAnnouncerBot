using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;
using DndCore;

namespace DHDM
{
	public class TableColumnCompletionProvider : CharacterTriggerProvider
	{
		public TableColumnCompletionProvider()
		{
			ProviderName = CompletionProviderNames.DndTableColumn;
			TriggerKey = '"';
		}

		public override CompletionWindow Complete(TextArea textArea)
		{
			CompletionWindow completionWindow = new CompletionWindow(textArea);
			string lineLeft = textArea.Document.GetLineLeftOf(textArea.Caret.Offset);
			string tableName = lineLeft.EverythingBetweenNarrow("Table(\"", "\"");
			if (string.IsNullOrWhiteSpace(tableName))
				return null;

			List<string> tableColumns = AllTables.GetColumns(tableName);

			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			foreach (string tableColumn in tableColumns)
			{
				data.Add(new TextStringCompletionData(tableColumn));
			}

			return completionWindow;
		}
	}
}
