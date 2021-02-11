using BotCore;
using DndCore;
using System.Linq;
using System.Threading.Tasks;

namespace Streamloots
{
	public static class StreamlootsBackgroundTask
	{
		public static void Start(MySecureString streamlootsId)
		{
			Task.Run(async () =>
			{
				IStreamlootsService service = new StreamlootsService(streamlootsId);
				await service.Connect();

				while (true)
				{
					await Task.Delay(60000);
				}
			});
		}
	}
}
