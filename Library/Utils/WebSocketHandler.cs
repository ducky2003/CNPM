using System.Net.WebSockets;
using System.Text;

namespace Library.Utils
{
    // WebSocketHandler.cs
    public class WebSocketHandler
    {
        private readonly List<(WebSocket, string)> _sockets = new List<(WebSocket, string)>();
        private string GetCurrentPage(HttpContext context)
        {
            // Extract and return the current page or area identifier from the request
            return context.Request.Path.ToString(); // Adjust as needed based on your URL structure
        }
        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var currentPage = GetCurrentPage(context);

            _sockets.Add((webSocket, currentPage));

            while (webSocket.State == WebSocketState.Open)
            {
                await Task.Delay(500); // Adjust the delay as needed
            }
            _sockets.Remove(_sockets.FirstOrDefault(s => s.Item1 == webSocket));

        }

        public async Task SendMessageAsync(string message, string targetPage)
        {
            foreach (var (socket, currentPage) in _sockets)
            {
                if (socket.State == WebSocketState.Open && currentPage == targetPage)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}