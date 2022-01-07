namespace TaleSpireExplore
{
	partial class EdtMiniGrouper
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EdtMiniGrouper));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.trkHue = new System.Windows.Forms.TrackBar();
			this.lbGroupName = new System.Windows.Forms.Label();
			this.btnEdit = new System.Windows.Forms.Button();
			this.lbInstructions = new System.Windows.Forms.Label();
			this.lstMembers = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnHide = new System.Windows.Forms.Button();
			this.btnShowAll = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Group Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Base Color:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(87, 54);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(230, 17);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// trkHue
			// 
			this.trkHue.Location = new System.Drawing.Point(74, 33);
			this.trkHue.Maximum = 100;
			this.trkHue.Name = "trkHue";
			this.trkHue.Size = new System.Drawing.Size(253, 45);
			this.trkHue.TabIndex = 7;
			this.trkHue.TickFrequency = 10;
			this.trkHue.Scroll += new System.EventHandler(this.trkHue_Scroll);
			// 
			// lbGroupName
			// 
			this.lbGroupName.AutoSize = true;
			this.lbGroupName.Location = new System.Drawing.Point(77, 6);
			this.lbGroupName.Name = "lbGroupName";
			this.lbGroupName.Size = new System.Drawing.Size(0, 13);
			this.lbGroupName.TabIndex = 9;
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(10, 98);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(75, 31);
			this.btnEdit.TabIndex = 10;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lbInstructions
			// 
			this.lbInstructions.AutoSize = true;
			this.lbInstructions.Location = new System.Drawing.Point(91, 116);
			this.lbInstructions.Name = "lbInstructions";
			this.lbInstructions.Size = new System.Drawing.Size(114, 13);
			this.lbInstructions.TabIndex = 11;
			this.lbInstructions.Text = "Click to Edit the group.";
			// 
			// lstMembers
			// 
			this.lstMembers.FormattingEnabled = true;
			this.lstMembers.Location = new System.Drawing.Point(10, 160);
			this.lstMembers.Name = "lstMembers";
			this.lstMembers.Size = new System.Drawing.Size(299, 134);
			this.lstMembers.TabIndex = 12;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Members:";
			// 
			// btnHide
			// 
			this.btnHide.Location = new System.Drawing.Point(10, 309);
			this.btnHide.Name = "btnHide";
			this.btnHide.Size = new System.Drawing.Size(75, 31);
			this.btnHide.TabIndex = 14;
			this.btnHide.Text = "Hide All";
			this.btnHide.UseVisualStyleBackColor = true;
			// 
			// btnShowAll
			// 
			this.btnShowAll.Location = new System.Drawing.Point(107, 309);
			this.btnShowAll.Name = "btnShowAll";
			this.btnShowAll.Size = new System.Drawing.Size(75, 31);
			this.btnShowAll.TabIndex = 15;
			this.btnShowAll.Text = "Show All";
			this.btnShowAll.UseVisualStyleBackColor = true;
			// 
			// EdtMiniGrouper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnShowAll);
			this.Controls.Add(this.btnHide);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lstMembers);
			this.Controls.Add(this.lbInstructions);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.lbGroupName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.trkHue);
			this.Name = "EdtMiniGrouper";
			this.Size = new System.Drawing.Size(330, 445);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TrackBar trkHue;
		private System.Windows.Forms.Label lbGroupName;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Label lbInstructions;
		private System.Windows.Forms.ListBox lstMembers;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnHide;
		private System.Windows.Forms.Button btnShowAll;
	}
}
