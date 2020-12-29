using System.Linq;
using System.Threading.Tasks;

namespace DHDM
{
	public interface IStreamlootsService
	{
		Task<bool> Connect();
		Task Disconnect();
	}
}
