using FluentNHibernate.Cfg.Db;
using ToDoList.Aplicacion;
using ToDoList.DataAccess;
using FluentValidation.AspNetCore;

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
builder.Services.AddSwaggerGen();


SessionManager.Instance.BuildSessionFactories(builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();


DummyDataHelper.CrearDummyData();

app.Run();

