namespace TaleSpireExplore
{
	partial class FrmExplorer
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnLoadClasses = new System.Windows.Forms.Button();
			this.tbxScratch = new System.Windows.Forms.TextBox();
			this.tbxLog = new System.Windows.Forms.TextBox();
			this.btnSetTime = new System.Windows.Forms.Button();
			this.txtTime = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtSunDirection = new System.Windows.Forms.TextBox();
			this.btnSetSun = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnEffects = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtEffectName = new System.Windows.Forms.TextBox();
			this.btnShowEffect = new System.Windows.Forms.Button();
			this.btnShowRelationEffect = new System.Windows.Forms.Button();
			this.chkListenToEvents = new System.Windows.Forms.CheckBox();
			this.chkTrackFlashlight = new System.Windows.Forms.CheckBox();
			this.lblFlashlightStatus = new System.Windows.Forms.Label();
			this.btnGetRuler = new System.Windows.Forms.Button();
			this.btnSpectatorMode = new System.Windows.Forms.Button();
			this.btnPlayer = new System.Windows.Forms.Button();
			this.btnFlashlightOn = new System.Windows.Forms.Button();
			this.btnFlashlightOff = new System.Windows.Forms.Button();
			this.btnAddEffects = new System.Windows.Forms.Button();
			this.btnParticleSystemOn = new System.Windows.Forms.Button();
			this.btnParticleSystemOff = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cmbPrefabs = new System.Windows.Forms.ComboBox();
			this.btnAttackJanus = new System.Windows.Forms.Button();
			this.btnClearAttack = new System.Windows.Forms.Button();
			this.btnTestEffects = new System.Windows.Forms.Button();
			this.btnDeserialize = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnLoadClasses
			// 
			this.btnLoadClasses.Location = new System.Drawing.Point(12, 12);
			this.btnLoadClasses.Name = "btnLoadClasses";
			this.btnLoadClasses.Size = new System.Drawing.Size(107, 28);
			this.btnLoadClasses.TabIndex = 0;
			this.btnLoadClasses.Text = "Load Creatures";
			this.btnLoadClasses.UseVisualStyleBackColor = true;
			this.btnLoadClasses.Click += new System.EventHandler(this.btnLoadClasses_Click);
			// 
			// tbxScratch
			// 
			this.tbxScratch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.tbxScratch.Location = new System.Drawing.Point(181, 15);
			this.tbxScratch.Multiline = true;
			this.tbxScratch.Name = "tbxScratch";
			this.tbxScratch.Size = new System.Drawing.Size(366, 456);
			this.tbxScratch.TabIndex = 1;
			this.tbxScratch.WordWrap = false;
			// 
			// tbxLog
			// 
			this.tbxLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.tbxLog.Location = new System.Drawing.Point(563, 31);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.Size = new System.Drawing.Size(423, 440);
			this.tbxLog.TabIndex = 2;
			this.tbxLog.WordWrap = false;
			// 
			// btnSetTime
			// 
			this.btnSetTime.Location = new System.Drawing.Point(12, 116);
			this.btnSetTime.Name = "btnSetTime";
			this.btnSetTime.Size = new System.Drawing.Size(86, 23);
			this.btnSetTime.TabIndex = 3;
			this.btnSetTime.Text = "Set Time";
			this.btnSetTime.UseVisualStyleBackColor = true;
			this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
			// 
			// txtTime
			// 
			this.txtTime.Location = new System.Drawing.Point(50, 96);
			this.txtTime.Name = "txtTime";
			this.txtTime.Size = new System.Drawing.Size(48, 20);
			this.txtTime.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Time:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 149);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Sun:";
			// 
			// txtSunDirection
			// 
			this.txtSunDirection.Location = new System.Drawing.Point(52, 145);
			this.txtSunDirection.Name = "txtSunDirection";
			this.txtSunDirection.Size = new System.Drawing.Size(48, 20);
			this.txtSunDirection.TabIndex = 7;
			// 
			// btnSetSun
			// 
			this.btnSetSun.Location = new System.Drawing.Point(12, 166);
			this.btnSetSun.Name = "btnSetSun";
			this.btnSetSun.Size = new System.Drawing.Size(88, 23);
			this.btnSetSun.TabIndex = 6;
			this.btnSetSun.Text = "Set Sun Pos";
			this.btnSetSun.UseVisualStyleBackColor = true;
			this.btnSetSun.Click += new System.EventHandler(this.btnSetSun_Click);
			// 
			// btnClear
			// 
			this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClear.Location = new System.Drawing.Point(911, 5);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(75, 23);
			this.btnClear.TabIndex = 10;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnEffects
			// 
			this.btnEffects.Location = new System.Drawing.Point(12, 282);
			this.btnEffects.Name = "btnEffects";
			this.btnEffects.Size = new System.Drawing.Size(118, 23);
			this.btnEffects.TabIndex = 11;
			this.btnEffects.Text = "List Effects...";
			this.btnEffects.UseVisualStyleBackColor = true;
			this.btnEffects.Click += new System.EventHandler(this.btnEffects_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 315);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 14;
			this.label4.Text = "Effect:";
			// 
			// txtEffectName
			// 
			this.txtEffectName.Location = new System.Drawing.Point(50, 311);
			this.txtEffectName.Name = "txtEffectName";
			this.txtEffectName.Size = new System.Drawing.Size(80, 20);
			this.txtEffectName.TabIndex = 13;
			this.txtEffectName.Text = "MagicMissile";
			// 
			// btnShowEffect
			// 
			this.btnShowEffect.Location = new System.Drawing.Point(12, 337);
			this.btnShowEffect.Name = "btnShowEffect";
			this.btnShowEffect.Size = new System.Drawing.Size(86, 23);
			this.btnShowEffect.TabIndex = 12;
			this.btnShowEffect.Text = "Play Effect";
			this.btnShowEffect.UseVisualStyleBackColor = true;
			this.btnShowEffect.Click += new System.EventHandler(this.btnShowEffect_Click);
			// 
			// btnShowRelationEffect
			// 
			this.btnShowRelationEffect.Location = new System.Drawing.Point(12, 366);
			this.btnShowRelationEffect.Name = "btnShowRelationEffect";
			this.btnShowRelationEffect.Size = new System.Drawing.Size(118, 22);
			this.btnShowRelationEffect.TabIndex = 15;
			this.btnShowRelationEffect.Text = "Show Relation Effect";
			this.btnShowRelationEffect.UseVisualStyleBackColor = true;
			this.btnShowRelationEffect.Click += new System.EventHandler(this.btnShowRelationEffect_Click);
			// 
			// chkListenToEvents
			// 
			this.chkListenToEvents.AutoSize = true;
			this.chkListenToEvents.Location = new System.Drawing.Point(563, 11);
			this.chkListenToEvents.Name = "chkListenToEvents";
			this.chkListenToEvents.Size = new System.Drawing.Size(105, 17);
			this.chkListenToEvents.TabIndex = 16;
			this.chkListenToEvents.Text = "Listen to Events:";
			this.chkListenToEvents.UseVisualStyleBackColor = true;
			this.chkListenToEvents.CheckedChanged += new System.EventHandler(this.chkListenToEvents_CheckedChanged);
			// 
			// chkTrackFlashlight
			// 
			this.chkTrackFlashlight.AutoSize = true;
			this.chkTrackFlashlight.Location = new System.Drawing.Point(10, 438);
			this.chkTrackFlashlight.Name = "chkTrackFlashlight";
			this.chkTrackFlashlight.Size = new System.Drawing.Size(101, 17);
			this.chkTrackFlashlight.TabIndex = 18;
			this.chkTrackFlashlight.Text = "Track Flashlight";
			this.chkTrackFlashlight.UseVisualStyleBackColor = true;
			this.chkTrackFlashlight.CheckedChanged += new System.EventHandler(this.chkTrackFlashlight_CheckedChanged);
			// 
			// lblFlashlightStatus
			// 
			this.lblFlashlightStatus.AutoSize = true;
			this.lblFlashlightStatus.Location = new System.Drawing.Point(27, 458);
			this.lblFlashlightStatus.Name = "lblFlashlightStatus";
			this.lblFlashlightStatus.Size = new System.Drawing.Size(69, 13);
			this.lblFlashlightStatus.TabIndex = 19;
			this.lblFlashlightStatus.Text = "(not tracking)";
			// 
			// btnGetRuler
			// 
			this.btnGetRuler.Location = new System.Drawing.Point(13, 48);
			this.btnGetRuler.Name = "btnGetRuler";
			this.btnGetRuler.Size = new System.Drawing.Size(86, 23);
			this.btnGetRuler.TabIndex = 20;
			this.btnGetRuler.Text = "Get Ruler";
			this.btnGetRuler.UseVisualStyleBackColor = true;
			this.btnGetRuler.Click += new System.EventHandler(this.btnGetRuler_Click);
			// 
			// btnSpectatorMode
			// 
			this.btnSpectatorMode.Location = new System.Drawing.Point(12, 206);
			this.btnSpectatorMode.Name = "btnSpectatorMode";
			this.btnSpectatorMode.Size = new System.Drawing.Size(47, 23);
			this.btnSpectatorMode.TabIndex = 21;
			this.btnSpectatorMode.Text = "Spec";
			this.btnSpectatorMode.UseVisualStyleBackColor = true;
			this.btnSpectatorMode.Click += new System.EventHandler(this.btnSpectatorMode_Click);
			// 
			// btnPlayer
			// 
			this.btnPlayer.Location = new System.Drawing.Point(65, 206);
			this.btnPlayer.Name = "btnPlayer";
			this.btnPlayer.Size = new System.Drawing.Size(48, 23);
			this.btnPlayer.TabIndex = 21;
			this.btnPlayer.Text = "Player";
			this.btnPlayer.UseVisualStyleBackColor = true;
			this.btnPlayer.Click += new System.EventHandler(this.btnPlayer_Click);
			// 
			// btnFlashlightOn
			// 
			this.btnFlashlightOn.Location = new System.Drawing.Point(10, 409);
			this.btnFlashlightOn.Name = "btnFlashlightOn";
			this.btnFlashlightOn.Size = new System.Drawing.Size(47, 23);
			this.btnFlashlightOn.TabIndex = 21;
			this.btnFlashlightOn.Text = "F - On";
			this.btnFlashlightOn.UseVisualStyleBackColor = true;
			this.btnFlashlightOn.Click += new System.EventHandler(this.btnFlashlightOn_Click);
			// 
			// btnFlashlightOff
			// 
			this.btnFlashlightOff.Location = new System.Drawing.Point(63, 409);
			this.btnFlashlightOff.Name = "btnFlashlightOff";
			this.btnFlashlightOff.Size = new System.Drawing.Size(48, 23);
			this.btnFlashlightOff.TabIndex = 21;
			this.btnFlashlightOff.Text = "F - Off";
			this.btnFlashlightOff.UseVisualStyleBackColor = true;
			this.btnFlashlightOff.Click += new System.EventHandler(this.btnFlashlightOff_Click);
			// 
			// btnAddEffects
			// 
			this.btnAddEffects.Location = new System.Drawing.Point(12, 253);
			this.btnAddEffects.Name = "btnAddEffects";
			this.btnAddEffects.Size = new System.Drawing.Size(118, 23);
			this.btnAddEffects.TabIndex = 11;
			this.btnAddEffects.Text = "Add Effect...";
			this.btnAddEffects.UseVisualStyleBackColor = true;
			this.btnAddEffects.Click += new System.EventHandler(this.btnAddEffects_Click);
			// 
			// btnParticleSystemOn
			// 
			this.btnParticleSystemOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnParticleSystemOn.Location = new System.Drawing.Point(56, 542);
			this.btnParticleSystemOn.Name = "btnParticleSystemOn";
			this.btnParticleSystemOn.Size = new System.Drawing.Size(57, 23);
			this.btnParticleSystemOn.TabIndex = 21;
			this.btnParticleSystemOn.Text = "Again";
			this.btnParticleSystemOn.UseVisualStyleBackColor = true;
			this.btnParticleSystemOn.Click += new System.EventHandler(this.btnParticleSystemOn_Click);
			// 
			// btnParticleSystemOff
			// 
			this.btnParticleSystemOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnParticleSystemOff.Location = new System.Drawing.Point(131, 542);
			this.btnParticleSystemOff.Name = "btnParticleSystemOff";
			this.btnParticleSystemOff.Size = new System.Drawing.Size(86, 23);
			this.btnParticleSystemOff.TabIndex = 21;
			this.btnParticleSystemOff.Text = "Destroy Last";
			this.btnParticleSystemOff.UseVisualStyleBackColor = true;
			this.btnParticleSystemOff.Click += new System.EventHandler(this.btnParticleSystemOff_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 518);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 13);
			this.label3.TabIndex = 23;
			this.label3.Text = "Prefab:";
			// 
			// cmbPrefabs
			// 
			this.cmbPrefabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmbPrefabs.FormattingEnabled = true;
			this.cmbPrefabs.Location = new System.Drawing.Point(56, 515);
			this.cmbPrefabs.MaxDropDownItems = 42;
			this.cmbPrefabs.Name = "cmbPrefabs";
			this.cmbPrefabs.Size = new System.Drawing.Size(162, 21);
			this.cmbPrefabs.TabIndex = 24;
			this.cmbPrefabs.DropDown += new System.EventHandler(this.cmbPrefabs_DropDown);
			this.cmbPrefabs.SelectedIndexChanged += new System.EventHandler(this.cmbPrefabs_SelectedIndexChanged);
			// 
			// btnAttackJanus
			// 
			this.btnAttackJanus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAttackJanus.Location = new System.Drawing.Point(224, 513);
			this.btnAttackJanus.Name = "btnAttackJanus";
			this.btnAttackJanus.Size = new System.Drawing.Size(122, 52);
			this.btnAttackJanus.TabIndex = 21;
			this.btnAttackJanus.Text = "Attack Janus!";
			this.btnAttackJanus.UseVisualStyleBackColor = true;
			this.btnAttackJanus.Click += new System.EventHandler(this.btnAttackJanus_Click);
			// 
			// btnClearAttack
			// 
			this.btnClearAttack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClearAttack.Location = new System.Drawing.Point(352, 513);
			this.btnClearAttack.Name = "btnClearAttack";
			this.btnClearAttack.Size = new System.Drawing.Size(122, 52);
			this.btnClearAttack.TabIndex = 21;
			this.btnClearAttack.Text = "Clear Attack!";
			this.btnClearAttack.UseVisualStyleBackColor = true;
			this.btnClearAttack.Click += new System.EventHandler(this.btnClearAttack_Click);
			// 
			// btnTestEffects
			// 
			this.btnTestEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnTestEffects.Location = new System.Drawing.Point(499, 513);
			this.btnTestEffects.Name = "btnTestEffects";
			this.btnTestEffects.Size = new System.Drawing.Size(90, 31);
			this.btnTestEffects.TabIndex = 21;
			this.btnTestEffects.Text = "Test Effects";
			this.btnTestEffects.UseVisualStyleBackColor = true;
			this.btnTestEffects.Click += new System.EventHandler(this.btnTestEffects_Click);
			// 
			// btnDeserialize
			// 
			this.btnDeserialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeserialize.Location = new System.Drawing.Point(181, 476);
			this.btnDeserialize.Name = "btnDeserialize";
			this.btnDeserialize.Size = new System.Drawing.Size(90, 31);
			this.btnDeserialize.TabIndex = 21;
			this.btnDeserialize.Text = "Deserialize";
			this.btnDeserialize.UseVisualStyleBackColor = true;
			this.btnDeserialize.Click += new System.EventHandler(this.btnDeserialize_Click);
			// 
			// FrmExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1004, 577);
			this.Controls.Add(this.cmbPrefabs);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnParticleSystemOff);
			this.Controls.Add(this.btnDeserialize);
			this.Controls.Add(this.btnTestEffects);
			this.Controls.Add(this.btnClearAttack);
			this.Controls.Add(this.btnAttackJanus);
			this.Controls.Add(this.btnParticleSystemOn);
			this.Controls.Add(this.btnFlashlightOff);
			this.Controls.Add(this.btnFlashlightOn);
			this.Controls.Add(this.btnPlayer);
			this.Controls.Add(this.btnSpectatorMode);
			this.Controls.Add(this.btnGetRuler);
			this.Controls.Add(this.lblFlashlightStatus);
			this.Controls.Add(this.chkTrackFlashlight);
			this.Controls.Add(this.chkListenToEvents);
			this.Controls.Add(this.btnShowRelationEffect);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtEffectName);
			this.Controls.Add(this.btnShowEffect);
			this.Controls.Add(this.btnAddEffects);
			this.Controls.Add(this.btnEffects);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtSunDirection);
			this.Controls.Add(this.btnSetSun);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtTime);
			this.Controls.Add(this.btnSetTime);
			this.Controls.Add(this.tbxLog);
			this.Controls.Add(this.tbxScratch);
			this.Controls.Add(this.btnLoadClasses);
			this.Name = "FrmExplorer";
			this.Text = "FrmExplorer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmExplorer_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnLoadClasses;
		private System.Windows.Forms.TextBox tbxScratch;
		private System.Windows.Forms.TextBox tbxLog;
		private System.Windows.Forms.Button btnSetTime;
		private System.Windows.Forms.TextBox txtTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtSunDirection;
		private System.Windows.Forms.Button btnSetSun;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnEffects;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtEffectName;
		private System.Windows.Forms.Button btnShowEffect;
		private System.Windows.Forms.Button btnShowRelationEffect;
		private System.Windows.Forms.CheckBox chkListenToEvents;
		private System.Windows.Forms.CheckBox chkTrackFlashlight;
		private System.Windows.Forms.Label lblFlashlightStatus;
		private System.Windows.Forms.Button btnGetRuler;
		private System.Windows.Forms.Button btnSpectatorMode;
		private System.Windows.Forms.Button btnPlayer;
		private System.Windows.Forms.Button btnFlashlightOn;
		private System.Windows.Forms.Button btnFlashlightOff;
		private System.Windows.Forms.Button btnAddEffects;
		private System.Windows.Forms.Button btnParticleSystemOn;
		private System.Windows.Forms.Button btnParticleSystemOff;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbPrefabs;
		private System.Windows.Forms.Button btnAttackJanus;
		private System.Windows.Forms.Button btnClearAttack;
		private System.Windows.Forms.Button btnTestEffects;
		private System.Windows.Forms.Button btnDeserialize;
	}
}