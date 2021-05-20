using System;
using System.Linq;

namespace TaleSpireExplore
{
	public interface IValueEditor
	{
		void SetValue(object newValue);
		Type GetValueType();
		void Initialize(IValueChangedListener valueChangedListener);
	}
}
