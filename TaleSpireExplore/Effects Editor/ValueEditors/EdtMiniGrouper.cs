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
	public partial class EdtMiniGrouper : UserControl, IValueEditor
	{
		string data;
		public EdtMiniGrouper()
		{
			InitializeComponent();
		}

		public IValueChangedListener ValueChangedListener { get; set; }

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
				lbInstructions.Text = "Click a mini to add/remove to/from the group.";
				btnEdit.Text = "Done";
			}
			else
			{
				editing = false;
				lbInstructions.Text = "Click to Edit the group.";
				btnEdit.Text = "Edit";
			}
		}
	}
}
