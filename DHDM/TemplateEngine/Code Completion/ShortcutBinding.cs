//#define profiling
using System;
using System.Linq;
using System.Windows.Input;
using CodingSeb.ExpressionEvaluator;
using DndCore;
using ICSharpCode.AvalonEdit.Editing;

namespace DHDM
{
	public class ShortcutBinding
	{
		public Key Key { get; set; }
		public KeyboardModifiers Modifiers { get; set; } = KeyboardModifiers.None;

		public string Context { get; set; } = string.Empty;
		public CodeEditorCommand Command { get; set; }
		public bool SendKeyToEditorAfter { get; set; } = false;
		public ShortcutBinding(KeyboardModifiers modifiers, Key key, string context, CodeEditorCommand command)
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
			return Expressions.GetBool(Context, null, null, null, null, textArea);
		}

		public void Invoke()
		{
			
		}

		void ExecuteCommand(TextArea textArea)
		{
			Command.Execute(textArea);
		}

		public void InvokeCommand(TextArea textArea)
		{
			Command.Execute(textArea);
		}

		public bool Matches(Key key, KeyboardModifiers activeModifiers)
		{
			if (key == Key.Oem2)
				key = Key.Divide;
			return key == Key && activeModifiers == Modifiers;
		}
	}
}
