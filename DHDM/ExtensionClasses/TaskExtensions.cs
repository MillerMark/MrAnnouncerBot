//#define profiling
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DHDM
{
	public static class TaskExtensions
	{
		
		public static async void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null)
		{
			try
			{
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception ex) when (onException != null)
			{
				onException(ex);
			}
		}
	}
}