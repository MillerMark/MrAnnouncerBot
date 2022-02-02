//#define profiling
using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;
using DndCore;
using GoogleHelper;
using SharedCore;
using ICSharpCode.AvalonEdit.Editing;

namespace SuperAvalonEdit
{
	[SheetName("IDE")]
	[TabName("Shortcuts")]
	public class ShortcutBinding
	{
		[Column]
		public Key Key { get; set; }

		[Column]
		public KeyboardModifiers Modifiers { get; set; } = KeyboardModifiers.None;

		[Column]
		public string Context { get; set; } = string.Empty;

		[Column("Command")]
		public string CommandTypeName { get; set; }
		
		[Column]
		public string Parameters { get; set; }

		List<string> parameterList;
		public List<string> ParameterList
		{
			get
			{
				if (parameterList == null)
				{
					parameterList = new List<string>();
					string[] individualParameters = Parameters.Split(',');
					// HACK: This code could have issues with commas in strings, but should cover 99% of expect use cases. Does not handle commas in quoted strings, for example.
					foreach (string individualParameter in individualParameters)
						parameterList.Add(Expressions.GetStr(individualParameter));
				}
				return parameterList;
			}
		}
		CodeEditorCommand command;
		public CodeEditorCommand Command
		{
			get
			{
				if (command == null)
				{
					Type typeToCreate = Type.GetType($"{nameof(SuperAvalonEdit)}.{CommandTypeName}");
					if (typeToCreate == null)
						return null;

					if (ParameterList.Count == 0)
						command = Activator.CreateInstance(typeToCreate) as CodeEditorCommand;
					else if (ParameterList.Count == 1)
						command = Activator.CreateInstance(typeToCreate, ParameterList[0]) as CodeEditorCommand;
					else if (ParameterList.Count == 2)
						command = Activator.CreateInstance(typeToCreate, ParameterList[0], ParameterList[1]) as CodeEditorCommand;
					else if (ParameterList.Count == 3)
						command = Activator.CreateInstance(typeToCreate, ParameterList[0], ParameterList[1], ParameterList[2]) as CodeEditorCommand;
					else if (ParameterList.Count == 4)
						command = Activator.CreateInstance(typeToCreate, ParameterList[0], ParameterList[1], ParameterList[2], ParameterList[3]) as CodeEditorCommand;
					else
						System.Diagnostics.Debugger.Break();
				}
				return command;
			}
			set
			{
				command = value;
			}
		}
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

		public void ExecuteCommand(TextArea textArea, TextCompletionEngine textCompletionEngine)
		{
			if (Command == null)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}
			Command.Execute(textCompletionEngine, textArea);
		}

		public bool Matches(Key key, KeyboardModifiers activeModifiers)
		{
			if (key == Key.Oem2)
				key = Key.Divide;
			return key == Key && activeModifiers == Modifiers;
		}
	}
}
