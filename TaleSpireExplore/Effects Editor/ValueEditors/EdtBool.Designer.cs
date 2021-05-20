namespace TaleSpireExplore
{
	partial class EdtBool
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
			this.rbFalse = new System.Windows.Forms.RadioButton();
			this.rbTrue = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// rbFalse
			// 
			this.rbFalse.AutoSize = true;
			this.rbFalse.Location = new System.Drawing.Point(14, 13);
			this.rbFalse.Name = "rbFalse";
			this.rbFalse.Size = new System.Drawing.Size(50, 17);
			this.rbFalse.TabIndex = 0;
			this.rbFalse.TabStop = true;
			this.rbFalse.Text = "False";
			this.rbFalse.UseVisualStyleBackColor = true;
			this.rbFalse.CheckedChanged += new System.EventHandler(this.Bool_CheckedChanged);
			// 
			// rbTrue
			// 
			this.rbTrue.AutoSize = true;
			this.rbTrue.Location = new System.Drawing.Point(14, 36);
			this.rbTrue.Name = "rbTrue";
			this.rbTrue.Size = new System.Drawing.Size(47, 17);
			this.rbTrue.TabIndex = 0;
			this.rbTrue.TabStop = true;
			this.rbTrue.Text = "True";
			this.rbTrue.UseVisualStyleBackColor = true;
			this.rbTrue.CheckedChanged += new System.EventHandler(this.Bool_CheckedChanged);
			// 
			// EdtBool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.rbTrue);
			this.Controls.Add(this.rbFalse);
			this.Name = "EdtBool";
			this.Size = new System.Drawing.Size(230, 66);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton rbFalse;
		private System.Windows.Forms.RadioButton rbTrue;
	}
}
