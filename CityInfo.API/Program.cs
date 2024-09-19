using Microsoft.AspNetCore.StaticFiles;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{

    //can do options.inputformatter/outputformatter and the first formatter for each will be set at the default one 

    options.ReturnHttpNotAcceptable = true;//To prevent sending data if our format doesnt match the format the client requests
}).AddNewtonsoftJson() //replaces default json formatters with json.net
    .AddXmlDataContractSerializerFormatters(); //Added xml format to our responses by calling this one method


//Testing Problem details
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Testing additional info with bad request");
        ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // allows us to inject fileextensiontypecontent provider to provide support for files with different content types e.g. application/pdf

var app = builder.Build();

// Configure the HTTP request pipeline.
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
