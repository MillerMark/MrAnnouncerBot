using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaleSpireExplore
{
	public partial class FrmNameEffect : Form
	{
		public FrmNameEffect()
		{
			InitializeComponent();
		}

		public static string GetName(Form parent, string caption, string currentName)
		{
			FrmNameEffect frmNameEffect = new FrmNameEffect();
			frmNameEffect.tbxPropertyName.Text = currentName;
			frmNameEffect.Parent = parent;
			frmNameEffect.Text = caption;

			if (frmNameEffect.ShowDialog() == DialogResult.OK)
				return frmNameEffect.tbxPropertyName.Text;

			return currentName;
		}

		private void FrmNameEffect_Load(object sender, EventArgs e)
		{
			tbxPropertyName.SelectAll();
			tbxPropertyName.Focus();
		}
	}
}
