using Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualProcessingTests;

namespace AnimatedLiveVideoPngProcessor
{
	public partial class FrmMovementFileBuilder : Form
	{
		public FrmMovementFileBuilder()
		{
			InitializeComponent();
		}

		List<string> filesToProcess;
		string movementFileName;
		void ProcessFiles(string fileName)
		{
			string directoryName, rootName;
			ImageUtils.GetFilesToAnalyze(fileName, out directoryName, out rootName, out filesToProcess);

			movementFileName = System.IO.Path.Combine(directoryName, rootName + ".movement");
		}

		private void btnSelectFiles_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = openFileDialog.ShowDialog(this);
			if (dialogResult == DialogResult.OK)
			{
				openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
				ProcessFiles(openFileDialog.FileName);
			}
		}

		void ShowProgress(int progressPercent)
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(() => ShowProgress(progressPercent)));
			}
			else
			{
				prgConvertingFiles.Value = progressPercent;
			}
		}

		private void btnConvertToMovementFile_Click(object sender, EventArgs e)
		{
			prgConvertingFiles.Visible = true;
			if (filesToProcess == null)
			{
				MessageBox.Show("Select files first!");
				return;
			}

			List<ObsTransform> results = new List<ObsTransform>();
			const double frameRate = 30;  // 30 frames per second
			const double intervalBetweenFramesSeconds = 1 /* frame */ / frameRate;
			
			ObsTransform lastProcessImageResults = null;
			int frameIndex = 0;
			int totalFilesToProcess = filesToProcess.Count;
			foreach (string file in filesToProcess)
			{
				ObsTransform processImageResults = TestImageHelper.ProcessImage(file);
				processImageResults.FrameIndex = frameIndex;
				frameIndex++;
				ShowProgress(100 * frameIndex / totalFilesToProcess);

				if (processImageResults.Matches(lastProcessImageResults))
				{
					lastProcessImageResults.Duration += intervalBetweenFramesSeconds;
					lastProcessImageResults.FrameCount++;
				}
				else
				{
					processImageResults.Duration = intervalBetweenFramesSeconds;
					results.Add(processImageResults);
					lastProcessImageResults = processImageResults;
				}
			}

			// TODO: Do we delete all *.pngs?

			// Save the movement data...
			string serializedObject = JsonConvert.SerializeObject(results, Formatting.Indented);
			System.IO.File.WriteAllText(movementFileName, serializedObject);
			prgConvertingFiles.Visible = false;
		}
	}
}
