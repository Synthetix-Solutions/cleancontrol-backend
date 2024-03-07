#region

using System.Reflection;
using System.Text.Json.Serialization;
using CleanControlBackend.Routes;
using CleanControlBackend.Schemas;
using CleanControlDb;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
   .AddValidatorsFromAssemblyContaining<Program>()
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
										   , Contact = new OpenApiContact { Name = "Example Contact", Url = new Uri("https://example.com/contact") }
										   , License = new OpenApiLicense { Name = "Example License", Url = new Uri("https://example.com/license") }
										 }
										);
					  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
					  options.EnableAnnotations();
				  }
				 )
   .Configure<JsonOptions>(options => { options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("CleanControl"));

dataSourceBuilder.MapEnum<Role>();
var dataSource = dataSourceBuilder.Build();

builder
   .Services
   .AddDbContext<CleancontrolContext>(
									  o => CleanControlDb.CleancontrolContext.BuildOptions(
																						   o
																						 , dataSource
																						 , dataSourceBuilder
																						  )
									 )
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

IEnumerable<Action<WebApplication>> mappers = [UsersEndpoints.Map,ProductsEndpoints.Map, TasksEndpoints.Map, CleaningRunsEndpoints.Map, RoomsEndpoints.Map];

foreach (var mapper in mappers)
	mapper(app);

app.MapGet("/tibsi/brain", () => TypedResults.StatusCode(410));
app.MapGet("/tibsi/dick", () => TypedResults.StatusCode(416));

app.Run();

