//#define profiling
using System;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;

namespace DHDM
{
	[TextCommandName("ShowTooltip")]
	public class ShowTooltipCommand : ITextCommand
	{
		public ShowTooltipCommand()
		{

		}

		public void AllExpansionsComplete(TextArea textArea)
		{
		}

		public void Execute(TextArea textArea, TextDocument document)
		{
			if (Application.Current.MainWindow is MainWindow mainWindow)
				mainWindow.ShowParameterTooltipIfNecessary();
		}

		public void ExpansionComplete(TextArea textArea)
		{

		}
	}
}
