using System;
using System.Linq;
using System.Windows.Forms;

namespace TaleSpireExplore
{
	public interface IValueChangedListener
	{
		void ValueHasChanged(IValueEditor editor, object value);
	}
}
