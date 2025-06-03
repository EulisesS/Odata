using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using ODataService.Data;
using ODataService.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar OData y el modelo
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Product>("Products"); 


// Base de datos en memoria
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ProductosDB"));

// Configurar servicios OData
builder.Services.AddControllers().AddOData(opt =>
    opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100).AddRouteComponents("odata", modelBuilder.GetEdmModel())
);

builder.Services.AddCors();

var app = builder.Build();

// Usar CORS
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Datos iniciales
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Products.AddRange(
        new Product { Id = 1, Nombre = "Laptop", Precio = 1000 },
        new Product { Id = 2, Nombre = "Mouse", Precio = 50 },
        new Product { Id = 3, Nombre = "Teclado", Precio = 80 }
    );
    db.SaveChanges();
}

app.Run();
