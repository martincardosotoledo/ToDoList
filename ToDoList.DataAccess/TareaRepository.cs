using NHibernate;
using System.Net.Http.Headers;
using ToDoList.Comun.Excepciones;
using ToDoList.Dominio;

namespace ToDoList.DataAccess
{
    public class TareaRepository
    {
        private ISession Session { get { return SessionManager.Instance.GetCurrentSession(); } }

        public object ErrorType { get; private set; }

        public IList<Tarea> TraerTodas()
        {
            return Session.QueryOver<Tarea>()
                          .List();
        }

        public void Guardar(Tarea tarea)
        {
            Session.SaveOrUpdate(tarea);
        }

        public void Eliminar(int id)
        {
            try
            {
                Session.Delete(Session.Load<Tarea>(id));
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                throw new ToDoException(string.Empty, ex, ToDoList.Comun.Excepciones.ErrorType.NotFound);
            }
        }

        public Tarea Traer(int idTarea)
        {
            return Session.Get<Tarea>(idTarea);
        }

        public Tarea Cargar(int idTarea)
        {
            return Session.Get<Tarea>(idTarea);
        }
    }
}
