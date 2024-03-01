#region

using System.Reflection;
using System.Text.Json.Serialization;
using CleanControlBackend;
using CleanControlBackend.Routes;
using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using Npgsql;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


const string allowAllPolicyName = "AllowAll";

builder
   .Services
   .AddCors(
			o => o.AddPolicy(
							 allowAllPolicyName
						   , p => p
								 .AllowAnyOrigin()
								 .AllowAnyMethod()
								 .AllowAnyHeader()
							)
		   )
   .AddProblemDetails()
   .AddEndpointsApiExplorer()
   .AddSwaggerGen(
				  options => {
					  options.SwaggerDoc(
										 "v1"
									   , new OpenApiInfo {
															 Version = "v1"
														   , Title = "ToDo API"
														   , Description = "An ASP.NET Core Web API for managing ToDo items"
														   , TermsOfService = new Uri("https://example.com/terms")
														   , Contact
																 = new OpenApiContact {
																						  Name = "Example Contact"
																						, Url = new Uri("https://example.com/contact")
																					  }
														   , License = new OpenApiLicense {
																							  Name = "Example License"
																							, Url = new Uri("https://example.com/license")
																						  }
														 }
										);
					  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
					  options.EnableAnnotations();
				  }
				 )
   .Configure<JsonOptions>(options => { options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; });

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("CleanControl"));

dataSourceBuilder.MapEnum<Role>();
var dataSource = dataSourceBuilder.Build();

builder
   .Services
   .AddDbContext<CleancontrolContext>(OptionsBuilder)
   .AddAuthorization(Policies.AddPolicies)
   .AddIdentityApiEndpoints<CleanControlUser>()
   .AddEntityFrameworkStores<CleancontrolContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler();
	app.UseStatusCodePages();
}
app.UseCors(allowAllPolicyName);

app
   .UseSwagger()
   .UseSwaggerUI();

app.UseHttpsRedirection();
app.MapIdentityApi<CleanControlUser>();

IEnumerable<Action<WebApplication>> mappers = [ProductsEndpoints.Map, TasksEndpoints.Map, CleaningRunsEndpoints.Map, RoomsEndpoints.Map];

foreach (var mapper in mappers)
	mapper(app);

app
   .MapGet("/tibsi/brain", StatusCodeHttpResult () => TypedResults.StatusCode(410))
   .WithOpenApi();
app.MapGet("/tibsi/dick", StatusCodeHttpResult () => TypedResults.StatusCode(416));

app.Run();
return;

void OptionsBuilder(DbContextOptionsBuilder options) =>
	options
	   .UseLazyLoadingProxies()
	   .UseNpgsql(
				  dataSource
				, o => {
					  var searchPaths = dataSourceBuilder.ConnectionStringBuilder.SearchPath?.Split(',');
					  // Workaround for "__EFMigrationsHistory already exists on dbContext.Database.Migrate();"
					  // https://github.com/npgsql/efcore.pg/issues/2878
					  if (searchPaths is not { Length: > 0 })
						  return;
					  var mainSchema = searchPaths[0];
					  o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, mainSchema);
				  }
				 );
