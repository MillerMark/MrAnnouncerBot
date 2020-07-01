//#define profiling
using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.IO;
using System.Collections.Generic;

namespace DHDM
{
	public class FileCompletionProvider : CharacterTriggerProvider
	{
		public string Extension { get; set; }
		public string BaseFolder { get; set; }
		public bool AllowSubFolders { get; set; }
		public FileCompletionProvider()
		{

		}

		public override bool ShouldComplete(TextArea textArea, char lastKeyPressed, string requestedProviderName)
		{
			return (lastKeyPressed == '"' || lastKeyPressed == '/') && ProviderName == requestedProviderName;
		}

		public override CompletionWindow Complete(TextArea textArea)
		{
			try
			{
				int offset = textArea.Caret.Offset;

				string filter = textArea.Document.GetStringLeftOf(offset);


				string fullPattern = $"{BaseFolder}{filter}*.{Extension}";
				string filePattern = Path.GetFileName(fullPattern);
				string folderName = Path.GetDirectoryName(fullPattern);
				string[] files = Directory.GetFiles(folderName, filePattern);

				string[] directories = null;
				if (AllowSubFolders)
				{
					directories = Directory.GetDirectories(folderName);
					if ((directories == null || directories.Length == 0) && (files == null || files.Length == 0))
						return null;
				}
				else
				{
					if (files == null || files.Length == 0)
						return null;
				}

				CompletionWindow completionWindow = new CompletionWindow(textArea);
				IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
				if (files != null)
					foreach (string fileName in files)
					{
						data.Add(new FileCompletionData(fileName));
					}
				if (directories != null)
					foreach (string directoryName in directories)
					{
						data.Add(new FolderCompletionData(directoryName));
					}

				return completionWindow;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
