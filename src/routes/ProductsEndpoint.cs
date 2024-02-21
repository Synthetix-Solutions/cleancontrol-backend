namespace cleancontrol_backend.routes;

public static class ProductsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/products").MapApi().WithOpenApi().WithTags("Products");
	}

	private static RouteGroupBuilder MapApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<IEnumerable<Schemas.InventoryItem>>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.InventoryItem>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.InventoryItem>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.InventoryItem>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok<Schemas.InventoryItem>(null));

		return group;
	}
}
