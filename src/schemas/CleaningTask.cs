namespace CleanControlBackend.Schemas;

public record CleaningTask(Guid Id, string name, string? Description, int? recurrenceInterval, bool onCheckout);
