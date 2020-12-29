using System.Linq;

namespace DHDM
{
	public class StreamlootsCard
	{
		public string type { get; set; }
		public string message { get; set; }
		public string imageUrl { get; set; }
		public string videoUrl { get; set; }
		public string soundUrl { get; set; }

		public string CardName => data.cardName;
		public string UserName => data.Username;

		public string GetField(string fieldName)
		{
			return data.GetField(fieldName);
		}

		public StreamlootsCardData data { get; set; }
	}
}
