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
			this.label3 = new System.Windows.Forms.Label();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnEffects = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtEffectName = new System.Windows.Forms.TextBox();
			this.btnShowEffect = new System.Windows.Forms.Button();
			this.btnShowRelationEffect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnLoadClasses
			// 
			this.btnLoadClasses.Location = new System.Drawing.Point(12, 12);
			this.btnLoadClasses.Name = "btnLoadClasses";
			this.btnLoadClasses.Size = new System.Drawing.Size(86, 30);
			this.btnLoadClasses.TabIndex = 0;
			this.btnLoadClasses.Text = "Load Classes";
			this.btnLoadClasses.UseVisualStyleBackColor = true;
			this.btnLoadClasses.Click += new System.EventHandler(this.btnLoadClasses_Click);
			// 
			// tbxScratch
			// 
			this.tbxScratch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.tbxScratch.Location = new System.Drawing.Point(124, 15);
			this.tbxScratch.Multiline = true;
			this.tbxScratch.Name = "tbxScratch";
			this.tbxScratch.Size = new System.Drawing.Size(423, 422);
			this.tbxScratch.TabIndex = 1;
			this.tbxScratch.WordWrap = false;
			// 
			// tbxLog
			// 
			this.tbxLog.Location = new System.Drawing.Point(563, 31);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.Size = new System.Drawing.Size(423, 406);
			this.tbxLog.TabIndex = 2;
			this.tbxLog.WordWrap = false;
			// 
			// btnSetTime
			// 
			this.btnSetTime.Location = new System.Drawing.Point(12, 86);
			this.btnSetTime.Name = "btnSetTime";
			this.btnSetTime.Size = new System.Drawing.Size(86, 23);
			this.btnSetTime.TabIndex = 3;
			this.btnSetTime.Text = "Set Time";
			this.btnSetTime.UseVisualStyleBackColor = true;
			this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
			// 
			// txtTime
			// 
			this.txtTime.Location = new System.Drawing.Point(50, 60);
			this.txtTime.Name = "txtTime";
			this.txtTime.Size = new System.Drawing.Size(48, 20);
			this.txtTime.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Time:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 149);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Sun:";
			// 
			// txtSunDirection
			// 
			this.txtSunDirection.Location = new System.Drawing.Point(50, 145);
			this.txtSunDirection.Name = "txtSunDirection";
			this.txtSunDirection.Size = new System.Drawing.Size(48, 20);
			this.txtSunDirection.TabIndex = 7;
			// 
			// btnSetSun
			// 
			this.btnSetSun.Location = new System.Drawing.Point(12, 171);
			this.btnSetSun.Name = "btnSetSun";
			this.btnSetSun.Size = new System.Drawing.Size(86, 23);
			this.btnSetSun.TabIndex = 6;
			this.btnSetSun.Text = "Set Sun Pos";
			this.btnSetSun.UseVisualStyleBackColor = true;
			this.btnSetSun.Click += new System.EventHandler(this.btnSetSun_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(560, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Events:";
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
			this.btnEffects.Location = new System.Drawing.Point(14, 235);
			this.btnEffects.Name = "btnEffects";
			this.btnEffects.Size = new System.Drawing.Size(84, 23);
			this.btnEffects.TabIndex = 11;
			this.btnEffects.Text = "List Effects...";
			this.btnEffects.UseVisualStyleBackColor = true;
			this.btnEffects.Click += new System.EventHandler(this.btnEffects_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 285);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 14;
			this.label4.Text = "Effect:";
			// 
			// txtEffectName
			// 
			this.txtEffectName.Location = new System.Drawing.Point(52, 281);
			this.txtEffectName.Name = "txtEffectName";
			this.txtEffectName.Size = new System.Drawing.Size(48, 20);
			this.txtEffectName.TabIndex = 13;
			// 
			// btnShowEffect
			// 
			this.btnShowEffect.Location = new System.Drawing.Point(14, 307);
			this.btnShowEffect.Name = "btnShowEffect";
			this.btnShowEffect.Size = new System.Drawing.Size(86, 23);
			this.btnShowEffect.TabIndex = 12;
			this.btnShowEffect.Text = "Play Effect";
			this.btnShowEffect.UseVisualStyleBackColor = true;
			this.btnShowEffect.Click += new System.EventHandler(this.btnShowEffect_Click);
			// 
			// btnShowRelationEffect
			// 
			this.btnShowRelationEffect.Location = new System.Drawing.Point(14, 336);
			this.btnShowRelationEffect.Name = "btnShowRelationEffect";
			this.btnShowRelationEffect.Size = new System.Drawing.Size(86, 44);
			this.btnShowRelationEffect.TabIndex = 15;
			this.btnShowRelationEffect.Text = "Show Relation Effect";
			this.btnShowRelationEffect.UseVisualStyleBackColor = true;
			this.btnShowRelationEffect.Click += new System.EventHandler(this.btnShowRelationEffect_Click);
			// 
			// FrmExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1004, 450);
			this.Controls.Add(this.btnShowRelationEffect);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtEffectName);
			this.Controls.Add(this.btnShowEffect);
			this.Controls.Add(this.btnEffects);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.label3);
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
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnEffects;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtEffectName;
		private System.Windows.Forms.Button btnShowEffect;
		private System.Windows.Forms.Button btnShowRelationEffect;
	}
}