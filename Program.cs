using signalRtest;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials());
});

builder.Services.AddSingleton<GameTimer>();
builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapHub<SignalrHub>("/hub");

app.Services.GetRequiredService<GameTimer>().StartTimer();

app.Run();
