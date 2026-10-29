using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebAPI.Contexts;
using WebAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opciones =>
    {
        // Devuelve los nombres tal como están en las clases (ID_Producto, NombreProducto...)
        opciones.JsonSerializerOptions.PropertyNamingPolicy = null;
        // Evita el error de "ciclo" al devolver objetos que se referencian entre sí (ej: Producto -> Categoria)
        opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
        opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
               )
    );

// Permite que el frontend en Angular (otro puerto) consuma la API
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirFrontend", politica =>
        politica.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod());
});


var app = builder.Build();

// Los SP avisan las reglas de negocio con THROW; aquí se convierten en 400 / 404 con { "mensaje": "..." }
app.Use(ManejadorErroresSql.Manejar);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseCors("PermitirFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "API Sistema de Inventario funcionando...");

app.Run();
