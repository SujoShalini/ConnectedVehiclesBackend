using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddSingleton(
    serviceProvider =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        string serviceBusConnectionString = configuration["Azure:ServiceBusConnectionString"];
        string queueName = configuration["Azure:QueueName"];
        return new VehiceStatusService(serviceBusConnectionString, queueName);
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
