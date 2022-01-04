namespace TaleSpireExplore
{
	partial class EdtFloat
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbxValue = new System.Windows.Forms.TextBox();
			this.trkValue = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.trkValue)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Value:";
			// 
			// tbxValue
			// 
			this.tbxValue.Location = new System.Drawing.Point(55, 8);
			this.tbxValue.Name = "tbxValue";
			this.tbxValue.Size = new System.Drawing.Size(260, 20);
			this.tbxValue.TabIndex = 1;
			this.tbxValue.TextChanged += new System.EventHandler(this.tbxValue_TextChanged);
			// 
			// trkValue
			// 
			this.trkValue.Location = new System.Drawing.Point(47, 34);
			this.trkValue.Name = "trkValue";
			this.trkValue.Size = new System.Drawing.Size(276, 45);
			this.trkValue.TabIndex = 2;
			this.trkValue.Value = 5;
			this.trkValue.Visible = false;
			this.trkValue.Scroll += new System.EventHandler(this.trkValue_Scroll);
			this.trkValue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkValue_MouseUp);
			// 
			// EdtFloat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbxValue);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trkValue);
			this.Name = "EdtFloat";
			this.Size = new System.Drawing.Size(330, 69);
			((System.ComponentModel.ISupportInitialize)(this.trkValue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxValue;
		private System.Windows.Forms.TrackBar trkValue;
	}
}
