namespace cleancontrol_backend.routes;

public static class ProductsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/products").MapApi().WithOpenApi().WithTags("Products");
	}

	private static RouteGroupBuilder MapApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}
}
