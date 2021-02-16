//#define profiling
using System;
using SharedCore;
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
using GoogleHelper;

namespace AvalonEdit
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

		private const int IntervalBeforeSavingCodeChangesMs = 2700;
		public TextCompletionEngine(TextEditor tbxCode)
		{
			needToSaveCodeTimer = new DispatcherTimer(DispatcherPriority.Send);
			needToSaveCodeTimer.Tick += new EventHandler(SaveCodeChangesFromTimer);
			needToSaveCodeTimer.Interval = TimeSpan.FromMilliseconds(IntervalBeforeSavingCodeChangesMs);
			TbxCode = tbxCode;
			HookEvents();
			RefreshCompletionProviders();
		}

		void HookEvents()
		{
			TbxCode.TextArea.TextEntered += TextArea_TextEntered;
			TbxCode.TextArea.TextEntering += TextArea_TextEntering;
			TbxCode.TextArea.KeyDown += TextArea_KeyDown;
			TbxCode.TextArea.Caret.PositionChanged += Caret_PositionChanged;
			TbxCode.TextChanged += TextChanged;
			TbxCode.LostFocus += TextArea_LostFocus;
		}

		private void Caret_PositionChanged(object sender, EventArgs e)
		{
			needToSaveCodeTimer.Stop();
			needToSaveCodeTimer.Start();
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
			if (completionWindow != null)
				return;

			InvokeCodeCompletionIfNecessary(e.Text);
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
			public string ActiveMethodCall { get; set; }
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
				result.ActiveMethodCall = dndFunction.Name;
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
				CompleteCode();
			else if (lastKeyPressed.Length > 0)
			{
				EditorProviderDetails expectedProviderDetails = GetCompletionProviderDetails();

				foreach (CompletionProvider completionProvider in completionProviders)
				{
					if (string.IsNullOrEmpty(expectedProviderDetails.Name) || completionProvider.ProviderName == expectedProviderDetails.Name)
						if (completionProvider.ShouldComplete(TbxCode.TextArea, lastKeyPressed[0], expectedProviderDetails.Name))
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



		public void RefreshCompletionProviders()
		{
			completionProviders = new List<CompletionProvider>();
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
				List = "DenseSmoke; Poof; SmokeBlast; SmokeWave; SmokeColumn; SparkBurst; Water; Fireworks; EmbersLarge; EmbersMedium; Fumes; FireBall; BloodGush; SparkMagicA; SparkMagicB; SparkMagicC; SparkMagicD; SparkMagicE; SparkShower; SparkTrailBurst; SwirlSmokeA; SwirlSmokeB; SwirlSmokeC",
				TriggerKey = '"'
			});
			AddCompletionProvider(new TextListCompletionProvider()
			{
				ProviderName = CompletionProviderNames.WindupEffectName,
				List = "Fire; Smoke; Fairy; Wide; Ghost; Trails; Plasma; Lightning",
				TriggerKey = '"'
			});
			AddCompletionProvider(new TextListCompletionProvider()
			{
				ProviderName = CompletionProviderNames.TrailingEffectName,
				List = AllTrailingEffects.GetList("; "),
				TriggerKey = '"'
			});
			AddCompletionProvider(new TextListCompletionProvider()
			{
				ProviderName = CompletionProviderNames.DndTableName,
				List = "Barbarian; Ranger; Bard; Paladin; ArcaneTrickster; Rogue; Wizard; Druid; Sorcerer; Sorcerer.SpellSlotsFromSorceryPoints",
				TriggerKey = '"'
			});
			AddCompletionProvider(new TableColumnCompletionProvider());
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

		List<DndFunction> GetDndFunctionsStartingWith(EditorProviderDetails expectedProviderDetails, string tokenLeftOfCaret)
		{
			List<DndFunction> dndFunctions = Expressions.GetFunctionsStartingWith(tokenLeftOfCaret);

			if (expectedProviderDetails.Type == null)
				return dndFunctions;

			List<DndFunction> filteredFunctions = new List<DndFunction>();
			foreach (DndFunction dndFunction in dndFunctions)
			{
				ReturnTypeAttribute returnType = dndFunction.GetType().GetCustomAttribute<ReturnTypeAttribute>();
				if (returnType != null && returnType.Matches(expectedProviderDetails.Type))
					filteredFunctions.Add(dndFunction);
			}
			return filteredFunctions;
		}

		List<string> GetEnumEntries(EditorProviderDetails expectedProviderDetails)
		{
			List<string> enumEntries = new List<string>();
			if (expectedProviderDetails.Type != null && expectedProviderDetails.Type.IsEnum)
				foreach (object enumElement in expectedProviderDetails.Type.GetEnumValues())
					enumEntries.Add(enumElement.ToString());
			return enumEntries;
		}

		void AddEnumEntries(EditorProviderDetails expectedProviderDetails, List<ICompletionData> completionData, string tokenLeftOfCaret)
		{
			List<string> enumEntries = GetEnumEntries(expectedProviderDetails);
			if (enumEntries != null)
				foreach (string enumEntry in enumEntries)
				{
					if (!string.IsNullOrWhiteSpace(enumEntry))
						if (string.IsNullOrWhiteSpace(tokenLeftOfCaret) || enumEntry.ToLower().StartsWith(tokenLeftOfCaret))
							completionData.Add(new IdentifierCompletionData(enumEntry, $"{expectedProviderDetails.Type.Name}.{enumEntry}"));
				}
		}

		void AddDndFunctionsStartingWith(EditorProviderDetails expectedProviderDetails, List<ICompletionData> completionData, string tokenLeftOfCaret)
		{
			List<DndFunction> dndFunctions = GetDndFunctionsStartingWith(expectedProviderDetails, tokenLeftOfCaret);
			if (dndFunctions == null)
				return;
			foreach (DndFunction dndFunction in dndFunctions)
				if (!string.IsNullOrWhiteSpace(dndFunction.Name))
					completionData.Add(new FunctionCompletionData(dndFunction));
		}


		void GetAllKnownTokens()
		{
			allKnownTokens = new List<PropertyCompletionInfo>();
			foreach (DndVariable dndVariable in Expressions.variables)
			{
				if (TypeHelper.IsGenericDescendant(typeof(DndEnumValue<>), dndVariable.GetType()))
					continue;
				List<PropertyCompletionInfo> completionInfo = dndVariable.GetCompletionInfo();
				if (completionInfo != null)
					allKnownTokens.AddRange(completionInfo);
			}
			allKnownTokens = allKnownTokens.OrderBy(x => x.Name).ToList();
		}

		List<PropertyCompletionInfo> allKnownTokens;
		void AddDndPropertiesStartingWith(EditorProviderDetails expectedProviderDetails, List<ICompletionData> completionData, string tokenLeftOfCaret)
		{
			if (allKnownTokens == null)
				GetAllKnownTokens();
			foreach (PropertyCompletionInfo propertyCompletionInfo in allKnownTokens)
			{
				if (expectedProviderDetails.Type != null)
				{
					if (propertyCompletionInfo.Type.HasFlag(ExpressionType.@enum) && propertyCompletionInfo.EnumTypeName != expectedProviderDetails.Type.Name)
						continue;
					if (!TypeHelper.Matches(propertyCompletionInfo.Type, expectedProviderDetails.Type))
						continue;
				}
				if (string.IsNullOrWhiteSpace(tokenLeftOfCaret) || propertyCompletionInfo.Name.ToLower().StartsWith(tokenLeftOfCaret.ToLower()))
					completionData.Add(new IdentifierCompletionData(propertyCompletionInfo.Name, propertyCompletionInfo.Description));
			}
		}

		public void CompleteCode()
		{
			int offset = TbxCode.TextArea.Caret.Offset;

			string tokenLeftOfCaret = TbxCode.Document.GetIdentifierLeftOf(offset);

			List<ICompletionData> completionData = new List<ICompletionData>();
			EditorProviderDetails expectedProviderDetails = GetCompletionProviderDetails();
			 AddEnumEntries(expectedProviderDetails, completionData, tokenLeftOfCaret);
			AddDndFunctionsStartingWith(expectedProviderDetails, completionData, tokenLeftOfCaret);
			AddDndPropertiesStartingWith(expectedProviderDetails, completionData, tokenLeftOfCaret);

			if (completionData.Count == 0)
				return;
			completionWindow = new CompletionWindow(TbxCode.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			foreach (ICompletionData item in completionData)
			{
				data.Add(item);
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

		string GetActiveMethodCall(TextArea textArea)
		{
			return textArea.Document.GetActiveMethodCall(textArea.Caret.Offset);
		}

		public DndFunction GetActiveDndFunction(TextArea textArea)
		{
			string activeMethodCall = GetActiveMethodCall(textArea);
			return Expressions.functions.FirstOrDefault(x => x.Name == activeMethodCall);
		}

		public void TextArea_KeyDown(object sender, KeyEventArgs e)
		{
			foreach (ShortcutBinding shortcutBinding in shortcutBindings)
			{
				if (shortcutBinding.Matches(e.Key, Modifiers.Active) && shortcutBinding.IsAvailable(TbxCode.TextArea))
				{
					shortcutBinding.ExecuteCommand(TbxCode.TextArea, this);
					if (!shortcutBinding.SendKeyToEditorAfter)
					{
						e.Handled = true;
					}
					break;
				}
			}
		}
		
		public void ReloadShortcuts()
		{
			shortcutBindings.Clear();
			shortcutBindings = GoogleSheets.Get<ShortcutBinding>();
			//shortcutBindings.Add(new ShortcutBinding(KeyboardModifiers.None, Key.Divide, "MultilineSelection == true && !SelectionIsCommented", new SelectionEmbedding("//")));
			//shortcutBindings.Add(new ShortcutBinding(KeyboardModifiers.None, Key.Divide, "MultilineSelection == true && SelectionIsCommented", new SelectionTrim("//")));
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
		public void HideCodeCompletionWindow()
		{
			if (completionWindow != null)
				completionWindow.Close();
		}

	}
}
