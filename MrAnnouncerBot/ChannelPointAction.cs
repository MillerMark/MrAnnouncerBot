using SheetsPersist;
using System;
using System.Linq;

namespace MrAnnouncerBot
{
	[Document("Mr. Announcer Guy")]
	[Sheet("ChannelPointActions")]
	public class ChannelPointAction
	{
		[Column]
		public string ID { get; set; }
		
		[Column]
		public string Title { get; set; }
		
		[Column]
		public string SceneToPlay { get; set; }
		
		[Column]
		public string StateToSet { get; set; }
		public ChannelPointAction()
		{

		}
	}
}
