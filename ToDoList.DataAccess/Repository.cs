using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Comun.Excepciones;
using ToDoList.Dominio;

namespace ToDoList.DataAccess
{
    public abstract class Repository<T> where T : Entidad
    {
        protected ISession Session { get { return SessionManager.Instance.GetCurrentSession(); } }

        public IList<T> TraerTodas()
        {
            return Session.QueryOver<T>()
                          .List();
        }

        public void Guardar(T entidad)
        {
            Session.SaveOrUpdate(entidad);
        }

        public void Eliminar(int id)
        {
            try
            {
                Session.Delete(Session.Load<T>(id));
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                throw new ToDoException(string.Empty, ex, ToDoList.Comun.Excepciones.ErrorType.NotFound);
            }
        }

        public T Traer(int id)
        {
            try
            {
                return Session.Get<T>(id);
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                throw new ToDoException(string.Empty, ex, ToDoList.Comun.Excepciones.ErrorType.NotFound);
            }
        }

        public T Cargar(int id)
        {
            return Session.Load<T>(id);
        }
    }
}
