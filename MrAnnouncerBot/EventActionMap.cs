using SheetsPersist;

namespace MrAnnouncerBot {
	[Document("Mr. Announcer Guy")]
	[Sheet("EventActions")]
	public class EventActionMap {
		[Column] public string EventName { get; set; }
		[Column] public string Action { get; set; }
	}
}
