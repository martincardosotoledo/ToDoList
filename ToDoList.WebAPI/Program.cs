using FluentNHibernate.Cfg.Db;
using ToDoList.Aplicacion;
using ToDoList.DataAccess;
using FluentValidation.AspNetCore;
using ToDoList.WebAPI;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        // Registra los validadores en el ensamblado actual
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "To-do list", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


SessionManager.Instance.BuildSessionFactories(builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<NHibernateSessionMiddleware>();

app.MapControllers();

app.Run();


DummyDataHelper.CrearDummyData();


