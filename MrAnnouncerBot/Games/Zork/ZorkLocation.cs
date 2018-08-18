using System.Collections.Generic;

namespace MrAnnouncerBot.Games.Zork
{
    public class ZorkLocation
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<ZorkItem> Items { get; set; }

        public ZorkLocation()
        {
            Items = new List<ZorkItem>();
        }
    }
}