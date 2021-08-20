namespace TaleSpireExplore
{
	partial class EdtColor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EdtColor));
			this.btnSetColor = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tbxHtml = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.trkHue = new System.Windows.Forms.TrackBar();
			this.trkSat = new System.Windows.Forms.TrackBar();
			this.trkLight = new System.Windows.Forms.TrackBar();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label5 = new System.Windows.Forms.Label();
			this.trkMultiplier = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkMultiplier)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSetColor
			// 
			this.btnSetColor.Location = new System.Drawing.Point(9, 8);
			this.btnSetColor.Name = "btnSetColor";
			this.btnSetColor.Size = new System.Drawing.Size(139, 32);
			this.btnSetColor.TabIndex = 3;
			this.btnSetColor.UseVisualStyleBackColor = true;
			this.btnSetColor.Click += new System.EventHandler(this.btnSetColor_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "HTML:";
			// 
			// tbxHtml
			// 
			this.tbxHtml.Location = new System.Drawing.Point(48, 49);
			this.tbxHtml.Name = "tbxHtml";
			this.tbxHtml.Size = new System.Drawing.Size(100, 20);
			this.tbxHtml.TabIndex = 5;
			this.tbxHtml.TextChanged += new System.EventHandler(this.tbxHtml_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Hue:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 137);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Sat:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 190);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(33, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Light:";
			// 
			// trkHue
			// 
			this.trkHue.Location = new System.Drawing.Point(43, 77);
			this.trkHue.Maximum = 100;
			this.trkHue.Name = "trkHue";
			this.trkHue.Size = new System.Drawing.Size(284, 45);
			this.trkHue.TabIndex = 7;
			this.trkHue.TickFrequency = 10;
			this.trkHue.Scroll += new System.EventHandler(this.trkColor_Scroll);
			// 
			// trkSat
			// 
			this.trkSat.Location = new System.Drawing.Point(43, 133);
			this.trkSat.Maximum = 100;
			this.trkSat.Name = "trkSat";
			this.trkSat.Size = new System.Drawing.Size(284, 45);
			this.trkSat.TabIndex = 7;
			this.trkSat.TickFrequency = 10;
			this.trkSat.Scroll += new System.EventHandler(this.trkColor_Scroll);
			// 
			// trkLight
			// 
			this.trkLight.Location = new System.Drawing.Point(43, 187);
			this.trkLight.Maximum = 100;
			this.trkLight.Name = "trkLight";
			this.trkLight.Size = new System.Drawing.Size(284, 45);
			this.trkLight.TabIndex = 7;
			this.trkLight.TickFrequency = 10;
			this.trkLight.Scroll += new System.EventHandler(this.trkColor_Scroll);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(57, 98);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(260, 12);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(7, 262);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Multiplier:";
			// 
			// trkMultiplier
			// 
			this.trkMultiplier.Location = new System.Drawing.Point(57, 254);
			this.trkMultiplier.Maximum = 200;
			this.trkMultiplier.Minimum = 10;
			this.trkMultiplier.Name = "trkMultiplier";
			this.trkMultiplier.Size = new System.Drawing.Size(270, 45);
			this.trkMultiplier.TabIndex = 9;
			this.trkMultiplier.TickFrequency = 10;
			this.trkMultiplier.Value = 10;
			this.trkMultiplier.Scroll += new System.EventHandler(this.trkMultiplier_Scroll);
			// 
			// EdtColor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.trkMultiplier);
			this.Controls.Add(this.trkLight);
			this.Controls.Add(this.trkSat);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxHtml);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSetColor);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.trkHue);
			this.Name = "EdtColor";
			this.Size = new System.Drawing.Size(330, 445);
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkMultiplier)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnSetColor;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxHtml;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TrackBar trkHue;
		private System.Windows.Forms.TrackBar trkSat;
		private System.Windows.Forms.TrackBar trkLight;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TrackBar trkMultiplier;
	}
}
