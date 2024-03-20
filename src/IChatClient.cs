#region

using CleanControlBackend.Schemas;

namespace Chat {
	/// <summary>
	/// Typed proxy for the SignalR client
	/// </summary>
	public interface IChatClient {
		/// <summary>
		/// When a message is received
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Task ReceiveMessage(Message message);
		/// <summary>
		/// When chat history is received
		/// </summary>
		/// <param name="messages">List of messages</param>
		/// <returns></returns>
		Task RecieveChatHistory(IEnumerable<Message> messages);
		/// <summary>
		/// When chats are received
		/// </summary>
		/// <param name="chats">All chats the user is part of, including the latest message</param>
		/// <returns></returns>
		Task RecieveChats(IEnumerable<CleanControlBackend.Schemas.Chat> chats);
	}
}

#endregion
