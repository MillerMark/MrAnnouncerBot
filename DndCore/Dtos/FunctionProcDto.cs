using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class FunctionProcDto
	{
		public string Name { get; set; }
		public string FunctionName { get; set; }
		public List<string> Parameters { get; set; }
		public FunctionProcDto()
		{

		}
		public void ProcessArguments()
		{
			FunctionName = DndUtils.GetName(Name);
			Parameters = DndUtils.GetParameters(Name);
		}
	}
}

