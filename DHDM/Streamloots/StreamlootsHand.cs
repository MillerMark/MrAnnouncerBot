using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class StreamlootsHand
	{
		public int CharacterId { get; set; }
		public bool IsShown { get; set; }
		public StreamlootsCard SelectedCard { get; set; }
		public List<StreamlootsCard> Cards { get; set; } = new List<StreamlootsCard>();
		public StreamlootsHand()
		{

		}
	}
}
