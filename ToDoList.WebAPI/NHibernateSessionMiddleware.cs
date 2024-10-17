using NHibernate;
using ToDoList.DataAccess;

namespace ToDoList.WebAPI
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
            var session = SessionManager.Instance.GetFactory().OpenSession();

            try
            {
                NHibernate.Context.CurrentSessionContext.Bind(session);

                await _next(context); 

                if (session.GetCurrentTransaction() != null && session.GetCurrentTransaction().IsActive)
                {
                    await session.GetCurrentTransaction().CommitAsync();
                }
            }
            catch
            {
                if (session.GetCurrentTransaction() != null && session.GetCurrentTransaction().IsActive)
                {
                    await session.GetCurrentTransaction().RollbackAsync();
                }

                throw;
            }
            finally
            {
                // Hacer unbind y cerrar la sesión
                NHibernate.Context.CurrentSessionContext.Unbind(session.SessionFactory);
                session.Close();
            }
        }
    }
}
