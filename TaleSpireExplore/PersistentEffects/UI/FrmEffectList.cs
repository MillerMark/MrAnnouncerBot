using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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
		const int INT_MaxListHeight = 832;
		Point lastLocation;

		private void SetLocation(Point topLeftOfTaleSpire)
		{
			if (lastLocation == topLeftOfTaleSpire)
				return;
			Location = new Point(topLeftOfTaleSpire.X + INT_PropertyEditorMargin, topLeftOfTaleSpire.Y + WindowHelper.TaleSpireTitleBarHeight);
			lastLocation = topLeftOfTaleSpire;
		}
		
		public Point UpperLeft
		{
			get
			{
				Point location = WindowHelper.GetTaleSpireTopLeft();
				location.Offset(14, 156);  // Get this down below the atmosphere button.
				return location;
			}
		}
		

		private void FrmEffectsList_Load(object sender, EventArgs e)
		{
			ShowEffectsList();
			SetLocation(UpperLeft);
			ResizeToFitButton();
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
			SetLocation(UpperLeft);
		}

		void SetHeightAndWidth()
		{
			int height = lstEffects.Items.Count * lstEffects.ItemHeight;
			int maxWidthSoFar = 0;
			
			foreach (string effectName in lstEffects.Items)
			{
				Size measureText = TextRenderer.MeasureText(effectName, lstEffects.Font);
				if (measureText.Width > maxWidthSoFar)
					maxWidthSoFar = measureText.Width;
			}

			lstEffects.Height = Math.Min(INT_MaxListHeight, height);
			lstEffects.Width = maxWidthSoFar + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
		}

		public void ShowEffectsList()
		{
			RefreshEffectsList();
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

		bool showingFullList;

		public void ResizeToFitList()
		{
			this.Width = lstEffects.Width;
			this.Height = btnEffects.Height + lstEffects.Height;
		}

		public void ResizeToFitButton()
		{
			this.Width = btnEffects.Width;
			this.Height = btnEffects.Height;
		}

		void RefreshEffectsList()
		{
			lstEffects.Items.Clear();

			List<string> allKnownEffects = KnownEffects.GetAllNames();
			allKnownEffects.Sort();

			foreach (string knownEffect in allKnownEffects)
				lstEffects.Items.Add(knownEffect);
		}

		private void btnEffects_Click(object sender, EventArgs e)
		{
			showingFullList = !showingFullList;
			if (showingFullList)
			{
				btnEffects.Text = "<<";
				ShowEffectsList();
				ResizeToFitList();
			}
			else
			{
				btnEffects.Text = "Add";
				ResizeToFitButton();
			}
		}
	}
}
