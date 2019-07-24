using System;
using System.Linq;
using System.Collections.Generic;

namespace MrAnnouncerBot
{
	public class LiveStreamData<T>
	{
		/* {
				"data":
						[
							{
								"id":"34970270896",
								"user_id":"237584851",
								"user_name":"CodeRushed",
								"game_id":"509670",
								"type":"live",
								"title":"Testing only. Feel free to disregard.",
								"viewer_count":0,
								"started_at":"2019-07-20T01:10:23Z",
								"language":"en",
								"thumbnail_url":"https://static-cdn.jtvnw.net/previews-ttv/live_user_coderushed-{width}x{height}.jpg",
								"tag_ids":["6ea6bca4-4712-4ab9-a906-e3336a9d8039"]
							}
						],
				"pagination":
						{
							"cursor":"eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6MX19"
						}
				}"
		 */

		public List<T> data { get; set; }
		public TwitchPagination pagination { get; set; }
		public LiveStreamData()
		{


		}
	}
}