namespace CleanControlBackend.Schemas;

public record Message(Guid id, Guid senderId, Guid receiverId, string message, DateTime sentAt);

