using System;
using System.Linq;
using System.Windows.Controls;

namespace DndMapSpike
{
	public interface IPropertyEditor
	{
		string PropertyName { get; set; }
		PropertyType PropertyType { get; set; }
	}
}

