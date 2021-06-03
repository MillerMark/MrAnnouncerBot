namespace TaleSpireExplore
{
	partial class EdtMesh
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
			this.cmbMesh = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// cmbMesh
			// 
			this.cmbMesh.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.cmbMesh.FormattingEnabled = true;
			this.cmbMesh.Location = new System.Drawing.Point(3, 3);
			this.cmbMesh.MaxDropDownItems = 22;
			this.cmbMesh.Name = "cmbMesh";
			this.cmbMesh.Size = new System.Drawing.Size(224, 21);
			this.cmbMesh.Sorted = true;
			this.cmbMesh.TabIndex = 0;
			this.cmbMesh.DropDown += new System.EventHandler(this.cmbMesh_DropDown);
			this.cmbMesh.SelectedIndexChanged += new System.EventHandler(this.cmbMesh_SelectedIndexChanged);
			// 
			// EdtMesh
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cmbMesh);
			this.Name = "EdtMesh";
			this.Size = new System.Drawing.Size(230, 35);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbMesh;
	}
}
