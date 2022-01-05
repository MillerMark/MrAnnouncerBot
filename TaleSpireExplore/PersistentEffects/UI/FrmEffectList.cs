using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleSpireCore;
using Unity.Mathematics;

namespace TaleSpireExplore
{
	public partial class FrmEffectList : Form
	{
		System.Timers.Timer windowDragTimer;
		System.Timers.Timer effectDropTimer;

		public FrmEffectList()
		{
			InitializeComponent();
			windowDragTimer = new System.Timers.Timer();
			windowDragTimer.Interval = 20;
			windowDragTimer.Elapsed += WindowDragTimer_Elapsed;

			effectDropTimer = new System.Timers.Timer();
			effectDropTimer.Interval = 250;
			effectDropTimer.Elapsed += EffectDropTimer_Elapsed;
			MouseManager.OnApplicationFocusChange += MouseManager_OnApplicationFocusChange;
		}

		private void EffectDropTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			effectDropTimer.Stop();
			
			if (selectedEffect == null)
				return;

			float3 pointerPos = RulerHelpers.GetPointerPos();
			Talespire.Log.Debug($"Drop \"{selectedEffect}\" at pointer ({pointerPos.x}, {pointerPos.y}, {pointerPos.z}).");
			Talespire.PersistentEffects.Create(selectedEffect);
			selectedEffect = null;
		}

		private void MouseManager_OnApplicationFocusChange(bool hasFocus)
		{
			if (hasFocus && waitingForApplicationClick)
			{
				waitingForApplicationClick = false;
				showingCategories = true;
				showingFullList = false;
				btnEffects.Text = "Add";
				lblCategory.Text = "";
				ResizeToFitButton();
				effectDropTimer.Start();  // Wait a moment before creating the effect so RulerHelpers.GetPointerPos() reflects the latest mouse click position;
			}
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
			PopulateEntries();
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
			int height = lstEntries.Items.Count * lstEntries.ItemHeight;
			int maxWidthSoFar = 0;
			
			foreach (string effectName in lstEntries.Items)
			{
				Size measureText = TextRenderer.MeasureText(effectName, lstEntries.Font);
				if (measureText.Width > maxWidthSoFar)
					maxWidthSoFar = measureText.Width;
			}

			lstEntries.Height = Math.Min(INT_MaxListHeight, height);
			lstEntries.Width = maxWidthSoFar + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
		}

		public void PopulateEntries()
		{
			RefreshEntries();
			SetHeightAndWidth();
		}

		private void LstEntries_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (showingCategories)
				{
					selectedCategory = lstEntries.SelectedItem as string;
					lblCategory.Text = selectedCategory;
					lblCategory.Visible = true;
					showingCategories = false;
					PopulateEntries();
					ResizeFormToFitList();
				}
				else
				{
					selectedEffect = lstEntries.SelectedItem as string;
					waitingForApplicationClick = true;
				}
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex, nameof(LstEntries_SelectedIndexChanged));
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

		public void ResizeFormToFitList()
		{
			this.Width = lstEntries.Width;
			this.Height = btnEffects.Height + lstEntries.Height;
		}

		public void ResizeToFitButton()
		{
			this.Width = btnEffects.Width;
			this.Height = btnEffects.Height;
		}

		bool showingCategories = true;
		string selectedCategory;
		string selectedEffect;
		bool waitingForApplicationClick;
		void RefreshEntries()
		{
			lstEntries.Items.Clear();

			List<string> allEntries;

			if (showingCategories)
				allEntries = KnownEffects.GetAllCategories();
			else
				allEntries = KnownEffects.GetNamesFromCategory(selectedCategory);

			allEntries.Sort();

			foreach (string knownEffect in allEntries)
				lstEntries.Items.Add(knownEffect);
		}

		private void btnEffects_Click(object sender, EventArgs e)
		{
			if (!showingCategories)
			{
				showingCategories = true;
				lblCategory.Visible = false;
				PopulateEntries();
				ResizeFormToFitList();
			}
			else
			{
				showingFullList = !showingFullList;
				if (showingFullList)
				{
					btnEffects.Text = "<<";
					PopulateEntries();
					ResizeFormToFitList();
				}
				else
				{
					btnEffects.Text = "Add";
					ResizeToFitButton();
				}
			}
		}
	}
}
