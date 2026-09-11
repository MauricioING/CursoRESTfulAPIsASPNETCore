using BibliotecaAPI.Datos;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// �rea de servicios

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

// �rea de middlewares

app.MapControllers();

app.Run();