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

namespace TaleSpireExplore
{
	public partial class EdtMiniGrouper : UserControl, IValueEditor, IScriptEditor
	{
		string data;
		public EdtMiniGrouper()
		{
			InitializeComponent();
			Disposed += OnDispose;
			Talespire.Minis.MiniSelected += PersistentEffectsManager_MiniSelected;
		}

		private void OnDispose(object sender, EventArgs e)
		{
			Talespire.Minis.MiniSelected -= PersistentEffectsManager_MiniSelected;
		}

		void RefreshMemberList()
		{
			lstMembers.Items.Clear();
			
			if (Guard.IsNull(MiniGrouperScript, "Script")) return;
			
			foreach (string member in MiniGrouperScript.Members)
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
			ChangeString result = new ChangeString();
			result.SetValue(data);
			return result;
		}

		public void ValueChanged(object newValue, bool committedChange = true)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
		}

		public Type GetValueType()
		{
			return typeof(MiniGrouper);
		}

		public void SetValue(object newValue)
		{
			Talespire.Log.Indent($"EdtMiniGrouper.SetValue - {newValue}");
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
			}
		}

		public void InitializeInstance(MonoBehaviour script)
		{
			MiniGrouperScript = script as MiniGrouper;
			RefreshMemberList();
			RefreshTrackHue();
			MiniGrouperScript?.RefreshIndicators();
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
			trkHue.Value = (int)Math.Round(hueSatLight.Hue * 100.0);
		}

		private void trkHue_Scroll(object sender, EventArgs e)
		{
			if (MiniGrouperScript == null)
				return;
			HueSatLight hueSatLight = new HueSatLight(trkHue.Value / 100.0, 1, 0.5);
			System.Drawing.Color asRGB = hueSatLight.AsRGB;
			MiniGrouperScript.IndicatorColor = new UnityEngine.Color(asRGB.R / 255.0f, asRGB.G / 255.0f, asRGB.B / 255.0f);
			MiniGrouperScript.RefreshIndicators();
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
	}
}
