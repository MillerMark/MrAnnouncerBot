using SheetsPersist;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		[Document("Mr. Announcer Guy")]
		[Sheet("Restrictions")]
		public class RestrictedSceneDto
		{
			[Column]
			public string SceneName { get; set; }
			public RestrictedSceneDto()
			{
				
			}
		}
	}
}