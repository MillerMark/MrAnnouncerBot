using DndCore;
using ICSharpCode.AvalonEdit;
using SharedCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AvalonEdit
{
	/// <summary>
	/// Interaction logic for AvalonEditor.xaml
	/// </summary>
	public partial class AvalonEditor : UserControl
	{
		public event EventHandler CodeChanged;
		public event KeyEventHandler PreviewEditorKeyDown;
		public event EventHandler RequestTextSave;
		public AvalonEditor()
		{
			InitializeComponent();
			ShowTooltipCommand.ShowParameterTooltipIfNecessary += ShowTooltipCommand_ShowParameterTooltipIfNecessary;
			CodeCompletionCommand.ExpansionCompleted += CodeCompletionCommand_ExpansionCompleted;
			TextEditor.PreviewKeyDown += TextEditor_PreviewKeyDown;
		}

		public static readonly DependencyProperty ShowSaveBarProperty = DependencyProperty.Register("ShowSaveBar", typeof(bool), typeof(AvalonEditor), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowSaveBarChanged)));

		private static void OnShowSaveBarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			AvalonEditor avalonEditor = o as AvalonEditor;
			if (avalonEditor != null)
				avalonEditor.OnShowSaveBarChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		protected virtual void OnShowSaveBarChanged(bool oldValue, bool newValue)
		{
			if (newValue)
				spSaveStatus.Visibility = Visibility.Visible;
			else
				spSaveStatus.Visibility = Visibility.Collapsed;
		}

		protected virtual void OnRequestTextSave(object sender, EventArgs e)
		{
			RequestTextSave?.Invoke(sender, e);
		}

		private void TextCompletionEngine_RequestTextSave(object sender, EventArgs e)
		{
			OnRequestTextSave(this, e);
		}

		private void TextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			OnPreviewKeyDown(this, e);
		}

		protected virtual void OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			PreviewEditorKeyDown?.Invoke(sender, e);
		}

		protected virtual void OnCodeChanged(object sender, EventArgs e)
		{
			CodeChanged?.Invoke(sender, e);
		}
		public bool ShowSaveBar
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(ShowSaveBarProperty);
			}
			set
			{
				SetValue(ShowSaveBarProperty, value);
			}
		}
		public TextEditor TextEditor
		{
			get
			{
				return tbxCode;
			}
		}

		public TextCompletionEngine TextCompletionEngine { get; set; }

		public void ShowStatusSavingCode()
		{
			tbStatus.Visibility = Visibility.Visible;
			iconSaving.Visibility = Visibility.Visible;
			iconSaved.Visibility = Visibility.Hidden;
		}

		public void ShowStatusCodeIsSaved()
		{
			tbStatus.Visibility = Visibility.Hidden;
			iconSaving.Visibility = Visibility.Hidden;
			iconSaved.Visibility = Visibility.Visible;
		}

		public void HideAllCodeChangedStatusUI()
		{
			tbStatus.Visibility = Visibility.Hidden;
			iconSaving.Visibility = Visibility.Hidden;
			iconSaved.Visibility = Visibility.Hidden;
		}

		public bool CodeCompletionWindowIsUp()
		{
			if (TextCompletionEngine.CompletionWindow == null)
				return false;

			return TextCompletionEngine.CompletionWindow.Visibility == Visibility.Visible;
		}

		int updateCount;

		public void BeginUpdate()
		{
			updateCount++;
		}

		public void EndUpdate()
		{
			updateCount--;
		}

		public bool changingInternally
		{
			get
			{
				return updateCount > 0;
			}
		}

		TemplateEngine templateEngine;

		public TemplateEngine TemplateEngine
		{
			get
			{
				if (templateEngine == null)
					templateEngine = new TemplateEngine();
				return templateEngine;
			}
		}

		object lastParameterTooltip;

		private Size MeasureString(string candidate)
		{
			var formattedText = new FormattedText(
					candidate,
					CultureInfo.CurrentCulture,
					FlowDirection.LeftToRight,
					new Typeface(TextEditor.FontFamily, TextEditor.FontStyle, TextEditor.FontWeight, TextEditor.FontStretch),
					TextEditor.FontSize,
					Brushes.Black,
					new NumberSubstitution(),
					1);

			return new Size(formattedText.Width, formattedText.Height);
		}

		double spaceWidth = 0;
		int lastLineShownTooltip;
		void ShowParameterTooltip(object content, int parameterStartOffset)
		{
			if (content == null)
				return;
			int thisLine = TextEditor.TextArea.Caret.Line;
			if (content is string && (string)lastParameterTooltip == (string)content)
				if (lastLineShownTooltip == thisLine)
					if (TextCompletionEngine.ParameterToolTip.IsOpen)
						return;

			lastLineShownTooltip = thisLine;
			lastParameterTooltip = content;
			if (spaceWidth == 0)
				spaceWidth = MeasureString("M").Width;
			double adjustLeft = TextEditor.TextArea.Caret.Offset - parameterStartOffset - 0.5;
			Rect caret = TextEditor.TextArea.Caret.CalculateCaretRectangle();
			TextCompletionEngine.ParameterToolTip.HorizontalOffset = Math.Round(caret.Right - adjustLeft * spaceWidth);
			TextCompletionEngine.ParameterToolTip.VerticalOffset = caret.Bottom + 9;
			TextCompletionEngine.ParameterToolTip.Content = content;
			TextCompletionEngine.ParameterToolTip.IsOpen = true;
			if (TextCompletionEngine.CompletionWindow != null && TextCompletionEngine.CompletionWindow.IsVisible)
			{
				TextCompletionEngine.CompletionWindow.Top += TextCompletionEngine.ParameterToolTip.ActualHeight;
			}
		}

		void HideParameterTooltip()
		{
			TextCompletionEngine.ParameterToolTip.IsOpen = false;
		}

		private void TbxCode_AvalonTextChanged(object sender, EventArgs e)
		{
			if (!changingInternally)
				ShowStatusSavingCode();
			OnCodeChanged(this, EventArgs.Empty);
		}

		private void CodeCompletionCommand_ExpansionCompleted(object sender, TextAreaEventArgs ea)
		{
			InvokeCodeCompletion();
		}

		private void ShowTooltipCommand_ShowParameterTooltipIfNecessary(object sender, TextAreaEventArgs ea)
		{
			ShowParameterTooltipIfNecessary();
		}

		private void TbxCode_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && Modifiers.NoModifiersDown)
			{
				CodeTemplate templateToExpand = TemplateEngine.GetTemplateToExpand(TextEditor);
				if (templateToExpand != null)
				{
					e.Handled = true;
					TextCompletionEngine.HideCodeCompletionWindow();
					TemplateEngine.ExpandTemplate(TextEditor, templateToExpand);
				}
			}
			else if (IsNavKey(e.Key) || e.Key == Key.OemComma && Modifiers.NoModifiersDown)
			{
				StartTooltipTimer();
			}
		}

		bool IsNavKey(Key key)
		{
			switch (key)
			{
				case Key.End:
				case Key.Home:
				case Key.Left:
				case Key.Up:
				case Key.Right:
				case Key.Down:
				case Key.PageUp:
				case Key.PageDown:
					return true;
			}
			return false;
		}

		DispatcherTimer tooltipTimer;

		private void StartTooltipTimer()
		{
			if (tooltipTimer == null)
			{
				CreateToolipTimer();
			}
			tooltipTimer.Start();
		}

		private void CreateToolipTimer()
		{
			tooltipTimer = new DispatcherTimer();
			tooltipTimer.Interval = TimeSpan.FromMilliseconds(50);
			tooltipTimer.Tick += TooltipTimer_Tick;
		}

		string GetParameterTooltip(int parameterNumber)
		{
			DndFunction dndFunction = TextCompletionEngine.GetActiveDndFunction(TextEditor.TextArea);
			if (dndFunction == null)
				return null;

			IEnumerable<ParamAttribute> customAttributes = dndFunction.GetType().GetCustomAttributes(typeof(ParamAttribute)).Cast<ParamAttribute>().ToList();
			if (customAttributes == null)
				return null;

			ParamAttribute paramAttribute = customAttributes.FirstOrDefault(x => x.Index == parameterNumber);
			if (paramAttribute == null)
				return null;

			return paramAttribute.Description;
		}

		private void TooltipTimer_Tick(object sender, EventArgs e)
		{
			tooltipTimer.Stop();
			Dispatcher.Invoke(() =>
			{
				ShowParameterTooltipIfNecessary();
			});
		}

		int GetParameterNumber()
		{
			return TextEditor.Document.GetParameterNumberAtPosition(TextEditor.TextArea.Caret.Offset);
		}

		int GetParameterStartOffset()
		{
			return TextEditor.Document.GetParameterStartOffset(TextEditor.TextArea.Caret.Offset);
		}

		public void ShowParameterTooltipIfNecessary()
		{
			int parameterNumber = GetParameterNumber();
			if (parameterNumber > 0)
			{
				int parameterStartOffset = GetParameterStartOffset();
				string parameterTooltip = GetParameterTooltip(parameterNumber);
				ShowParameterTooltip(parameterTooltip, parameterStartOffset);
			}
			else
				HideParameterTooltip();
		}

		void LoadAvalonSyntaxHighlighter()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"{nameof(AvalonEdit)}.CSharp.xml";

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream, new System.Xml.XmlReaderSettings()))
			{
				TextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
			}
		}
		public void InvokeCodeCompletion()
		{
			TextCompletionEngine.InvokeCodeCompletion();
		}

		void LoadTextCompletionEngine()
		{
			if (TextCompletionEngine != null)
				TextCompletionEngine.RequestTextSave -= TextCompletionEngine_RequestTextSave;

			TextCompletionEngine = new TextCompletionEngine(TextEditor);
			TextCompletionEngine.ReloadShortcuts();
			TextCompletionEngine.RequestTextSave += TextCompletionEngine_RequestTextSave;
		}

		private void TbxCode_KeyDown(object sender, KeyEventArgs e)
		{

		}

		public void Load()
		{
			LoadAvalonSyntaxHighlighter();
			LoadTextCompletionEngine();
		}
		public void ReloadTemplates()
		{
			TemplateEngine.ReloadTemplates();
		}
		public void ReloadShortcuts()
		{
			TextCompletionEngine.ReloadShortcuts();
		}

		private void TbxCode_MouseDown(object sender, MouseButtonEventArgs e)
		{
			StartTooltipTimer();
		}

		public void SetText(string code)
		{
			TextCompletionEngine.SetText(code);
		}

		public void RefreshCompletionProviders()
		{
			TextCompletionEngine.RefreshCompletionProviders();
		}

	}
}
