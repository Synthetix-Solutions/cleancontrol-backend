namespace cleancontrol_backend.routes;

public static class UsersEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/users").MapUserApi().WithOpenApi().WithTags("Users");
	}

	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok(new User(Guid.Empty, "John Doe", "adsf")));
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}

	/// <summary>
	///     I bin a Juser
	/// </summary>
	/// <param name="Id">Hilf ma</param>
	/// <param name="Name">i spia</param>
	/// <param name="Email">mi nimma</param>
	public record User(Guid Id, string Name, string Email);
}
