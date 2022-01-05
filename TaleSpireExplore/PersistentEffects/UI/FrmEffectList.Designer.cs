namespace TaleSpireExplore
{
	partial class FrmEffectList
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
			this.lstEntries = new System.Windows.Forms.ListBox();
			this.btnEffects = new System.Windows.Forms.Button();
			this.lblCategory = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lstEntries
			// 
			this.lstEntries.BackColor = System.Drawing.Color.Black;
			this.lstEntries.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstEntries.Font = new System.Drawing.Font("Georgia", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstEntries.ForeColor = System.Drawing.Color.White;
			this.lstEntries.FormattingEnabled = true;
			this.lstEntries.ItemHeight = 16;
			this.lstEntries.Location = new System.Drawing.Point(1, 23);
			this.lstEntries.Name = "lstEntries";
			this.lstEntries.Size = new System.Drawing.Size(143, 96);
			this.lstEntries.TabIndex = 0;
			this.lstEntries.SelectedIndexChanged += new System.EventHandler(this.LstEntries_SelectedIndexChanged);
			// 
			// btnEffects
			// 
			this.btnEffects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
			this.btnEffects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEffects.Location = new System.Drawing.Point(0, 0);
			this.btnEffects.Margin = new System.Windows.Forms.Padding(0);
			this.btnEffects.Name = "btnEffects";
			this.btnEffects.Size = new System.Drawing.Size(40, 23);
			this.btnEffects.TabIndex = 1;
			this.btnEffects.Text = "Add";
			this.btnEffects.UseVisualStyleBackColor = false;
			this.btnEffects.Click += new System.EventHandler(this.btnEffects_Click);
			// 
			// lblCategory
			// 
			this.lblCategory.AutoSize = true;
			this.lblCategory.Font = new System.Drawing.Font("Georgia", 10F);
			this.lblCategory.Location = new System.Drawing.Point(43, 5);
			this.lblCategory.Name = "lblCategory";
			this.lblCategory.Size = new System.Drawing.Size(0, 17);
			this.lblCategory.TabIndex = 2;
			this.lblCategory.Visible = false;
			// 
			// FrmEffectList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(86, 232);
			this.ControlBox = false;
			this.Controls.Add(this.lblCategory);
			this.Controls.Add(this.btnEffects);
			this.Controls.Add(this.lstEntries);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FrmEffectList";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FrmEffectsList_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lstEntries;
		private System.Windows.Forms.Button btnEffects;
		private System.Windows.Forms.Label lblCategory;
	}
}