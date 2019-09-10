using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllFunctions
	{
		static AllFunctions()
		{
			Functions = CsvData.Get<FunctionDto>(Folders.InCoreData("DnD - Functions.csv"), false);
			foreach (FunctionDto functionDto in Functions)
			{
				functionDto.ProcessArguments();
			}
		}

		public static List<FunctionDto> Functions { get; private set; }

		public static FunctionDto Get(string functionName)
		{
			return Functions.FirstOrDefault(x => x.FunctionName == functionName);
		}

	}
}
