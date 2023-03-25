using MarketingScheduler.Models;
using MarketingScheduler.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


// Use the ConfigurationBuilder directly to load the environment variables
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = s.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(configuration["DB_CONN_STR"]);
});
builder.Services.AddControllers();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<CalendarService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketingScheduler API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketing Scheduler V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
