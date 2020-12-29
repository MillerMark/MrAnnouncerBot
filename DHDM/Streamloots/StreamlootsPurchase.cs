using System.Linq;

namespace DHDM
{
	public class StreamlootsPurchase
	{
		public string type { get; set; }
		public string imageUrl { get; set; }
		public string soundUrl { get; set; }
		public StreamlootsPurchaseData data { get; set; }
		public string Purchaser => data.Username;
		public string Recipient => data.Recipient;
		public int Quantity => data.Quantity;
	}
}
