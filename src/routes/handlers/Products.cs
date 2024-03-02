#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Product = CleanControlBackend.Schemas.Product;

#endregion

namespace CleanControlBackend.Routes.Handlers;

public static class Products {
	public static Results<Ok, ProblemHttpResult> DeleteProduct(Guid productId, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		db.Products.Remove(dbProduct);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	public static Results<Ok<Product>, ProblemHttpResult> UpdateProduct(Guid productId, Product product, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		dbProduct.Name = product.name;
		dbProduct.InventoryQuantity = product.inventoryQuantity;

		var returnProduct = new Product(
										dbProduct.Id
									  , dbProduct.Name
									  , dbProduct.InventoryQuantity
									  , dbProduct.Image
									   );
		return TypedResults.Ok(returnProduct);
	}

	public static Results<ProblemHttpResult, Ok<Product>> GetProduct(Guid productId, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok<Product>(null);
	}

	public static Ok<Product> CreateProduct(Product product, CleancontrolContext db) {
		var dbProduct = new CleanControlDb.Product { Name = product.name, InventoryQuantity = product.inventoryQuantity, Image = product.image };
		db.Products.Add(dbProduct);

		var returnProduct = new Product(
										dbProduct.Id
									  , dbProduct.Name
									  , dbProduct.InventoryQuantity
									  , dbProduct.Image
									   );
		db.SaveChanges();

		return TypedResults.Ok(returnProduct);
	}

	public static Ok<IEnumerable<Product>> GetAllProducts(CleancontrolContext db) {
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
