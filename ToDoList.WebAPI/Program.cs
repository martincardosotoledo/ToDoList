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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TodoCrud", policy =>
    {
        policy.RequireClaim("scope", "todo-crud");
    });
});

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
    c.SwaggerDoc("v1", 
        new OpenApiInfo { 
            Title = "To-do list", 
            Version = "v1",
            Description = "API para la administración de una lista de tareas pendientes",
            Contact = new OpenApiContact
            {
                Name = "Martín Cardoso",
                Email = "martincardosotoledo@gmail.com"
            }
        });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    var security = new OpenApiSecurityScheme    
    {
        Name = HeaderNames.Authorization,    
        Type = SecuritySchemeType.ApiKey,    
        In = ParameterLocation.Header,    
        Description = "JWT Authorization header",  
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,  
            Type = ReferenceType.SecurityScheme  
        }
    };

    c.AddSecurityDefinition(security.Reference.Id, security);  
    c.AddSecurityRequirement(new OpenApiSecurityRequirement   
        {{security, Array.Empty<string>()}});
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


app.UseMiddleware<NHibernateSessionMiddleware>();
app.UseMiddleware<NHibernateExceptionMiddleware>();

app.MapControllers();

DummyDataHelper.CrearDummyData();

app.Run();