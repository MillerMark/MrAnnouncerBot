namespace TaleSpireExplore
{
	partial class FrmEffectEditor
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
			this.btnCreateEmpty = new System.Windows.Forms.Button();
			this.btnCopyJson = new System.Windows.Forms.Button();
			this.tbxJson = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trvEffectHierarchy = new System.Windows.Forms.TreeView();
			this.ctxGameObjects = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.addScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnAddPrefab = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tbxSearch = new System.Windows.Forms.TextBox();
			this.btnFindPrevious = new System.Windows.Forms.Button();
			this.btnFindNext = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.pnlProperties = new System.Windows.Forms.Panel();
			this.btnClearEffect = new System.Windows.Forms.Button();
			this.btnTestEffect = new System.Windows.Forms.Button();
			this.lblPropertyName = new System.Windows.Forms.Label();
			this.trvProperties = new System.Windows.Forms.TreeView();
			this.btnStopAllParticleSystems = new System.Windows.Forms.Button();
			this.btnStartAllParticleSystems = new System.Windows.Forms.Button();
			this.btnAttackJanus = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.tbxTravelTime = new System.Windows.Forms.TextBox();
			this.btnMeshRendererMaterial = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnParticleSystemRenderer = new System.Windows.Forms.Button();
			this.btnLocalScale = new System.Windows.Forms.Button();
			this.btnLocalPosition = new System.Windows.Forms.Button();
			this.btnCloneExisting = new System.Windows.Forms.Button();
			this.btnExploreMini = new System.Windows.Forms.Button();
			this.btnExploreExisting = new System.Windows.Forms.Button();
			this.btnCreateEffect = new System.Windows.Forms.Button();
			this.btnSaveEffect = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.txtEffectName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btnTaleSpireCamera = new System.Windows.Forms.Button();
			this.btnLocalEulerAngles = new System.Windows.Forms.Button();
			this.btnShapeRadius = new System.Windows.Forms.Button();
			this.btnAddPropTest = new System.Windows.Forms.Button();
			this.ctxGameObjects.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCreateEmpty
			// 
			this.btnCreateEmpty.Location = new System.Drawing.Point(141, 10);
			this.btnCreateEmpty.Name = "btnCreateEmpty";
			this.btnCreateEmpty.Size = new System.Drawing.Size(123, 26);
			this.btnCreateEmpty.TabIndex = 2;
			this.btnCreateEmpty.Text = "Create Empty...";
			this.toolTip1.SetToolTip(this.btnCreateEmpty, "Add a new GameObject to the effect.");
			this.btnCreateEmpty.UseVisualStyleBackColor = true;
			this.btnCreateEmpty.Click += new System.EventHandler(this.btnCreateEmpty_Click);
			// 
			// btnCopyJson
			// 
			this.btnCopyJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCopyJson.Location = new System.Drawing.Point(953, 12);
			this.btnCopyJson.Name = "btnCopyJson";
			this.btnCopyJson.Size = new System.Drawing.Size(84, 26);
			this.btnCopyJson.TabIndex = 2;
			this.btnCopyJson.Text = "Copy JSON";
			this.btnCopyJson.UseVisualStyleBackColor = true;
			this.btnCopyJson.Click += new System.EventHandler(this.btnCopyJson_Click);
			// 
			// tbxJson
			// 
			this.tbxJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxJson.Location = new System.Drawing.Point(751, 43);
			this.tbxJson.Multiline = true;
			this.tbxJson.Name = "tbxJson";
			this.tbxJson.Size = new System.Drawing.Size(286, 544);
			this.tbxJson.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(748, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "JSON:";
			// 
			// trvEffectHierarchy
			// 
			this.trvEffectHierarchy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.trvEffectHierarchy.CheckBoxes = true;
			this.trvEffectHierarchy.ContextMenuStrip = this.ctxGameObjects;
			this.trvEffectHierarchy.HideSelection = false;
			this.trvEffectHierarchy.Location = new System.Drawing.Point(12, 133);
			this.trvEffectHierarchy.Name = "trvEffectHierarchy";
			this.trvEffectHierarchy.Size = new System.Drawing.Size(269, 454);
			this.trvEffectHierarchy.TabIndex = 5;
			this.trvEffectHierarchy.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvEffectHierarchy_AfterCheck);
			this.trvEffectHierarchy.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvEffectHierarchy_NodeMouseClick);
			// 
			// ctxGameObjects
			// 
			this.ctxGameObjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addScriptToolStripMenuItem});
			this.ctxGameObjects.Name = "ctxGameObjects";
			this.ctxGameObjects.Size = new System.Drawing.Size(130, 54);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(126, 6);
			// 
			// addScriptToolStripMenuItem
			// 
			this.addScriptToolStripMenuItem.Name = "addScriptToolStripMenuItem";
			this.addScriptToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.addScriptToolStripMenuItem.Text = "Add Script";
			this.addScriptToolStripMenuItem.DropDownOpening += new System.EventHandler(this.addScriptToolStripMenuItem_DropDownOpening);
			// 
			// btnAddPrefab
			// 
			this.btnAddPrefab.Location = new System.Drawing.Point(12, 10);
			this.btnAddPrefab.Name = "btnAddPrefab";
			this.btnAddPrefab.Size = new System.Drawing.Size(123, 26);
			this.btnAddPrefab.TabIndex = 0;
			this.btnAddPrefab.Text = "Add Prefab...";
			this.toolTip1.SetToolTip(this.btnAddPrefab, "Add a new prefab child node to the effect.");
			this.btnAddPrefab.UseVisualStyleBackColor = true;
			this.btnAddPrefab.Click += new System.EventHandler(this.btnAddPrefab_Click);
			// 
			// tbxSearch
			// 
			this.tbxSearch.Location = new System.Drawing.Point(320, 29);
			this.tbxSearch.Name = "tbxSearch";
			this.tbxSearch.Size = new System.Drawing.Size(81, 20);
			this.tbxSearch.TabIndex = 10;
			this.toolTip1.SetToolTip(this.tbxSearch, "Enter property text to search");
			this.tbxSearch.TextChanged += new System.EventHandler(this.tbxSearch_TextChanged);
			// 
			// btnFindPrevious
			// 
			this.btnFindPrevious.Location = new System.Drawing.Point(407, 28);
			this.btnFindPrevious.Name = "btnFindPrevious";
			this.btnFindPrevious.Size = new System.Drawing.Size(30, 23);
			this.btnFindPrevious.TabIndex = 3;
			this.btnFindPrevious.Text = "<<";
			this.toolTip1.SetToolTip(this.btnFindPrevious, "Find previous");
			this.btnFindPrevious.UseVisualStyleBackColor = true;
			this.btnFindPrevious.Click += new System.EventHandler(this.btnFindPrevious_Click);
			// 
			// btnFindNext
			// 
			this.btnFindNext.Location = new System.Drawing.Point(443, 28);
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Size = new System.Drawing.Size(30, 23);
			this.btnFindNext.TabIndex = 3;
			this.btnFindNext.Text = ">>";
			this.toolTip1.SetToolTip(this.btnFindNext, "Find next");
			this.btnFindNext.UseVisualStyleBackColor = true;
			this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(284, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Properties:";
			// 
			// pnlProperties
			// 
			this.pnlProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pnlProperties.Location = new System.Drawing.Point(483, 192);
			this.pnlProperties.Name = "pnlProperties";
			this.pnlProperties.Size = new System.Drawing.Size(262, 395);
			this.pnlProperties.TabIndex = 7;
			// 
			// btnClearEffect
			// 
			this.btnClearEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClearEffect.Location = new System.Drawing.Point(172, 593);
			this.btnClearEffect.Name = "btnClearEffect";
			this.btnClearEffect.Size = new System.Drawing.Size(109, 26);
			this.btnClearEffect.TabIndex = 3;
			this.btnClearEffect.Text = "Delete Everything";
			this.btnClearEffect.UseVisualStyleBackColor = true;
			this.btnClearEffect.Click += new System.EventHandler(this.btnClearEffect_Click);
			// 
			// btnTestEffect
			// 
			this.btnTestEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTestEffect.Location = new System.Drawing.Point(857, 12);
			this.btnTestEffect.Name = "btnTestEffect";
			this.btnTestEffect.Size = new System.Drawing.Size(90, 26);
			this.btnTestEffect.TabIndex = 3;
			this.btnTestEffect.Text = "Test Effect";
			this.btnTestEffect.UseVisualStyleBackColor = true;
			this.btnTestEffect.Click += new System.EventHandler(this.btnTestEffect_Click);
			// 
			// lblPropertyName
			// 
			this.lblPropertyName.AutoSize = true;
			this.lblPropertyName.Location = new System.Drawing.Point(480, 176);
			this.lblPropertyName.Name = "lblPropertyName";
			this.lblPropertyName.Size = new System.Drawing.Size(37, 13);
			this.lblPropertyName.TabIndex = 8;
			this.lblPropertyName.Text = "Value:";
			// 
			// trvProperties
			// 
			this.trvProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.trvProperties.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.trvProperties.HideSelection = false;
			this.trvProperties.Location = new System.Drawing.Point(287, 53);
			this.trvProperties.Name = "trvProperties";
			this.trvProperties.Size = new System.Drawing.Size(186, 534);
			this.trvProperties.TabIndex = 9;
			this.trvProperties.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvProperties_BeforeExpand);
			this.trvProperties.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.trvProperties_DrawNode);
			this.trvProperties.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvProperties_AfterSelect);
			// 
			// btnStopAllParticleSystems
			// 
			this.btnStopAllParticleSystems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStopAllParticleSystems.Location = new System.Drawing.Point(387, 593);
			this.btnStopAllParticleSystems.Name = "btnStopAllParticleSystems";
			this.btnStopAllParticleSystems.Size = new System.Drawing.Size(86, 26);
			this.btnStopAllParticleSystems.TabIndex = 4;
			this.btnStopAllParticleSystems.Text = "Stop All PS";
			this.btnStopAllParticleSystems.UseVisualStyleBackColor = true;
			this.btnStopAllParticleSystems.Click += new System.EventHandler(this.btnStopAllParticleSystems_Click);
			// 
			// btnStartAllParticleSystems
			// 
			this.btnStartAllParticleSystems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStartAllParticleSystems.Location = new System.Drawing.Point(287, 593);
			this.btnStartAllParticleSystems.Name = "btnStartAllParticleSystems";
			this.btnStartAllParticleSystems.Size = new System.Drawing.Size(86, 26);
			this.btnStartAllParticleSystems.TabIndex = 4;
			this.btnStartAllParticleSystems.Text = "Start All PS";
			this.btnStartAllParticleSystems.UseVisualStyleBackColor = true;
			this.btnStartAllParticleSystems.Click += new System.EventHandler(this.btnStartAllParticleSystems_Click);
			// 
			// btnAttackJanus
			// 
			this.btnAttackJanus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAttackJanus.Location = new System.Drawing.Point(655, 593);
			this.btnAttackJanus.Name = "btnAttackJanus";
			this.btnAttackJanus.Size = new System.Drawing.Size(90, 26);
			this.btnAttackJanus.TabIndex = 3;
			this.btnAttackJanus.Text = "Attack Janus";
			this.btnAttackJanus.UseVisualStyleBackColor = true;
			this.btnAttackJanus.Click += new System.EventHandler(this.btnAttackJanus_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(482, 600);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(66, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Travel Time:";
			// 
			// tbxTravelTime
			// 
			this.tbxTravelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbxTravelTime.Location = new System.Drawing.Point(548, 597);
			this.tbxTravelTime.Name = "tbxTravelTime";
			this.tbxTravelTime.Size = new System.Drawing.Size(100, 20);
			this.tbxTravelTime.TabIndex = 12;
			// 
			// btnMeshRendererMaterial
			// 
			this.btnMeshRendererMaterial.Enabled = false;
			this.btnMeshRendererMaterial.Location = new System.Drawing.Point(481, 81);
			this.btnMeshRendererMaterial.Name = "btnMeshRendererMaterial";
			this.btnMeshRendererMaterial.Size = new System.Drawing.Size(137, 23);
			this.btnMeshRendererMaterial.TabIndex = 13;
			this.btnMeshRendererMaterial.Text = "<MeshRenderer>.material";
			this.btnMeshRendererMaterial.UseVisualStyleBackColor = true;
			this.btnMeshRendererMaterial.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(480, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Jump to:";
			// 
			// btnParticleSystemRenderer
			// 
			this.btnParticleSystemRenderer.Enabled = false;
			this.btnParticleSystemRenderer.Location = new System.Drawing.Point(481, 59);
			this.btnParticleSystemRenderer.Name = "btnParticleSystemRenderer";
			this.btnParticleSystemRenderer.Size = new System.Drawing.Size(181, 23);
			this.btnParticleSystemRenderer.TabIndex = 13;
			this.btnParticleSystemRenderer.Text = "<ParticleSystemRenderer>.material";
			this.btnParticleSystemRenderer.UseVisualStyleBackColor = true;
			this.btnParticleSystemRenderer.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// btnLocalScale
			// 
			this.btnLocalScale.Enabled = false;
			this.btnLocalScale.Location = new System.Drawing.Point(481, 125);
			this.btnLocalScale.Name = "btnLocalScale";
			this.btnLocalScale.Size = new System.Drawing.Size(126, 23);
			this.btnLocalScale.TabIndex = 14;
			this.btnLocalScale.Text = "<Transform>.localScale";
			this.btnLocalScale.UseVisualStyleBackColor = true;
			this.btnLocalScale.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// btnLocalPosition
			// 
			this.btnLocalPosition.Enabled = false;
			this.btnLocalPosition.Location = new System.Drawing.Point(481, 103);
			this.btnLocalPosition.Name = "btnLocalPosition";
			this.btnLocalPosition.Size = new System.Drawing.Size(137, 23);
			this.btnLocalPosition.TabIndex = 14;
			this.btnLocalPosition.Text = "<Transform>.localPosition";
			this.btnLocalPosition.UseVisualStyleBackColor = true;
			this.btnLocalPosition.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// btnCloneExisting
			// 
			this.btnCloneExisting.Location = new System.Drawing.Point(141, 39);
			this.btnCloneExisting.Name = "btnCloneExisting";
			this.btnCloneExisting.Size = new System.Drawing.Size(123, 26);
			this.btnCloneExisting.TabIndex = 2;
			this.btnCloneExisting.Text = "Clone Existing...";
			this.btnCloneExisting.UseVisualStyleBackColor = true;
			this.btnCloneExisting.Click += new System.EventHandler(this.btnCloneExisting_Click);
			// 
			// btnExploreMini
			// 
			this.btnExploreMini.Location = new System.Drawing.Point(12, 39);
			this.btnExploreMini.Name = "btnExploreMini";
			this.btnExploreMini.Size = new System.Drawing.Size(123, 26);
			this.btnExploreMini.TabIndex = 2;
			this.btnExploreMini.Text = "Explore Mini...";
			this.btnExploreMini.UseVisualStyleBackColor = true;
			this.btnExploreMini.Click += new System.EventHandler(this.btnExploreMini_Click);
			// 
			// btnExploreExisting
			// 
			this.btnExploreExisting.Location = new System.Drawing.Point(12, 69);
			this.btnExploreExisting.Name = "btnExploreExisting";
			this.btnExploreExisting.Size = new System.Drawing.Size(123, 26);
			this.btnExploreExisting.TabIndex = 2;
			this.btnExploreExisting.Text = "Explore Existing...";
			this.btnExploreExisting.UseVisualStyleBackColor = true;
			this.btnExploreExisting.Click += new System.EventHandler(this.btnExploreExisting_Click);
			// 
			// btnCreateEffect
			// 
			this.btnCreateEffect.Location = new System.Drawing.Point(141, 69);
			this.btnCreateEffect.Name = "btnCreateEffect";
			this.btnCreateEffect.Size = new System.Drawing.Size(123, 26);
			this.btnCreateEffect.TabIndex = 2;
			this.btnCreateEffect.Text = "Known Effect...";
			this.btnCreateEffect.UseVisualStyleBackColor = true;
			this.btnCreateEffect.Click += new System.EventHandler(this.btnCreateEffect_Click);
			// 
			// btnSaveEffect
			// 
			this.btnSaveEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSaveEffect.Enabled = false;
			this.btnSaveEffect.Location = new System.Drawing.Point(953, 593);
			this.btnSaveEffect.Name = "btnSaveEffect";
			this.btnSaveEffect.Size = new System.Drawing.Size(84, 26);
			this.btnSaveEffect.TabIndex = 15;
			this.btnSaveEffect.Text = "Save Effect";
			this.btnSaveEffect.UseVisualStyleBackColor = true;
			this.btnSaveEffect.Click += new System.EventHandler(this.btnSaveEffect_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(748, 600);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Name:";
			// 
			// txtEffectName
			// 
			this.txtEffectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEffectName.Location = new System.Drawing.Point(789, 597);
			this.txtEffectName.Name = "txtEffectName";
			this.txtEffectName.Size = new System.Drawing.Size(158, 20);
			this.txtEffectName.TabIndex = 12;
			this.txtEffectName.TextChanged += new System.EventHandler(this.txtEffectName_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(284, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(30, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "Find:";
			// 
			// btnTaleSpireCamera
			// 
			this.btnTaleSpireCamera.Location = new System.Drawing.Point(12, 101);
			this.btnTaleSpireCamera.Name = "btnTaleSpireCamera";
			this.btnTaleSpireCamera.Size = new System.Drawing.Size(123, 26);
			this.btnTaleSpireCamera.TabIndex = 2;
			this.btnTaleSpireCamera.Text = "TaleSpire Camera";
			this.btnTaleSpireCamera.UseVisualStyleBackColor = true;
			this.btnTaleSpireCamera.Click += new System.EventHandler(this.btnTaleSpireCamera_Click);
			// 
			// btnLocalEulerAngles
			// 
			this.btnLocalEulerAngles.Enabled = false;
			this.btnLocalEulerAngles.Location = new System.Drawing.Point(481, 147);
			this.btnLocalEulerAngles.Name = "btnLocalEulerAngles";
			this.btnLocalEulerAngles.Size = new System.Drawing.Size(155, 23);
			this.btnLocalEulerAngles.TabIndex = 17;
			this.btnLocalEulerAngles.Text = "<Transform>.localEulerAngles";
			this.btnLocalEulerAngles.UseVisualStyleBackColor = true;
			this.btnLocalEulerAngles.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// btnShapeRadius
			// 
			this.btnShapeRadius.Enabled = false;
			this.btnShapeRadius.Location = new System.Drawing.Point(479, 37);
			this.btnShapeRadius.Name = "btnShapeRadius";
			this.btnShapeRadius.Size = new System.Drawing.Size(161, 23);
			this.btnShapeRadius.TabIndex = 13;
			this.btnShapeRadius.Text = "<ParticleSystem>.shape.radius";
			this.btnShapeRadius.UseVisualStyleBackColor = true;
			this.btnShapeRadius.Click += new System.EventHandler(this.JumpToButton_Click);
			// 
			// btnAddPropTest
			// 
			this.btnAddPropTest.Location = new System.Drawing.Point(141, 100);
			this.btnAddPropTest.Name = "btnAddPropTest";
			this.btnAddPropTest.Size = new System.Drawing.Size(123, 26);
			this.btnAddPropTest.TabIndex = 2;
			this.btnAddPropTest.Text = "Add Prop";
			this.btnAddPropTest.UseVisualStyleBackColor = true;
			this.btnAddPropTest.Click += new System.EventHandler(this.btnAddPropTest_Click);
			// 
			// FrmEffectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1045, 626);
			this.Controls.Add(this.btnLocalEulerAngles);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnSaveEffect);
			this.Controls.Add(this.btnLocalPosition);
			this.Controls.Add(this.btnLocalScale);
			this.Controls.Add(this.btnShapeRadius);
			this.Controls.Add(this.btnParticleSystemRenderer);
			this.Controls.Add(this.btnMeshRendererMaterial);
			this.Controls.Add(this.txtEffectName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.tbxTravelTime);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnAttackJanus);
			this.Controls.Add(this.btnClearEffect);
			this.Controls.Add(this.btnStartAllParticleSystems);
			this.Controls.Add(this.btnStopAllParticleSystems);
			this.Controls.Add(this.btnTestEffect);
			this.Controls.Add(this.btnFindNext);
			this.Controls.Add(this.btnFindPrevious);
			this.Controls.Add(this.tbxSearch);
			this.Controls.Add(this.trvProperties);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblPropertyName);
			this.Controls.Add(this.pnlProperties);
			this.Controls.Add(this.trvEffectHierarchy);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxJson);
			this.Controls.Add(this.btnCopyJson);
			this.Controls.Add(this.btnExploreMini);
			this.Controls.Add(this.btnAddPropTest);
			this.Controls.Add(this.btnTaleSpireCamera);
			this.Controls.Add(this.btnExploreExisting);
			this.Controls.Add(this.btnCreateEffect);
			this.Controls.Add(this.btnCloneExisting);
			this.Controls.Add(this.btnCreateEmpty);
			this.Controls.Add(this.btnAddPrefab);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(816, 270);
			this.Name = "FrmEffectEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Effect Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEffectEditor_FormClosing);
			this.ctxGameObjects.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnCreateEmpty;
		private System.Windows.Forms.Button btnCopyJson;
		private System.Windows.Forms.TextBox tbxJson;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView trvEffectHierarchy;
		private System.Windows.Forms.Button btnAddPrefab;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel pnlProperties;
		private System.Windows.Forms.Label lblPropertyName;
		private System.Windows.Forms.TreeView trvProperties;
		private System.Windows.Forms.Button btnTestEffect;
		private System.Windows.Forms.TextBox tbxSearch;
		private System.Windows.Forms.Button btnFindPrevious;
		private System.Windows.Forms.Button btnFindNext;
		private System.Windows.Forms.Button btnClearEffect;
		private System.Windows.Forms.Button btnStopAllParticleSystems;
		private System.Windows.Forms.Button btnStartAllParticleSystems;
		private System.Windows.Forms.Button btnAttackJanus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbxTravelTime;
		private System.Windows.Forms.Button btnMeshRendererMaterial;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnParticleSystemRenderer;
		private System.Windows.Forms.Button btnLocalScale;
		private System.Windows.Forms.Button btnLocalPosition;
		private System.Windows.Forms.ContextMenuStrip ctxGameObjects;
		private System.Windows.Forms.ToolStripMenuItem addScriptToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.Button btnCloneExisting;
		private System.Windows.Forms.Button btnExploreMini;
		private System.Windows.Forms.Button btnExploreExisting;
		private System.Windows.Forms.Button btnCreateEffect;
		private System.Windows.Forms.Button btnSaveEffect;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtEffectName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnTaleSpireCamera;
		private System.Windows.Forms.Button btnLocalEulerAngles;
		private System.Windows.Forms.Button btnShapeRadius;
		private System.Windows.Forms.Button btnAddPropTest;
	}
}