#region

using CleanControlBackend.Schemas;
using CleanControlDb;

#endregion

namespace CleanControlBackend.Routes;

public static class ProductsEndpoints {
	public static void Map(WebApplication app, CleancontrolContext db) {
		app.MapGroup("/products").MapApi(db).WithOpenApi().WithTags("Products");
	}

	private static RouteGroupBuilder MapApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<IEnumerable<InventoryItem>>(null))
		   .WithDescription("Fetches all products")
		   .WithSummary("Get all products");

		group
		   .MapPost("/", () => TypedResults.Ok<InventoryItem>(null))
		   .WithDescription("Creates a new product")
		   .WithSummary("Create a new product");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<InventoryItem>(null))
		   .WithDescription("Fetches a product by its ID")
		   .WithSummary("Get a product by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<InventoryItem>(null))
		   .WithDescription("Updates a product by its ID")
		   .WithSummary("Update a product");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok<InventoryItem>(null))
		   .WithDescription("Deletes a product by its ID")
		   .WithSummary("Delete a product");

		return group;
	}
}
