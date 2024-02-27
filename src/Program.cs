#region

using System.Reflection;
using System.Text.Json.Serialization;
using CleanControlBackend.Routes;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder
   .Services.AddEndpointsApiExplorer()
   .AddSwaggerGen(
				  options => {
					  options.SwaggerDoc(
										 "v1"
									   , new() {
												   Version = "v1"
												 , Title = "ToDo API"
												 , Description = "An ASP.NET Core Web API for managing ToDo items"
												 , TermsOfService = new("https://example.com/terms")
												 , Contact = new() { Name = "Example Contact", Url = new("https://example.com/contact") }
												 , License = new() { Name = "Example License", Url = new("https://example.com/license") }
											   }
										);
					  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
					  options.EnableAnnotations();
				  }
				 )
   .Configure<JsonOptions>(options => { options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; })
   .AddDbContext<CleancontrolContext>(options => CleancontrolContext.Configure(options, builder.Configuration))
   .AddAuthorization()
   .AddIdentityApiEndpoints<IdentityUser>()
   .AddEntityFrameworkStores<CleancontrolContext>();

var app = builder.Build();

app.UseSwagger().UseSwaggerUI();

app.UseHttpsRedirection();
app.MapIdentityApi<IdentityUser>();

IEnumerable<Action<WebApplication>> mappers = [UsersEndpoints.Map, ProductsEndpoints.Map, TasksEndpoints.Map, CleaningRunsEndpoints.Map, RoomsEndpoints.Map];

foreach (var mapper in mappers) mapper(app);

app.MapGet("/tibsi/brain", StatusCodeHttpResult () => TypedResults.StatusCode(410)).WithOpenApi();
app.MapGet("/tibsi/dick", StatusCodeHttpResult () => TypedResults.StatusCode(416));

app.Run();
