namespace cleancontrol_backend.Schemas;

public record CleaningTeam(Guid id, string name, IEnumerable<User> cleaners);
