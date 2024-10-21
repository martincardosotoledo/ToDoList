using NHibernate;
using NHibernate.Context;
using System.Runtime.CompilerServices;
using ToDoList.DataAccess;

namespace ToDoList.WebAPI.Middleware
{
    public class NHibernateSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public NHibernateSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            NHibernate.ISession? session = null;
            
            try
            {
                await _next(context);

                session = NHibernate.Context.CurrentSessionContext.HasBind(SessionManager.Instance.GetFactory()) ?
                          SessionManager.Instance.GetFactory().GetCurrentSession() : 
                          null;
            }
            catch
            {
                if (session != null && session.GetCurrentTransaction() != null && session.GetCurrentTransaction().IsActive)
                {
                    await session.GetCurrentTransaction().RollbackAsync();
                }

                throw;
            }
            finally
            {
                if (session != null) { 
                    NHibernate.Context.CurrentSessionContext.Unbind(session.SessionFactory);
                    session.Close();
                }
            }
        }
    }
}
