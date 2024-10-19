using NHibernate.Exceptions;
using NHibernate;
using Microsoft.AspNetCore.Mvc;

namespace ToDoList.WebAPI.Middleware
{
    public class NHibernateExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public NHibernateExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (StaleObjectStateException)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync("Concurrency conflict.");
                throw;
            }
            catch (ObjectNotFoundException)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Entity not found",
                    Type = ""
                };

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (GenericADOException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Database error.");
                throw;
            }
        }
    }
}
