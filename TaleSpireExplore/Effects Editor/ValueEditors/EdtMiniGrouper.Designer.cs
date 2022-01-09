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
			this.btnHideAll = new System.Windows.Forms.Button();
			this.btnShowAll = new System.Windows.Forms.Button();
			this.btnMatchAltitude = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtNewName = new System.Windows.Forms.TextBox();
			this.btnRenameAll = new System.Windows.Forms.Button();
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
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Ring Color:";
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
			this.trkHue.Maximum = 360;
			this.trkHue.Name = "trkHue";
			this.trkHue.Size = new System.Drawing.Size(253, 45);
			this.trkHue.TabIndex = 7;
			this.trkHue.TickFrequency = 10;
			this.trkHue.Scroll += new System.EventHandler(this.trkHue_Scroll);
			this.trkHue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkHue_MouseUp);
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
			this.btnEdit.Location = new System.Drawing.Point(10, 81);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(67, 31);
			this.btnEdit.TabIndex = 10;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lbInstructions
			// 
			this.lbInstructions.AutoSize = true;
			this.lbInstructions.Location = new System.Drawing.Point(81, 90);
			this.lbInstructions.Name = "lbInstructions";
			this.lbInstructions.Size = new System.Drawing.Size(129, 13);
			this.lbInstructions.TabIndex = 11;
			this.lbInstructions.Text = "<< Click to Edit the group.";
			// 
			// lstMembers
			// 
			this.lstMembers.FormattingEnabled = true;
			this.lstMembers.Location = new System.Drawing.Point(10, 143);
			this.lstMembers.Name = "lstMembers";
			this.lstMembers.Size = new System.Drawing.Size(130, 134);
			this.lstMembers.TabIndex = 12;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 127);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Members:";
			// 
			// btnHideAll
			// 
			this.btnHideAll.Location = new System.Drawing.Point(146, 143);
			this.btnHideAll.Name = "btnHideAll";
			this.btnHideAll.Size = new System.Drawing.Size(68, 31);
			this.btnHideAll.TabIndex = 14;
			this.btnHideAll.Text = "Hide All";
			this.btnHideAll.UseVisualStyleBackColor = true;
			this.btnHideAll.Click += new System.EventHandler(this.btnHideAll_Click);
			// 
			// btnShowAll
			// 
			this.btnShowAll.Location = new System.Drawing.Point(146, 180);
			this.btnShowAll.Name = "btnShowAll";
			this.btnShowAll.Size = new System.Drawing.Size(68, 31);
			this.btnShowAll.TabIndex = 15;
			this.btnShowAll.Text = "Show All";
			this.btnShowAll.UseVisualStyleBackColor = true;
			this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
			// 
			// btnMatchAltitude
			// 
			this.btnMatchAltitude.Location = new System.Drawing.Point(146, 217);
			this.btnMatchAltitude.Name = "btnMatchAltitude";
			this.btnMatchAltitude.Size = new System.Drawing.Size(96, 31);
			this.btnMatchAltitude.TabIndex = 16;
			this.btnMatchAltitude.Text = "Match Altitude";
			this.btnMatchAltitude.UseVisualStyleBackColor = true;
			this.btnMatchAltitude.Click += new System.EventHandler(this.btnMatchAltitude_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 293);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Rename Members:";
			// 
			// txtNewName
			// 
			this.txtNewName.Location = new System.Drawing.Point(109, 290);
			this.txtNewName.Name = "txtNewName";
			this.txtNewName.Size = new System.Drawing.Size(112, 20);
			this.txtNewName.TabIndex = 18;
			// 
			// btnRenameAll
			// 
			this.btnRenameAll.Location = new System.Drawing.Point(230, 286);
			this.btnRenameAll.Name = "btnRenameAll";
			this.btnRenameAll.Size = new System.Drawing.Size(79, 31);
			this.btnRenameAll.TabIndex = 19;
			this.btnRenameAll.Text = "Rename All";
			this.btnRenameAll.UseVisualStyleBackColor = true;
			// 
			// EdtMiniGrouper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnRenameAll);
			this.Controls.Add(this.txtNewName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnMatchAltitude);
			this.Controls.Add(this.btnShowAll);
			this.Controls.Add(this.btnHideAll);
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
		private System.Windows.Forms.Button btnHideAll;
		private System.Windows.Forms.Button btnShowAll;
		private System.Windows.Forms.Button btnMatchAltitude;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtNewName;
		private System.Windows.Forms.Button btnRenameAll;
	}
}
