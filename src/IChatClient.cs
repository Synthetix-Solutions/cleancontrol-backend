#region

using CleanControlBackend.Schemas;

#endregion

public interface IChatClient {
	Task ReceiveMessage(Message message);
	Task RecieveChatHistory(IEnumerable<Message> messages);
	Task RecieveChats(IEnumerable<Chat> chats);
}
