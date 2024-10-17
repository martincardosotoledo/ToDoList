using FluentValidation;
using FluentValidation.Results;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using ToDoList.Comun.Excepciones;
using ToDoList.DataAccess;
using ToDoList.Dominio;

namespace ToDoList.Aplicacion
{
    public class TareasService
    {
        public IList<TareaDTO> TraerTodas()
        {
            return (from t in new TareaRepository().TraerTodas()
                    select new TareaDTO { 
                        ID = t.ID,
                        Titulo = t.Titulo,
                        Descripcion = t.Descripcion,
                        FechaCreacion = t.FechaCreacion,   
                        Estado = Enum.GetName<EstadoTarea>(t.Estado)
                    }).ToList();
        }

        public void CrearTarea(string titulo, string descripcion)
        {
            var repo = new TareaRepository();

            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                Tarea tarea = Tarea.Crear(titulo: titulo, descripcion: descripcion);

                new TareaValidator().ValidateAndThrow(tarea);

                repo.Guardar(tarea);

                tran.Commit();
            }
        }


        public void ActualizarTarea(int idTarea, string titulo, string descripcion, string estado)
        {
            var repo = new TareaRepository();

            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                Tarea tarea = repo.Cargar(idTarea);

                //if (tarea is null)
                //    return Result<int>.Failure("");

                tarea.Titulo = titulo;
                tarea.Descripcion = descripcion;
                tarea.Estado = ConvertirEstadoTareaDesdeDescripcion(estado);

                new TareaValidator().ValidateAndThrow(tarea);

                repo.Guardar(tarea);

                tran.Commit();
            }
        }

        public void EliminarTarea(int idTarea)
        {
            var repo = new TareaRepository();

            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                repo.Eliminar(idTarea);

                tran.Commit();
            }
        }

        private EstadoTarea ConvertirEstadoTareaDesdeDescripcion(string descripcion)
        {
            switch (descripcion)
            {
                case EstadosTarea.PENDIENTE:
                    return EstadoTarea.Pendiente;
                case EstadosTarea.EN_PROGRESO:
                    return EstadoTarea.EnProgreso;
                case EstadosTarea.COMPLETADA:
                    return EstadoTarea.Completada;
                default:
                    throw new ArgumentOutOfRangeException(nameof(descripcion));
            }
        }

        public static class EstadosTarea
        {
            public const string PENDIENTE = "pendiente";
            public const string EN_PROGRESO = "en progreso";
            public const string COMPLETADA = "completada";

            public static IEnumerable<string> ObtenerEstados()
            {
                yield return EstadosTarea.PENDIENTE;
                yield return EstadosTarea.EN_PROGRESO;
                yield return EstadosTarea.COMPLETADA;
            }
        }

        //public class Errores
        //{
        //    public static int NotFound;
        //}

    }


    public static class DummyDataHelper {
        public static void CrearDummyData()
        {
            var tareas = new List<Tarea>
            {
                Tarea.Crear("Comprar harina", "Que sea 0000"),
                Tarea.Crear("Pasear al perro", "2 veces por día, mañana y tarde"),
                Tarea.Crear("Pagar la luz", "Vence el 26")
            };

            //using (ISession session = SessionManager.Instance.GetCurrentSession())
            {
                var repo = new TareaRepository();

                using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
                {
                    foreach (var tarea in tareas)
                        SessionManager.Instance.GetCurrentSession().Save(tarea);

                    tran.Commit();
                }
            }

        }
    }

    public class TareaDTO
    {
        public int ID { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCreacion
        {
            get; set;
        }
    }
}
