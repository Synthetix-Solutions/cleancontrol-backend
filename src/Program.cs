#region

using System.Reflection;
using System.Text.Json.Serialization;
using CleanControlBackend.Routes;
using CleanControlDb;
using Microsoft.AspNetCore.Http.Json;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
								   options.SwaggerDoc("v1"
													, new() {
																Version = "v1", Title = "ToDo API"
															  , Description = "An ASP.NET Core Web API for managing ToDo items"
															  , TermsOfService = new("https://example.com/terms")
															  , Contact = new() {
																					Name = "Example Contact"
																				  , Url = new("https://example.com/contact")
																				}
															  , License = new() {
																					Name = "Example License"
																				  , Url = new("https://example.com/license")
																				}
															}
													 );
								   var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
								   options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
							   }
							  );

builder.Services.Configure<JsonOptions>(options => { options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; });

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var db = new CleancontrolContext();

IEnumerable<Action<WebApplication, CleancontrolContext>> mappers = [UsersEndpoints.Map, ProductsEndpoints.Map, TasksEndpoints.Map, CleaningRunsEndpoints.Map, RoomsEndpoints.Map];

foreach (var mapper in mappers) mapper(app, db);

app.MapGet("/tibsi/brain", () => TypedResults.StatusCode(410));
app.MapGet("/tibsi/dick", () => TypedResults.StatusCode(416));

app.Run();
