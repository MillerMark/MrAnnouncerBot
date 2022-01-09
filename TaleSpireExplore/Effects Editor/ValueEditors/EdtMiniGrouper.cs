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
		public EdtMiniGrouper()
		{
			InitializeComponent();
			Disposed += OnDispose;
			Talespire.Minis.MiniSelected += PersistentEffectsManager_MiniSelected;
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

			foreach (string member in MiniGrouperScript.Data.Members)
			{
				CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(member);
				if (creatureBoardAsset != null)
					lstMembers.Items.Add(creatureBoardAsset.GetOnlyCreatureName());
			}
		}

		private void PersistentEffectsManager_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (!editing)
				return;

			if (Guard.IsNull(MiniGrouperScript, "Script")) return;

			if (ea.Mini.Creature.CreatureId.ToString() == MiniGrouperScript.OwnerID)
			{
				Talespire.Log.Error($"Cannot add grouper to itself!!!!");
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
			if (!editing)
			{
				editing = true;
				PersistentEffectsManager.SuppressPersistentEffectUI = true;  // So we no longer show or hide the PE UI when selecting minis.
				lbInstructions.Text = "Click a mini to add or remove it from the group.";
				btnEdit.Text = "Done";
			}
			else
			{
				PersistentEffectsManager.SuppressPersistentEffectUI = false;
				editing = false;
				lbInstructions.Text = "<< Click to Edit the group.";
				btnEdit.Text = "Edit";
				if (MiniGrouperScript != null)
					MiniGrouperScript.UpdateMemberList();
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

				// TODO: Figure out what we're going to store in the "$Group" property, if anything.
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
	}
}
