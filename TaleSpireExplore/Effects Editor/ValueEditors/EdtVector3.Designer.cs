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
			this.btnAllHalves = new System.Windows.Forms.Button();
			this.btnAllTwos = new System.Windows.Forms.Button();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
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
			this.tbxVector3.BackColor = System.Drawing.Color.Black;
			this.tbxVector3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxVector3.ForeColor = System.Drawing.Color.White;
			this.tbxVector3.Location = new System.Drawing.Point(51, 10);
			this.tbxVector3.Name = "tbxVector3";
			this.tbxVector3.Size = new System.Drawing.Size(225, 20);
			this.tbxVector3.TabIndex = 1;
			this.tbxVector3.TextChanged += new System.EventHandler(this.tbxVector3_TextChanged);
			// 
			// trkOffsetX
			// 
			this.trkOffsetX.LargeChange = 10;
			this.trkOffsetX.Location = new System.Drawing.Point(90, 203);
			this.trkOffsetX.Maximum = 50;
			this.trkOffsetX.Minimum = -50;
			this.trkOffsetX.Name = "trkOffsetX";
			this.trkOffsetX.Size = new System.Drawing.Size(208, 45);
			this.trkOffsetX.TabIndex = 2;
			this.trkOffsetX.TickFrequency = 10;
			this.trkOffsetX.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetX.Leave += new System.EventHandler(this.trkOffset_Leave);
			this.trkOffsetX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// trkOffsetY
			// 
			this.trkOffsetY.LargeChange = 10;
			this.trkOffsetY.Location = new System.Drawing.Point(282, 6);
			this.trkOffsetY.Maximum = 50;
			this.trkOffsetY.Minimum = -50;
			this.trkOffsetY.Name = "trkOffsetY";
			this.trkOffsetY.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trkOffsetY.Size = new System.Drawing.Size(45, 208);
			this.trkOffsetY.TabIndex = 2;
			this.trkOffsetY.TickFrequency = 10;
			this.trkOffsetY.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trkOffsetY.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// trkOffsetZ
			// 
			this.trkOffsetZ.LargeChange = 10;
			this.trkOffsetZ.Location = new System.Drawing.Point(282, 211);
			this.trkOffsetZ.Maximum = 50;
			this.trkOffsetZ.Minimum = -50;
			this.trkOffsetZ.Name = "trkOffsetZ";
			this.trkOffsetZ.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trkOffsetZ.Size = new System.Drawing.Size(45, 208);
			this.trkOffsetZ.TabIndex = 2;
			this.trkOffsetZ.TickFrequency = 10;
			this.trkOffsetZ.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trkOffsetZ.Scroll += new System.EventHandler(this.trkOffset_Scroll);
			this.trkOffsetZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkOffset_MouseUp);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(69, 200);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 24);
			this.label2.TabIndex = 0;
			this.label2.Text = "x:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(261, 301);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 24);
			this.label3.TabIndex = 0;
			this.label3.Text = "z:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(261, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 24);
			this.label4.TabIndex = 0;
			this.label4.Text = "y:";
			// 
			// btnAllZeros
			// 
			this.btnAllZeros.BackColor = System.Drawing.Color.Black;
			this.btnAllZeros.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnAllZeros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAllZeros.Location = new System.Drawing.Point(7, 36);
			this.btnAllZeros.Name = "btnAllZeros";
			this.btnAllZeros.Size = new System.Drawing.Size(55, 23);
			this.btnAllZeros.TabIndex = 3;
			this.btnAllZeros.Text = "0, 0, 0";
			this.btnAllZeros.UseVisualStyleBackColor = false;
			this.btnAllZeros.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btnAllOnes
			// 
			this.btnAllOnes.BackColor = System.Drawing.Color.Black;
			this.btnAllOnes.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnAllOnes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAllOnes.Location = new System.Drawing.Point(160, 36);
			this.btnAllOnes.Name = "btnAllOnes";
			this.btnAllOnes.Size = new System.Drawing.Size(55, 23);
			this.btnAllOnes.TabIndex = 3;
			this.btnAllOnes.Text = "1, 1, 1";
			this.btnAllOnes.UseVisualStyleBackColor = false;
			this.btnAllOnes.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btnAllHalves
			// 
			this.btnAllHalves.BackColor = System.Drawing.Color.Black;
			this.btnAllHalves.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnAllHalves.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAllHalves.Location = new System.Drawing.Point(68, 36);
			this.btnAllHalves.Name = "btnAllHalves";
			this.btnAllHalves.Size = new System.Drawing.Size(86, 23);
			this.btnAllHalves.TabIndex = 3;
			this.btnAllHalves.Text = "0.5, 0.5, 0.5";
			this.btnAllHalves.UseVisualStyleBackColor = false;
			this.btnAllHalves.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btnAllTwos
			// 
			this.btnAllTwos.BackColor = System.Drawing.Color.Black;
			this.btnAllTwos.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnAllTwos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAllTwos.Location = new System.Drawing.Point(221, 36);
			this.btnAllTwos.Name = "btnAllTwos";
			this.btnAllTwos.Size = new System.Drawing.Size(55, 23);
			this.btnAllTwos.TabIndex = 3;
			this.btnAllTwos.Text = "2, 2, 2";
			this.btnAllTwos.UseVisualStyleBackColor = false;
			this.btnAllTwos.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(73, 335);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(48, 17);
			this.radioButton1.TabIndex = 4;
			this.radioButton1.Text = "x 0.1";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.Click += new System.EventHandler(this.ChangeMultiplier);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Checked = true;
			this.radioButton2.Location = new System.Drawing.Point(73, 358);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(39, 17);
			this.radioButton2.TabIndex = 4;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "x 1";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.Click += new System.EventHandler(this.ChangeMultiplier);
			// 
			// radioButton3
			// 
			this.radioButton3.AutoSize = true;
			this.radioButton3.Location = new System.Drawing.Point(73, 381);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(45, 17);
			this.radioButton3.TabIndex = 4;
			this.radioButton3.Text = "x 10";
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.Click += new System.EventHandler(this.ChangeMultiplier);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(206, 180);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(79, 17);
			this.checkBox2.TabIndex = 6;
			this.checkBox2.Text = "Lock X && Y";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.Visible = false;
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(206, 240);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(79, 17);
			this.checkBox3.TabIndex = 6;
			this.checkBox3.Text = "Lock X && Z";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.Visible = false;
			// 
			// checkBox4
			// 
			this.checkBox4.AutoSize = true;
			this.checkBox4.Location = new System.Drawing.Point(248, 421);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(79, 17);
			this.checkBox4.TabIndex = 6;
			this.checkBox4.Text = "Lock Y && Z";
			this.checkBox4.UseVisualStyleBackColor = true;
			this.checkBox4.Visible = false;
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.Black;
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(7, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(55, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "90, 0, 0";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.Color.Black;
			this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Location = new System.Drawing.Point(68, 65);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(55, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "0, 90, 0";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// button3
			// 
			this.button3.BackColor = System.Drawing.Color.Black;
			this.button3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button3.Location = new System.Drawing.Point(129, 65);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(55, 23);
			this.button3.TabIndex = 3;
			this.button3.Text = "0, 0, 90";
			this.button3.UseVisualStyleBackColor = false;
			this.button3.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// button4
			// 
			this.button4.BackColor = System.Drawing.Color.Black;
			this.button4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button4.Location = new System.Drawing.Point(206, 87);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(49, 23);
			this.button4.TabIndex = 3;
			this.button4.Text = "-90";
			this.button4.UseVisualStyleBackColor = false;
			this.button4.Click += new System.EventHandler(this.changeY_Click);
			// 
			// button5
			// 
			this.button5.BackColor = System.Drawing.Color.Black;
			this.button5.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button5.Location = new System.Drawing.Point(206, 116);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(49, 23);
			this.button5.TabIndex = 3;
			this.button5.Text = "+90";
			this.button5.UseVisualStyleBackColor = false;
			this.button5.Click += new System.EventHandler(this.changeY_Click);
			// 
			// button6
			// 
			this.button6.BackColor = System.Drawing.Color.Black;
			this.button6.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button6.Location = new System.Drawing.Point(15, 187);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(49, 23);
			this.button6.TabIndex = 3;
			this.button6.Text = "-90";
			this.button6.UseVisualStyleBackColor = false;
			this.button6.Click += new System.EventHandler(this.changeX_Click);
			// 
			// button7
			// 
			this.button7.BackColor = System.Drawing.Color.Black;
			this.button7.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button7.Location = new System.Drawing.Point(15, 216);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(49, 23);
			this.button7.TabIndex = 3;
			this.button7.Text = "+90";
			this.button7.UseVisualStyleBackColor = false;
			this.button7.Click += new System.EventHandler(this.changeX_Click);
			// 
			// button8
			// 
			this.button8.BackColor = System.Drawing.Color.Black;
			this.button8.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button8.Location = new System.Drawing.Point(206, 290);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(49, 23);
			this.button8.TabIndex = 3;
			this.button8.Text = "-90";
			this.button8.UseVisualStyleBackColor = false;
			this.button8.Click += new System.EventHandler(this.changeZ_Click);
			// 
			// button9
			// 
			this.button9.BackColor = System.Drawing.Color.Black;
			this.button9.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button9.Location = new System.Drawing.Point(206, 319);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(49, 23);
			this.button9.TabIndex = 3;
			this.button9.Text = "+90";
			this.button9.UseVisualStyleBackColor = false;
			this.button9.Click += new System.EventHandler(this.changeZ_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(48, 315);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Slider Multiplier:";
			// 
			// EdtVector3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkBox4);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.radioButton3);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.btnAllTwos);
			this.Controls.Add(this.btnAllHalves);
			this.Controls.Add(this.btnAllOnes);
			this.Controls.Add(this.button9);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnAllZeros);
			this.Controls.Add(this.trkOffsetY);
			this.Controls.Add(this.trkOffsetZ);
			this.Controls.Add(this.tbxVector3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trkOffsetX);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "EdtVector3";
			this.Size = new System.Drawing.Size(330, 445);
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
		private System.Windows.Forms.Button btnAllHalves;
		private System.Windows.Forms.Button btnAllTwos;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Label label5;
	}
}
