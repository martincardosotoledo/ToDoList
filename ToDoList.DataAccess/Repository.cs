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
            Session.Delete(Session.Load<T>(id));
        }

        public T Traer(int id)
        {
            T entidad = Session.Get<T>(id);
            
            if (entidad == null)
                throw new NHibernate.ObjectNotFoundException(id, nameof(T));

            return entidad;
        }

        public T Cargar(int id)
        {
            return Session.Load<T>(id);
        }
    }
}
