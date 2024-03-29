﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public partial class EdtBool : UserControl, IValueEditor
	{
		public EdtBool()
		{
			InitializeComponent();
		}

		public IValueChangedListener ValueChangedListener { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public void ValueChanged(object newValue)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, true);
		}

		public Type GetValueType()
		{
			return typeof(bool);
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeBool result = new ChangeBool();
			result.SetValue(rbTrue.Checked);
			return result;
		}

		public void SetValue(object newValue)
		{
			if (newValue is bool @bool)
				if (@bool)
					rbTrue.Checked = true;
				else
					rbFalse.Checked = true;
		}

		private void Bool_CheckedChanged(object sender, EventArgs e)
		{
			ValueChanged(rbTrue.Checked);
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
