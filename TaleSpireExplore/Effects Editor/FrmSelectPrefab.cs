using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public partial class FrmSelectPrefab : Form
	{
		public FrmSelectPrefab()
		{
			InitializeComponent();
		}

		public static string SelectPrefab(Control owner)
		{
			FrmSelectPrefab frmSelectPrefab = new FrmSelectPrefab();
			if (frmSelectPrefab.ShowDialog(owner) == DialogResult.OK)
				return frmSelectPrefab.lstPrefabs.SelectedItem as string;
			return null;
		}

		private void FrmSelectPrefab_Load(object sender, EventArgs e)
		{
			if (lstPrefabs.Items.Count == 0)
				foreach (string item in Talespire.Prefabs.AllNames.OrderBy(x => x).ToList())
					lstPrefabs.Items.Add(item);
			
		}
	}
}
