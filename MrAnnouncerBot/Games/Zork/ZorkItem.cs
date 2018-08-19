using System.Collections.Generic;

namespace MrAnnouncerBot.Games.Zork
{
    public class ZorkItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOpen { get; set; }
        public bool IsContainer { get; set; }
        public List<ZorkItem> Contents { get; set; }
        public ZorkItem()
        {
            Contents = new List<ZorkItem>();
        }
    }
}