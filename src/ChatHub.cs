#region

using Chat;
using CleanControlBackend.Schemas.Logic;
using CleanControlDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
/// Hub for Chat functionality
/// </summary>
/// <param name="db"></param>
/// <param name="userManager"></param>
[SignalRHub]
[Authorize]
public class ChatHub(CleancontrolContext db, UserManager<CleanControlUser> userManager) : Hub<IChatClient> {
	private CleanControlUser CallingUser =>
		userManager.GetUserAsync(Context.User!)
				   .Result
	 ?? throw new InvalidOperationException();

	/// <summary>
	/// Creates a new message
	/// </summary>
	/// <param name="recipientIds">User IDs of the message recipients</param>
	/// <param name="message">Content of the message</param>
	public async Task NewMessage(IEnumerable<Guid> recipientIds, string message) {
		var newMessages = recipientIds.Select(
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

	/// <summary>
	/// Gets the chat history (received messages) for the calling user.
	/// </summary>
	/// <param name="senderId">ID of the other User in the conversation.</param>
	/// <param name="dateFrom">The start date for the period for which to retrieve chat history. Messages sent on or after this date will be included in the chat history.</param>
	/// <param name="dateTo">The end date for the period for which to retrieve chat history. Messages sent up to and including this date will be included in the chat history.</param>
	/// <returns>The chat history for the calling user within the specified date range.</returns>
	public async Task GetChatHistory(Guid senderId ,DateTime dateFrom, DateTime dateTo) {


		var chatHistory = MessageHelpers.GetMessagesForUser(
															CallingUser.Id
														   ,senderId
														  , dateFrom
														  , dateTo
														  , db
														   ).ToList();

		await Clients.Caller.RecieveChatHistory(chatHistory);
	}

	/// <summary>
	/// Retrieves the chat messages for the calling user.
	/// </summary>
	/// <returns>The chat instances for the calling user.</returns>
	public async Task GetChats() {
		var chats = MessageHelpers.GetChatsForUser(
														 CallingUser.Id
													   , db
													   , userManager
														).ToList();
		await Clients.Caller.RecieveChats(chats);
	}
}
