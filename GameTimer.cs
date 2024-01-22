using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using signalRtest;
using signalRtest.Models;
using System.Text.Json;

public class GameTimer
{
    private readonly IHubContext<SignalrHub> _hubContext;
    private readonly GameManager _gameManager;
    private Timer _timer;

    public GameTimer(IHubContext<SignalrHub> hubContext, GameManager gameManager)
    {
        _hubContext = hubContext;
        _gameManager = gameManager;
    }

    public void StartTimer()
    {
        _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
    }

    private async void TimerCallback(object state)
    {
        Console.WriteLine("players " + _gameManager.gameState.Players.Count);
        foreach(var player in _gameManager.gameState.Players)
        {
            Console.WriteLine(player.id);
        }
        foreach (var player in _gameManager.gameState.Players)
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

        await _hubContext.Clients.All.SendAsync("messageReceived", _gameManager.gameState);
    }
}
