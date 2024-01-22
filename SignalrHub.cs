using Microsoft.AspNetCore.SignalR;
using signalRtest.Models;
using System.Text.Json;

namespace signalRtest
{
    public class SignalrHub : Hub
    {
        private readonly Timer timer;
        private readonly IHubContext<SignalrHub> _hubContext;
        private readonly GameManager _gameManager;
        private static int ticktime = 0;

        public SignalrHub(IHubContext<SignalrHub> hubContext, GameManager gameManager)
        {
            _hubContext = hubContext;
            _gameManager = gameManager; 
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("New Connection");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Lost connection to someone: {Context.ConnectionId}");

            var playerToRemove = _gameManager.gameState.Players.FirstOrDefault(x => x.id == Context.ConnectionId);
            if (playerToRemove != null)
            {
                _gameManager.gameState.Players.Remove(playerToRemove);

                var removedPlayerEvent = new SocketEvent
                {
                    eventType = "removePlayer",
                    playerObject = playerToRemove
                };

                _hubContext.Clients.All.SendAsync("NewMessage", "Server", JsonSerializer.Serialize(removedPlayerEvent));
            }

            return base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string user, string message)
        {
            try
            {
                var socketEvent = JsonSerializer.Deserialize<SocketEvent>(message);

                if (socketEvent.eventType == "createPlayer")
                {
                    Console.WriteLine("createPlayer called");
                    var player = socketEvent.playerObject;
                    _gameManager.gameState.Players.Add(player);
                }
                else if (socketEvent.eventType == "updateMovement")
                {
                    Console.WriteLine("updateMovement called");
                    var playerByConnectionId = _gameManager.gameState.Players.FirstOrDefault(x => x.id == user);
                    playerByConnectionId.gameData.Direction = socketEvent.direction;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
