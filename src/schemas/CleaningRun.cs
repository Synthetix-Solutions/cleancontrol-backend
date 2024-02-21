namespace cleancontrol_backend.Schemas;

public record CleaningRun(Guid id, DateTime date, CleaningTeam cleaningTeam, Room startingRoom);

