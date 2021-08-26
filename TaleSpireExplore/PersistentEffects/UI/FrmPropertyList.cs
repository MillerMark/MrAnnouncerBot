using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using TaleSpireExplore.Unmanaged;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;
using System.Diagnostics;

namespace TaleSpireExplore
{
	public partial class FrmPropertyList : Form
	{
		static FrmPersistentEffectPropertyEditor frmPersistentEffectPropertyEditor;
		List<EffectProperty> EffectProperties = new List<EffectProperty>();
		public FrmPropertyList()
		{
			InitializeComponent();
			if (frmPersistentEffectPropertyEditor == null)
				frmPersistentEffectPropertyEditor = new FrmPersistentEffectPropertyEditor();
			lstProperties.SelectedIndex = 0;
		}

		public void AddProperty(string name, Type type, string path)
		{
			EffectProperties.Add(new EffectProperty(name, type, path));
		}

		public void ClearProperties()
		{
			EffectProperties.Clear();
		}

		void SetHeightAndWidth()
		{
			const int topBottomMargin = 12;
			const int leftRightMargin = 12;
			int height = lstProperties.Items.Count * lstProperties.ItemHeight + topBottomMargin;
			int maxWidthSoFar = 0;
			foreach (EffectProperty effectProperty in EffectProperties)
			{
				Size measureText = TextRenderer.MeasureText(effectProperty.Name, lstProperties.Font);
				if (measureText.Width > maxWidthSoFar)
					maxWidthSoFar = measureText.Width;
			}
			Height = height;
			lstProperties.Height = height;
			Width = maxWidthSoFar + leftRightMargin;
		}

		public void ShowPropertyList()
		{
			Talespire.Log.Debug($"ShowPropertyList()");
			foreach (EffectProperty effectProperty in EffectProperties)
				lstProperties.Items.Add(effectProperty);
			Talespire.Log.Debug($"SetHeightAndWidth();");
			SetHeightAndWidth();
			Locate();
		}

		const int INT_PropertyEditorWidth = 330;

		public void Locate()
		{
			IntPtr hWnd = GetTaleSpireWindow();

			if (hWnd != IntPtr.Zero)
			{
				Native.GetWindowRect(hWnd, out RECT lpRect);
				SetLocation(lpRect);
			}
		}

		private static IntPtr GetTaleSpireWindow()
		{
			return Native.FindWindow("UnityWndClass", "TaleSpire");
		}

		private void SetLocation(RECT lpRect)
		{
			Location = new Point(lpRect.Right - INT_PropertyEditorWidth - Width, lpRect.Top + 56);
		}

		private void lstProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			EffectProperty effectProperty = lstProperties.SelectedItem as EffectProperty;
			if (effectProperty != null)
				Talespire.Log.Warning($"{effectProperty.Name} selected!");

			frmPersistentEffectPropertyEditor.Controls.Clear();
			effectProperty.Type
			frmPersistentEffectPropertyEditor.Controls.Add()
		}

		private void FrmPropertyList_Load(object sender, EventArgs e)
		{
			ShowPropertyList();
		}
	}
}
