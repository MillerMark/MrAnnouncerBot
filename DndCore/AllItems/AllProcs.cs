using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllProcs
	{
		static AllProcs()
		{

		}

		static void LoadData()
		{
			Procs = CsvToSheetsHelper.Get<ProcDto>(Folders.InCoreData("DnD - Procs.csv"));
			foreach (ProcDto procDto in Procs)
			{
				procDto.ProcessArguments();
			}
		}
		static List<ProcDto> procs;
		public static List<ProcDto> Procs
		{
			get
			{
				if (procs == null)
					LoadData();
				return procs;
			}
			private set
			{
				procs = value;
			}
		}

		public static ProcDto Get(string procName)
		{
			return Procs.FirstOrDefault(x => x.FunctionName == procName);
		}

		public static void Invalidate()
		{
			procs = null;
		}

	}
}
