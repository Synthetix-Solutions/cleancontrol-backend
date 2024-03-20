namespace CleanControlBackend.Schemas;
/// <summary>
///     Represents a message DTO.
/// </summary>
/// <param name="id">The unique identifier of the message. This is read-only.</param>
/// <param name="senderId">The unique identifier of the sender of the message.</param>
/// <param name="receiverId">The unique identifier of the receiver of the message.</param>
/// <param name="message">The content of the message.</param>
/// <param name="sentAt">The date and time when the message was sent.</param>
public record Message(Guid id, Guid senderId, Guid receiverId, string message, DateTime sentAt);
