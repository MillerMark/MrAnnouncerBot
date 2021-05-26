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
			this.btnAddPrefab = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.pnlProperties = new System.Windows.Forms.Panel();
			this.btnClearEffect = new System.Windows.Forms.Button();
			this.btnTestEffect = new System.Windows.Forms.Button();
			this.btnApplyChange = new System.Windows.Forms.Button();
			this.lblPropertyName = new System.Windows.Forms.Label();
			this.trvProperties = new System.Windows.Forms.TreeView();
			this.tbxSearch = new System.Windows.Forms.TextBox();
			this.btnFindPrevious = new System.Windows.Forms.Button();
			this.btnFindNext = new System.Windows.Forms.Button();
			this.pnlProperties.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCreateEmpty
			// 
			this.btnCreateEmpty.Location = new System.Drawing.Point(121, 14);
			this.btnCreateEmpty.Name = "btnCreateEmpty";
			this.btnCreateEmpty.Size = new System.Drawing.Size(99, 26);
			this.btnCreateEmpty.TabIndex = 2;
			this.btnCreateEmpty.Text = "Create Empty...";
			this.toolTip1.SetToolTip(this.btnCreateEmpty, "Add a new GameObject to the effect.");
			this.btnCreateEmpty.UseVisualStyleBackColor = true;
			// 
			// btnCopyJson
			// 
			this.btnCopyJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCopyJson.Location = new System.Drawing.Point(823, 12);
			this.btnCopyJson.Name = "btnCopyJson";
			this.btnCopyJson.Size = new System.Drawing.Size(84, 26);
			this.btnCopyJson.TabIndex = 2;
			this.btnCopyJson.Text = "Copy JSON";
			this.btnCopyJson.UseVisualStyleBackColor = true;
			// 
			// tbxJson
			// 
			this.tbxJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxJson.Location = new System.Drawing.Point(686, 46);
			this.tbxJson.Multiline = true;
			this.tbxJson.Name = "tbxJson";
			this.tbxJson.ReadOnly = true;
			this.tbxJson.Size = new System.Drawing.Size(221, 413);
			this.tbxJson.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(683, 27);
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
			this.trvEffectHierarchy.HideSelection = false;
			this.trvEffectHierarchy.Location = new System.Drawing.Point(12, 46);
			this.trvEffectHierarchy.Name = "trvEffectHierarchy";
			this.trvEffectHierarchy.Size = new System.Drawing.Size(208, 413);
			this.trvEffectHierarchy.TabIndex = 5;
			this.trvEffectHierarchy.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvEffectHierarchy_AfterCheck);
			this.trvEffectHierarchy.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvEffectHierarchy_NodeMouseClick);
			// 
			// btnAddPrefab
			// 
			this.btnAddPrefab.Location = new System.Drawing.Point(12, 14);
			this.btnAddPrefab.Name = "btnAddPrefab";
			this.btnAddPrefab.Size = new System.Drawing.Size(92, 26);
			this.btnAddPrefab.TabIndex = 0;
			this.btnAddPrefab.Text = "Add Prefab...";
			this.toolTip1.SetToolTip(this.btnAddPrefab, "Add a new prefab child node to the effect.");
			this.btnAddPrefab.UseVisualStyleBackColor = true;
			this.btnAddPrefab.Click += new System.EventHandler(this.btnAddPrefab_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(228, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Properties:";
			// 
			// pnlProperties
			// 
			this.pnlProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pnlProperties.Controls.Add(this.btnClearEffect);
			this.pnlProperties.Controls.Add(this.btnTestEffect);
			this.pnlProperties.Controls.Add(this.btnApplyChange);
			this.pnlProperties.Location = new System.Drawing.Point(444, 46);
			this.pnlProperties.Name = "pnlProperties";
			this.pnlProperties.Size = new System.Drawing.Size(232, 413);
			this.pnlProperties.TabIndex = 7;
			// 
			// btnClearEffect
			// 
			this.btnClearEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearEffect.Location = new System.Drawing.Point(3, 355);
			this.btnClearEffect.Name = "btnClearEffect";
			this.btnClearEffect.Size = new System.Drawing.Size(99, 26);
			this.btnClearEffect.TabIndex = 3;
			this.btnClearEffect.Text = "Clear Effect";
			this.btnClearEffect.UseVisualStyleBackColor = true;
			this.btnClearEffect.Click += new System.EventHandler(this.btnClearEffect_Click);
			// 
			// btnTestEffect
			// 
			this.btnTestEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTestEffect.Location = new System.Drawing.Point(3, 387);
			this.btnTestEffect.Name = "btnTestEffect";
			this.btnTestEffect.Size = new System.Drawing.Size(99, 26);
			this.btnTestEffect.TabIndex = 3;
			this.btnTestEffect.Text = "Test Effect";
			this.btnTestEffect.UseVisualStyleBackColor = true;
			this.btnTestEffect.Click += new System.EventHandler(this.btnTestEffect_Click);
			// 
			// btnApplyChange
			// 
			this.btnApplyChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApplyChange.Location = new System.Drawing.Point(133, 387);
			this.btnApplyChange.Name = "btnApplyChange";
			this.btnApplyChange.Size = new System.Drawing.Size(99, 26);
			this.btnApplyChange.TabIndex = 2;
			this.btnApplyChange.Text = "Apply Change";
			this.btnApplyChange.UseVisualStyleBackColor = true;
			this.btnApplyChange.Click += new System.EventHandler(this.btnApplyChange_Click);
			// 
			// lblPropertyName
			// 
			this.lblPropertyName.AutoSize = true;
			this.lblPropertyName.Location = new System.Drawing.Point(441, 27);
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
			this.trvProperties.Location = new System.Drawing.Point(231, 71);
			this.trvProperties.Name = "trvProperties";
			this.trvProperties.Size = new System.Drawing.Size(203, 388);
			this.trvProperties.TabIndex = 9;
			this.trvProperties.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvProperties_BeforeExpand);
			this.trvProperties.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.trvProperties_DrawNode);
			this.trvProperties.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvProperties_AfterSelect);
			// 
			// tbxSearch
			// 
			this.tbxSearch.Location = new System.Drawing.Point(231, 46);
			this.tbxSearch.Name = "tbxSearch";
			this.tbxSearch.Size = new System.Drawing.Size(131, 20);
			this.tbxSearch.TabIndex = 10;
			this.tbxSearch.TextChanged += new System.EventHandler(this.tbxSearch_TextChanged);
			// 
			// btnFindPrevious
			// 
			this.btnFindPrevious.Location = new System.Drawing.Point(368, 45);
			this.btnFindPrevious.Name = "btnFindPrevious";
			this.btnFindPrevious.Size = new System.Drawing.Size(30, 23);
			this.btnFindPrevious.TabIndex = 3;
			this.btnFindPrevious.Text = "<<";
			this.btnFindPrevious.UseVisualStyleBackColor = true;
			this.btnFindPrevious.Click += new System.EventHandler(this.btnFindPrevious_Click);
			// 
			// btnFindNext
			// 
			this.btnFindNext.Location = new System.Drawing.Point(404, 45);
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Size = new System.Drawing.Size(30, 23);
			this.btnFindNext.TabIndex = 3;
			this.btnFindNext.Text = ">>";
			this.btnFindNext.UseVisualStyleBackColor = true;
			this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
			// 
			// FrmEffectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(915, 471);
			this.Controls.Add(this.btnFindNext);
			this.Controls.Add(this.btnFindPrevious);
			this.Controls.Add(this.tbxSearch);
			this.Controls.Add(this.trvProperties);
			this.Controls.Add(this.lblPropertyName);
			this.Controls.Add(this.pnlProperties);
			this.Controls.Add(this.trvEffectHierarchy);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxJson);
			this.Controls.Add(this.btnCopyJson);
			this.Controls.Add(this.btnCreateEmpty);
			this.Controls.Add(this.btnAddPrefab);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(816, 270);
			this.Name = "FrmEffectEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Effect Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEffectEditor_FormClosing);
			this.pnlProperties.ResumeLayout(false);
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
		private System.Windows.Forms.Button btnApplyChange;
		private System.Windows.Forms.TreeView trvProperties;
		private System.Windows.Forms.Button btnTestEffect;
		private System.Windows.Forms.TextBox tbxSearch;
		private System.Windows.Forms.Button btnFindPrevious;
		private System.Windows.Forms.Button btnFindNext;
		private System.Windows.Forms.Button btnClearEffect;
	}
}