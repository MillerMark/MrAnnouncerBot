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
	public partial class EdtFloat : UserControl, IValueEditor
	{
		public EdtFloat()
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
			return typeof(float);
		}

		public void SetValue(object newValue)
		{
			if (newValue is float @float)
				tbxValue.Text = @float.ToString();
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			ChangeFloat result = new ChangeFloat();
			if (float.TryParse(tbxValue.Text.Trim(), out float value))
				result.SetValue(value);
			return result;
		}

		private void tbxValue_TextChanged(object sender, EventArgs e)
		{
			if (float.TryParse(tbxValue.Text.Trim(), out float result))
				ValueChanged(result);
		}

		public void EditingProperty(string name)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}
	}
}
