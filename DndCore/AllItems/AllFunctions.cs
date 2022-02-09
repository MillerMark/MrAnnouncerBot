﻿using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllFunctions
	{
		static AllFunctions()
		{
			
		}

		static void LoadData()
		{
			Functions = CsvToSheetsHelper.Get<FunctionDto>(Folders.InCoreData("DnD - Functions.csv"));
			foreach (FunctionDto functionDto in Functions)
			{
				functionDto.ProcessArguments();
			}
		}
		static List<FunctionDto> functions;
		public static List<FunctionDto> Functions
		{
			get
			{
				if (functions == null)
					LoadData();
				return functions;
			}
			private set
			{
				functions = value;
			}
		}

		public static FunctionDto Get(string functionName)
		{
			return Functions.FirstOrDefault(x => x.FunctionName == functionName);
		}

		public static void Invalidate()
		{
			functions = null;
		}
	}
}
