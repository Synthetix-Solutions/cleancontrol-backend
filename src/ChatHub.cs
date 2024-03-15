#region

using CleanControlBackend.Schemas.logic;
using CleanControlDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

#endregion

namespace CleanControlBackend.Schemas;

[SignalRHub]
// [Authorize(Policies.AdminOrCleanerOnly)]
public class ChatHub(CleancontrolContext db, UserManager<CleanControlUser> userManager) : Hub<IChatClient> {
	private CleanControlUser CallingUser =>
		userManager.GetUserAsync(Context.User!)
				   .Result
	 ?? throw new InvalidOperationException();

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
		var chatHistory = MessageHelpers.GetMessagesForUser(
															CallingUser.Id
														  , dateFrom
														  , dateTo
														  , db
														   );
		await Clients.Caller.RecieveChatHistory(chatHistory);
	}

	public async Task GetChats() {
		var chats = await MessageHelpers.GetChatsForUser(
														 CallingUser.Id
													   , db
													   , userManager
														);
		await Clients.Caller.RecieveChats(chats);
	}
}
