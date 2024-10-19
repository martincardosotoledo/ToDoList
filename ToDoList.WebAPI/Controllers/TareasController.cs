using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Aplicacion;
using ToDoList.Comun.Excepciones;
using ToDoList.Dominio;

namespace ToDoList.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {

        /// <summary>
        /// Devuelve las tareas
        /// </summary>
        /// <returns>Lista de tareas</returns>
        [HttpGet]
        [Route("")]
        [Route("{idTarea:int}")] // hay que definir 2 rutas ya que openAPI no soporte parámetros de ruta opcionales y la UI generada de swagger por lo tanto tampoco
        public IActionResult Traer(TareasService tareasService, int? idTarea = null)
        {
            throw new Exception("sss");
            if (idTarea.HasValue)
                return Ok(tareasService.Traer(idTarea.Value));
            else
                return Ok(tareasService.TraerTodas());
        }

        /// <summary>
        /// Crea una tarea
        /// </summary>
        /// <param name="tarea">Datos de la tarea a crear</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrearTarea(
            [FromBody] TareaViewModel tarea,
            TareasService tareasService)
        {
            int idTarea = tareasService.CrearTarea(
                titulo: tarea.Titulo!,
                descripcion: tarea.Descripcion!
            );

            return Ok($"IdTarea: {idTarea}");
        }

        /// <summary>
        /// Actualiza una tarea
        /// </summary>
        /// <param name="idTarea">ID de la tarea</param>
        /// <param name="tarea">Datos de la tarea</param>
        /// <returns></returns>
        [HttpPut("{idTarea:int:min(1)}")]
        public IActionResult ActualizarTarea(
            int idTarea,
            [FromBody] TareaEdicionViewModel tarea,
            TareasService tareasService)
        {
            tareasService.ActualizarTarea(
                idTarea: idTarea,
                titulo: tarea.Titulo!,
                descripcion: tarea.Descripcion!,
                estado: tarea.Estado!
            );

            return Ok();
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="idTarea">ID de la tarea</param>
        /// <returns></returns>
        /// <response code="200">La tarea se eliminó exitosamente</response>
        /// <response code="404">No se encontró una tarea con el ID especificado</response>
        [HttpDelete("{idTarea:int:min(1)}")]
        public IActionResult EliminarTarea(int idTarea,
            TareasService tareasService)
        {
            new TareasService().EliminarTarea(idTarea: idTarea);
            return Ok();
        }

    //    [ApiExplorerSettings(IgnoreApi = true)]
    //    [Route("/error-development")]
    //    public IActionResult HandleErrorDevelopment(
    //[FromServices] IHostEnvironment hostEnvironment)
    //    {
    //        if (!hostEnvironment.IsDevelopment())
    //        {
    //            return NotFound();
    //        }

    //        var exceptionHandlerFeature =
    //            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

    //        return Problem(
    //            detail: exceptionHandlerFeature.Error.StackTrace,
    //            title: exceptionHandlerFeature.Error.Message);
    //    }

    //    [ApiExplorerSettings(IgnoreApi = true)]
    //    [Route("/error")]
    //    public IActionResult HandleError() =>
    //        Problem();
    }

    public class TareaViewModel
    {
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }

    }

    public class TareaEdicionViewModel : TareaViewModel
    {
        public string? Estado { get; set; }
    }

    public class TareaViewModelValidator : AbstractValidator<TareaViewModel>
    {
        public TareaViewModelValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descripcion).MaximumLength(500);
        }
    }

    public class TareaEdicionViewModelValidator : AbstractValidator<TareaEdicionViewModel>
    {
        public TareaEdicionViewModelValidator()
        {
            Include(new TareaViewModelValidator());
            RuleFor(x => x.Estado)
               .Must(estado => TareasService.EstadosTarea.ObtenerEstados().Contains(estado))
               .WithMessage($"El estado sólo puede ser uno de los siguientes valores: {string.Join(", ", TareasService.EstadosTarea.ObtenerEstados())}");
        }
    }
}
