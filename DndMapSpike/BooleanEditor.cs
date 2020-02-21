using System;
using System.Linq;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class BooleanEditor : CheckBox
	{

		public BooleanEditor()
		{

		}
		public string PropertyName { get; set; }
		public PropertyType PropertyType { get; set; }
	}
}

