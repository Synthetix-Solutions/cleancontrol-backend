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

	private static Results<Ok, ProblemHttpResult> DeleteProduct(Guid id, CleancontrolContext db) {
		var dbProduct = db.Products.Find(id);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		db.Products.Remove(dbProduct);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	private static Results<Ok<Product>, ProblemHttpResult> UpdateProduct(Guid id, Product product, CleancontrolContext db) {
		var dbProduct = db.Products.Find(id);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		dbProduct.Name = product.name;
		dbProduct.InventoryQuantity = product.inventoryQuantity;

		var returnProduct = new Product(
										dbProduct.Id
									  , dbProduct.Name
									  , dbProduct.InventoryQuantity
									  , dbProduct.Image
									   );
		return TypedResults.Ok<Product>(returnProduct);
	}

	private static Results<ProblemHttpResult, Ok<Product>> GetProduct(Guid id, CleancontrolContext db) {
		var dbProduct = db.Products.Find(id);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok<Product>(null);
	}

	private static Ok<Product> CreateProduct(Product product, CleancontrolContext db) {
		var dbProduct = new CleanControlDb.Product { Name = product.name, InventoryQuantity = product.inventoryQuantity, Image = product.image };
		db.Products.Add(dbProduct);

		var returnProduct = new Product(
										dbProduct.Id
									  , dbProduct.Name
									  , dbProduct.InventoryQuantity
									  , dbProduct.Image
									   );
		db.SaveChanges();

		return TypedResults.Ok<Product>(returnProduct);
	}

	private static Ok<IEnumerable<Product>> GetAllProducts(CleancontrolContext db) {
		var products = db.Products.Select(
										  product => new Product(
																 product.Id
															   , product.Name
															   , product.InventoryQuantity
															   , product.Image
																)
										 );
		return TypedResults.Ok<IEnumerable<Product>>(products);
	}
}
