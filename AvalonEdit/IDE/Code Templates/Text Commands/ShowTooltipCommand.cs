//#define profiling
using System;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;

namespace AvalonEdit
{
	[TextCommandName("ShowTooltip")]
	public class ShowTooltipCommand : ITextCommand
	{
		public static event TextAreaEventHandler ShowParameterTooltipIfNecessary;
		public static void OnShowParameterTooltipIfNecessary(object sender, TextAreaEventArgs e)
		{
			ShowParameterTooltipIfNecessary?.Invoke(sender, e);
		}
		public ShowTooltipCommand()
		{

		}

		public void AllExpansionsComplete(TextArea textArea)
		{
		}

		public void Execute(TextArea textArea, TextDocument document)
		{
			OnShowParameterTooltipIfNecessary(this, new TextAreaEventArgs(textArea));
		}

		public void ExpansionComplete(TextArea textArea)
		{

		}
	}
}
