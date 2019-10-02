using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndCoreExceptionEventArgs : EventArgs
	{

		public DndCoreExceptionEventArgs(Exception ex)
		{
			Ex = ex;
		}

		public Exception Ex { get; private set; }
	}
}

