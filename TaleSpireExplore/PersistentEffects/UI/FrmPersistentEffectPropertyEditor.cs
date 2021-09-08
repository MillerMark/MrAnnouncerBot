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
	public partial class FrmPersistentEffectPropertyEditor : Form
	{
		public FrmPersistentEffectPropertyEditor()
		{
			InitializeComponent();
		}

		public void SetLocation(Point taleSpireTopRight)
		{
			const int INT_PropertyEditorMargin = 8;
			Location = new Point(taleSpireTopRight.X - (Width + INT_PropertyEditorMargin), taleSpireTopRight.Y + WindowHelper.TaleSpireTitleBarHeight);
		}

		private void FrmPersistentEffectPropertyEditor_Load(object sender, EventArgs e)
		{
			Talespire.Log.Warning($"SetLocation(WindowHelper.GetTaleSpireTopRight());");
			SetLocation(WindowHelper.GetTaleSpireTopRight());
			Talespire.Log.Warning($"----");

		}
	}
}
