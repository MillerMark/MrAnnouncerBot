//#define profiling
using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;
using DndCore;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class ShortcutBinding
	{
		public char Key { get; set; }
		public Modifiers Modifiers { get; set; } = Modifiers.None;

		public string Context { get; set; } = string.Empty;
		public CodeEditorCommand Command { get; set; }
		public bool SendKeyToEditorAfter { get; set; } = false;
		public ShortcutBinding(Modifiers modifiers, char key, string context, CodeEditorCommand command)
		{
			Key = key;
			Modifiers = modifiers;
			Context = context;
			Command = command;
		}
		public ShortcutBinding()
		{

		}
		public bool IsAvailable(TextArea textArea)
		{
			if (string.IsNullOrWhiteSpace(Context))
				return true;
			return Expressions.GetBool(Context);
		}
		public void Invoke()
		{
			
		}
	}
}
