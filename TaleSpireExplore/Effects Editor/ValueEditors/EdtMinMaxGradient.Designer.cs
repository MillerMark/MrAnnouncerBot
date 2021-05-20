namespace TaleSpireExplore
{
	partial class EdtMinMaxGradient
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
			this.rbColor = new System.Windows.Forms.RadioButton();
			this.rbGradient = new System.Windows.Forms.RadioButton();
			this.rbTwoColor = new System.Windows.Forms.RadioButton();
			this.rbTwoGradients = new System.Windows.Forms.RadioButton();
			this.rbRandomColor = new System.Windows.Forms.RadioButton();
			this.btnSetColor1 = new System.Windows.Forms.Button();
			this.btnSetColor2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// rbColor
			// 
			this.rbColor.AutoSize = true;
			this.rbColor.Location = new System.Drawing.Point(20, 16);
			this.rbColor.Name = "rbColor";
			this.rbColor.Size = new System.Drawing.Size(49, 17);
			this.rbColor.TabIndex = 0;
			this.rbColor.TabStop = true;
			this.rbColor.Text = "Color";
			this.rbColor.UseVisualStyleBackColor = true;
			this.rbColor.CheckedChanged += new System.EventHandler(this.ColorMode_CheckedChanged);
			// 
			// rbGradient
			// 
			this.rbGradient.AutoSize = true;
			this.rbGradient.Location = new System.Drawing.Point(20, 39);
			this.rbGradient.Name = "rbGradient";
			this.rbGradient.Size = new System.Drawing.Size(65, 17);
			this.rbGradient.TabIndex = 1;
			this.rbGradient.TabStop = true;
			this.rbGradient.Text = "Gradient";
			this.rbGradient.UseVisualStyleBackColor = true;
			this.rbGradient.CheckedChanged += new System.EventHandler(this.ColorMode_CheckedChanged);
			// 
			// rbTwoColor
			// 
			this.rbTwoColor.AutoSize = true;
			this.rbTwoColor.Location = new System.Drawing.Point(20, 62);
			this.rbTwoColor.Name = "rbTwoColor";
			this.rbTwoColor.Size = new System.Drawing.Size(78, 17);
			this.rbTwoColor.TabIndex = 2;
			this.rbTwoColor.TabStop = true;
			this.rbTwoColor.Text = "Two Colors";
			this.rbTwoColor.UseVisualStyleBackColor = true;
			this.rbTwoColor.CheckedChanged += new System.EventHandler(this.ColorMode_CheckedChanged);
			// 
			// rbTwoGradients
			// 
			this.rbTwoGradients.AutoSize = true;
			this.rbTwoGradients.Location = new System.Drawing.Point(20, 85);
			this.rbTwoGradients.Name = "rbTwoGradients";
			this.rbTwoGradients.Size = new System.Drawing.Size(94, 17);
			this.rbTwoGradients.TabIndex = 2;
			this.rbTwoGradients.TabStop = true;
			this.rbTwoGradients.Text = "Two Gradients";
			this.rbTwoGradients.UseVisualStyleBackColor = true;
			this.rbTwoGradients.CheckedChanged += new System.EventHandler(this.ColorMode_CheckedChanged);
			// 
			// rbRandomColor
			// 
			this.rbRandomColor.AutoSize = true;
			this.rbRandomColor.Location = new System.Drawing.Point(20, 108);
			this.rbRandomColor.Name = "rbRandomColor";
			this.rbRandomColor.Size = new System.Drawing.Size(92, 17);
			this.rbRandomColor.TabIndex = 2;
			this.rbRandomColor.TabStop = true;
			this.rbRandomColor.Text = "Random Color";
			this.rbRandomColor.UseVisualStyleBackColor = true;
			this.rbRandomColor.CheckedChanged += new System.EventHandler(this.ColorMode_CheckedChanged);
			// 
			// btnSetColor1
			// 
			this.btnSetColor1.Location = new System.Drawing.Point(143, 10);
			this.btnSetColor1.Name = "btnSetColor1";
			this.btnSetColor1.Size = new System.Drawing.Size(75, 23);
			this.btnSetColor1.TabIndex = 3;
			this.btnSetColor1.UseVisualStyleBackColor = true;
			this.btnSetColor1.Click += new System.EventHandler(this.btnSetColor1_Click);
			// 
			// btnSetColor2
			// 
			this.btnSetColor2.Location = new System.Drawing.Point(143, 39);
			this.btnSetColor2.Name = "btnSetColor2";
			this.btnSetColor2.Size = new System.Drawing.Size(75, 23);
			this.btnSetColor2.TabIndex = 3;
			this.btnSetColor2.UseVisualStyleBackColor = true;
			this.btnSetColor2.Click += new System.EventHandler(this.btnSetColor2_Click);
			// 
			// EdtMinMaxGradient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSetColor2);
			this.Controls.Add(this.btnSetColor1);
			this.Controls.Add(this.rbRandomColor);
			this.Controls.Add(this.rbTwoGradients);
			this.Controls.Add(this.rbTwoColor);
			this.Controls.Add(this.rbGradient);
			this.Controls.Add(this.rbColor);
			this.Name = "EdtMinMaxGradient";
			this.Size = new System.Drawing.Size(230, 141);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton rbColor;
		private System.Windows.Forms.RadioButton rbGradient;
		private System.Windows.Forms.RadioButton rbTwoColor;
		private System.Windows.Forms.RadioButton rbTwoGradients;
		private System.Windows.Forms.RadioButton rbRandomColor;
		private System.Windows.Forms.Button btnSetColor1;
		private System.Windows.Forms.Button btnSetColor2;
	}
}
