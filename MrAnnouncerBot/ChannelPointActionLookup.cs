using System.Collections.Generic;
using System.Linq;

namespace MrAnnouncerBot
{
    public static class ChannelPointActionLookup
    {
        public static ChannelPointAction Find(IList<ChannelPointAction> actions, string id, string title)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var byId = actions.FirstOrDefault(x => x.ID == id);
                if (byId != null) return byId;
            }
            return actions.FirstOrDefault(x => string.Compare(x.Title, title, true) == 0);
        }
    }
}
