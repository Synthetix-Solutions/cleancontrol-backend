#region

#endregion

#region

using CleanControlBackend.Routes.Handlers;
using CleanControlBackend.Schemas;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

#endregion

namespace CleanControlBackend.Routes;

/// <summary>
///     Contains endpoints for /products
/// </summary>
public static class ProductsEndpoints {
	/// <summary>
	///     Maps routes for /products
	/// </summary>
	/// <param name="app"></param>
	public static void Map(WebApplication app) {
		app
		   .MapGroup("/products")
		   .MapApi()
		   .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Products");
	}

	/// <summary>
	///     Maps /products
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private static RouteGroupBuilder MapApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", Products.GetAllProducts)
		   .WithDescription("Fetches all products")
		   .WithSummary("Get all products");

		group
		   .MapPost("/", Products.CreateProduct)
		   .WithDescription("Creates a new product")
		   .WithSummary("Create a new product");

		group
		   .MapGet("/{productId:guid}", Products.GetProduct)
		   .WithDescription("Fetches a product by its ID")
		   .WithSummary("Get a product by ID");

		group
		   .MapPut("/{productId:guid}", Products.UpdateProduct)
		   .WithDescription("Updates a product by its ID")
		   .WithSummary("Update a product");

		group
		   .MapDelete("/{productId:guid}", Products.DeleteProduct)
		   .WithDescription("Deletes a product by its ID")
		   .WithSummary("Delete a product");

		return group;
	}
}
