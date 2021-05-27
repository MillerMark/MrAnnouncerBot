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
				ValueChangedListener.ValueHasChanged(this, newValue);
		}

		public Type GetValueType()
		{
			return typeof(int);
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			return null;
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

		private void tbxValue_TextChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			if (int.TryParse(tbxValue.Text.Trim(), out int result))
				ValueChanged(result);
		}
	}
}
