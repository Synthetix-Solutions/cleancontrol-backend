#region

using System.Reflection;
using System.Text.Json.Serialization;
using cleancontrol_backend.routes;
using Microsoft.AspNetCore.Http.Json;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
								   options.SwaggerDoc("v1", new() {
																	  Version = "v1",
																	  Title = "ToDo API",
																	  Description =
																		  "An ASP.NET Core Web API for managing ToDo items",
																	  TermsOfService = new("https://example.com/terms"),
																	  Contact = new() {
																						  Name = "Example Contact",
																						  Url =
																							  new("https://example.com/contact")
																					  },
																	  License = new() {
																						  Name = "Example License",
																						  Url =
																							  new("https://example.com/license")
																					  }
																  });
								   var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
								   options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
							   });

builder.Services.Configure<JsonOptions>(options => {
											options.SerializerOptions.DefaultIgnoreCondition =
												JsonIgnoreCondition.WhenWritingNull;
										});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

UsersEndpoints.Map(app);
ProductsEndpoints.Map(app);
TasksEndpoints.Map(app);
CleaningRunsEndpoints.Map(app);
RoomsEndpoints.Map(app);
InventoryEndpoints.Map(app);
app.Run();

