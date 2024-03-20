namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a chat record.
/// </summary>
/// <param name="sender">The user who sent the last message in the chat.</param>
/// <param name="lastMessage">The last message sent in the chat.</param>
public record Chat(User sender, Message lastMessage);
