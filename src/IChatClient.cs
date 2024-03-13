
using CleanControlBackend.Schemas;

public interface IChatClient {
	Task ReceiveMessage(Guid userId, Message message);
	Task RecieveChatHistory(IEnumerable<Message> messages);
	Task GetChats(IEnumerable<Chat> chats);
}
