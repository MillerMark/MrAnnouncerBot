namespace TaleSpireExplore
{
	partial class EdtMaterial
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
			this.cmbMaterial = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// cmbMaterial
			// 
			this.cmbMaterial.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.cmbMaterial.FormattingEnabled = true;
			this.cmbMaterial.Location = new System.Drawing.Point(3, 3);
			this.cmbMaterial.MaxDropDownItems = 22;
			this.cmbMaterial.Name = "cmbMaterial";
			this.cmbMaterial.Size = new System.Drawing.Size(315, 21);
			this.cmbMaterial.Sorted = true;
			this.cmbMaterial.TabIndex = 0;
			this.cmbMaterial.DropDown += new System.EventHandler(this.cmbMaterial_DropDown);
			this.cmbMaterial.SelectedIndexChanged += new System.EventHandler(this.cmbMaterial_SelectedIndexChanged);
			// 
			// EdtMaterial
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cmbMaterial);
			this.Name = "EdtMaterial";
			this.Size = new System.Drawing.Size(330, 35);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbMaterial;
	}
}
