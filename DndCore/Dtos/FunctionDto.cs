using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class FunctionDto : PropertyDto
	{
		public string FunctionName { get; set; }
		public List<string> Parameters { get; set; }
		public FunctionDto()
		{

		}
		public void ProcessArguments()
		{
			FunctionName = DndUtils.GetName(Name);
			Parameters = DndUtils.GetParameters(Name);
		}
	}
}

