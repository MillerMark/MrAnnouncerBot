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
			this.lstEffects = new System.Windows.Forms.ListBox();
			this.btnEffects = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstEffects
			// 
			this.lstEffects.BackColor = System.Drawing.Color.Black;
			this.lstEffects.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstEffects.Font = new System.Drawing.Font("Georgia", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lstEffects.ForeColor = System.Drawing.Color.White;
			this.lstEffects.FormattingEnabled = true;
			this.lstEffects.ItemHeight = 16;
			this.lstEffects.Location = new System.Drawing.Point(1, 23);
			this.lstEffects.Name = "lstEffects";
			this.lstEffects.Size = new System.Drawing.Size(143, 96);
			this.lstEffects.TabIndex = 0;
			this.lstEffects.SelectedIndexChanged += new System.EventHandler(this.LstEffects_SelectedIndexChanged);
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
			// FrmEffectList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(86, 232);
			this.ControlBox = false;
			this.Controls.Add(this.btnEffects);
			this.Controls.Add(this.lstEffects);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FrmEffectList";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FrmEffectsList_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lstEffects;
		private System.Windows.Forms.Button btnEffects;
	}
}