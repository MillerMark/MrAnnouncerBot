using Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnimatedLiveVideoPngProcessor
{
	public partial class FrmMovementFileBuilder : Form
	{
		public FrmMovementFileBuilder()
		{
			InitializeComponent();
		}

		void ProcessFiles(string fileName)
		{
			string directoryName, rootName;
			List<string> files;
			ImageUtils.GetFilesToAnalyze(fileName, out directoryName, out rootName, out files);
			
			
			
			string movementFileName = System.IO.Path.Combine(directoryName, rootName + ".movement");

			// TODO: Process *.pngs to collect the movement data.
			// TODO: Do we delete all *.pngs?

			// TODO: Save the movement data...
			//System.IO.File.WriteAllText(movementFileName, $"Left = {baseMargins.Left}{Environment.NewLine}Top = {baseMargins.Top}");

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
	}
}
