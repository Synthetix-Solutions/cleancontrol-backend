#region

using System.Reflection;
using System.Text.Json.Serialization;
using CleanControlBackend.Routes;
using CleanControlBackend.Schemas;
using CleanControlDb;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
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
								 .WithOrigins(
											  "http://localhost:5240"
											, "https://gourav-d.github.io"
											, "http://49.13.203.173:3000"
											 )
								 .AllowAnyMethod()
								 .AllowAnyHeader()
								 .AllowCredentials()
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
										   , Title = "CleanControl API"
										   , Description = "API for CleanControl"
										   , TermsOfService = new Uri("https://synthetix-solutions.com/legal/terms")
										   , Contact = new OpenApiContact {
												 Name = "Contact", Url = new Uri("https://synthetix-solutions.com/contact")
											 }
										   , License = new OpenApiLicense {
												 Name = "License", Url = new Uri("https://synthetix-solutions.com/license")
											 }
										 }
										);
					  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
					  options.EnableAnnotations();
					  options.AddSecurityDefinition(
													"Bearer"
												  , new OpenApiSecurityScheme {
														In = ParameterLocation.Header
													  , Description = "Fake JWT Token"
													  , Name = "Authorization"
													  , Scheme = "Bearer"
													  , Type = SecuritySchemeType.ApiKey
													}
												   );
					  options.AddSecurityRequirement(
													 new OpenApiSecurityRequirement {
														 {
															 new OpenApiSecurityScheme {
																 Reference = new OpenApiReference {
																	 Type = ReferenceType.SecurityScheme, Id = "Bearer"
																 }
															   , Scheme = "oauth2"
															   , Name = "Bearer"
															   , In = ParameterLocation.Header
															 }
														   , []
														 }
													 }
													);
					  options.AddSignalRSwaggerGen();
				  }
				 )
   .Configure<JsonOptions>(
						   options => {
							   options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
							   options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
						   }
						  );

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("CleanControl"));

dataSourceBuilder.MapEnum<Role>($"{dataSourceBuilder.ConnectionStringBuilder.SearchPath}.role");
dataSourceBuilder.MapEnum<CleaningRunPhase>($"{dataSourceBuilder.ConnectionStringBuilder.SearchPath}.cleaning_run_phase");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddSignalR();

builder
   .Services
   .AddDbContext<CleancontrolContext>(
									  o => CleancontrolContext.BuildOptions(
																			o
																		  , dataSource
																		  , dataSourceBuilder
																		   )
									 )
   .AddAuthorization(Policies.AddPolicies)
   .AddIdentityApiEndpoints<CleanControlUser>()
   .AddRoles<IdentityRole<Guid>>()
   .AddEntityFrameworkStores<CleancontrolContext>();


var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler();
	app.UseStatusCodePages();
}

app.Use(async (context, next) => await AuthQueryStringToHeader(context, next));
app.UseCors(allowAllPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app
   .UseSwagger()
   .UseSwaggerUI();

app.UseHttpsRedirection();
app.MapIdentityApi<CleanControlUser>();
// app.UseAuthentication();
// app.UseAuthorization();


IEnumerable<Action<WebApplication>> mappers = [
												  UsersEndpoints.Map
												, ProductsEndpoints.Map
												, TasksEndpoints.Map
												, CleaningRunsEndpoints.Map
												, RoomsEndpoints.Map
											  ];

foreach (var mapper in mappers)
	mapper(app);

app.MapGet("/tibsi/brain", () => TypedResults.StatusCode(410));
app.MapGet("/tibsi/dick", () => TypedResults.StatusCode(416));
// app.MapSignalR();

app.MapHub<ChatHub>("/chatHub");

using var scope = app.Services.CreateScope();
await CreateRolesAndUsers(scope.ServiceProvider);

app.Run();
return;

static async Task AuthQueryStringToHeader(HttpContext context, Func<Task> next) {
	if (string.IsNullOrWhiteSpace(context.Request.Headers.Authorization) && context.Request.QueryString.HasValue) {
		var token = context
				   .Request
				   .QueryString
				   .Value
				   .Split('&')
				   .FirstOrDefault(pair => pair.StartsWith("access_token="))?["access_token=".Length..];
		if (!string.IsNullOrWhiteSpace(token)) {
			context.Request.Headers.Append("Authorization", $"Bearer {token}");
		}
	}

	await next?.Invoke()!;
}

static async Task CreateRolesAndUsers(IServiceProvider serviceProvider) {
	var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
	var userManager = serviceProvider.GetRequiredService<UserManager<CleanControlUser>>();

	var x = await roleManager.RoleExistsAsync("Admin");
	if (!x) {
		var adminRole = new IdentityRole<Guid> { Name = "Admin" };
		await roleManager.CreateAsync(adminRole);
		var cleanerRole = new IdentityRole<Guid> { Name = "Cleaner" };
		await roleManager.CreateAsync(cleanerRole);

		var user = new CleanControlUser {
			UserName = "tolliver@ss.at"
		  , Email = "tolliver@ss.at"
		  , Name = "Tolliver"
		  , IsAdUser = false
		};

		const string userPwd = "String12!";

		var chkUser = await userManager.CreateAsync(user, userPwd);

		if (chkUser.Succeeded)
			await userManager.AddToRoleAsync(user, "Admin");
	}
}
