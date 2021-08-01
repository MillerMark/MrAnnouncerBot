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
			this.components = new System.ComponentModel.Container();
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
			this.lblCreateAt = new System.Windows.Forms.Label();
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
			this.btnGetActiveGameObjects = new System.Windows.Forms.Button();
			this.btnEditPrefabs = new System.Windows.Forms.Button();
			this.btnTestSetIndicatorColor = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnSetScale = new System.Windows.Forms.Button();
			this.txtScale = new System.Windows.Forms.TextBox();
			this.btnGhostUnghost = new System.Windows.Forms.Button();
			this.btnTest2 = new System.Windows.Forms.Button();
			this.btnSetCameraPosition = new System.Windows.Forms.Button();
			this.tbxCameraPosition = new System.Windows.Forms.TextBox();
			this.btnSetCameraHeight = new System.Windows.Forms.Button();
			this.tbxCameraHeight = new System.Windows.Forms.TextBox();
			this.tbxAssetId = new System.Windows.Forms.TextBox();
			this.chkTrackAngle = new System.Windows.Forms.CheckBox();
			this.lblRotationStatus = new System.Windows.Forms.Label();
			this.btnAngle = new System.Windows.Forms.Button();
			this.btnSet1 = new System.Windows.Forms.Button();
			this.tbx1 = new System.Windows.Forms.TextBox();
			this.btnSet2 = new System.Windows.Forms.Button();
			this.tbx2 = new System.Windows.Forms.TextBox();
			this.btnSet3 = new System.Windows.Forms.Button();
			this.tbx3 = new System.Windows.Forms.TextBox();
			this.btnSkyScraper = new System.Windows.Forms.Button();
			this.tbxTargetLocation = new System.Windows.Forms.TextBox();
			this.btnBattleZone = new System.Windows.Forms.Button();
			this.tbxGhostId = new System.Windows.Forms.TextBox();
			this.lblFlashlightStatus = new System.Windows.Forms.Label();
			this.btnCamera1 = new System.Windows.Forms.Button();
			this.btnCamera2 = new System.Windows.Forms.Button();
			this.btnReloadSpellEffects = new System.Windows.Forms.Button();
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
			this.tbxScratch.Size = new System.Drawing.Size(366, 471);
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
			this.tbxLog.Size = new System.Drawing.Size(423, 455);
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
			this.txtSunDirection.Location = new System.Drawing.Point(48, 142);
			this.txtSunDirection.Name = "txtSunDirection";
			this.txtSunDirection.Size = new System.Drawing.Size(50, 20);
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
			this.btnClear.Location = new System.Drawing.Point(911, 2);
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
			// lblCreateAt
			// 
			this.lblCreateAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCreateAt.AutoSize = true;
			this.lblCreateAt.Location = new System.Drawing.Point(224, 533);
			this.lblCreateAt.Name = "lblCreateAt";
			this.lblCreateAt.Size = new System.Drawing.Size(52, 13);
			this.lblCreateAt.TabIndex = 19;
			this.lblCreateAt.Text = "create at:";
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
			this.btnSpectatorMode.Size = new System.Drawing.Size(75, 23);
			this.btnSpectatorMode.TabIndex = 21;
			this.btnSpectatorMode.Text = "Spectator";
			this.btnSpectatorMode.UseVisualStyleBackColor = true;
			this.btnSpectatorMode.Click += new System.EventHandler(this.btnSpectatorMode_Click);
			// 
			// btnPlayer
			// 
			this.btnPlayer.Location = new System.Drawing.Point(93, 206);
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
			this.btnParticleSystemOn.Location = new System.Drawing.Point(56, 557);
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
			this.btnParticleSystemOff.Location = new System.Drawing.Point(131, 557);
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
			this.label3.Location = new System.Drawing.Point(10, 533);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 13);
			this.label3.TabIndex = 23;
			this.label3.Text = "Prefab:";
			// 
			// cmbPrefabs
			// 
			this.cmbPrefabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmbPrefabs.FormattingEnabled = true;
			this.cmbPrefabs.Location = new System.Drawing.Point(56, 530);
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
			this.btnAttackJanus.Location = new System.Drawing.Point(224, 557);
			this.btnAttackJanus.Name = "btnAttackJanus";
			this.btnAttackJanus.Size = new System.Drawing.Size(99, 23);
			this.btnAttackJanus.TabIndex = 21;
			this.btnAttackJanus.Text = "Attack Janus!";
			this.btnAttackJanus.UseVisualStyleBackColor = true;
			this.btnAttackJanus.Click += new System.EventHandler(this.btnAttackJanus_Click);
			// 
			// btnClearAttack
			// 
			this.btnClearAttack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClearAttack.Location = new System.Drawing.Point(329, 557);
			this.btnClearAttack.Name = "btnClearAttack";
			this.btnClearAttack.Size = new System.Drawing.Size(99, 23);
			this.btnClearAttack.TabIndex = 21;
			this.btnClearAttack.Text = "Clear Attack!";
			this.btnClearAttack.UseVisualStyleBackColor = true;
			this.btnClearAttack.Click += new System.EventHandler(this.btnClearAttack_Click);
			// 
			// btnTestEffects
			// 
			this.btnTestEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnTestEffects.Location = new System.Drawing.Point(457, 557);
			this.btnTestEffects.Name = "btnTestEffects";
			this.btnTestEffects.Size = new System.Drawing.Size(90, 23);
			this.btnTestEffects.TabIndex = 21;
			this.btnTestEffects.Text = "Test Effects";
			this.btnTestEffects.UseVisualStyleBackColor = true;
			this.btnTestEffects.Click += new System.EventHandler(this.btnTestEffects_Click);
			// 
			// btnGetActiveGameObjects
			// 
			this.btnGetActiveGameObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnGetActiveGameObjects.Location = new System.Drawing.Point(181, 492);
			this.btnGetActiveGameObjects.Name = "btnGetActiveGameObjects";
			this.btnGetActiveGameObjects.Size = new System.Drawing.Size(127, 31);
			this.btnGetActiveGameObjects.TabIndex = 21;
			this.btnGetActiveGameObjects.Text = "Get Game Objects";
			this.btnGetActiveGameObjects.UseVisualStyleBackColor = true;
			this.btnGetActiveGameObjects.Click += new System.EventHandler(this.btnGetActiveGameObjects_Click);
			// 
			// btnEditPrefabs
			// 
			this.btnEditPrefabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnEditPrefabs.Location = new System.Drawing.Point(578, 533);
			this.btnEditPrefabs.Name = "btnEditPrefabs";
			this.btnEditPrefabs.Size = new System.Drawing.Size(135, 56);
			this.btnEditPrefabs.TabIndex = 21;
			this.btnEditPrefabs.Text = "Effect Editor...";
			this.btnEditPrefabs.UseVisualStyleBackColor = true;
			this.btnEditPrefabs.Click += new System.EventHandler(this.btnEditPrefabs_Click);
			// 
			// btnTestSetIndicatorColor
			// 
			this.btnTestSetIndicatorColor.Location = new System.Drawing.Point(992, 31);
			this.btnTestSetIndicatorColor.Name = "btnTestSetIndicatorColor";
			this.btnTestSetIndicatorColor.Size = new System.Drawing.Size(115, 31);
			this.btnTestSetIndicatorColor.TabIndex = 21;
			this.btnTestSetIndicatorColor.Text = "Set Base Indicator";
			this.toolTip1.SetToolTip(this.btnTestSetIndicatorColor, "Sets the Base Indicator for Merkin. Shift+click to clear.");
			this.btnTestSetIndicatorColor.UseVisualStyleBackColor = true;
			this.btnTestSetIndicatorColor.Click += new System.EventHandler(this.btnTestSetIndicatorColor_Click);
			// 
			// btnSetScale
			// 
			this.btnSetScale.Location = new System.Drawing.Point(1061, 68);
			this.btnSetScale.Name = "btnSetScale";
			this.btnSetScale.Size = new System.Drawing.Size(77, 31);
			this.btnSetScale.TabIndex = 21;
			this.btnSetScale.Text = "Set Scale";
			this.btnSetScale.UseVisualStyleBackColor = true;
			this.btnSetScale.Click += new System.EventHandler(this.btnSetScale_Click);
			// 
			// txtScale
			// 
			this.txtScale.Location = new System.Drawing.Point(992, 73);
			this.txtScale.Name = "txtScale";
			this.txtScale.Size = new System.Drawing.Size(63, 20);
			this.txtScale.TabIndex = 25;
			this.txtScale.Text = "1";
			// 
			// btnGhostUnghost
			// 
			this.btnGhostUnghost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGhostUnghost.Location = new System.Drawing.Point(836, 521);
			this.btnGhostUnghost.Name = "btnGhostUnghost";
			this.btnGhostUnghost.Size = new System.Drawing.Size(88, 31);
			this.btnGhostUnghost.TabIndex = 26;
			this.btnGhostUnghost.Text = "Ghost:";
			this.btnGhostUnghost.UseVisualStyleBackColor = true;
			this.btnGhostUnghost.Click += new System.EventHandler(this.btnGhostToggle_Click);
			// 
			// btnTest2
			// 
			this.btnTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTest2.Location = new System.Drawing.Point(836, 558);
			this.btnTest2.Name = "btnTest2";
			this.btnTest2.Size = new System.Drawing.Size(88, 31);
			this.btnTest2.TabIndex = 26;
			this.btnTest2.Text = "Test 2";
			this.btnTest2.UseVisualStyleBackColor = true;
			this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
			// 
			// btnSetCameraPosition
			// 
			this.btnSetCameraPosition.Location = new System.Drawing.Point(1061, 115);
			this.btnSetCameraPosition.Name = "btnSetCameraPosition";
			this.btnSetCameraPosition.Size = new System.Drawing.Size(126, 26);
			this.btnSetCameraPosition.TabIndex = 21;
			this.btnSetCameraPosition.Text = "Set Camera Position";
			this.btnSetCameraPosition.UseVisualStyleBackColor = true;
			this.btnSetCameraPosition.Click += new System.EventHandler(this.btnSetCameraPosition_Click);
			// 
			// tbxCameraPosition
			// 
			this.tbxCameraPosition.Location = new System.Drawing.Point(992, 119);
			this.tbxCameraPosition.Name = "tbxCameraPosition";
			this.tbxCameraPosition.Size = new System.Drawing.Size(63, 20);
			this.tbxCameraPosition.TabIndex = 25;
			this.tbxCameraPosition.Text = "0, 0, 0";
			// 
			// btnSetCameraHeight
			// 
			this.btnSetCameraHeight.Location = new System.Drawing.Point(1061, 149);
			this.btnSetCameraHeight.Name = "btnSetCameraHeight";
			this.btnSetCameraHeight.Size = new System.Drawing.Size(126, 26);
			this.btnSetCameraHeight.TabIndex = 21;
			this.btnSetCameraHeight.Text = "Set Camera Height";
			this.btnSetCameraHeight.UseVisualStyleBackColor = true;
			this.btnSetCameraHeight.Click += new System.EventHandler(this.btnSetCameraHeight_Click);
			// 
			// tbxCameraHeight
			// 
			this.tbxCameraHeight.Location = new System.Drawing.Point(992, 153);
			this.tbxCameraHeight.Name = "tbxCameraHeight";
			this.tbxCameraHeight.Size = new System.Drawing.Size(63, 20);
			this.tbxCameraHeight.TabIndex = 25;
			this.tbxCameraHeight.Text = "10";
			// 
			// tbxAssetId
			// 
			this.tbxAssetId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxAssetId.Location = new System.Drawing.Point(930, 564);
			this.tbxAssetId.Name = "tbxAssetId";
			this.tbxAssetId.Size = new System.Drawing.Size(389, 20);
			this.tbxAssetId.TabIndex = 25;
			this.tbxAssetId.Text = "28f7710e-9f79-425b-ac6e-216677454ec3";
			// 
			// chkTrackAngle
			// 
			this.chkTrackAngle.AutoSize = true;
			this.chkTrackAngle.Location = new System.Drawing.Point(10, 483);
			this.chkTrackAngle.Name = "chkTrackAngle";
			this.chkTrackAngle.Size = new System.Drawing.Size(84, 17);
			this.chkTrackAngle.TabIndex = 18;
			this.chkTrackAngle.Text = "Track Angle";
			this.chkTrackAngle.UseVisualStyleBackColor = true;
			this.chkTrackAngle.CheckedChanged += new System.EventHandler(this.chkTrackAngle_CheckedChanged);
			// 
			// lblRotationStatus
			// 
			this.lblRotationStatus.AutoSize = true;
			this.lblRotationStatus.Location = new System.Drawing.Point(27, 503);
			this.lblRotationStatus.Name = "lblRotationStatus";
			this.lblRotationStatus.Size = new System.Drawing.Size(69, 13);
			this.lblRotationStatus.TabIndex = 19;
			this.lblRotationStatus.Text = "(not tracking)";
			// 
			// btnAngle
			// 
			this.btnAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAngle.Location = new System.Drawing.Point(102, 483);
			this.btnAngle.Name = "btnAngle";
			this.btnAngle.Size = new System.Drawing.Size(58, 31);
			this.btnAngle.TabIndex = 21;
			this.btnAngle.Text = "Angle?";
			this.btnAngle.UseVisualStyleBackColor = true;
			this.btnAngle.Click += new System.EventHandler(this.btnAngle_Click);
			// 
			// btnSet1
			// 
			this.btnSet1.Location = new System.Drawing.Point(1061, 181);
			this.btnSet1.Name = "btnSet1";
			this.btnSet1.Size = new System.Drawing.Size(126, 26);
			this.btnSet1.TabIndex = 21;
			this.btnSet1.Text = "Set Mini Rotation";
			this.btnSet1.UseVisualStyleBackColor = true;
			this.btnSet1.Click += new System.EventHandler(this.btnSet1_Click);
			// 
			// tbx1
			// 
			this.tbx1.Location = new System.Drawing.Point(992, 185);
			this.tbx1.Name = "tbx1";
			this.tbx1.Size = new System.Drawing.Size(63, 20);
			this.tbx1.TabIndex = 25;
			this.tbx1.Text = "10";
			// 
			// btnSet2
			// 
			this.btnSet2.Location = new System.Drawing.Point(1061, 213);
			this.btnSet2.Name = "btnSet2";
			this.btnSet2.Size = new System.Drawing.Size(126, 26);
			this.btnSet2.TabIndex = 21;
			this.btnSet2.Text = "Target Rotate Up";
			this.btnSet2.UseVisualStyleBackColor = true;
			this.btnSet2.Click += new System.EventHandler(this.btnSet2_Click);
			// 
			// tbx2
			// 
			this.tbx2.Location = new System.Drawing.Point(992, 217);
			this.tbx2.Name = "tbx2";
			this.tbx2.Size = new System.Drawing.Size(63, 20);
			this.tbx2.TabIndex = 25;
			this.tbx2.Text = "10";
			// 
			// btnSet3
			// 
			this.btnSet3.Location = new System.Drawing.Point(1061, 245);
			this.btnSet3.Name = "btnSet3";
			this.btnSet3.Size = new System.Drawing.Size(126, 26);
			this.btnSet3.TabIndex = 21;
			this.btnSet3.Text = "Target Rotate Forward";
			this.btnSet3.UseVisualStyleBackColor = true;
			this.btnSet3.Click += new System.EventHandler(this.btnSet3_Click);
			// 
			// tbx3
			// 
			this.tbx3.Location = new System.Drawing.Point(992, 249);
			this.tbx3.Name = "tbx3";
			this.tbx3.Size = new System.Drawing.Size(63, 20);
			this.tbx3.TabIndex = 25;
			this.tbx3.Text = "10";
			// 
			// btnSkyScraper
			// 
			this.btnSkyScraper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSkyScraper.Location = new System.Drawing.Point(384, 530);
			this.btnSkyScraper.Name = "btnSkyScraper";
			this.btnSkyScraper.Size = new System.Drawing.Size(73, 21);
			this.btnSkyScraper.TabIndex = 26;
			this.btnSkyScraper.Text = "Skyscraper";
			this.btnSkyScraper.UseVisualStyleBackColor = true;
			this.btnSkyScraper.Click += new System.EventHandler(this.btnSkyScraper_Click);
			// 
			// tbxTargetLocation
			// 
			this.tbxTargetLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbxTargetLocation.Location = new System.Drawing.Point(282, 531);
			this.tbxTargetLocation.Name = "tbxTargetLocation";
			this.tbxTargetLocation.Size = new System.Drawing.Size(96, 20);
			this.tbxTargetLocation.TabIndex = 7;
			this.tbxTargetLocation.Text = "-1,0.2,0";
			// 
			// btnBattleZone
			// 
			this.btnBattleZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnBattleZone.Location = new System.Drawing.Point(463, 530);
			this.btnBattleZone.Name = "btnBattleZone";
			this.btnBattleZone.Size = new System.Drawing.Size(73, 21);
			this.btnBattleZone.TabIndex = 26;
			this.btnBattleZone.Text = "Battle Zone";
			this.btnBattleZone.UseVisualStyleBackColor = true;
			this.btnBattleZone.Click += new System.EventHandler(this.btnBattleZone_Click);
			// 
			// tbxGhostId
			// 
			this.tbxGhostId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxGhostId.Location = new System.Drawing.Point(930, 527);
			this.tbxGhostId.Name = "tbxGhostId";
			this.tbxGhostId.Size = new System.Drawing.Size(389, 20);
			this.tbxGhostId.TabIndex = 25;
			this.tbxGhostId.Text = "35400cec-9539-424f-b185-00569d4850c4";
			// 
			// lblFlashlightStatus
			// 
			this.lblFlashlightStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblFlashlightStatus.AutoSize = true;
			this.lblFlashlightStatus.Location = new System.Drawing.Point(27, 458);
			this.lblFlashlightStatus.Name = "lblFlashlightStatus";
			this.lblFlashlightStatus.Size = new System.Drawing.Size(52, 13);
			this.lblFlashlightStatus.TabIndex = 19;
			this.lblFlashlightStatus.Text = "create at:";
			// 
			// btnCamera1
			// 
			this.btnCamera1.Location = new System.Drawing.Point(1193, 115);
			this.btnCamera1.Name = "btnCamera1";
			this.btnCamera1.Size = new System.Drawing.Size(66, 26);
			this.btnCamera1.TabIndex = 21;
			this.btnCamera1.Text = "Camera 1";
			this.btnCamera1.UseVisualStyleBackColor = true;
			this.btnCamera1.Click += new System.EventHandler(this.btnCamera1_Click);
			// 
			// btnCamera2
			// 
			this.btnCamera2.Location = new System.Drawing.Point(1265, 116);
			this.btnCamera2.Name = "btnCamera2";
			this.btnCamera2.Size = new System.Drawing.Size(66, 26);
			this.btnCamera2.TabIndex = 21;
			this.btnCamera2.Text = "Camera 2";
			this.btnCamera2.UseVisualStyleBackColor = true;
			this.btnCamera2.Click += new System.EventHandler(this.btnCamera2_Click);
			// 
			// btnReloadSpellEffects
			// 
			this.btnReloadSpellEffects.Location = new System.Drawing.Point(989, 282);
			this.btnReloadSpellEffects.Name = "btnReloadSpellEffects";
			this.btnReloadSpellEffects.Size = new System.Drawing.Size(169, 26);
			this.btnReloadSpellEffects.TabIndex = 21;
			this.btnReloadSpellEffects.Text = "Reload Spell Effects";
			this.btnReloadSpellEffects.UseVisualStyleBackColor = true;
			this.btnReloadSpellEffects.Click += new System.EventHandler(this.btnReloadSpellEffects_Click);
			// 
			// FrmExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1331, 592);
			this.Controls.Add(this.btnTest2);
			this.Controls.Add(this.btnBattleZone);
			this.Controls.Add(this.btnSkyScraper);
			this.Controls.Add(this.btnGhostUnghost);
			this.Controls.Add(this.tbx3);
			this.Controls.Add(this.tbx2);
			this.Controls.Add(this.tbx1);
			this.Controls.Add(this.tbxCameraHeight);
			this.Controls.Add(this.tbxCameraPosition);
			this.Controls.Add(this.tbxGhostId);
			this.Controls.Add(this.tbxAssetId);
			this.Controls.Add(this.txtScale);
			this.Controls.Add(this.cmbPrefabs);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnParticleSystemOff);
			this.Controls.Add(this.btnSet3);
			this.Controls.Add(this.btnSet2);
			this.Controls.Add(this.btnSet1);
			this.Controls.Add(this.btnSetCameraHeight);
			this.Controls.Add(this.btnCamera2);
			this.Controls.Add(this.btnReloadSpellEffects);
			this.Controls.Add(this.btnCamera1);
			this.Controls.Add(this.btnSetCameraPosition);
			this.Controls.Add(this.btnAngle);
			this.Controls.Add(this.btnGetActiveGameObjects);
			this.Controls.Add(this.btnSetScale);
			this.Controls.Add(this.btnTestSetIndicatorColor);
			this.Controls.Add(this.btnEditPrefabs);
			this.Controls.Add(this.btnTestEffects);
			this.Controls.Add(this.btnClearAttack);
			this.Controls.Add(this.btnAttackJanus);
			this.Controls.Add(this.btnParticleSystemOn);
			this.Controls.Add(this.btnFlashlightOff);
			this.Controls.Add(this.btnFlashlightOn);
			this.Controls.Add(this.btnPlayer);
			this.Controls.Add(this.btnSpectatorMode);
			this.Controls.Add(this.btnGetRuler);
			this.Controls.Add(this.lblRotationStatus);
			this.Controls.Add(this.lblFlashlightStatus);
			this.Controls.Add(this.lblCreateAt);
			this.Controls.Add(this.chkTrackAngle);
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
			this.Controls.Add(this.tbxTargetLocation);
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
		private System.Windows.Forms.Label lblCreateAt;
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
		private System.Windows.Forms.Button btnGetActiveGameObjects;
		private System.Windows.Forms.Button btnEditPrefabs;
		private System.Windows.Forms.Button btnTestSetIndicatorColor;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnSetScale;
		private System.Windows.Forms.TextBox txtScale;
		private System.Windows.Forms.Button btnGhostUnghost;
		private System.Windows.Forms.Button btnTest2;
		private System.Windows.Forms.Button btnSetCameraPosition;
		private System.Windows.Forms.TextBox tbxCameraPosition;
		private System.Windows.Forms.Button btnSetCameraHeight;
		private System.Windows.Forms.TextBox tbxCameraHeight;
		private System.Windows.Forms.TextBox tbxAssetId;
		private System.Windows.Forms.CheckBox chkTrackAngle;
		private System.Windows.Forms.Label lblRotationStatus;
		private System.Windows.Forms.Button btnAngle;
		private System.Windows.Forms.Button btnSet1;
		private System.Windows.Forms.TextBox tbx1;
		private System.Windows.Forms.Button btnSet2;
		private System.Windows.Forms.TextBox tbx2;
		private System.Windows.Forms.Button btnSet3;
		private System.Windows.Forms.TextBox tbx3;
		private System.Windows.Forms.Button btnSkyScraper;
		private System.Windows.Forms.TextBox tbxTargetLocation;
		private System.Windows.Forms.Button btnBattleZone;
		private System.Windows.Forms.TextBox tbxGhostId;
		private System.Windows.Forms.Label lblFlashlightStatus;
		private System.Windows.Forms.Button btnCamera1;
		private System.Windows.Forms.Button btnCamera2;
		private System.Windows.Forms.Button btnReloadSpellEffects;
	}
}