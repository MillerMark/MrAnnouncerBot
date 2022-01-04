using System;
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
	public partial class EdtString : UserControl, IValueEditor
	{
		public EdtString()
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
			return typeof(string);
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeString result = new ChangeString();
			result.SetValue(txtStringValue.Text);
			return result;
		}

		public void SetValue(object newValue)
		{
			if (newValue != null)
				txtStringValue.Text = $"{newValue}";
		}

		private void txtStringValue_TextChanged(object sender, EventArgs e)
		{
			ValueChanged(txtStringValue.Text);
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
