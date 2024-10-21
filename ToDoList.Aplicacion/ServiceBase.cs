using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DataAccess;
using ToDoList.Dominio;

namespace ToDoList.Aplicacion
{
    public abstract class ServiceBase
    {
        protected T UnitOfWork<T>(Func<T> func)
        {
            T result;

            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                result = func.Invoke();

                tran.Commit();
            }

            return result;
        }

        protected void UnitOfWork(Action action)
        {
            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                action.Invoke();

                tran.Commit();
            }
        }
    }
}
