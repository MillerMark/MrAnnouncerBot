using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public partial class FrmEffectList : Form
	{
		System.Timers.Timer windowDragTimer;

		public FrmEffectList()
		{
			InitializeComponent();
			windowDragTimer = new System.Timers.Timer();
			windowDragTimer.Interval = 20;
			windowDragTimer.Elapsed += WindowDragTimer_Elapsed;
		}

		const int INT_PropertyEditorMargin = 8;
		Point lastLocation;

		private void SetLocation(Point topLeftOfTaleSpire)
		{
			if (lastLocation == topLeftOfTaleSpire)
				return;
			Location = new Point(topLeftOfTaleSpire.X + INT_PropertyEditorMargin, topLeftOfTaleSpire.Y + WindowHelper.TaleSpireTitleBarHeight);
			lastLocation = topLeftOfTaleSpire;
		}

		private void FrmPropertyList_Load(object sender, EventArgs e)
		{
			ShowEffectsList();

			SetLocation(WindowHelper.GetTaleSpireTopLeft());
			windowDragTimer.Start();
			WindowHelper.FocusTaleSpire();
		}

		public void PrepForClose()
		{
			windowDragTimer.Stop();
			windowDragTimer.Elapsed -= WindowDragTimer_Elapsed;
			windowDragTimer = null;
		}

		private void WindowDragTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			SetLocation(WindowHelper.GetTaleSpireTopLeft());
		}

		void SetHeightAndWidth()
		{
			const int topBottomMargin = 12;
			const int leftRightMargin = 12;
			int height = lstEffects.Items.Count * lstEffects.ItemHeight + topBottomMargin;
			int maxWidthSoFar = 0;
			// TODO: Search known effects and measure text to determine proper width...
			//foreach (EffectProperty effectProperty in EffectProperties)
			//{
			//	Size measureText = TextRenderer.MeasureText(effectProperty.Name, lstEffects.Font);
			//	if (measureText.Width > maxWidthSoFar)
			//		maxWidthSoFar = measureText.Width;
			//}
			Height = height;
			lstEffects.Height = height;
			Width = maxWidthSoFar + leftRightMargin;
		}

		public void ShowEffectsList()
		{
			// TODO: Add effects....
			//lstEffects.Items.Add(effect);
			//lstEffects.SelectedIndex = 0;

			SetHeightAndWidth();
		}

		private void LstEffects_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// TODO: Do something based on selection index changing...
				//EffectProperty effectProperty = lstEffects.SelectedItem as EffectProperty;
				//if (effectProperty != null)
				//{
				//	Talespire.Log.Warning($"{effectProperty.Name} selected!");
				//	WindowHelper.FocusTaleSpire();
				//}
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex, nameof(LstEffects_SelectedIndexChanged));
			}
		}
	}
}
