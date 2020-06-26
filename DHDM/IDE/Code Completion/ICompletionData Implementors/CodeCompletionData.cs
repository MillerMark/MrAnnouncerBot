//#define profiling
using System;
using System.Reflection;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using DndCore;

namespace DHDM
{
	/// <summary>
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the
	/// completion drop down.
	/// </summary>
	public class IdentifierCompletionData : ICompletionData
	{
		public string ToolTip { get; set; }
		public IdentifierCompletionData(string text)
		{
			Text = text;
			ToolTip = text;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}


		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return Text; }
		}

		public object Description
		{
			get { return ToolTip; }
		}

		public double Priority => 0;
		public DndFunction Function { get; set; }
		public string Text { get; set; }

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			string tokenLeftOfCaret = textArea.Document.GetIdentifierLeftOf(textArea.Caret.Offset);
			textArea.Document.Replace(textArea.Caret.Offset - tokenLeftOfCaret.Length, tokenLeftOfCaret.Length, "");
			string caret = TemplateEngine.GetCommand("Caret");
			string showTooltip = TemplateEngine.GetCommand("ShowTooltip");
			TemplateEngine.Expand(textArea, $"{Text}{caret}{showTooltip}");
		}
	}

	/// <summary>
	/// Implements AvalonEdit ICompletionData interface to provide the entries in the
	/// completion drop down.
	/// </summary>
	public class FunctionCompletionData : ICompletionData
	{
		public string ToolTip { get; set; }
		public FunctionCompletionData(DndFunction function)
		{
			Function = function;
			Text = function.Name;
			TooltipAttribute tooltipAttribute = function.GetType().GetCustomAttribute<TooltipAttribute>();
			if (tooltipAttribute != null)
				ToolTip = tooltipAttribute.DisplayText;
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}

		public string Text { get; private set; }

		// Use this property if you want to show a fancy UIElement in the list.
		public object Content
		{
			get { return Text; }
		}

		public object Description
		{
			get { return ToolTip; }
		}

		public double Priority => 0;
		public DndFunction Function { get; set; }

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			string tokenLeftOfCaret = textArea.Document.GetIdentifierLeftOf(textArea.Caret.Offset);
			textArea.Document.Replace(textArea.Caret.Offset - tokenLeftOfCaret.Length, tokenLeftOfCaret.Length, "");
			string caret = TemplateEngine.GetCommand("Caret");
			string showTooltip = TemplateEngine.GetCommand("ShowTooltip");
			TemplateEngine.Expand(textArea, $"{Text}({caret}{showTooltip});");
		}
	}
}
