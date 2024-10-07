using CityInfo.API;
using CityInfo.API.DBContext;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AutoMapper;


//serilog logger created 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//commented out due to using serilog instead
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
builder.Host.UseSerilog();

// add logger


// Add services to the container.

builder.Services.AddControllers(options =>
{

    //can do options.inputformatter/outputformatter and the first formatter for each will be set at the default one 

    options.ReturnHttpNotAcceptable = true;//To prevent sending data if our format doesnt match the format the client requests
}).AddNewtonsoftJson() //replaces default json formatters with json.net
    .AddXmlDataContractSerializerFormatters(); //Added xml format to our responses by calling this one method



//Testing Problem details
builder.Services.AddProblemDetails();



//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//    {
//        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Testing additional info with bad request");
//        ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//    };
//});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // allows us to inject fileextensiontypecontent provider to provide support for files with different content types e.g. application/pdf

#if DEBUG
//adding local mail service (interface, implementation class), IoC Principle, trying to decouple implementations from services 
builder.Services.AddTransient<IMailService,LocalMailService>();
#elif RELEASE //we use the cloud mail service
builder.Services.AddTransient<IMailService,CloudMailService>();
#endif
//adding datastore as a singleton
//builder.Services.AddSingleton<CityInfo.API.ICityInfoRepository>();

//adding my DbContext
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

//Creating scoped datastore
builder.Services.AddScoped<CityInfo.API.Services.ICityInfoRepository, CityInfoRepository>();

//using Automapper to map from city object variables with dto
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
//check for if the application is not in development mode
if (!app.Environment.IsDevelopment())app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
