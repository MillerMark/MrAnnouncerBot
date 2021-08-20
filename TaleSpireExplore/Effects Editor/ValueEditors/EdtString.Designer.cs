namespace TaleSpireExplore
{
	partial class EdtString
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
			this.txtStringValue = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtStringValue
			// 
			this.txtStringValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtStringValue.Location = new System.Drawing.Point(3, 3);
			this.txtStringValue.Name = "txtStringValue";
			this.txtStringValue.Size = new System.Drawing.Size(324, 20);
			this.txtStringValue.TabIndex = 0;
			this.txtStringValue.TextChanged += new System.EventHandler(this.txtStringValue_TextChanged);
			// 
			// EdtString
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtStringValue);
			this.Name = "EdtString";
			this.Size = new System.Drawing.Size(330, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtStringValue;
	}
}
