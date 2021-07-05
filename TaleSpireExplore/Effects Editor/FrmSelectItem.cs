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
		public FrmSelectItem()
		{
			InitializeComponent();
		}

		public static string SelectPrefab(Control owner)
		{
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.Prefabs;
			frmSelectItem.label1.Text = "Prefab:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}

		public static string SelectGameObject(Control owner)
		{
			Talespire.GameObjects.InvalidateFound();
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.ExistingGameObjects;
			frmSelectItem.label1.Text = "Game Object:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}

		private void FrmSelectItem_Load(object sender, EventArgs e)
		{
			if (lstItems.Items.Count == 0)
				if (SelectionType == SelectionType.Prefabs)
					foreach (string item in Talespire.Prefabs.AllNames.OrderBy(x => x).ToList())
						lstItems.Items.Add(item);
				else if (SelectionType == SelectionType.ExistingGameObjects)
					foreach (string item in Talespire.GameObjects.GetAllNames((ModifierKeys & Keys.Shift) != Keys.Shift).OrderBy(x => x).ToList())
					{
						if (item.Contains("lb") || item.Contains("pot") || item.Contains("Pot") || item.Contains("Bottle") || item.Contains("bottle"))
							Talespire.Log.Warning($"Found a \"{item}\"!");
						lstItems.Items.Add(item);
					}
				else if (SelectionType == SelectionType.Minis)
					foreach (string miniId in Talespire.Minis.GetAllNamesAndIds().OrderBy(x => x).ToList())
						lstItems.Items.Add(miniId);
				else if (SelectionType == SelectionType.EffectName)
					foreach (string effect in KnownEffects.GetAllNames().OrderBy(x => x).ToList())
						lstItems.Items.Add(effect);
		}

		public static CreatureBoardAsset SelectMini(FrmEffectEditor owner)
		{
			FrmSelectItem frmSelectItem = new FrmSelectItem();
			frmSelectItem.SelectionType = SelectionType.Minis;
			frmSelectItem.label1.Text = "Mini:";
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
			frmSelectItem.label1.Text = "Effect:";
			if (frmSelectItem.ShowDialog(owner) == DialogResult.OK)
				return frmSelectItem.lstItems.SelectedItem as string;
			return null;
		}
	}
}
