using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CleanControlBackend.Schemas;
[SignalRHub]
[Authorize(Policies.AdminOrCleanerOnly)]
public class ChatHub : Hub
{
	public async Task NewMessage(Guid userId, string message) =>
		await Clients.All.SendAsync("messageReceived", userId, message);
}
