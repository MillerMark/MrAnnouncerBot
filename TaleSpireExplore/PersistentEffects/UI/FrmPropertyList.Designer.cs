namespace TaleSpireExplore
{
	partial class FrmPropertyList
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
			this.lstProperties = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lstProperties
			// 
			this.lstProperties.BackColor = System.Drawing.Color.Black;
			this.lstProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstProperties.Font = new System.Drawing.Font("Georgia", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstProperties.ForeColor = System.Drawing.Color.White;
			this.lstProperties.FormattingEnabled = true;
			this.lstProperties.ItemHeight = 16;
			this.lstProperties.Location = new System.Drawing.Point(0, 0);
			this.lstProperties.Name = "lstProperties";
			this.lstProperties.Size = new System.Drawing.Size(143, 96);
			this.lstProperties.TabIndex = 0;
			this.lstProperties.SelectedIndexChanged += new System.EventHandler(this.lstProperties_SelectedIndexChanged);
			// 
			// FrmPropertyList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(144, 117);
			this.ControlBox = false;
			this.Controls.Add(this.lstProperties);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmPropertyList";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FrmPropertyList_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lstProperties;
	}
}