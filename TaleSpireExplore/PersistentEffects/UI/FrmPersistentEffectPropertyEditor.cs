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

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				// turn on WS_EX_TOOLWINDOW style bit
				cp.ExStyle |= 0x80;
				return cp;
			}
		}

		public void SetLocation(Point taleSpireTopRight)
		{
			const int INT_PropertyEditorMargin = 8;
			Location = new Point(taleSpireTopRight.X - (Width + INT_PropertyEditorMargin), taleSpireTopRight.Y + WindowHelper.TaleSpireTitleBarHeight);
		}

		private void FrmPersistentEffectPropertyEditor_Load(object sender, EventArgs e)
		{
			SetLocation(WindowHelper.GetTaleSpireTopRight());
		}
	}
}
