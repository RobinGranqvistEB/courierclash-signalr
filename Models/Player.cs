namespace signalRtest.Models
{
    public class Player
    {
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public int score { get; set; }
        public string connectionId { get; set; }
        public GameData gameData { get; set; }
            = new GameData()
            {
                Position = new Position() { X = 10, Y = 10 },
                Direction = "up",
                HasPackage = false
            };
    }
}
