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
	public partial class FrmSelectItem : Form
	{
		public SelectionType SelectionType { get; set; }
		System.Timers.Timer filterTimer;
		public FrmSelectItem()
		{
			InitializeComponent();
			filterTimer = new System.Timers.Timer();
			filterTimer.Interval = 0.35;
			filterTimer.Elapsed += FilterTimer_Elapsed;
			lstItems
				.GetType()
				.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
				.SetValue(lstItems, true, null);
		}

		public static string SelectPrefab(Control owner)
		{
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.Prefabs;
			frmSelectItem.lblTitle.Text = "Prefab:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}

		public static string SelectGameObject(Control owner)
		{
			Talespire.GameObjects.InvalidateFound();
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.ExistingGameObjects;
			frmSelectItem.lblTitle.Text = "Game Object:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}

		private void FrmSelectItem_Load(object sender, EventArgs e)
		{
			topLevelOnly = (ModifierKeys & Keys.Shift) != Keys.Shift;
			LoadEntries();
			tbxFilter.Focus();
		}

		private void LoadEntries()
		{
			if (lstItems.Items.Count > 0)
				return;

			try
			{
				lstItems.BeginUpdate();
				if (SelectionType == SelectionType.Prefabs)
					foreach (string item in Talespire.Prefabs.AllNames.Where(y => FilterCatches(y)).OrderBy(x => x).ToList())
						lstItems.Items.Add(item);
				else if (SelectionType == SelectionType.ExistingGameObjects)
				{
					foreach (string item in Talespire.GameObjects.GetAllNames(topLevelOnly).Where(y => FilterCatches(y)).OrderBy(x => x).ToList())
					{
						//if (item.Contains("lb") || item.Contains("pot") || item.Contains("Pot") || item.Contains("Bottle") || item.Contains("bottle"))
						//	Talespire.Log.Warning($"Found a \"{item}\"!");
						lstItems.Items.Add(item);
					}
				}
				else if (SelectionType == SelectionType.Minis)
					foreach (string miniId in Talespire.Minis.GetAllNamesAndIds().Where(y => FilterCatches(y)).OrderBy(x => x).ToList())
						lstItems.Items.Add(miniId);
				else if (SelectionType == SelectionType.EffectName)
					foreach (string effect in KnownEffects.GetAllNames().Where(y => FilterCatches(y)).OrderBy(x => x).ToList())
						lstItems.Items.Add(effect);

			}
			finally
			{
				lstItems.EndUpdate();
			}
		}

		public static CreatureBoardAsset SelectMini(FrmEffectEditor owner)
		{
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.Minis;
			frmSelectItem.lblTitle.Text = "Mini:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
			{
				string selectedCreatureNameAndId = frmSelectItem.lstItems.SelectedItem as string;
				return Talespire.Minis.GetCreatureBoardAsset(selectedCreatureNameAndId);
			}
			return null;
		}
		public static string SelectEffectName(FrmEffectEditor owner)
		{
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.EffectName;
			frmSelectItem.lblTitle.Text = "Effect:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}

		string ActiveFilter;
		bool topLevelOnly;

		void ChangeFilter(string text)
		{
			string lowerFilter = text.ToLower().Trim();
			if (lowerFilter == ActiveFilter)
				return;
			ActiveFilter = lowerFilter;
			string saveSelectedItem = lstItems.SelectedItem as string;
			lstItems.Items.Clear();
			LoadEntries();
			if (!string.IsNullOrWhiteSpace(saveSelectedItem))
			{
				//saveSelectedItem
				foreach (object item in lstItems.Items)
					if (item is string str)
						if (string.Compare(saveSelectedItem, str) == 0 || string.Compare(saveSelectedItem, ActiveFilter) == 0)
						{
							lstItems.SelectedItem = item;
							return;
						}
			}
		}

		private void FilterTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			filterTimer.Stop();
			ChangeFilter(tbxFilter.Text);
		}

		bool FilterCatches(string item)
		{
			if (string.IsNullOrWhiteSpace(ActiveFilter))
				return true;
			return item.ToLower().Contains(ActiveFilter);
		}

		private void tbxFilter_TextChanged(object sender, EventArgs e)
		{
			filterTimer.Stop();
			filterTimer.Start();
		}

		private void lstItems_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Talespire.Log.Warning($"Double-click! - AcceptSelected();");
			AcceptSelected();
		}

		private void AcceptSelected()
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void tbxFilter_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				Talespire.Log.Warning($"Enter key pressed!");
				int indexFilter = lstItems.Items.IndexOf(tbxFilter.Text);
				if (indexFilter >= 0)
				{
					Talespire.Log.Debug($"\"{tbxFilter.Text}\" found!!!");
					lstItems.SelectedIndex = indexFilter;
					AcceptSelected();
				}
				else
					Talespire.Log.Error($"\"{tbxFilter.Text}\" NOT found!!!");
			}
		}
	}
}
