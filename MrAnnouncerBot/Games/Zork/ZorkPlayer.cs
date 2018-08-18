namespace MrAnnouncerBot.Games.Zork
{
    public class ZorkPlayer
    {
        public string UserId { get; set; }
        public ZorkLocation Location { get; set; }
        public int Score { get; set; }
        public int Moves { get; set; }
    }
}