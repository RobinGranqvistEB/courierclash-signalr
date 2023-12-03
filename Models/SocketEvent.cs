namespace signalRtest.Models
{
    public class SocketEvent
    {
        public string eventType { get; set; }
        public Player playerObject { get; set; }
        public string playerId { get; set; }
        public string direction { get; set; }
    }
}
