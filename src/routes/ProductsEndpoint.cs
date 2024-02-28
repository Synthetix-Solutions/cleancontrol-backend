#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Product = CleanControlBackend.Schemas.Product;

#endregion

namespace CleanControlBackend.Routes;

public static class ProductsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/products")
		   .MapApi()
		   .WithOpenApi()
		   .WithTags("Products");
	}

	private static RouteGroupBuilder MapApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllProducts)
			 .WithDescription("Fetches all products")
			 .WithSummary("Get all products");

		group.MapPost("/", CreateProduct)
			 .WithDescription("Creates a new product")
			 .WithSummary("Create a new product");

		group.MapGet("/{id}", GetProduct)
			 .WithDescription("Fetches a product by its ID")
			 .WithSummary("Get a product by ID");

		group.MapPut("/{id}", UpdateProduct)
			 .WithDescription("Updates a product by its ID")
			 .WithSummary("Update a product");

		group.MapDelete("/{id}", DeleteProduct)
			 .WithDescription("Deletes a product by its ID")
			 .WithSummary("Delete a product");

		return group;
	}

	private static Ok<Product> DeleteProduct(Guid id) => TypedResults.Ok<Product>(null);

	private static Ok<Product> UpdateProduct(Guid id) => TypedResults.Ok<Product>(null);

	private static Ok<Product> GetProduct(Guid id) => TypedResults.Ok<Product>(null);

	private static Ok<Product> CreateProduct() => TypedResults.Ok<Product>(null);

	private static Ok<IEnumerable<Product>> GetAllProducts(CleancontrolContext db) {
		var dbProducts = db.Products;
		return TypedResults.Ok<IEnumerable<Product>>(null);
	}
}
