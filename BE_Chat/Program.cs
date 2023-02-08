using BE_Chat;
using BE_Chat.Handlers;
using BE_Chat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ConnectionService>();
builder.Services.AddSingleton<ChatHandler>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("WebSocketServer");
    });
});

var wso = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
    ReceiveBufferSize = 4 * 1024
};

app.UseWebSockets(wso);
app.UseMiddleware<WebSocketMiddleware>();

app.Run();
