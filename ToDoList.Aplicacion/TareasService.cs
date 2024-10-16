using FluentValidation;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using ToDoList.DataAccess;
using ToDoList.Dominio;

namespace ToDoList.Aplicacion
{
    public class TareasService
    {
        public IList<TareaDTO> TraerTodas()
        {
            IList<TareaDTO> tareaDTOs = null;

            using (ISession session = SessionManager.Instance.GetFactory().OpenSession())
            {
                var repo = new TareaRepository(session);

                tareaDTOs = (from t in repo.TraerTodas()
                             select new TareaDTO { 
                                 ID = t.ID,
                                 Titulo = t.Titulo,
                                 Descripcion = t.Descripcion,
                                 FechaCreacion = t.FechaCreacion,   
                                 Estado = Enum.GetName<EstadoTarea>(t.Estado)
                             }).ToList();
            }

            return tareaDTOs;
        }

        public void CrearTarea(string titulo, string descripcion)
        {
            using (ISession session = SessionManager.Instance.GetFactory().OpenSession())
            {
                var repo = new TareaRepository(session);

                using (ITransaction tran = session.BeginTransaction())
                {
                    Tarea tarea = Tarea.Crear(titulo: titulo, descripcion: descripcion);

                    new TareaValidator().ValidateAndThrow(tarea);

                    repo.Guardar(tarea);

                    tran.Commit();
                }
            }
        }


        public void ActualizarTarea(int idTarea, string titulo, string descripcion, string estado)
        {
            using (ISession session = SessionManager.Instance.GetFactory().OpenSession())
            {
                var repo = new TareaRepository(session);

                using (ITransaction tran = session.BeginTransaction())
                {
                    Tarea tarea = repo.Traer(idTarea);

                    tarea.Titulo = titulo;
                    tarea.Descripcion = descripcion;
                    tarea.Estado = ConvertirEstadoTareaDesdeDescripcion(estado);

                    new TareaValidator().ValidateAndThrow(tarea);

                    repo.Guardar(tarea);

                    tran.Commit();
                }
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

            using (ISession session = SessionManager.Instance.GetFactory().OpenSession())
            {
                var repo = new TareaRepository(session);

                using (ITransaction tran = session.BeginTransaction())
                {
                    foreach (var tarea in tareas)
                        session.Save(tarea);
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
