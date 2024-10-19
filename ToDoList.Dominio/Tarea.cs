using FluentValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoList.Dominio
{
    public class Tarea : Entidad
    {
        public static Tarea Crear(
            string titulo,
            string descripcion)
        { 
            Tarea tarea = new Tarea();
            tarea.Titulo = titulo;
            tarea.Descripcion  = descripcion;
            tarea.Estado = EstadoTarea.Pendiente;
            tarea.FechaCreacion = DateTime.Now;
            return tarea;
        }

        protected Tarea() { }

        public virtual string Titulo { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual EstadoTarea Estado { get; set; }

        public virtual DateTime FechaCreacion { get; protected set; }

    }

    public enum EstadoTarea
    {
        Pendiente,
        EnProgreso,
        Completada
    }

    public class TareaValidator : AbstractValidator<Tarea>
    {
        public TareaValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descripcion).MaximumLength(500);
        }
    }
}
