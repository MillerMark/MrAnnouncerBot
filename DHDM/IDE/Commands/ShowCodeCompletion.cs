using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class ShowCodeCompletion : CodeEditorCommand
	{
		public ShowCodeCompletion(string parameters)
		{
		}

		public override void Execute(TextCompletionEngine textCompletionEngine, TextArea textArea)
		{
			textCompletionEngine.CompleteCode();
		}

		public string Prefix { get; set; }
	}
}
