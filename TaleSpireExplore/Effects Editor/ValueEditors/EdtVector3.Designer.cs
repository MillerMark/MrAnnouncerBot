namespace TaleSpireExplore
{
	partial class EdtVector3
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
			this.tbxVector3 = new System.Windows.Forms.TextBox();
			this.trkOffsetX = new System.Windows.Forms.TrackBar();
			this.trkOffsetY = new System.Windows.Forms.TrackBar();
			this.trkOffsetZ = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnAllZeros = new System.Windows.Forms.Button();
			this.btnAllOnes = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetZ)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "x, y, z:";
			// 
			// tbxVector3
			// 
			this.tbxVector3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxVector3.Location = new System.Drawing.Point(51, 10);
			this.tbxVector3.Name = "tbxVector3";
			this.tbxVector3.Size = new System.Drawing.Size(125, 20);
			this.tbxVector3.TabIndex = 1;
			this.tbxVector3.TextChanged += new System.EventHandler(this.tbxVector3_TextChanged);
			// 
			// trkOffsetX
			// 
			this.trkOffsetX.LargeChange = 10;
			this.trkOffsetX.Location = new System.Drawing.Point(14, 161);
			this.trkOffsetX.Maximum = 50;
			this.trkOffsetX.Minimum = -50;
			this.trkOffsetX.Name = "trkOffsetX";
			this.trkOffsetX.Size = new System.Drawing.Size(175, 45);
			this.trkOffsetX.TabIndex = 2;
			this.trkOffsetX.TickFrequency = 10;
			this.trkOffsetX.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetX.Leave += new System.EventHandler(this.trkOffset_Leave);
			this.trkOffsetX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// trkOffsetY
			// 
			this.trkOffsetY.LargeChange = 10;
			this.trkOffsetY.Location = new System.Drawing.Point(183, 3);
			this.trkOffsetY.Maximum = 50;
			this.trkOffsetY.Minimum = -50;
			this.trkOffsetY.Name = "trkOffsetY";
			this.trkOffsetY.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trkOffsetY.Size = new System.Drawing.Size(45, 165);
			this.trkOffsetY.TabIndex = 2;
			this.trkOffsetY.TickFrequency = 10;
			this.trkOffsetY.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trkOffsetY.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// trkOffsetZ
			// 
			this.trkOffsetZ.LargeChange = 10;
			this.trkOffsetZ.Location = new System.Drawing.Point(183, 174);
			this.trkOffsetZ.Maximum = 50;
			this.trkOffsetZ.Minimum = -50;
			this.trkOffsetZ.Name = "trkOffsetZ";
			this.trkOffsetZ.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trkOffsetZ.Size = new System.Drawing.Size(45, 165);
			this.trkOffsetZ.TabIndex = 2;
			this.trkOffsetZ.TickFrequency = 10;
			this.trkOffsetZ.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trkOffsetZ.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 164);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "x:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(173, 248);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "z:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(173, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(15, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "y:";
			// 
			// btnAllZeros
			// 
			this.btnAllZeros.Location = new System.Drawing.Point(7, 36);
			this.btnAllZeros.Name = "btnAllZeros";
			this.btnAllZeros.Size = new System.Drawing.Size(55, 23);
			this.btnAllZeros.TabIndex = 3;
			this.btnAllZeros.Text = "0, 0, 0";
			this.btnAllZeros.UseVisualStyleBackColor = true;
			this.btnAllZeros.Click += new System.EventHandler(this.btnAllZeros_Click);
			// 
			// btnAllOnes
			// 
			this.btnAllOnes.Location = new System.Drawing.Point(7, 67);
			this.btnAllOnes.Name = "btnAllOnes";
			this.btnAllOnes.Size = new System.Drawing.Size(55, 23);
			this.btnAllOnes.TabIndex = 3;
			this.btnAllOnes.Text = "1, 1, 1";
			this.btnAllOnes.UseVisualStyleBackColor = true;
			this.btnAllOnes.Click += new System.EventHandler(this.btnAllOnes_Click);
			// 
			// EdtVector3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnAllOnes);
			this.Controls.Add(this.btnAllZeros);
			this.Controls.Add(this.trkOffsetY);
			this.Controls.Add(this.trkOffsetZ);
			this.Controls.Add(this.tbxVector3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trkOffsetX);
			this.Name = "EdtVector3";
			this.Size = new System.Drawing.Size(230, 345);
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetZ)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxVector3;
		private System.Windows.Forms.TrackBar trkOffsetX;
		private System.Windows.Forms.TrackBar trkOffsetY;
		private System.Windows.Forms.TrackBar trkOffsetZ;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnAllZeros;
		private System.Windows.Forms.Button btnAllOnes;
	}
}
