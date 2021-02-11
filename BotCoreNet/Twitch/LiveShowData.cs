using System;
using System.Linq;

namespace BotCore
{
	public class LiveShowData
	{
		public string id { get; set; }
		public string user_id { get; set; }
		public string user_name { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public string created_at { get; set; }
		public string published_at { get; set; }
		public string url { get; set; }
		public string thumbnail_url { get; set; }
		public string viewable { get; set; }
		public int view_count { get; set; }
		public string language { get; set; }
		public string type { get; set; }
		public string duration { get; set; }
		public LiveShowData()
		{

		}
	}
}