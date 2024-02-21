namespace CleanControlBackend.Schemas;

public record CleaningTeam(Guid id, string name, IEnumerable<User> cleaners);
