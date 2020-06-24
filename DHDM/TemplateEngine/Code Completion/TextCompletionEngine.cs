//#define profiling
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using DndCore;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Threading;

namespace DHDM
{
	public class TextCompletionEngine
	{
		bool textHasChanged;
		public event EventHandler RequestTextSave;
		
		protected virtual void OnRequestTextSave(object sender, EventArgs e)
		{
			RequestTextSave?.Invoke(sender, e);
			textHasChanged = false;
		}

		ToolTip parameterToolTip;
		public ToolTip ParameterToolTip
		{
			get
			{
				if (parameterToolTip == null)
					parameterToolTip = new ToolTip()
					{
						Placement = PlacementMode.Relative,
						PlacementTarget = TbxCode
					};
				return parameterToolTip;
			}
		}

		private const int IntervalBeforeSavingCodeChangesMs = 1200;
		public TextCompletionEngine(TextEditor tbxCode)
		{
			needToSaveCodeTimer = new DispatcherTimer(DispatcherPriority.Send);
			needToSaveCodeTimer.Tick += new EventHandler(SaveCodeChangesFromTimer);
			needToSaveCodeTimer.Interval = TimeSpan.FromMilliseconds(IntervalBeforeSavingCodeChangesMs);
			TbxCode = tbxCode;
			HookEvents();
			CreateCompletionProviders();
		}

		void HookEvents()
		{
			TbxCode.TextArea.TextEntered += TextArea_TextEntered;
			TbxCode.TextArea.TextEntering += TextArea_TextEntering;
			TbxCode.TextArea.KeyDown += TextArea_KeyDown;
			TbxCode.TextChanged += TextChanged;
			TbxCode.LostFocus += TextArea_LostFocus;
		}

		CompletionWindow completionWindow;


		bool IsIdentifierKey(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;
			return CodeUtils.IsIdentifierCharacter(text[0]);
		}

		public void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			//Expressions.functions
			if (completionWindow != null)
				return;

			InvokeCodeCompletionIfNecessary(e.Text);
			//TextJustChanged();
		}

		void TextChanged(object sender, EventArgs e)
		{
			TextJustChanged();
		}

		void TextJustChanged()
		{
			if (settingCodeInternally)
				return;
			textHasChanged = true;
			needToSaveCodeTimer.Stop();
			needToSaveCodeTimer.Start();
		}

		List<CompletionProvider> completionProviders = new List<CompletionProvider>();

		public class EditorProviderDetails
		{
			public string Name { get; set; }
			public Type Type { get; set; }
			public EditorProviderDetails()
			{
			}
		}

		EditorProviderDetails GetCompletionProviderDetails()
		{
			EditorProviderDetails result = new EditorProviderDetails();
			
			DndFunction dndFunction = GetActiveDndFunction(TbxCode.TextArea);
			if (dndFunction != null)
			{
				IEnumerable<ParamAttribute> customAttributes = dndFunction.GetType().GetCustomAttributes(typeof(ParamAttribute)).Cast<ParamAttribute>().ToList();
				if (customAttributes != null)
				{
					int parameterNumber = TbxCode.Document.GetParameterNumberAtPosition(TbxCode.CaretOffset);
					ParamAttribute paramAttribute = customAttributes.FirstOrDefault(x => x.Index == parameterNumber);
					if (paramAttribute != null)
					{
						result.Name = paramAttribute.Editor;
						result.Type = paramAttribute.Type;
					}
				}
			}
			return result;
		}

		void InvokeCodeCompletionIfNecessary(string lastKeyPressed)
		{
			if (lastKeyPressed == null)
				return;
			if (IsIdentifierKey(lastKeyPressed))
				CompleteIdentifier();
			else if (lastKeyPressed.Length > 0)
			{
				EditorProviderDetails expectedProviderDetails = GetCompletionProviderDetails();

				foreach (CompletionProvider completionProvider in completionProviders)
				{
					if (string.IsNullOrEmpty(expectedProviderDetails.Name) || completionProvider.ProviderName == expectedProviderDetails.Name)
						if (completionProvider.ShouldComplete(TbxCode.TextArea, lastKeyPressed[0]))
						{
							completionWindow = completionProvider.Complete(TbxCode.TextArea);
							break;
						}
				}
				ShowCompletionWindow();
			}
		}

		void AddCompletionProvider(CompletionProvider completionProvider)
		{
			completionProviders.Add(completionProvider);
		}

		void CreateCompletionProviders()
		{
			const string soundFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\SoundEffects\";
			AddCompletionProvider(new FileCompletionProvider()
			{
				Extension = "mp3",
				BaseFolder = soundFolder,
				AllowSubFolders = true,
				ProviderName = CompletionProviderNames.SoundFile,
				TriggerKey = '"'
			});
			AddCompletionProvider(new TextListCompletionProvider()
			{
				ProviderName = CompletionProviderNames.AnimationEffectName,
				List = "DenseSmoke; Poof; SmokeBlast; SmokeWave; SparkBurst; Water; SparkShower; EmbersLarge; EmbersMedium; Fumes; FireBall; BloodGush",
				TriggerKey = '"'
			});
		}

		private void ShowCompletionWindow()
		{
			if (completionWindow == null)
				return;

			completionWindow.Show();

			if (ParameterToolTip.IsOpen)
			{
				completionWindow.Top += ParameterToolTip.ActualHeight;
			}
			completionWindow.Closed += delegate
			{
				completionWindow = null;
			};
		}

		private void CompleteIdentifier()
		{
			int offset = TbxCode.TextArea.Caret.Offset;

			string tokenLeftOfCaret = TbxCode.Document.GetIdentifierLeftOf(offset);

			if (string.IsNullOrWhiteSpace(tokenLeftOfCaret))
				return;


			List<string> enumEntries = new List<string>();
			EditorProviderDetails expectedProviderDetails = GetCompletionProviderDetails();
			if (expectedProviderDetails.Type != null && expectedProviderDetails.Type.IsEnum)
				foreach (object enumElement in expectedProviderDetails.Type.GetEnumValues())
					enumEntries.Add(enumElement.ToString());

			List<DndFunction> dndFunctions = Expressions.GetFunctionsStartingWith(tokenLeftOfCaret);
			if ((dndFunctions == null || dndFunctions.Count == 0) && enumEntries.Count == 0)
				return;

			completionWindow = new CompletionWindow(TbxCode.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			foreach (DndFunction dndFunction in dndFunctions)
			{
				if (!string.IsNullOrWhiteSpace(dndFunction.Name))
					data.Add(new FunctionCompletionData(dndFunction));
			}
			foreach (string enumEntry in enumEntries)
			{
				if (!string.IsNullOrWhiteSpace(enumEntry))
					data.Add(new IdentifierCompletionData(enumEntry));
			}
			ShowCompletionWindow();
		}

		List<ShortcutBinding> shortcutBindings = new List<ShortcutBinding>();
		DispatcherTimer needToSaveCodeTimer;

		public void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Length > 0 && completionWindow != null)
			{
				if (!char.IsLetterOrDigit(e.Text[0]))
				{
					// Whenever a non-letter is typed while the completion window is open,
					// insert the currently selected element.
					completionWindow.CompletionList.RequestInsertion(e);
				}
			}
			// Do not set e.Handled=true.
			// We still want to insert the character that was typed.
		}

		public string CharacterLeft
		{
			get
			{
				if (TbxCode.CaretOffset <= 1)
					return null;
				return TbxCode.Document.GetText(TbxCode.CaretOffset - 1, 1);
			}
		}


		public void InvokeCodeCompletion()
		{
			InvokeCodeCompletionIfNecessary(CharacterLeft);
		}

		public TextEditor TbxCode { get; set; }
		public CompletionWindow CompletionWindow { get => completionWindow; set => completionWindow = value; }
		public bool SettingCodeInternally { get => settingCodeInternally; set => settingCodeInternally = value; }

		public DndFunction GetActiveDndFunction(TextArea textArea)
		{
			string activeMethodCall = textArea.Document.GetActiveMethodCall(textArea.Caret.Offset);
			return Expressions.functions.FirstOrDefault(x => x.Name == activeMethodCall);
		}

		public void TextArea_KeyDown(object sender, KeyEventArgs e)
		{
			foreach (ShortcutBinding shortcutBinding in shortcutBindings)
			{
				if (shortcutBinding.Matches(e.Key, Modifiers.Active) && shortcutBinding.IsAvailable(TbxCode.TextArea))
				{
					shortcutBinding.InvokeCommand(TbxCode.TextArea);
					if (!shortcutBinding.SendKeyToEditorAfter)
					{
						e.Handled = true;
					}
					break;
				}
			}
		}
		
		public void LoadShortcuts()
		{
			shortcutBindings.Add(new ShortcutBinding(KeyboardModifiers.None, Key.Divide, "MultilineSelection == true && !SelectionIsCommented", new SelectionEmbedding("//")));
			shortcutBindings.Add(new ShortcutBinding(KeyboardModifiers.None, Key.Divide, "MultilineSelection == true && SelectionIsCommented", new SelectionTrim("//")));
		}

		static TextCompletionEngine()
		{
			BaseContext.LoadAll();
		}

		void SaveCodeChangesFromTimer(object sender, EventArgs e)
		{
			SaveTextNow();
			needToSaveCodeTimer.Stop();
		}
		void SaveTextNow()
		{
			if (textHasChanged)
				OnRequestTextSave(this, EventArgs.Empty);
		}
		public void TextArea_LostFocus(object sender, RoutedEventArgs e)
		{
			SaveTextNow();
		}

		bool settingCodeInternally;
		public void SetText(string code)
		{
			settingCodeInternally = true;
			try
			{
				TbxCode.Text = code;
				TbxCode.Document.UndoStack.ClearAll();
			}
			finally
			{
				settingCodeInternally = false;
			}
		}

	}
}
