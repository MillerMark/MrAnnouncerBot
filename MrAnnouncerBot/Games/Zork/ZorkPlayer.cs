﻿using System.Collections.Generic;

namespace MrAnnouncerBot.Games.Zork
{
    public class ZorkPlayer
    {
        public string UserId { get; set; }
        public ZorkLocation Location { get; set; }
        public int Score { get; set; }
        public int Moves { get; set; }
        public List<ZorkItem> Inventory { get; }

        public ZorkPlayer()
        {
            Inventory = new List<ZorkItem>();
        }
    }
}