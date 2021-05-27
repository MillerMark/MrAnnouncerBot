using System;
using System.Linq;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public interface IValueEditor
	{
		void SetValue(object newValue);
		Type GetValueType();
		void Initialize(IValueChangedListener valueChangedListener);
		BasePropertyChanger GetPropertyChanger();
	}
}
