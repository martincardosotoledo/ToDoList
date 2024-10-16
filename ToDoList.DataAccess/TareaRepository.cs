using NHibernate;
using System.Net.Http.Headers;
using ToDoList.Dominio;

namespace ToDoList.DataAccess
{
    public class TareaRepository
    {
        private ISession session = null;

        public TareaRepository(ISession session)
        {
            this.session = session;
        }

        public IList<Tarea> TraerTodas()
        {
            return session.QueryOver<Tarea>()
                          .List();
        }

        public void Guardar(Tarea tarea)
        {
            session.SaveOrUpdate(tarea);
        }

        public Tarea Traer(int idTarea)
        {
            return session.Get<Tarea>(idTarea);
        }
    }
}
