using CleanControlBackend.Schemas.logic;
using CleanControlDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CleanControlBackend.Schemas;

[SignalRHub]
// [Authorize(Policies.AdminOrCleanerOnly)]
public class ChatHub() : Hub<IChatClient> {
	private UserManager<CleanControlUser> userManager;
	private CleanControlUser CallingUser => userManager.GetUserAsync(Context.User!).Result ?? throw new InvalidOperationException();
	private CleancontrolContext db;
	public async Task NewMessage(IEnumerable<Guid> userIds, string message) {
		var newMessages = userIds.Select(
										 id => {
											 var newMessage = MessageHelpers.CreateMessage(
																						   CallingUser.Id
																						 , id
																						 , message
																						 , db
																						  );

											 return Clients
												   .User(id.ToString())
												   .ReceiveMessage(newMessage);
										 }
										);

		await Task.WhenAll(newMessages);
	}

	public async Task GetChatHistory(DateTime dateFrom, DateTime dateTo) {
		var chatHistory = logic.MessageHelpers.GetMessagesForUser(
																  CallingUser.Id
																, dateFrom
																, dateTo
																, db
																 );
		await Clients.Caller.RecieveChatHistory(chatHistory);
	}

	public async Task GetChats() {
		var chats = await logic.MessageHelpers.GetChatsForUser(CallingUser.Id, db, userManager);
		await Clients.Caller.RecieveChats(chats);
	}
}
