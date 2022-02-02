//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	public class TextAreaEventArgs : EventArgs
	{
		public TextArea TextArea { get; set; }
		public TextAreaEventArgs(TextArea textArea)
		{
			TextArea = textArea;
		}
		public TextAreaEventArgs()
		{

		}
	}
}
