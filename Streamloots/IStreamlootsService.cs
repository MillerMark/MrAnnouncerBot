using System.Linq;
using System.Threading.Tasks;

namespace Streamloots
{
	public interface IStreamlootsService
	{
		Task<bool> Connect();
		Task Disconnect();
	}
}
