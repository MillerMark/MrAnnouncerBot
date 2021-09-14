﻿
namespace AnimatedLiveVideoPngProcessor
{
	partial class FrmMovementFileBuilder
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSelectFiles = new System.Windows.Forms.Button();
			this.btnConvertToMovementFile = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// btnSelectFiles
			// 
			this.btnSelectFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelectFiles.Location = new System.Drawing.Point(0, 0);
			this.btnSelectFiles.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnSelectFiles.Name = "btnSelectFiles";
			this.btnSelectFiles.Size = new System.Drawing.Size(113, 32);
			this.btnSelectFiles.TabIndex = 0;
			this.btnSelectFiles.Text = "Select Files...";
			this.btnSelectFiles.UseVisualStyleBackColor = true;
			this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
			// 
			// btnConvertToMovementFile
			// 
			this.btnConvertToMovementFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConvertToMovementFile.Location = new System.Drawing.Point(130, 0);
			this.btnConvertToMovementFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnConvertToMovementFile.Name = "btnConvertToMovementFile";
			this.btnConvertToMovementFile.Size = new System.Drawing.Size(206, 36);
			this.btnConvertToMovementFile.TabIndex = 1;
			this.btnConvertToMovementFile.Text = "Convert to Movement File";
			this.btnConvertToMovementFile.UseVisualStyleBackColor = true;
			this.btnConvertToMovementFile.Click += new System.EventHandler(this.btnConvertToMovementFile_Click);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(0, 41);
			this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(494, 315);
			this.panel1.TabIndex = 2;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "*.png";
			this.openFileDialog.Filter = "Png files|*.png";
			this.openFileDialog.InitialDirectory = "D:\\Dropbox\\DX\\Twitch\\Assets\\AnimatedLiveVideo";
			this.openFileDialog.Title = "Open Animation";
			// 
			// FrmMovementFileBuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 366);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnConvertToMovementFile);
			this.Controls.Add(this.btnSelectFiles);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "FrmMovementFileBuilder";
			this.Text = "Png To Movement";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSelectFiles;
		private System.Windows.Forms.Button btnConvertToMovementFile;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
	}
}

