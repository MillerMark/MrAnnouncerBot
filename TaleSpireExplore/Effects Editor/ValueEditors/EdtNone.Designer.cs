﻿namespace TaleSpireExplore
{
	partial class EdtNone
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbxClassInfo = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbxClassInfo
			// 
			this.tbxClassInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxClassInfo.Location = new System.Drawing.Point(3, 3);
			this.tbxClassInfo.Multiline = true;
			this.tbxClassInfo.Name = "tbxClassInfo";
			this.tbxClassInfo.ReadOnly = true;
			this.tbxClassInfo.Size = new System.Drawing.Size(324, 288);
			this.tbxClassInfo.TabIndex = 0;
			// 
			// EdtNone
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbxClassInfo);
			this.Name = "EdtNone";
			this.Size = new System.Drawing.Size(330, 294);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxClassInfo;
	}
}
