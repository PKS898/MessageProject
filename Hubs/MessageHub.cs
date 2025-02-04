#nullable enable
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MessageProject.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client {ConnectionId} disconnected with error", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client {ConnectionId} disconnected gracefully", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int sequenceNumber, string text)
        {
            _logger.LogInformation("Received message: {Text} (Seq: {Seq})", text, sequenceNumber);

            // Проверяем, есть ли активные клиенты
            var clients = Clients.All;
            bool hasClients = clients != null;
            _logger.LogInformation("WebSocket clients connected: {Status}", hasClients ? "Yes" : "No");

            // Если клиенты есть, отправляем сообщение
            if (hasClients)
            {
                await Clients.All.SendAsync("ReceiveMessage", sequenceNumber, text, DateTime.UtcNow);
                _logger.LogInformation("Broadcasting message to all clients: {Text}", text);
            }
            else
            {
                _logger.LogWarning("No connected WebSocket clients. Message not sent.");
            }
        }
    }
}
