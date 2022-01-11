using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UnityEngine.ParticleSystem;
using UnityEngine;
using TaleSpireCore;
using Newtonsoft.Json;

namespace TaleSpireExplore
{
	public partial class EdtMiniGrouper : UserControl, IValueEditor, IScriptEditor
	{
		System.Timers.Timer updateMemberListTimer;
		public EdtMiniGrouper()
		{
			InitializeComponent();
			Disposed += OnDispose;
			Talespire.Minis.MiniSelected += PersistentEffectsManager_MiniSelected;
			updateMemberListTimer = new System.Timers.Timer();
			updateMemberListTimer.Interval = 500;
			updateMemberListTimer.Elapsed += UpdateMemberListTimer_Elapsed;
		}

		private void OnDispose(object sender, EventArgs e)
		{
			Talespire.Minis.MiniSelected -= PersistentEffectsManager_MiniSelected;
			if (MiniGrouperScript != null)
			{
				Talespire.Log.Warning($"OnDispose / MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;");
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
			}
		}

		void RefreshMemberList()
		{
			lstMembers.Items.Clear();

			if (Guard.IsNull(MiniGrouperScript, "Script")) return;

			List<string> notFoundCreatures = new List<string>();
			foreach (string member in MiniGrouperScript.Data.Members)
			{
				CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(member);
				if (creatureBoardAsset != null)
					lstMembers.Items.Add(new GroupMember(creatureBoardAsset));
				else
					notFoundCreatures.Add(member);
			}

			MiniGrouperScript.RemoveMembers(notFoundCreatures);
		}

		private void PersistentEffectsManager_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (!editing)
				return;

			if (Guard.IsNull(MiniGrouperScript, "Script")) return;

			if (ea.Mini.Creature.CreatureId.ToString() == MiniGrouperScript.OwnerID)
			{
				ea.Mini.Creature.Speak("Click \"Done\" to finish group editing.");
				//Talespire.Log.Error($"Cannot add grouper to itself!!!!");
				return;
			}

			MiniGrouperScript.ToggleMember(ea.Mini);
			RefreshMemberList();
		}

		public IValueChangedListener ValueChangedListener { get; set; }
		public MiniGrouper MiniGrouperScript { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			Talespire.Log.Debug($"EdtMiniGrouper.GetPropertyChanger() -- lastSerializedData = \"{LastSerializedData}\"");
			ChangeString result = new ChangeString();
			result.SetValue(LastSerializedData);
			return result;
		}

		public void ValueChanged(object newValue, bool committedChange = true)
		{
			Talespire.Log.Indent();
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
			Talespire.Log.Unindent();
		}

		public Type GetValueType()
		{
			return typeof(MiniGrouper);
		}

		public void SetValue(object newValue)
		{
			Talespire.Log.Indent($"EdtMiniGrouper.SetValue - \"{newValue}\"");
			try
			{

			}
			finally
			{
				Talespire.Log.Unindent();
			}
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}

		bool editing;
		public string LastSerializedData { get; set; }
		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (editing)
				StopEditMode();
			else
				StartEditMode();
		}

		private void StopEditMode()
		{
			PersistentEffectsManager.SuppressPersistentEffectUI = false;
			editing = false;
			lbInstructions.Text = "<< Click to Edit the group.";
			btnEdit.Text = "Edit";
			if (MiniGrouperScript != null)
				MiniGrouperScript.UpdateMemberList();
			btnEdit.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
			btnEdit.ForeColor = System.Drawing.Color.White;
			EnableControlsBasedOnMemberCount();
		}

		private void StartEditMode()
		{
			editing = true;
			PersistentEffectsManager.SuppressPersistentEffectUI = true;  // So we no longer show or hide the PE UI when selecting minis.
			lbInstructions.Text = "Click a mini to add or remove it from the group.";
			btnEdit.Text = "Done";
			btnEdit.BackColor = System.Drawing.Color.DarkRed;
			btnEdit.ForeColor = System.Drawing.Color.White;
		}

		void EnableControlsBasedOnMemberCount()
		{
			EnableMemberControls(lstMembers.Items.Count > 0);
			if (formationEditingMode == FormationEditingMode.Columns)
				SetColumnRange();
		}

		void EnableMemberControls(bool enabled)
		{
			txtNewName.Enabled = enabled;
			lblNewCreatureName.Enabled = enabled;
			tbxSetHp.Enabled = enabled;
			lblOutOf.Enabled = enabled;
			tbxMaxHp.Enabled = enabled;
			tbxDamage.Enabled = enabled;
			lbHP1.Enabled = enabled;
			lbHP2.Enabled = enabled;
			tbxHealth.Enabled = enabled;

			EnableButtons(enabled, btnMatchAltitude, btnRenameAll, btnHideAll, btnShowAll, btnLookAt, btnSetHp, btnKnockdown, btnDamage, btnHeal);
		}

		void EnableButtons(bool enabled, params Button[] buttons)
		{
			foreach (Button button in buttons)
			{
				if (enabled)
				{

				}
				else
				{

				}
			}
		}

		public void InitializeInstance(MonoBehaviour script)
		{
			MiniGrouperScript = script as MiniGrouper;
			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
				Talespire.Log.Warning($"InitializeInstance - MiniGrouperScript.StateChanged += MiniGrouperScript_StateChanged;");
				MiniGrouperScript.StateChanged += MiniGrouperScript_StateChanged;
			}
			else
				Talespire.Log.Error($"InitializeInstance - MiniGrouperScript is null!");

			RefreshMemberList();
			RefreshTrackHue();
			chkKnockdownZeroHpCreatures.Checked = MiniGrouperScript.Data.AutoKnockdown;
			MiniGrouperScript?.RefreshIndicators();
		}

		private void MiniGrouperScript_StateChanged(object sender, object e)
		{
			Talespire.Log.Indent();
			if (MiniGrouperScript != null)
			{
				Talespire.Log.Warning($"MiniGrouperScript.Data.RingHue = {MiniGrouperScript.Data.RingHue}");
				if (sender is MiniGrouper miniGrouper)
				{
					if (miniGrouper.Data?.RingHue != MiniGrouperScript.Data.RingHue)
						Talespire.Log.Error($"miniGrouper.Data?.RingHue != MiniGrouperScript.Data.RingHue");
				}
				LastSerializedData = JsonConvert.SerializeObject(MiniGrouperScript.Data);
				Talespire.Log.Warning($"MiniGrouperScript.Data = {LastSerializedData}");

				ValueChanged("");
			}
			else
				Talespire.Log.Error($"MiniGrouperScript is null!!!");
			Talespire.Log.Unindent();
		}

		private void RefreshTrackHue()
		{
			if (MiniGrouperScript == null)
			{
				trkHue.Value = 0;
				return;
			}

			byte red = (byte)Math.Round(MiniGrouperScript.IndicatorColor.r * 255.0);
			byte green = (byte)Math.Round(MiniGrouperScript.IndicatorColor.g * 255.0);
			byte blue = (byte)Math.Round(MiniGrouperScript.IndicatorColor.b * 255.0);
			System.Drawing.Color color = System.Drawing.Color.FromArgb(red, green, blue);
			HueSatLight hueSatLight = new HueSatLight(color);
			trkHue.Value = (int)Math.Round(hueSatLight.Hue * 360.0);
		}

		private void trkHue_Scroll(object sender, EventArgs e)
		{
			if (MiniGrouperScript == null)
				return;
			MiniGrouperScript.UpdateRingHue(trkHue.Value);
		}

		private void btnMatchAltitude_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.MatchAltitude();
		}

		private void btnHideAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.HideAll();
		}

		private void btnShowAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.ShowAll();
		}

		private void trkHue_MouseUp(object sender, MouseEventArgs e)
		{
			MiniGrouperScript.CommitRingHue(trkHue.Value);
		}

		private void lstMembers_SelectedIndexChanged(object sender, EventArgs e)
		{
			GroupMember groupMember = lstMembers.SelectedItem as GroupMember;
			if (groupMember != null)
				lblSelectedCreatureName.Text = $"Create more like {groupMember.Name}...";
			else
				lblSelectedCreatureName.Text = $"Create more creatures.";
		}

		private void chkKnockdownZeroHpCreatures_CheckedChanged(object sender, EventArgs e)
		{
			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.Data.AutoKnockdown = chkKnockdownZeroHpCreatures.Checked;
				MiniGrouperScript.DataChanged();
			}
		}

		private void btnHeal_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.Heal(GetInt(tbxHealth.Text));
		}

		private void btnDamage_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.Damage(GetInt(tbxDamage.Text));
		}

		FormationEditingMode formationEditingMode = FormationEditingMode.Columns;
		int lastRadius = 3;
		int lastColumnCount = 1;

		private void EditingCircular()
		{
			trkColumnsRadius.Minimum = 3;
			trkColumnsRadius.Maximum = 150;
			formationEditingMode = FormationEditingMode.Radius;
			SetVisibilityColumnsRadiusTracker(true);
			lblColumnsRadius.Text = "Radius:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the radius in circular formations.");
			UpdateColumnRadiusLabel();
		}

		private void EditingColumns()
		{
			formationEditingMode = FormationEditingMode.Columns;
			SetColumnRange();
			SetVisibilityColumnsRadiusTracker(true);
			lblColumnsRadius.Text = "Columns:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the number of columns in rectangular formations.");
			UpdateColumnRadiusLabel();
		}

		private void SetColumnRange()
		{
			trkColumnsRadius.Minimum = 1;
			trkColumnsRadius.Maximum = lstMembers.Items.Count;
		}

		private void rbRectangular_CheckedChanged(object sender, EventArgs e)
		{
			EditingColumns();
		}

		private void rbCircular_CheckedChanged(object sender, EventArgs e)
		{
			EditingCircular();
		}

		private void rbSemiCircle_CheckedChanged(object sender, EventArgs e)
		{
			EditingCircular();
		}

		private void rbTriangle_CheckedChanged(object sender, EventArgs e)
		{
			formationEditingMode = FormationEditingMode.None;
			SetVisibilityColumnsRadiusTracker(false);
		}

		private void rbGaggle_CheckedChanged(object sender, EventArgs e)
		{
			EditingColumns();
		}

		private void SetVisibilityColumnsRadiusTracker(bool isVisible)
		{
			lblColumnsRadius.Visible = isVisible;
			lblColumnRadiusValue.Visible = isVisible;
			trkColumnsRadius.Visible = isVisible;

		}

		private void trkColumnsRadius_Scroll(object sender, EventArgs e)
		{
			if (formationEditingMode == FormationEditingMode.Radius)
				lastRadius = trkColumnsRadius.Value;
			else
				lastColumnCount = trkColumnsRadius.Value;
			UpdateColumnRadiusLabel();
		}

		private void UpdateColumnRadiusLabel()
		{
			if (formationEditingMode == FormationEditingMode.Radius)
				lblColumnRadiusValue.Text = $"{lastRadius}ft";
			else
				lblColumnRadiusValue.Text = $"{lastColumnCount}";
		}

		private void trkSpacing_Scroll(object sender, EventArgs e)
		{
			lblSpacingValue.Text = $"{trkSpacing.Value}ft";
		}

		private void trkFormationRotation_Scroll(object sender, EventArgs e)
		{
			lblRotationValue.Text = $"{trkFormationRotation.Value}°";
			// TODO: Rotate the formation around the mini.
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			GroupMember groupMember = lstMembers.SelectedItem as GroupMember;
			// TODO: Create more members.
		}

		private void tbxGroupName_TextChanged(object sender, EventArgs e)
		{

		}

		private void btnLookAt_Click(object sender, EventArgs e)
		{

		}

		private void btnKnockdown_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.KnockdownToggle();
		}

		int GetInt(string text)
		{
			if (int.TryParse(text, out int result))
				return result;

			return 0;
		}
		private void btnSetHp_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.SetHp(GetInt(tbxSetHp.Text), GetInt(tbxMaxHp.Text));
		}

		private void txtNewName_TextChanged(object sender, EventArgs e)
		{
			btnRenameAll.Enabled = !string.IsNullOrWhiteSpace(txtNewName.Text);
		}

		private void btn_MouseEnter(object sender, EventArgs e)
		{
			if (sender is Button button)
				button.BackColor = System.Drawing.Color.FromArgb(12, 12, 12);
		}

		private void btn_MouseLeave(object sender, EventArgs e)
		{
			if (sender is Button button)
				button.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
		}

		private void btnEdit_MouseEnter(object sender, EventArgs e)
		{
			if (editing)
				return;

			btn_MouseEnter(sender, e);
		}

		private void btnEdit_MouseLeave(object sender, EventArgs e)
		{
			if (editing)
				return;

			btn_MouseLeave(sender, e);
		}

		private void UpdateMemberListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateMemberListTimer.Stop();
			RefreshMemberList();
		}

		void RefreshMemberListSoon()
		{
			updateMemberListTimer.Start();
		}

		private void btnRenameAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.RenameAll(txtNewName.Text);
			RefreshMemberListSoon();
		}
	}
}
