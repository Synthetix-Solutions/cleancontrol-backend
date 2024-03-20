#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Product = CleanControlBackend.Schemas.Product;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
/// Handlers for /products
/// </summary>
public static class Products {
	/// <summary>
	/// Deletes a product from the database.
	/// </summary>
	/// <param name="productId">The ID of the product to delete.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method deletes a product from the database. It does this by finding the product in the database using the provided product ID.
	/// If the product is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the product is found, it is removed from the database and the changes are saved.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok"/> result if the product was successfully deleted, or a <see cref="ProblemHttpResult"/> with an error message if the product was not found.</returns>
	public static Results<Ok, ProblemHttpResult> DeleteProduct(Guid productId, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		db.Products.Remove(dbProduct);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	/// <summary>
	/// Updates a product in the database.
	/// </summary>
	/// <param name="productId">The ID of the product to update.</param>
	/// <param name="product">The product object containing the updated information.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method updates a product in the database. It does this by finding the product in the database using the provided product ID.
	/// If the product is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the product is found, its properties are updated with the values from the provided product object and the changes are saved.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok{T}"/> result with the updated product, or a <see cref="ProblemHttpResult"/> with an error message if the product was not found.</returns>
	public static Results<Ok<Product>, ProblemHttpResult> UpdateProduct(Guid productId, Product product, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		dbProduct.Name = product.name;
		dbProduct.InventoryQuantity = product.inventoryQuantity;

		if (!string.IsNullOrWhiteSpace(product.image))
			dbProduct.Image = product.image;

		db.SaveChanges();

		var returnProduct = new Product(
										dbProduct.Id
									  , dbProduct.Name
									  , dbProduct.InventoryQuantity
									  , dbProduct.Image
									   );
		return TypedResults.Ok(returnProduct);
	}

	/// <summary>
	/// Retrieves a product from the database.
	/// </summary>
	/// <param name="productId">The ID of the product to retrieve.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method retrieves a product from the database. It does this by finding the product in the database using the provided product ID.
	/// If the product is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the product is found, it returns an <see cref="Ok{T}"/> result with the product.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok{T}"/> result with the product, or a <see cref="ProblemHttpResult"/> with an error message if the product was not found.</returns>
	public static Results<ProblemHttpResult, Ok<Product>> GetProduct(Guid productId, CleancontrolContext db) {
		var dbProduct = db.Products.Find(productId);
		if (dbProduct is null)
			return TypedResults.Problem($"Product with ID {productId} not found", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok<Product>(null);
	}

	/// <summary>
	/// Creates a new product in the database.
	/// </summary>
	/// <param name="product">The product object to be added to the database.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method creates a new product in the database. It does this by creating a new product object with the provided product details,
	/// adding it to the database, and then saving the changes to the database.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}"/> result with the created product.</returns>

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

	/// <summary>
	/// Retrieves all products from the database.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method retrieves all products from the database. It does this by selecting all products from the database and returning them as a list.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}"/> result with the list of all products.</returns>
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
