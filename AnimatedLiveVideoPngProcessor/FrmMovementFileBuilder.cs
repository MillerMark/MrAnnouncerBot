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

		private void btnConvertToMovementFile_Click(object sender, EventArgs e)
		{
			if (filesToProcess == null)
			{
				MessageBox.Show("Select files first!");
				return;
			}

			List<VisualProcessingResults> results = new List<VisualProcessingResults>();
			const double frameRate = 29.97;  // frames per second
			const double intervalBetweenFramesSeconds = 1 /* frame */ / frameRate;

			VisualProcessingResults lastProcessImageResults = null;
			foreach (string file in filesToProcess)
			{
				VisualProcessingResults processImageResults = TestImageHelper.ProcessImage(file);
				
				if (processImageResults.Matches(lastProcessImageResults))
					lastProcessImageResults.Duration += intervalBetweenFramesSeconds;
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
		}
	}
}
