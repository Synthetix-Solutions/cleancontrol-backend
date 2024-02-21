using cleancontrol_db;

namespace cleancontrol_backend.routes;

public static class UsersEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/users").MapUserApi().WithOpenApi().WithTags("Users");
	}

	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<Schemas.User[]>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.User>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.User>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.User>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok());

		return group;
	}
}
