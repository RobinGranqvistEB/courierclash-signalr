namespace signalRtest.Models
{
    public class GameState
    {
        public Position CurrentPackage { get; set; }
        public Position DropZone { get; set; }
        public List<Player> Players { get; set; } = new();
    }
}
