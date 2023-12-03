using Microsoft.AspNetCore.SignalR;
using signalRtest.Models;
using System.Text.Json;

namespace signalRtest
{
    public class SignalrHub : Hub
    {
        private static GameState gameStatus = new GameState();
        private readonly Timer timer;
        private readonly IHubContext<SignalrHub> hubContext;
        private static int ticktime = 0;

        public SignalrHub(IHubContext<SignalrHub> hubContext)
        {
            this.hubContext = hubContext;


            timer = new Timer(async _ =>
            {
                Console.WriteLine("players " + gameStatus.Players.Count);
                foreach (var player in gameStatus.Players)
                {
                    switch (player.gameData.Direction.ToLower())
                    {
                        case "up":
                            player.gameData.Position.Y += 1;
                            if (player.gameData.Position.Y >= 200)
                                player.gameData.Position.Y = 1;
                            break;
                        case "down":
                            player.gameData.Position.Y -= 1;
                            if (player.gameData.Position.Y < 1)
                                player.gameData.Position.Y = 200;
                            break;
                        case "left":
                            player.gameData.Position.X -= 1;

                            if (player.gameData.Position.X < 1)
                                player.gameData.Position.X = 200;
                            break;
                        case "right":
                            player.gameData.Position.X += 1;
                            if (player.gameData.Position.X >= 200)
                                player.gameData.Position.X = 1;
                            break;
                    }
                }

                Console.WriteLine("Updating gamestate... " + ticktime);
                ticktime += 500;
                await hubContext.Clients.All.SendAsync("messageReceived", gameStatus);
            }, null, 0, 500);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("New Connection");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Lost connection to someone");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewMessage(string user, string message)
        {
            try
            {
                var socketEvent = JsonSerializer.Deserialize<SocketEvent>(message);

                if (socketEvent.eventType == "createPlayer")
                {
                    var player = socketEvent.playerObject;
                    gameStatus.Players.Add(player);
                }
                else if (socketEvent.eventType == "updateMovement")
                {
                    //var playerFromList = gameStatus.Players.FirstOrDefault(x => x.id == socketEvent.playerId);
                    var playerByConnectionId = gameStatus.Players.FirstOrDefault(x => x.id == user);
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
