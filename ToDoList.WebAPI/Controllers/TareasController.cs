using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Aplicacion;
using ToDoList.Comun.Excepciones;
using ToDoList.Dominio;

namespace ToDoList.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TareasController : ControllerBase
    {
        /// <summary>
        /// Devuelve todas las tareas
        /// </summary>
        /// <returns>Lista de tareas</returns>
        [HttpGet]
        public IEnumerable<TareaDTO> TraerTodas()
        {
            return new TareasService().TraerTodas();
        }

        /// <summary>
        /// Crea una tarea
        /// </summary>
        /// <param name="tarea">Datos de la tarea a crear</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrearTarea([FromBody] TareaViewModel tarea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            new TareasService().CrearTarea(
                titulo: tarea.Titulo,
                descripcion: tarea.Descripcion
            );

            return Ok();
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
            [FromBody] TareaEdicionViewModel tarea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            new TareasService().ActualizarTarea(
                idTarea: idTarea,
                titulo: tarea.Titulo,
                descripcion: tarea.Descripcion,
                estado: tarea.Estado.ToLower()
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
        public IActionResult EliminarTarea(int idTarea)
        {
            try
            {
                new TareasService().EliminarTarea(idTarea: idTarea);
            }
            catch (ToDoException ex)
            {
                if (ex.ErrorType == ErrorType.NotFound)
                    return NotFound();
                else
                    return StatusCode(500);
            }

            return Ok();
        }
    }

    public class TareaViewModel
    {
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

    }

    public class TareaEdicionViewModel : TareaViewModel
    {
        public string Estado { get; set; }
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
