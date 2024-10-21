using FluentValidation;
using ToDoList.Comun.Excepciones;
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
            tarea.Descripcion = descripcion;
            tarea.Estado = EstadoTarea.Pendiente;
            tarea.FechaCreacion = DateTime.Now;
            return tarea;
        }

        protected Tarea() { }

        private string _titulo;
        private string _descripcion;
        private EstadoTarea _estado;

        public virtual string Titulo
        {
            get => _titulo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw ToDoException.ValidationError($"{nameof(Titulo)} no puede ser nulo o estar vacío");

                const int MAX_LENGTH = 100;
                if (value.Length > MAX_LENGTH)
                    throw ToDoException.ValidationError($"{nameof(Titulo)} no puede tener más de {MAX_LENGTH} caracteres");

                _titulo = value;
            }
        }

        public virtual string Descripcion
        {
            get => _descripcion; 
            set {
                const int MAX_LENGTH = 500;
                if (value.Length > MAX_LENGTH)
                    throw ToDoException.ValidationError($"{nameof(Descripcion)} no puede tener más de {MAX_LENGTH} caracteres");

                _descripcion = value;
            }
        }

        public virtual EstadoTarea Estado { 
            get => _estado; 
            set { 
                if (!Enum.IsDefined<EstadoTarea>(value))
                    throw ToDoException.ValidationError($"{value} no es un estado válido");

                _estado = value; 
            } 
        }

        public virtual DateTime FechaCreacion { get; protected set; }

    }

    public enum EstadoTarea
    {
        Pendiente,
        EnProgreso,
        Completada
    }
}
