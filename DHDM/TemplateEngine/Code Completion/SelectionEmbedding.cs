using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class SelectionEmbedding : CodeEditorCommand
	{
		public SelectionEmbedding(string prefix)
		{
			Prefix = prefix;
		}

		public override void Execute(TextArea textArea)
		{
			textArea.GetSelectionBounds(out int startLine, out int endLine);
			int startOffset = textArea.Document.GetOffset(startLine, 0);
			textArea.Document.UndoStack.StartUndoGroup();
			try
			{
				for (int i = startLine; i <= endLine; i++)
				{
					ICSharpCode.AvalonEdit.Document.DocumentLine line = textArea.Document.GetLineByNumber(i);
					textArea.Document.Insert(line.Offset, Prefix);
				}

				ICSharpCode.AvalonEdit.Document.DocumentLine endDocumentLine = textArea.Document.GetLineByNumber(endLine);
				textArea.Selection = Selection.Create(textArea, startOffset, endDocumentLine.EndOffset);
			}
			finally
			{
				textArea.Document.UndoStack.EndUndoGroup();
			}
		}

		public string Prefix { get; set; }
	}
}
