using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Aplicacion;
using ToDoList.Dominio;

namespace ToDoList.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TareasController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<TareaDTO> TraerTodas()
        {
            return new TareasService().TraerTodas();
        }

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
