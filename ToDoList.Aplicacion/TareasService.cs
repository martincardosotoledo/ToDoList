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
    public class TareasService : ServiceBase
    {
        public IList<TareaDTO> TraerTodas()
        {
            return (from t in new TareaRepository().TraerTodas()
                    select TareaDtoMapper.From(t)).ToList();
        }

        public TareaDTO Traer(int idTarea)
        {
            var tarea = new TareaRepository().Traer(idTarea);

            return TareaDtoMapper.From(tarea);
        }

        public int CrearTarea(string titulo, string descripcion)
        {
            return UnitOfWork(() =>
            {
                Tarea tarea = Tarea.Crear(titulo: titulo, descripcion: descripcion);

                new TareaRepository().Guardar(tarea); // mmm, cómo es que la tarea tiene un valor asignado aquí si se supone que es la base de datos la que debe asignarlo?

                return tarea.ID;
            });
        }


        public void ActualizarTarea(int idTarea, string titulo, string descripcion, string estado)
        {
            var repo = new TareaRepository();

            UnitOfWork(() =>
            {
                Tarea tarea = repo.Traer(idTarea);

                tarea.Titulo = titulo;
                tarea.Descripcion = descripcion;
                tarea.Estado = ConvertirEstadoTareaDesdeDescripcion(estado);

                repo.Guardar(tarea);
            });
        }

        public void EliminarTarea(int idTarea)
        {
            UnitOfWork(() =>
            {
                new TareaRepository().Eliminar(idTarea);
            });
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

        internal static class TareaDtoMapper
        {
            public static TareaDTO From(Tarea tarea)
            {
                return new TareaDTO
                {
                    ID = tarea.ID,
                    Titulo = tarea.Titulo,
                    Descripcion = tarea.Descripcion,
                    FechaCreacion = tarea.FechaCreacion,
                    Estado = Enum.GetName<EstadoTarea>(tarea.Estado)!
                };
            }
        }
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
