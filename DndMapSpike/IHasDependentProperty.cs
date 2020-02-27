using System;
using System.Linq;

namespace DndMapSpike
{
	public interface IHasDependentProperty
	{
		string DependentProperty { get; set; }
	}
}

