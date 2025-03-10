using Microsoft.EntityFrameworkCore;
using System.Net;
using TiendaAPI.Data;

var builder = WebApplication.CreateBuilder(args);
//CAMBIAR IP
builder.WebHost.UseUrls("http://192.168.100.189:7013");

// Add services to the container.
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDB")));

/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Parse("192.168.1.34"), 7014, listenOptions =>
    {
        listenOptions.UseHttps("\\Users\\Beep Montilla\\Desktop\\certificadossl.pfx", "inca2025");
    });
});*/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
