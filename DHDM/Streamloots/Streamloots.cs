using System.Linq;
using System.Threading.Tasks;

namespace DHDM
{
	public static class Streamloots
	{
		public static void Start(string streamlootsId)
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
