#region

using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a cleaning task DTO.
/// </summary>
/// <param name="Id">The unique identifier of the cleaning task. This is read-only.</param>
/// <param name="name">The name of the cleaning task.</param>
/// <param name="description">The description of the cleaning task. This can be null.</param>
/// <param name="recurrenceInterval">The recurrence interval of the cleaning task. This can be null.</param>
/// <param name="onCheckout">Indicates whether the cleaning task is on checkout.</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record CleaningTask([SwaggerSchema(ReadOnly = true)] Guid Id, string name, string? description, int? recurrenceInterval, bool onCheckout) {
	/// <summary>
	/// Creates a returnable cleaning task object from a database cleaning task object.
	/// </summary>
	/// <param name="dbTask">The database cleaning task object.</param>
	/// <remarks>
	/// This method creates a returnable cleaning task object from a database cleaning task object. It does this by creating a new cleaning task object and populating it with the properties of the database cleaning task object.
	/// The returnable cleaning task object is used to return cleaning task data to the client in a format that is suitable for the client.
	/// </remarks>
	/// <returns>A <see cref="CleaningTask"/> object that contains the data of the database cleaning task object in a format that is suitable for the client.</returns>
	public static CleaningTask FromDbRoomCleaningTask(CleanControlDb.CleaningTask dbTask) =>
		new(
			dbTask.Id
		  , dbTask.Name
		  , dbTask.Description
		  , dbTask.RecurrenceInterval
		  , dbTask.OnCheckout
		   );
};
