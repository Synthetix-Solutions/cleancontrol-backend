using CleanControlDb;
using Microsoft.AspNetCore.Identity;

namespace CleanControlBackend.Schemas.logic;

public static class MessageHelpers {
	public static Message CreateMessage(Guid senderId, Guid receiverId, string message, CleancontrolContext db) {
		var newMessage = new CleanControlDb.Message() {
			Sender = db.Users.Find(senderId) ?? throw new Exception($"Sender with ID {senderId} not found.")
		  , Receiver = db.Users.Find(receiverId) ?? throw new Exception($"Receiver with ID {receiverId} not found.")
		  , Content = message
		};

		db.Messages.Add(newMessage);
		var returnMessage = new Message(
										newMessage.Id
									  , newMessage.Sender.Id
									  , newMessage.Receiver.Id
									  , newMessage.Content
									  , newMessage.SentAt
									   );
		db.SaveChanges();
		return returnMessage;
	}

	public static async Task<IEnumerable<Chat>> GetChatsForUser(Guid userId, CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		var chats = db
				   .Messages
				   .Where(m => m.Sender.Id == userId)
				   .GroupBy(m => m.Receiver)
				   .Select(
						   async c => {
							   var lastMessage = c.MaxBy(m => m.SentAt);
							   return new Chat(
											   await Routes.Handlers.Users.GetReturnUser(userManager, c.Key)
											 , new Message(
														   lastMessage.Id
														 , lastMessage.Sender.Id
														 , lastMessage.Receiver.Id
														 , lastMessage.Content
														 , lastMessage.SentAt
														  )
											  );
						   }
						  );

		return await Task.WhenAll(chats);
	}

	public static IEnumerable<Message> GetMessagesForUser(Guid userId, DateTime dateFrom, DateTime dateTo, CleancontrolContext db) {
		return db
			   .Messages
			   .Where(m => m.Sender.Id == userId && m.SentAt >= dateFrom && m.SentAt <= dateTo)
			   .Select(m => new Message(
										m.Id
									  , m.Sender.Id
									  , m.Receiver.Id
									  , m.Content
									  , m.SentAt
									   )
					  );
	}
}
