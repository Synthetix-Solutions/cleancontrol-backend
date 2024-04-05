#region

using CleanControlDb;
using Microsoft.AspNetCore.Identity;

#endregion

namespace CleanControlBackend.Schemas.Logic;

/// <summary>
///     Helpers for messaging
/// </summary>
public static class MessageHelpers {
	/// <summary>
	///     Creates a new message between two users.
	/// </summary>
	/// <param name="senderId">The ID of the user sending the message.</param>
	/// <param name="receiverId">The ID of the user receiving the message.</param>
	/// <param name="message">The content of the message.</param>
	/// <param name="db">The database context.</param>
	/// <returns>A new Message object that has been saved to the database.</returns>
	/// <exception cref="ArgumentException">Thrown when either the sender or receiver cannot be found in the database.</exception>
	public static Message CreateMessage(Guid senderId, Guid receiverId, string message, CleancontrolContext db) {
		var newMessage = new CleanControlDb.Message {
			Sender = db.Users.Find(senderId) ?? throw new ArgumentException($"Sender with ID {senderId} not found.")
		  , Receiver = db.Users.Find(receiverId) ?? throw new ArgumentException($"Receiver with ID {receiverId} not found.")
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

	/// <summary>
	///     Retrieves the chat instances for a specific user.
	/// </summary>
	/// <param name="userId">The ID of the user for whom to retrieve chat instances.</param>
	/// <param name="db">The database context.</param>
	/// <param name="userManager">The user manager.</param>
	/// <remarks>
	///     This method retrieves all chat instances that the specified user is a part of.
	///     Each chat instance includes the participants and the messages exchanged.
	/// </remarks>
	/// <returns>
	///     A Task that represents the asynchronous operation. The task result contains the chat instances for the
	///     specified user.
	/// </returns>
	public static IEnumerable<Chat> GetChatsForUser(Guid userId, CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		return db
			  .Messages
			  .Where(m => m.Receiver.Id == userId || m.Sender.Id == userId)
			  .ToList()
			  .GroupBy(
					   m => m.Sender.Id == userId ? m.Receiver : m.Sender
					 , (sender, messages) => new { Sender = sender, LastMessage = messages.MaxBy(m => m.SentAt)! }
					  )
			  .Select(
					  chat => new Chat(
									   User.FromDbUser(userManager, chat.Sender)
										   .Result
									 , new Message(
												   chat.LastMessage.Id
												 , chat.LastMessage.Sender.Id
												 , chat.LastMessage.Receiver.Id
												 , chat.LastMessage.Content
												 , chat.LastMessage.SentAt
												  )
									  )
					 );
	}

	/// <summary>
	///     Retrieves the messages for a specific user within a specified date range.
	/// </summary>
	/// <param name="userId">The ID of the user for whom to retrieve messages.</param>
	/// <param name="dateFrom">The start date of the range within which to retrieve messages.</param>
	/// <param name="dateTo">The end date of the range within which to retrieve messages.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves all messages that the specified user has sent within the specified date range.
	///     Each message includes the sender, receiver, content, and the time it was sent.
	/// </remarks>
	/// <returns>
	///     An IEnumerable of Message objects that represent the messages sent by the user within the specified date
	///     range.
	/// </returns>
	public static IEnumerable<Message> GetMessagesForUser(Guid userId, Guid senderId, DateTime dateFrom, DateTime dateTo, CleancontrolContext db) {
		var dateFromUtc = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);
		var dateToUtc = DateTime.SpecifyKind(dateTo, DateTimeKind.Utc);

		return db
			  .Messages
			  .Where(
					 m => (m.Receiver.Id == userId && m.Sender.Id == senderId || m.Sender.Id == userId && m.Receiver.Id == senderId)
					   && m.SentAt >= dateFromUtc
					   && m.SentAt <= dateToUtc
					)
			  .Select(
					  m => new Message(
									   m.Id
									 , m.Sender.Id
									 , m.Receiver.Id
									 , m.Content
									 , m.SentAt
									  )
					 );
	}
}
