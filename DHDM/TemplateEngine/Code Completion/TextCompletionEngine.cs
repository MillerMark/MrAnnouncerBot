//#define profiling
using System;
using System.IO;
using System.Linq;
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

namespace DHDM
{
	public class TextCompletionEngine
	{
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
		public TextCompletionEngine(TextEditor tbxCode)
		{
			TbxCode = tbxCode;
			CreateCompletionProviders();
		}

		CompletionWindow completionWindow;


		bool IsIdentifierKey(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;
			return CodeUtils.IsIdentifierCharacter(text[0]); ;
		}

		bool ExpectingSoundPath()
		{
			int offset = TbxCode.TextArea.Caret.Offset;
			string lineLeftOfCaret = TbxCode.Document.GetLineLeftOf(offset);
			if (lineLeftOfCaret.Contains("AddSound"))
				return true;

			return false;
		}

		public void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			//Expressions.functions
			if (completionWindow != null)
				return;

			InvokeCodeCompletionIfNecessary(e.Text);
		}

		List<CompletionProvider> completionProviders = new List<CompletionProvider>();

		string GetExpectedProviderName()
		{
			string expectedProviderName = null;
			DndFunction dndFunction = GetActiveDndFunction(TbxCode.TextArea);
			if (dndFunction != null)
			{
				IEnumerable<ParamAttribute> customAttributes = dndFunction.GetType().GetCustomAttributes(typeof(ParamAttribute)).Cast<ParamAttribute>().ToList();
				if (customAttributes != null)
				{
					int parameterNumber = TbxCode.Document.GetParameterNumberAtPosition(TbxCode.CaretOffset);
					ParamAttribute paramAttribute = customAttributes.FirstOrDefault(x => x.Index == parameterNumber);
					if (paramAttribute != null)
						expectedProviderName = paramAttribute.Editor;
				}
			}
			return expectedProviderName;
		}

		void InvokeCodeCompletionIfNecessary(string lastKeyPressed)
		{
			if (lastKeyPressed == null)
				return;
			if (IsIdentifierKey(lastKeyPressed))
				CompleteIdentifier();
			else if (lastKeyPressed.Length > 0)
			{
				string expectedProviderName = GetExpectedProviderName();

				foreach (CompletionProvider completionProvider in completionProviders)
				{
					if (string.IsNullOrEmpty(expectedProviderName) || completionProvider.ProviderName == expectedProviderName)
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

			List<DndFunction> dndFunctions = Expressions.GetFunctionsStartingWith(tokenLeftOfCaret);
			if (dndFunctions == null || dndFunctions.Count == 0)
				return;

			completionWindow = new CompletionWindow(TbxCode.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
			foreach (DndFunction dndFunction in dndFunctions)
			{
				if (!string.IsNullOrWhiteSpace(dndFunction.Name))
					data.Add(new FunctionCompletionData(dndFunction));
			}
			ShowCompletionWindow();
		}

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

		public DndFunction GetActiveDndFunction(TextArea textArea)
		{
			string activeMethodCall = textArea.Document.GetActiveMethodCall(textArea.Caret.Offset);
			return Expressions.functions.FirstOrDefault(x => x.Name == activeMethodCall);
		}
	}
}
