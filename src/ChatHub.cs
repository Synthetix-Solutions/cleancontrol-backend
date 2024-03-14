using CleanControlDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CleanControlBackend.Schemas;

[SignalRHub]
[Authorize(Policies.AdminOrCleanerOnly)]
public class ChatHub(CleancontrolContext db) : Hub<IChatClient> {
	private CleancontrolContext DbContext = db;
	public async Task NewMessage(IEnumerable<Guid> userId, Message message) => await Clients.Users(userId).ReceiveMessage(null, message);
	public async Task GetChatHistory(DateTime from, DateTime to) => await Clients.Caller.RecieveChatHistory(null);
	public async Task GetChats() => await Clients.Caller.RecieveChats(null);

}
