using ApiMonitoreo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Inyeccion de dependencia
builder.Services.AddDbContext<MonitoreoContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("UseCors", policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("UseCors");
app.MapControllers();

app.Run();
