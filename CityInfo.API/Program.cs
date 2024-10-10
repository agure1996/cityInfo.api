using CityInfo.API;
using CityInfo.API.DBContext;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Asp.Versioning.ApiExplorer;

// Serilog logger configuration for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set the minimum log level
    .WriteTo.Console() // Log to console
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day) // Log to a file with daily rolling
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args); // Create a new web application builder

// Use Serilog for logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // Prevent sending data if the requested format doesn't match
})
.AddNewtonsoftJson() // Replace default JSON formatters with Newtonsoft.Json
.AddXmlDataContractSerializerFormatters(); // Support XML responses

// Add Problem Details service for standardized error responses
builder.Services.AddProblemDetails();

// Add the API Explorer for Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// File extension content type provider for serving files with specific content types
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Conditional service registration based on build configuration
#if DEBUG
// Local mail service for development
builder.Services.AddTransient<IMailService, LocalMailService>();
#elif RELEASE 
// Cloud mail service for production
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

// Register the DbContext with SQLite configuration
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

// Scoped service for repository pattern
builder.Services.AddScoped<CityInfo.API.Services.ICityInfoRepository, CityInfoRepository>();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// JWT authentication configuration
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForeignKey"]))
    };
});

// Adding Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromSpij", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Xamar City"); // Custom claim requirement
    });
});

// Setting up API versioning
builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true; // Enable version reporting
    setupAction.AssumeDefaultVersionWhenUnspecified = true; // Assume default version
    setupAction.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0); // Set default version
}).AddMvc()
.AddApiExplorer(setupAction =>
{
    setupAction.SubstituteApiVersionInUrl = true; // Enable version substitution in URL
});

// Swagger documentation setup
var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(setupAction =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        setupAction.SwaggerDoc(
            $"{description.GroupName}",
            new()
            {
                Title = "City Info API", // Title of the API
                Version = description.ApiVersion.ToString(), // API version
                Description = "Access cities and see their points of interest through this API." // API description
            });
    }

    // Include XML comments for Swagger documentation
    var cityApi = $"{Assembly.GetExecutingAssembly().GetName().Name}";
    var xmlCommentsFile = $"{cityApi}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});

var app = builder.Build(); // Build the application

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler(); // Use exception handler in non-development environments

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI(setupAction =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var desc in descriptions)
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json", // Endpoint for the Swagger UI
                desc.GroupName.ToUpperInvariant()); // Group name in uppercase
        }
    });
}

// Use middleware for HTTPS redirection, routing, authentication, and authorization
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization

// Map controllers to endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run(); // Run the application
