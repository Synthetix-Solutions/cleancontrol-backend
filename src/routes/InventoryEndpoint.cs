namespace cleancontrol_backend.routes;

public static class InventoryEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/inventory/items").MapInventoryItemsApi().WithOpenApi().WithTags("Inventory items");
	}

	private static RouteGroupBuilder MapInventoryItemsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}
}
