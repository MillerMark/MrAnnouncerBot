//#define profiling
using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	// TODO: Bring all of MainWindow's die rolling code over here.
	public static class DieRollManager
	{
		public static void Initialize()
		{
			RollSavingThrowsForAllTargetsFunction.SavingThrowForTargetsRequested += RollSavingThrowsForAllTargetsFunction_SavingThrowForTargetsRequested;
		}

		private static void RollSavingThrowsForAllTargetsFunction_SavingThrowForTargetsRequested(object sender, SavingThrowRollEventArgs ea)
		{
			
		}
	}
}


