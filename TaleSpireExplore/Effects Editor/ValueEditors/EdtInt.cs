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
	public partial class EdtInt : UserControl, IValueEditor
	{
		public EdtInt()
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
			return typeof(int);
		}

		bool changingInternally;
		public void SetValue(object newValue)
		{
			changingInternally = true;
			try
			{
				if (newValue is int @int)
					tbxValue.Text = @int.ToString();
			}
			finally
			{
				changingInternally = false;
			}
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeInt result = new ChangeInt();
			if (int.TryParse(tbxValue.Text.Trim(), out int value))
				result.SetValue(value);
			return result;
		}

		private void tbxValue_TextChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			if (int.TryParse(tbxValue.Text.Trim(), out int result))
				ValueChanged(result);
		}

		public void EditingProperty(string name)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
