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
			this.btn90x = new System.Windows.Forms.Button();
			this.btn90y = new System.Windows.Forms.Button();
			this.btn90z = new System.Windows.Forms.Button();
			this.btnY270 = new System.Windows.Forms.Button();
			this.btnY90 = new System.Windows.Forms.Button();
			this.btnX270 = new System.Windows.Forms.Button();
			this.btnX90 = new System.Windows.Forms.Button();
			this.btnZ270 = new System.Windows.Forms.Button();
			this.btnZ90 = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.rbScale = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.rbRotate = new System.Windows.Forms.RadioButton();
			this.rbTranslate = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetZ)).BeginInit();
			this.panel1.SuspendLayout();
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
			// btn90x
			// 
			this.btn90x.BackColor = System.Drawing.Color.Black;
			this.btn90x.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btn90x.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn90x.Location = new System.Drawing.Point(7, 65);
			this.btn90x.Name = "btn90x";
			this.btn90x.Size = new System.Drawing.Size(55, 23);
			this.btn90x.TabIndex = 3;
			this.btn90x.Text = "90, 0, 0";
			this.btn90x.UseVisualStyleBackColor = false;
			this.btn90x.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btn90y
			// 
			this.btn90y.BackColor = System.Drawing.Color.Black;
			this.btn90y.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btn90y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn90y.Location = new System.Drawing.Point(68, 65);
			this.btn90y.Name = "btn90y";
			this.btn90y.Size = new System.Drawing.Size(55, 23);
			this.btn90y.TabIndex = 3;
			this.btn90y.Text = "0, 90, 0";
			this.btn90y.UseVisualStyleBackColor = false;
			this.btn90y.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btn90z
			// 
			this.btn90z.BackColor = System.Drawing.Color.Black;
			this.btn90z.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btn90z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn90z.Location = new System.Drawing.Point(129, 65);
			this.btn90z.Name = "btn90z";
			this.btn90z.Size = new System.Drawing.Size(55, 23);
			this.btn90z.TabIndex = 3;
			this.btn90z.Text = "0, 0, 90";
			this.btn90z.UseVisualStyleBackColor = false;
			this.btn90z.Click += new System.EventHandler(this.btnPreset_Click);
			// 
			// btnY270
			// 
			this.btnY270.BackColor = System.Drawing.Color.Black;
			this.btnY270.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnY270.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnY270.Location = new System.Drawing.Point(206, 87);
			this.btnY270.Name = "btnY270";
			this.btnY270.Size = new System.Drawing.Size(49, 23);
			this.btnY270.TabIndex = 3;
			this.btnY270.Text = "-90";
			this.btnY270.UseVisualStyleBackColor = false;
			this.btnY270.Click += new System.EventHandler(this.changeY_Click);
			// 
			// btnY90
			// 
			this.btnY90.BackColor = System.Drawing.Color.Black;
			this.btnY90.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnY90.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnY90.Location = new System.Drawing.Point(206, 116);
			this.btnY90.Name = "btnY90";
			this.btnY90.Size = new System.Drawing.Size(49, 23);
			this.btnY90.TabIndex = 3;
			this.btnY90.Text = "+90";
			this.btnY90.UseVisualStyleBackColor = false;
			this.btnY90.Click += new System.EventHandler(this.changeY_Click);
			// 
			// btnX270
			// 
			this.btnX270.BackColor = System.Drawing.Color.Black;
			this.btnX270.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnX270.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnX270.Location = new System.Drawing.Point(15, 187);
			this.btnX270.Name = "btnX270";
			this.btnX270.Size = new System.Drawing.Size(49, 23);
			this.btnX270.TabIndex = 3;
			this.btnX270.Text = "-90";
			this.btnX270.UseVisualStyleBackColor = false;
			this.btnX270.Click += new System.EventHandler(this.changeX_Click);
			// 
			// btnX90
			// 
			this.btnX90.BackColor = System.Drawing.Color.Black;
			this.btnX90.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnX90.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnX90.Location = new System.Drawing.Point(15, 216);
			this.btnX90.Name = "btnX90";
			this.btnX90.Size = new System.Drawing.Size(49, 23);
			this.btnX90.TabIndex = 3;
			this.btnX90.Text = "+90";
			this.btnX90.UseVisualStyleBackColor = false;
			this.btnX90.Click += new System.EventHandler(this.changeX_Click);
			// 
			// btnZ270
			// 
			this.btnZ270.BackColor = System.Drawing.Color.Black;
			this.btnZ270.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnZ270.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnZ270.Location = new System.Drawing.Point(206, 290);
			this.btnZ270.Name = "btnZ270";
			this.btnZ270.Size = new System.Drawing.Size(49, 23);
			this.btnZ270.TabIndex = 3;
			this.btnZ270.Text = "-90";
			this.btnZ270.UseVisualStyleBackColor = false;
			this.btnZ270.Click += new System.EventHandler(this.changeZ_Click);
			// 
			// btnZ90
			// 
			this.btnZ90.BackColor = System.Drawing.Color.Black;
			this.btnZ90.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.btnZ90.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnZ90.Location = new System.Drawing.Point(206, 319);
			this.btnZ90.Name = "btnZ90";
			this.btnZ90.Size = new System.Drawing.Size(49, 23);
			this.btnZ90.TabIndex = 3;
			this.btnZ90.Text = "+90";
			this.btnZ90.UseVisualStyleBackColor = false;
			this.btnZ90.Click += new System.EventHandler(this.changeZ_Click);
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
			// panel1
			// 
			this.panel1.Controls.Add(this.rbScale);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.rbRotate);
			this.panel1.Controls.Add(this.rbTranslate);
			this.panel1.Location = new System.Drawing.Point(3, 444);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(274, 24);
			this.panel1.TabIndex = 12;
			// 
			// rbScale
			// 
			this.rbScale.AutoSize = true;
			this.rbScale.Location = new System.Drawing.Point(196, 2);
			this.rbScale.Name = "rbScale";
			this.rbScale.Size = new System.Drawing.Size(52, 17);
			this.rbScale.TabIndex = 15;
			this.rbScale.Text = "Scale";
			this.rbScale.UseVisualStyleBackColor = true;
			this.rbScale.CheckedChanged += new System.EventHandler(this.rbScale_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 4);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(33, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Style:";
			// 
			// rbRotate
			// 
			this.rbRotate.AutoSize = true;
			this.rbRotate.Location = new System.Drawing.Point(127, 2);
			this.rbRotate.Name = "rbRotate";
			this.rbRotate.Size = new System.Drawing.Size(57, 17);
			this.rbRotate.TabIndex = 13;
			this.rbRotate.Text = "Rotate";
			this.rbRotate.UseVisualStyleBackColor = true;
			this.rbRotate.CheckedChanged += new System.EventHandler(this.rbRotate_CheckedChanged);
			// 
			// rbTranslate
			// 
			this.rbTranslate.AutoSize = true;
			this.rbTranslate.Checked = true;
			this.rbTranslate.Location = new System.Drawing.Point(48, 2);
			this.rbTranslate.Name = "rbTranslate";
			this.rbTranslate.Size = new System.Drawing.Size(69, 17);
			this.rbTranslate.TabIndex = 12;
			this.rbTranslate.TabStop = true;
			this.rbTranslate.Text = "Translate";
			this.rbTranslate.UseVisualStyleBackColor = true;
			this.rbTranslate.CheckedChanged += new System.EventHandler(this.rbTranslate_CheckedChanged);
			// 
			// EdtVector3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.panel1);
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
			this.Controls.Add(this.btnZ90);
			this.Controls.Add(this.btnZ270);
			this.Controls.Add(this.btnX90);
			this.Controls.Add(this.btnX270);
			this.Controls.Add(this.btnY90);
			this.Controls.Add(this.btnY270);
			this.Controls.Add(this.btn90z);
			this.Controls.Add(this.btn90y);
			this.Controls.Add(this.btn90x);
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
			this.Size = new System.Drawing.Size(330, 470);
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkOffsetZ)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
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
		private System.Windows.Forms.Button btn90x;
		private System.Windows.Forms.Button btn90y;
		private System.Windows.Forms.Button btn90z;
		private System.Windows.Forms.Button btnY270;
		private System.Windows.Forms.Button btnY90;
		private System.Windows.Forms.Button btnX270;
		private System.Windows.Forms.Button btnX90;
		private System.Windows.Forms.Button btnZ270;
		private System.Windows.Forms.Button btnZ90;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton rbScale;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton rbRotate;
		private System.Windows.Forms.RadioButton rbTranslate;
	}
}
