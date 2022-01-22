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
			this.components = new System.ComponentModel.Container();
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
			this.txtNewName = new System.Windows.Forms.TextBox();
			this.btnRenameAll = new System.Windows.Forms.Button();
			this.tbxNumCreatures = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lblSelectedCreatureName = new System.Windows.Forms.Label();
			this.rbRectangular = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.rbCircular = new System.Windows.Forms.RadioButton();
			this.trkSpacing = new System.Windows.Forms.TrackBar();
			this.lblSpacing = new System.Windows.Forms.Label();
			this.lblColumnsRadius = new System.Windows.Forms.Label();
			this.trkColumnsRadius = new System.Windows.Forms.TrackBar();
			this.tbxGroupName = new System.Windows.Forms.TextBox();
			this.btnLookAt = new System.Windows.Forms.Button();
			this.lblRotation = new System.Windows.Forms.Label();
			this.trkFormationRotation = new System.Windows.Forms.TrackBar();
			this.lblOutOf = new System.Windows.Forms.Label();
			this.btnSetHp = new System.Windows.Forms.Button();
			this.tbxSetHp = new System.Windows.Forms.TextBox();
			this.tbxMaxHp = new System.Windows.Forms.TextBox();
			this.btnDamage = new System.Windows.Forms.Button();
			this.tbxDamage = new System.Windows.Forms.TextBox();
			this.lbHP1 = new System.Windows.Forms.Label();
			this.lbHP2 = new System.Windows.Forms.Label();
			this.btnHeal = new System.Windows.Forms.Button();
			this.tbxHealth = new System.Windows.Forms.TextBox();
			this.rbGaggle = new System.Windows.Forms.RadioButton();
			this.lblNewCreatureName = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.rbFormation = new System.Windows.Forms.RadioButton();
			this.rbFollowTheLeader = new System.Windows.Forms.RadioButton();
			this.rbLookTowardLeader = new System.Windows.Forms.RadioButton();
			this.rbLookTowardMovement = new System.Windows.Forms.RadioButton();
			this.rbLookTowardSpecificCreature = new System.Windows.Forms.RadioButton();
			this.rbLookTowardNearestMember = new System.Windows.Forms.RadioButton();
			this.rbLookTowardNearestOutsider = new System.Windows.Forms.RadioButton();
			this.btnDestroyGroup = new System.Windows.Forms.Button();
			this.btnReverseLine = new System.Windows.Forms.Button();
			this.rbFreeform = new System.Windows.Forms.RadioButton();
			this.lblRotationValue = new System.Windows.Forms.Label();
			this.lblColumnRadiusValue = new System.Windows.Forms.Label();
			this.lblSpacingValue = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.lblArcAngleValue = new System.Windows.Forms.Label();
			this.lblArcAngle = new System.Windows.Forms.Label();
			this.trkArcAngle = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSpacing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkColumnsRadius)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkFormationRotation)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkArcAngle)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Group Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 43);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Ring Color:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(87, 59);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(230, 13);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// trkHue
			// 
			this.trkHue.Location = new System.Drawing.Point(74, 38);
			this.trkHue.Maximum = 360;
			this.trkHue.Name = "trkHue";
			this.trkHue.Size = new System.Drawing.Size(253, 45);
			this.trkHue.TabIndex = 1;
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
			this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.btnEdit.ForeColor = System.Drawing.Color.White;
			this.btnEdit.Location = new System.Drawing.Point(6, 86);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(79, 44);
			this.btnEdit.TabIndex = 2;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = false;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			this.btnEdit.MouseEnter += new System.EventHandler(this.btnEdit_MouseEnter);
			this.btnEdit.MouseLeave += new System.EventHandler(this.btnEdit_MouseLeave);
			// 
			// lbInstructions
			// 
			this.lbInstructions.AutoSize = true;
			this.lbInstructions.Location = new System.Drawing.Point(89, 102);
			this.lbInstructions.Name = "lbInstructions";
			this.lbInstructions.Size = new System.Drawing.Size(129, 13);
			this.lbInstructions.TabIndex = 11;
			this.lbInstructions.Text = "<< Click to Edit the group.";
			// 
			// lstMembers
			// 
			this.lstMembers.FormattingEnabled = true;
			this.lstMembers.Location = new System.Drawing.Point(10, 194);
			this.lstMembers.Name = "lstMembers";
			this.lstMembers.Size = new System.Drawing.Size(126, 121);
			this.lstMembers.TabIndex = 4;
			this.lstMembers.SelectedIndexChanged += new System.EventHandler(this.lstMembers_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 178);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Members:";
			// 
			// btnHideAll
			// 
			this.btnHideAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnHideAll.ForeColor = System.Drawing.Color.White;
			this.btnHideAll.Location = new System.Drawing.Point(177, 347);
			this.btnHideAll.Name = "btnHideAll";
			this.btnHideAll.Size = new System.Drawing.Size(70, 32);
			this.btnHideAll.TabIndex = 14;
			this.btnHideAll.Text = "Hide";
			this.toolTip1.SetToolTip(this.btnHideAll, "Hides all members of the group.");
			this.btnHideAll.UseVisualStyleBackColor = false;
			this.btnHideAll.Click += new System.EventHandler(this.btnHideAll_Click);
			this.btnHideAll.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnHideAll.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// btnShowAll
			// 
			this.btnShowAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnShowAll.ForeColor = System.Drawing.Color.White;
			this.btnShowAll.Location = new System.Drawing.Point(252, 347);
			this.btnShowAll.Name = "btnShowAll";
			this.btnShowAll.Size = new System.Drawing.Size(70, 32);
			this.btnShowAll.TabIndex = 15;
			this.btnShowAll.Text = "Show";
			this.toolTip1.SetToolTip(this.btnShowAll, "Shows all members of the group.");
			this.btnShowAll.UseVisualStyleBackColor = false;
			this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
			this.btnShowAll.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnShowAll.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// btnMatchAltitude
			// 
			this.btnMatchAltitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnMatchAltitude.ForeColor = System.Drawing.Color.White;
			this.btnMatchAltitude.Location = new System.Drawing.Point(230, 93);
			this.btnMatchAltitude.Name = "btnMatchAltitude";
			this.btnMatchAltitude.Size = new System.Drawing.Size(92, 32);
			this.btnMatchAltitude.TabIndex = 3;
			this.btnMatchAltitude.Text = "Match Altitude";
			this.btnMatchAltitude.UseVisualStyleBackColor = false;
			this.btnMatchAltitude.Click += new System.EventHandler(this.btnMatchAltitude_Click);
			this.btnMatchAltitude.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnMatchAltitude.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// txtNewName
			// 
			this.txtNewName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.txtNewName.ForeColor = System.Drawing.Color.White;
			this.txtNewName.Location = new System.Drawing.Point(99, 141);
			this.txtNewName.Name = "txtNewName";
			this.txtNewName.Size = new System.Drawing.Size(121, 20);
			this.txtNewName.TabIndex = 18;
			this.toolTip1.SetToolTip(this.txtNewName, "The base name for all members (click Rename All to rename them)");
			this.txtNewName.TextChanged += new System.EventHandler(this.txtNewName_TextChanged);
			// 
			// btnRenameAll
			// 
			this.btnRenameAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnRenameAll.ForeColor = System.Drawing.Color.White;
			this.btnRenameAll.Location = new System.Drawing.Point(230, 135);
			this.btnRenameAll.Name = "btnRenameAll";
			this.btnRenameAll.Size = new System.Drawing.Size(92, 32);
			this.btnRenameAll.TabIndex = 19;
			this.btnRenameAll.Text = "Rename All";
			this.toolTip1.SetToolTip(this.btnRenameAll, "Renames all members using the specified base name (e.g., \"Orc 1\", \"Orc 2\", \"Orc 3" +
        "\", etc.)");
			this.btnRenameAll.UseVisualStyleBackColor = false;
			this.btnRenameAll.Click += new System.EventHandler(this.btnRenameAll_Click);
			this.btnRenameAll.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnRenameAll.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// tbxNumCreatures
			// 
			this.tbxNumCreatures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxNumCreatures.ForeColor = System.Drawing.Color.White;
			this.tbxNumCreatures.Location = new System.Drawing.Point(22, 353);
			this.tbxNumCreatures.Name = "tbxNumCreatures";
			this.tbxNumCreatures.Size = new System.Drawing.Size(37, 20);
			this.tbxNumCreatures.TabIndex = 20;
			this.tbxNumCreatures.Text = "1";
			// 
			// btnAdd
			// 
			this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnAdd.ForeColor = System.Drawing.Color.White;
			this.btnAdd.Location = new System.Drawing.Point(65, 347);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(92, 32);
			this.btnAdd.TabIndex = 21;
			this.btnAdd.Text = "Create More";
			this.btnAdd.UseVisualStyleBackColor = false;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnAdd.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnAdd.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// lblSelectedCreatureName
			// 
			this.lblSelectedCreatureName.AutoSize = true;
			this.lblSelectedCreatureName.Location = new System.Drawing.Point(7, 328);
			this.lblSelectedCreatureName.Name = "lblSelectedCreatureName";
			this.lblSelectedCreatureName.Size = new System.Drawing.Size(114, 13);
			this.lblSelectedCreatureName.TabIndex = 22;
			this.lblSelectedCreatureName.Text = "Create more creatures:";
			// 
			// rbRectangular
			// 
			this.rbRectangular.AutoSize = true;
			this.rbRectangular.Location = new System.Drawing.Point(18, 631);
			this.rbRectangular.Name = "rbRectangular";
			this.rbRectangular.Size = new System.Drawing.Size(74, 17);
			this.rbRectangular.TabIndex = 24;
			this.rbRectangular.Text = "Rectangle";
			this.toolTip1.SetToolTip(this.rbRectangular, "Creates a rectangular formation.");
			this.rbRectangular.UseVisualStyleBackColor = true;
			this.rbRectangular.CheckedChanged += new System.EventHandler(this.rbRectangular_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(7, 563);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(91, 13);
			this.label6.TabIndex = 25;
			this.label6.Text = "Instant Formation:";
			// 
			// rbCircular
			// 
			this.rbCircular.AutoSize = true;
			this.rbCircular.Location = new System.Drawing.Point(18, 654);
			this.rbCircular.Name = "rbCircular";
			this.rbCircular.Size = new System.Drawing.Size(51, 17);
			this.rbCircular.TabIndex = 26;
			this.rbCircular.Text = "Circle";
			this.toolTip1.SetToolTip(this.rbCircular, "Creates a circular formation.");
			this.rbCircular.UseVisualStyleBackColor = true;
			this.rbCircular.CheckedChanged += new System.EventHandler(this.rbCircular_CheckedChanged);
			// 
			// trkSpacing
			// 
			this.trkSpacing.Location = new System.Drawing.Point(147, 563);
			this.trkSpacing.Maximum = 20;
			this.trkSpacing.Name = "trkSpacing";
			this.trkSpacing.Size = new System.Drawing.Size(175, 45);
			this.trkSpacing.TabIndex = 28;
			this.trkSpacing.TickFrequency = 10;
			this.toolTip1.SetToolTip(this.trkSpacing, "Changes the spacing among group members.");
			this.trkSpacing.Value = 2;
			this.trkSpacing.Scroll += new System.EventHandler(this.trkSpacing_Scroll);
			this.trkSpacing.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkSpacing_MouseUp);
			// 
			// lblSpacing
			// 
			this.lblSpacing.AutoSize = true;
			this.lblSpacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSpacing.Location = new System.Drawing.Point(137, 545);
			this.lblSpacing.Name = "lblSpacing";
			this.lblSpacing.Size = new System.Drawing.Size(49, 13);
			this.lblSpacing.TabIndex = 29;
			this.lblSpacing.Text = "Spacing:";
			// 
			// lblColumnsRadius
			// 
			this.lblColumnsRadius.AutoSize = true;
			this.lblColumnsRadius.Location = new System.Drawing.Point(137, 608);
			this.lblColumnsRadius.Name = "lblColumnsRadius";
			this.lblColumnsRadius.Size = new System.Drawing.Size(50, 13);
			this.lblColumnsRadius.TabIndex = 31;
			this.lblColumnsRadius.Text = "Columns:";
			// 
			// trkColumnsRadius
			// 
			this.trkColumnsRadius.Location = new System.Drawing.Point(147, 626);
			this.trkColumnsRadius.Maximum = 1;
			this.trkColumnsRadius.Minimum = 1;
			this.trkColumnsRadius.Name = "trkColumnsRadius";
			this.trkColumnsRadius.Size = new System.Drawing.Size(175, 45);
			this.trkColumnsRadius.TabIndex = 30;
			this.trkColumnsRadius.TickFrequency = 10;
			this.toolTip1.SetToolTip(this.trkColumnsRadius, "Changes the number of columns in rectangular formations.");
			this.trkColumnsRadius.Value = 1;
			this.trkColumnsRadius.Scroll += new System.EventHandler(this.trkColumnsRadius_Scroll);
			// 
			// tbxGroupName
			// 
			this.tbxGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxGroupName.ForeColor = System.Drawing.Color.White;
			this.tbxGroupName.Location = new System.Drawing.Point(87, 7);
			this.tbxGroupName.Name = "tbxGroupName";
			this.tbxGroupName.Size = new System.Drawing.Size(235, 20);
			this.tbxGroupName.TabIndex = 0;
			this.tbxGroupName.TextChanged += new System.EventHandler(this.tbxGroupName_TextChanged);
			// 
			// btnLookAt
			// 
			this.btnLookAt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnLookAt.ForeColor = System.Drawing.Color.White;
			this.btnLookAt.Location = new System.Drawing.Point(125, 57);
			this.btnLookAt.Name = "btnLookAt";
			this.btnLookAt.Size = new System.Drawing.Size(55, 32);
			this.btnLookAt.TabIndex = 34;
			this.btnLookAt.Text = "Set...";
			this.toolTip1.SetToolTip(this.btnLookAt, "Click to set the target creature (after clicking this button)");
			this.btnLookAt.UseVisualStyleBackColor = false;
			this.btnLookAt.Click += new System.EventHandler(this.btnLookAt_Click);
			this.btnLookAt.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnLookAt.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// lblRotation
			// 
			this.lblRotation.AutoSize = true;
			this.lblRotation.Location = new System.Drawing.Point(7, 734);
			this.lblRotation.Name = "lblRotation";
			this.lblRotation.Size = new System.Drawing.Size(50, 13);
			this.lblRotation.TabIndex = 36;
			this.lblRotation.Text = "Rotation:";
			// 
			// trkFormationRotation
			// 
			this.trkFormationRotation.Location = new System.Drawing.Point(17, 752);
			this.trkFormationRotation.Maximum = 360;
			this.trkFormationRotation.Minimum = -360;
			this.trkFormationRotation.Name = "trkFormationRotation";
			this.trkFormationRotation.Size = new System.Drawing.Size(300, 45);
			this.trkFormationRotation.TabIndex = 35;
			this.trkFormationRotation.TickFrequency = 10;
			this.toolTip1.SetToolTip(this.trkFormationRotation, "Rotates the formation around the leader mini.");
			this.trkFormationRotation.Scroll += new System.EventHandler(this.trkFormationRotation_Scroll);
			this.trkFormationRotation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkFormationRotation_MouseDown);
			// 
			// lblOutOf
			// 
			this.lblOutOf.AutoSize = true;
			this.lblOutOf.Location = new System.Drawing.Point(119, 404);
			this.lblOutOf.Name = "lblOutOf";
			this.lblOutOf.Size = new System.Drawing.Size(34, 13);
			this.lblOutOf.TabIndex = 40;
			this.lblOutOf.Text = "out of";
			// 
			// btnSetHp
			// 
			this.btnSetHp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnSetHp.ForeColor = System.Drawing.Color.White;
			this.btnSetHp.Location = new System.Drawing.Point(10, 395);
			this.btnSetHp.Name = "btnSetHp";
			this.btnSetHp.Size = new System.Drawing.Size(62, 32);
			this.btnSetHp.TabIndex = 39;
			this.btnSetHp.Text = "Set HP";
			this.toolTip1.SetToolTip(this.btnSetHp, "Changes the HP (hit points) of all the creatures to the amounts specified. ");
			this.btnSetHp.UseVisualStyleBackColor = false;
			this.btnSetHp.Click += new System.EventHandler(this.btnSetHp_Click);
			this.btnSetHp.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnSetHp.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// tbxSetHp
			// 
			this.tbxSetHp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxSetHp.ForeColor = System.Drawing.Color.White;
			this.tbxSetHp.Location = new System.Drawing.Point(78, 401);
			this.tbxSetHp.Name = "tbxSetHp";
			this.tbxSetHp.Size = new System.Drawing.Size(37, 20);
			this.tbxSetHp.TabIndex = 38;
			this.tbxSetHp.Text = "10";
			this.toolTip1.SetToolTip(this.tbxSetHp, "The amount of hit points to give each creature.");
			// 
			// tbxMaxHp
			// 
			this.tbxMaxHp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxMaxHp.ForeColor = System.Drawing.Color.White;
			this.tbxMaxHp.Location = new System.Drawing.Point(159, 401);
			this.tbxMaxHp.Name = "tbxMaxHp";
			this.tbxMaxHp.Size = new System.Drawing.Size(37, 20);
			this.tbxMaxHp.TabIndex = 41;
			this.tbxMaxHp.Text = "10";
			this.toolTip1.SetToolTip(this.tbxMaxHp, "The maximum amount of hit points for each creature.");
			// 
			// btnDamage
			// 
			this.btnDamage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnDamage.ForeColor = System.Drawing.Color.White;
			this.btnDamage.Location = new System.Drawing.Point(10, 430);
			this.btnDamage.Name = "btnDamage";
			this.btnDamage.Size = new System.Drawing.Size(62, 32);
			this.btnDamage.TabIndex = 43;
			this.btnDamage.Text = "Damage";
			this.toolTip1.SetToolTip(this.btnDamage, "Applies damage to all members of the group.");
			this.btnDamage.UseVisualStyleBackColor = false;
			this.btnDamage.Click += new System.EventHandler(this.btnDamage_Click);
			this.btnDamage.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
			this.btnDamage.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
			// 
			// tbxDamage
			// 
			this.tbxDamage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxDamage.ForeColor = System.Drawing.Color.White;
			this.tbxDamage.Location = new System.Drawing.Point(78, 436);
			this.tbxDamage.Name = "tbxDamage";
			this.tbxDamage.Size = new System.Drawing.Size(37, 20);
			this.tbxDamage.TabIndex = 42;
			this.tbxDamage.Text = "10";
			this.toolTip1.SetToolTip(this.tbxDamage, "The amount of damage to give each creature.");
			// 
			// lbHP1
			// 
			this.lbHP1.AutoSize = true;
			this.lbHP1.Location = new System.Drawing.Point(119, 439);
			this.lbHP1.Name = "lbHP1";
			this.lbHP1.Size = new System.Drawing.Size(22, 13);
			this.lbHP1.TabIndex = 44;
			this.lbHP1.Text = "HP";
			// 
			// lbHP2
			// 
			this.lbHP2.AutoSize = true;
			this.lbHP2.Location = new System.Drawing.Point(257, 439);
			this.lbHP2.Name = "lbHP2";
			this.lbHP2.Size = new System.Drawing.Size(22, 13);
			this.lbHP2.TabIndex = 47;
			this.lbHP2.Text = "HP";
			// 
			// btnHeal
			// 
			this.btnHeal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnHeal.ForeColor = System.Drawing.Color.White;
			this.btnHeal.Location = new System.Drawing.Point(159, 430);
			this.btnHeal.Name = "btnHeal";
			this.btnHeal.Size = new System.Drawing.Size(51, 32);
			this.btnHeal.TabIndex = 46;
			this.btnHeal.Text = "Heal";
			this.toolTip1.SetToolTip(this.btnHeal, "Heals all members of the group.");
			this.btnHeal.UseVisualStyleBackColor = false;
			this.btnHeal.Click += new System.EventHandler(this.btnHeal_Click);
			// 
			// tbxHealth
			// 
			this.tbxHealth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
			this.tbxHealth.ForeColor = System.Drawing.Color.White;
			this.tbxHealth.Location = new System.Drawing.Point(216, 436);
			this.tbxHealth.Name = "tbxHealth";
			this.tbxHealth.Size = new System.Drawing.Size(37, 20);
			this.tbxHealth.TabIndex = 45;
			this.tbxHealth.Text = "10";
			this.toolTip1.SetToolTip(this.tbxHealth, "The amount of HP to heal each creature.");
			// 
			// rbGaggle
			// 
			this.rbGaggle.AutoSize = true;
			this.rbGaggle.Location = new System.Drawing.Point(18, 608);
			this.rbGaggle.Name = "rbGaggle";
			this.rbGaggle.Size = new System.Drawing.Size(59, 17);
			this.rbGaggle.TabIndex = 48;
			this.rbGaggle.Text = "Gaggle";
			this.toolTip1.SetToolTip(this.rbGaggle, "Creates a loosely-formed rectangular formation.");
			this.rbGaggle.UseVisualStyleBackColor = true;
			this.rbGaggle.CheckedChanged += new System.EventHandler(this.rbGaggle_CheckedChanged);
			// 
			// lblNewCreatureName
			// 
			this.lblNewCreatureName.AutoSize = true;
			this.lblNewCreatureName.Location = new System.Drawing.Point(7, 144);
			this.lblNewCreatureName.Name = "lblNewCreatureName";
			this.lblNewCreatureName.Size = new System.Drawing.Size(86, 13);
			this.lblNewCreatureName.TabIndex = 51;
			this.lblNewCreatureName.Text = "Creature Names:";
			// 
			// toolTip1
			// 
			this.toolTip1.ShowAlways = true;
			// 
			// rbFormation
			// 
			this.rbFormation.AutoSize = true;
			this.rbFormation.Checked = true;
			this.rbFormation.Location = new System.Drawing.Point(4, 26);
			this.rbFormation.Name = "rbFormation";
			this.rbFormation.Size = new System.Drawing.Size(71, 17);
			this.rbFormation.TabIndex = 58;
			this.rbFormation.TabStop = true;
			this.rbFormation.Text = "Formation";
			this.toolTip1.SetToolTip(this.rbFormation, "Members stay in formation as the leader moves.");
			this.rbFormation.UseVisualStyleBackColor = true;
			this.rbFormation.CheckedChanged += new System.EventHandler(this.rbFormation_CheckedChanged);
			// 
			// rbFollowTheLeader
			// 
			this.rbFollowTheLeader.AutoSize = true;
			this.rbFollowTheLeader.Location = new System.Drawing.Point(4, 3);
			this.rbFollowTheLeader.Name = "rbFollowTheLeader";
			this.rbFollowTheLeader.Size = new System.Drawing.Size(109, 17);
			this.rbFollowTheLeader.TabIndex = 59;
			this.rbFollowTheLeader.Text = "Follow the Leader";
			this.toolTip1.SetToolTip(this.rbFollowTheLeader, "Each group member follows the straight-line movements of the leader.");
			this.rbFollowTheLeader.UseVisualStyleBackColor = true;
			this.rbFollowTheLeader.CheckedChanged += new System.EventHandler(this.rbFollowTheLeader_CheckedChanged);
			// 
			// rbLookTowardLeader
			// 
			this.rbLookTowardLeader.AutoSize = true;
			this.rbLookTowardLeader.Location = new System.Drawing.Point(11, 43);
			this.rbLookTowardLeader.Name = "rbLookTowardLeader";
			this.rbLookTowardLeader.Size = new System.Drawing.Size(58, 17);
			this.rbLookTowardLeader.TabIndex = 59;
			this.rbLookTowardLeader.Text = "Leader";
			this.toolTip1.SetToolTip(this.rbLookTowardLeader, "Each group member follows the straight-line movements of the leader.");
			this.rbLookTowardLeader.UseVisualStyleBackColor = true;
			this.rbLookTowardLeader.CheckedChanged += new System.EventHandler(this.rbLookToward_CheckedChanged);
			// 
			// rbLookTowardMovement
			// 
			this.rbLookTowardMovement.AutoSize = true;
			this.rbLookTowardMovement.Checked = true;
			this.rbLookTowardMovement.Location = new System.Drawing.Point(11, 21);
			this.rbLookTowardMovement.Name = "rbLookTowardMovement";
			this.rbLookTowardMovement.Size = new System.Drawing.Size(75, 17);
			this.rbLookTowardMovement.TabIndex = 58;
			this.rbLookTowardMovement.TabStop = true;
			this.rbLookTowardMovement.Text = "Movement";
			this.toolTip1.SetToolTip(this.rbLookTowardMovement, "Members stay in formation as the leader moves.");
			this.rbLookTowardMovement.UseVisualStyleBackColor = true;
			this.rbLookTowardMovement.CheckedChanged += new System.EventHandler(this.rbLookToward_CheckedChanged);
			// 
			// rbLookTowardSpecificCreature
			// 
			this.rbLookTowardSpecificCreature.AutoSize = true;
			this.rbLookTowardSpecificCreature.Location = new System.Drawing.Point(11, 65);
			this.rbLookTowardSpecificCreature.Name = "rbLookTowardSpecificCreature";
			this.rbLookTowardSpecificCreature.Size = new System.Drawing.Size(106, 17);
			this.rbLookTowardSpecificCreature.TabIndex = 62;
			this.rbLookTowardSpecificCreature.Text = "Specific Creature";
			this.toolTip1.SetToolTip(this.rbLookTowardSpecificCreature, "Each group member follows the straight-line movements of the leader.");
			this.rbLookTowardSpecificCreature.UseVisualStyleBackColor = true;
			this.rbLookTowardSpecificCreature.CheckedChanged += new System.EventHandler(this.rbLookToward_CheckedChanged);
			// 
			// rbLookTowardNearestMember
			// 
			this.rbLookTowardNearestMember.AutoSize = true;
			this.rbLookTowardNearestMember.Location = new System.Drawing.Point(10, 87);
			this.rbLookTowardNearestMember.Name = "rbLookTowardNearestMember";
			this.rbLookTowardNearestMember.Size = new System.Drawing.Size(103, 17);
			this.rbLookTowardNearestMember.TabIndex = 63;
			this.rbLookTowardNearestMember.Text = "Nearest Member";
			this.toolTip1.SetToolTip(this.rbLookTowardNearestMember, "Each group member follows the straight-line movements of the leader.");
			this.rbLookTowardNearestMember.UseVisualStyleBackColor = true;
			this.rbLookTowardNearestMember.CheckedChanged += new System.EventHandler(this.rbLookToward_CheckedChanged);
			// 
			// rbLookTowardNearestOutsider
			// 
			this.rbLookTowardNearestOutsider.AutoSize = true;
			this.rbLookTowardNearestOutsider.Location = new System.Drawing.Point(10, 109);
			this.rbLookTowardNearestOutsider.Name = "rbLookTowardNearestOutsider";
			this.rbLookTowardNearestOutsider.Size = new System.Drawing.Size(104, 17);
			this.rbLookTowardNearestOutsider.TabIndex = 64;
			this.rbLookTowardNearestOutsider.Text = "Nearest Outsider";
			this.toolTip1.SetToolTip(this.rbLookTowardNearestOutsider, "Each group member follows the straight-line movements of the leader.");
			this.rbLookTowardNearestOutsider.UseVisualStyleBackColor = true;
			this.rbLookTowardNearestOutsider.CheckedChanged += new System.EventHandler(this.rbLookToward_CheckedChanged);
			// 
			// btnDestroyGroup
			// 
			this.btnDestroyGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnDestroyGroup.ForeColor = System.Drawing.Color.White;
			this.btnDestroyGroup.Location = new System.Drawing.Point(9, 685);
			this.btnDestroyGroup.Name = "btnDestroyGroup";
			this.btnDestroyGroup.Size = new System.Drawing.Size(101, 32);
			this.btnDestroyGroup.TabIndex = 61;
			this.btnDestroyGroup.Text = "Destroy Group";
			this.toolTip1.SetToolTip(this.btnDestroyGroup, "Shows all members of the group.");
			this.btnDestroyGroup.UseVisualStyleBackColor = false;
			this.btnDestroyGroup.Click += new System.EventHandler(this.btnDestroyGroup_Click);
			// 
			// btnReverseLine
			// 
			this.btnReverseLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnReverseLine.ForeColor = System.Drawing.Color.White;
			this.btnReverseLine.Location = new System.Drawing.Point(146, 494);
			this.btnReverseLine.Name = "btnReverseLine";
			this.btnReverseLine.Size = new System.Drawing.Size(92, 32);
			this.btnReverseLine.TabIndex = 62;
			this.btnReverseLine.Text = "Reverse Line";
			this.toolTip1.SetToolTip(this.btnReverseLine, "Shows all members of the group.");
			this.btnReverseLine.UseVisualStyleBackColor = false;
			this.btnReverseLine.Click += new System.EventHandler(this.btnReverseLine_Click);
			// 
			// rbFreeform
			// 
			this.rbFreeform.AutoSize = true;
			this.rbFreeform.Checked = true;
			this.rbFreeform.Location = new System.Drawing.Point(18, 585);
			this.rbFreeform.Name = "rbFreeform";
			this.rbFreeform.Size = new System.Drawing.Size(66, 17);
			this.rbFreeform.TabIndex = 63;
			this.rbFreeform.TabStop = true;
			this.rbFreeform.Text = "Freeform";
			this.toolTip1.SetToolTip(this.rbFreeform, "Creates a loosely-formed rectangular formation.");
			this.rbFreeform.UseVisualStyleBackColor = true;
			this.rbFreeform.CheckedChanged += new System.EventHandler(this.rbFreeform_CheckedChanged);
			// 
			// lblRotationValue
			// 
			this.lblRotationValue.AutoSize = true;
			this.lblRotationValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRotationValue.Location = new System.Drawing.Point(60, 734);
			this.lblRotationValue.Name = "lblRotationValue";
			this.lblRotationValue.Size = new System.Drawing.Size(19, 13);
			this.lblRotationValue.TabIndex = 54;
			this.lblRotationValue.Text = "0°";
			// 
			// lblColumnRadiusValue
			// 
			this.lblColumnRadiusValue.AutoSize = true;
			this.lblColumnRadiusValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblColumnRadiusValue.Location = new System.Drawing.Point(192, 608);
			this.lblColumnRadiusValue.Name = "lblColumnRadiusValue";
			this.lblColumnRadiusValue.Size = new System.Drawing.Size(14, 13);
			this.lblColumnRadiusValue.TabIndex = 53;
			this.lblColumnRadiusValue.Text = "1";
			// 
			// lblSpacingValue
			// 
			this.lblSpacingValue.AutoSize = true;
			this.lblSpacingValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSpacingValue.Location = new System.Drawing.Point(190, 545);
			this.lblSpacingValue.Name = "lblSpacingValue";
			this.lblSpacingValue.Size = new System.Drawing.Size(22, 13);
			this.lblSpacingValue.TabIndex = 52;
			this.lblSpacingValue.Text = "2ft";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 482);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 13);
			this.label4.TabIndex = 56;
			this.label4.Text = "Movement:";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.rbFollowTheLeader);
			this.panel1.Controls.Add(this.rbFormation);
			this.panel1.Location = new System.Drawing.Point(18, 498);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(127, 51);
			this.panel1.TabIndex = 57;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.rbLookTowardNearestOutsider);
			this.panel2.Controls.Add(this.rbLookTowardNearestMember);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Controls.Add(this.rbLookTowardLeader);
			this.panel2.Controls.Add(this.rbLookTowardMovement);
			this.panel2.Controls.Add(this.btnLookAt);
			this.panel2.Controls.Add(this.rbLookTowardSpecificCreature);
			this.panel2.Location = new System.Drawing.Point(142, 178);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(190, 136);
			this.panel2.TabIndex = 60;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(73, 13);
			this.label5.TabIndex = 61;
			this.label5.Text = "Look Toward:";
			// 
			// lblArcAngleValue
			// 
			this.lblArcAngleValue.AutoSize = true;
			this.lblArcAngleValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblArcAngleValue.Location = new System.Drawing.Point(190, 674);
			this.lblArcAngleValue.Name = "lblArcAngleValue";
			this.lblArcAngleValue.Size = new System.Drawing.Size(19, 13);
			this.lblArcAngleValue.TabIndex = 67;
			this.lblArcAngleValue.Text = "0°";
			this.lblArcAngleValue.Visible = false;
			// 
			// lblArcAngle
			// 
			this.lblArcAngle.AutoSize = true;
			this.lblArcAngle.Location = new System.Drawing.Point(137, 674);
			this.lblArcAngle.Name = "lblArcAngle";
			this.lblArcAngle.Size = new System.Drawing.Size(56, 13);
			this.lblArcAngle.TabIndex = 66;
			this.lblArcAngle.Text = "Arc Angle:";
			this.lblArcAngle.Visible = false;
			// 
			// trkArcAngle
			// 
			this.trkArcAngle.Location = new System.Drawing.Point(147, 692);
			this.trkArcAngle.Maximum = 360;
			this.trkArcAngle.Name = "trkArcAngle";
			this.trkArcAngle.Size = new System.Drawing.Size(175, 45);
			this.trkArcAngle.TabIndex = 65;
			this.trkArcAngle.TickFrequency = 10;
			this.toolTip1.SetToolTip(this.trkArcAngle, "Rotates the formation around the leader mini.");
			this.trkArcAngle.Visible = false;
			this.trkArcAngle.Scroll += new System.EventHandler(this.trkArcAngle_Scroll);
			this.trkArcAngle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkArcAngle_MouseUp);
			// 
			// EdtMiniGrouper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblSpacingValue);
			this.Controls.Add(this.lblSpacing);
			this.Controls.Add(this.lblArcAngleValue);
			this.Controls.Add(this.lblArcAngle);
			this.Controls.Add(this.trkArcAngle);
			this.Controls.Add(this.rbFreeform);
			this.Controls.Add(this.btnReverseLine);
			this.Controls.Add(this.btnDestroyGroup);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.lblColumnRadiusValue);
			this.Controls.Add(this.lblRotationValue);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblColumnsRadius);
			this.Controls.Add(this.rbGaggle);
			this.Controls.Add(this.lbHP2);
			this.Controls.Add(this.btnHeal);
			this.Controls.Add(this.tbxHealth);
			this.Controls.Add(this.lbHP1);
			this.Controls.Add(this.btnDamage);
			this.Controls.Add(this.tbxDamage);
			this.Controls.Add(this.tbxMaxHp);
			this.Controls.Add(this.lblOutOf);
			this.Controls.Add(this.btnSetHp);
			this.Controls.Add(this.tbxSetHp);
			this.Controls.Add(this.lblRotation);
			this.Controls.Add(this.tbxGroupName);
			this.Controls.Add(this.rbCircular);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.rbRectangular);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.tbxNumCreatures);
			this.Controls.Add(this.btnRenameAll);
			this.Controls.Add(this.txtNewName);
			this.Controls.Add(this.btnMatchAltitude);
			this.Controls.Add(this.btnShowAll);
			this.Controls.Add(this.btnHideAll);
			this.Controls.Add(this.lstMembers);
			this.Controls.Add(this.lbInstructions);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.lbGroupName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.trkHue);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.trkFormationRotation);
			this.Controls.Add(this.trkColumnsRadius);
			this.Controls.Add(this.trkSpacing);
			this.Controls.Add(this.lblNewCreatureName);
			this.Controls.Add(this.lblSelectedCreatureName);
			this.Name = "EdtMiniGrouper";
			this.Size = new System.Drawing.Size(335, 801);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSpacing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkColumnsRadius)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkFormationRotation)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkArcAngle)).EndInit();
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
		private System.Windows.Forms.TextBox txtNewName;
		private System.Windows.Forms.Button btnRenameAll;
		private System.Windows.Forms.TextBox tbxNumCreatures;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Label lblSelectedCreatureName;
		private System.Windows.Forms.RadioButton rbRectangular;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton rbCircular;
		private System.Windows.Forms.TrackBar trkSpacing;
		private System.Windows.Forms.Label lblSpacing;
		private System.Windows.Forms.Label lblColumnsRadius;
		private System.Windows.Forms.TrackBar trkColumnsRadius;
		private System.Windows.Forms.TextBox tbxGroupName;
		private System.Windows.Forms.Button btnLookAt;
		private System.Windows.Forms.Label lblRotation;
		private System.Windows.Forms.TrackBar trkFormationRotation;
		private System.Windows.Forms.Label lblOutOf;
		private System.Windows.Forms.Button btnSetHp;
		private System.Windows.Forms.TextBox tbxSetHp;
		private System.Windows.Forms.TextBox tbxMaxHp;
		private System.Windows.Forms.Button btnDamage;
		private System.Windows.Forms.TextBox tbxDamage;
		private System.Windows.Forms.Label lbHP1;
		private System.Windows.Forms.Label lbHP2;
		private System.Windows.Forms.Button btnHeal;
		private System.Windows.Forms.TextBox tbxHealth;
		private System.Windows.Forms.RadioButton rbGaggle;
		private System.Windows.Forms.Label lblNewCreatureName;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblRotationValue;
		private System.Windows.Forms.Label lblColumnRadiusValue;
		private System.Windows.Forms.Label lblSpacingValue;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton rbFollowTheLeader;
		private System.Windows.Forms.RadioButton rbFormation;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton rbLookTowardLeader;
		private System.Windows.Forms.RadioButton rbLookTowardMovement;
		private System.Windows.Forms.RadioButton rbLookTowardSpecificCreature;
		private System.Windows.Forms.RadioButton rbLookTowardNearestOutsider;
		private System.Windows.Forms.RadioButton rbLookTowardNearestMember;
		private System.Windows.Forms.Button btnDestroyGroup;
		private System.Windows.Forms.Button btnReverseLine;
		private System.Windows.Forms.RadioButton rbFreeform;
		private System.Windows.Forms.Label lblArcAngleValue;
		private System.Windows.Forms.Label lblArcAngle;
		private System.Windows.Forms.TrackBar trkArcAngle;
	}
}
