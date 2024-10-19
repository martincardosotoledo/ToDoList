using FluentNHibernate.Cfg.Db;
using ToDoList.Aplicacion;
using ToDoList.DataAccess;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.Reflection;
using NHibernate.Exceptions;
using NHibernate;
using ToDoList.WebAPI.Middleware;
using ToDoList.WebAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddFluentValidation(fv =>  
    {
        // Registra los validadores en el ensamblado actual
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
            "https://httpstatuses.com/404";
    }); ;

builder.Services.AddScoped<TareasService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "To-do list", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

SessionManager.Instance.BuildSessionFactories(builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//if (app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/error-development");
//}
//else
//{
//    app.UseExceptionHandler("/error");
//}



app.UseExceptionHandler();
app.UseMiddleware<NHibernateSessionMiddleware>();
app.UseMiddleware<NHibernateExceptionMiddleware>();

app.MapControllers();

DummyDataHelper.CrearDummyData();

app.Run();