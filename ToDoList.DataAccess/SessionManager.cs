using FluentNHibernate.Cfg;
using NHibernate.Context;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Tool.hbm2ddl;
using static System.Collections.Specialized.BitVector32;
using NHibernate.Cfg;

namespace ToDoList.DataAccess
{
    public class SessionManager
    {
        private static ISessionFactory _sessionFactory = null;
        private static Configuration _nhibernateConfig = null;

        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static SessionManager Instance
        {
            [DebuggerStepThrough]
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        public static Configuration NhibernateConfig { get; private set; }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        static SessionManager()
        {
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly SessionManager NHibernateSessionManager =
                new SessionManager();
        }

        #endregion

        private static object lockObject = new object();

        public void BuildSessionFactories(string appBasePath)
        {
            lock (lockObject)
            {
                if (_sessionFactory != null)
                    throw new Exception("Las session factories ya estaban inicializadas");

                NhibernateConfig = new NHibernate.Cfg.Configuration();
                NhibernateConfig.Configure(Path.Combine(appBasePath, @"nhibernate.config"));

                _sessionFactory = Fluently.Configure(NhibernateConfig)
                                          .Mappings(m =>
                                          {
                                              m.FluentMappings.AddFromAssemblyOf<SessionManager>();
                                          })
                                          .BuildSessionFactory();
            }
        }

        public ITransaction BeginTransaction(ISession session)
        {
            ITransaction transaction = session.Transaction;

            if (transaction == null)
            {
                transaction = session.BeginTransaction();
            }

            if (!transaction.IsActive)
                transaction.Begin();

            return transaction;
        }

        public void CommitTransaction(ITransaction transaction)
        {
            if (IsOpenTransaction(transaction))
            {
                transaction.Commit();
            }
        }



        private bool IsOpenTransaction(ITransaction transaction)
        {
            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public bool HasOpenTransaction(ISession session)
        {
            return IsOpenTransaction(session.Transaction);
        }

        /// <summary>
        /// Retorna la SessionFactory por default
        /// </summary>
        public ISessionFactory GetFactory()
        {
            return _sessionFactory;
        }

        public ISession GetCurrentSession()
        {
            if (!CurrentSessionContext.HasBind(GetFactory()))
            {
                var session = GetFactory().OpenSession();
                CurrentSessionContext.Bind(session);
            }

            return GetFactory().GetCurrentSession();
        }
    }
}
