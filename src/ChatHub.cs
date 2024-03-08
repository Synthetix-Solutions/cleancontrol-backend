using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CleanControlBackend.Schemas;
[SignalRHub]
public class ChatHub : Hub
{
	public async Task NewMessage(long username, string message) =>
		await Clients.All.SendAsync("messageReceived", username, message);
}
