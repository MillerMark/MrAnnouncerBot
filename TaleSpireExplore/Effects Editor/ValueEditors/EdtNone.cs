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
	public partial class EdtNone : UserControl, IValueEditor
	{
		public EdtNone()
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
			return null;
		}

		public void SetValue(object newValue)
		{
			tbxClassInfo.Text = $"No editor yet for {newValue.GetType().FullName}.";
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			return null;
		}
	}
}
