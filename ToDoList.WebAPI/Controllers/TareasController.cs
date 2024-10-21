using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ToDoList.Aplicacion;
using ToDoList.Comun.Excepciones;
using ToDoList.Dominio;

namespace ToDoList.WebAPI.Controllers
{
    [Authorize(Policy = "TodoCrud")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        /// <summary>
        /// Devuelve las tareas
        /// </summary>
        /// <returns>Lista de todas las tareas</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<TareaDTO>), StatusCodes.Status200OK)]
        public IActionResult Traer(TareasService tareasService)
        {
            IEnumerable<TareaDTO> tareas = tareasService.TraerTodas();
            return Ok(tareas);
        }

        /// <summary>
        /// Devuelve una tarea
        /// </summary>
        /// <returns>La tarea solicitada</returns>
        [HttpGet("{idTarea:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TareaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Traer(TareasService tareasService, int idTarea)
        {
            TareaDTO tarea = tareasService.Traer(idTarea);
            return Ok(tarea);
        }

        /// <summary>
        /// Crea una tarea
        /// </summary>
        /// <param name="tarea">Datos de la tarea a crear</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CrearTarea(
            [FromBody] TareaViewModel tarea,
            TareasService tareasService)
        {
            int idTarea = tareasService.CrearTarea(
                titulo: tarea.Titulo!,
                descripcion: tarea.Descripcion!
            );

            return CreatedAtAction(nameof(Traer), new { idTarea = idTarea}, null);
        }   

        /// <summary>
        /// Actualiza una tarea
        /// </summary>
        /// <param name="idTarea">ID de la tarea</param>
        /// <param name="tarea">Datos de la tarea</param>
        /// <response code="204">La tarea se acualizó exitosamente</response>
        /// <response code="404">No se encontró una tarea con el ID especificado</response>
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

            return NoContent();
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="idTarea">ID de la tarea</param>
        /// <returns></returns>
        /// <response code="204">La tarea se eliminó exitosamente</response>
        /// <response code="404">No se encontró una tarea con el ID especificado</response>
        [HttpDelete("{idTarea:int:min(1)}")]
        public IActionResult EliminarTarea(int idTarea,
            TareasService tareasService)
        {
            tareasService.EliminarTarea(idTarea: idTarea);
            return NoContent();
        }
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
