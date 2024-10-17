using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Reflection;
using System.Xml.Serialization;
using ToDoList.Aplicacion;
using ToDoList.DataAccess;
using ToDoList.Dominio;

namespace ToDoList.UnitTest
{
    [TestFixture]
    public class TareaTest
    {
        protected ISession session;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SessionManager.Instance.BuildSessionFactories("");
        }

        [SetUp]
        public void SetUp()
        {
            session = SessionManager.Instance.GetFactory().OpenSession();

            new SchemaExport(SessionManager.NhibernateConfig).Execute(
                useStdOut: true,
                execute: true,
                justDrop: false,
                connection: session.Connection,
                exportOutput: Console.Out);

            NHibernate.Context.CurrentSessionContext.Bind(session);

            DummyDataHelper.CrearDummyData();
        }

        [TearDown]
        public void TearDown()
        {
            if (session != null)
            {
                session.Dispose();
                NHibernate.Context.CurrentSessionContext.Unbind(SessionManager.Instance.GetFactory());
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            SessionManager.Instance.GetFactory().Dispose();
        }


        [Test]
        public void Tarea_no_se_actualiza_con_estado_invalido()
        {
            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new TareasService().ActualizarTarea(
                        idTarea: 1,
                        titulo: "",
                        descripcion: "",
                        estado: "estado invalido");
                });
            }
        }

        [Test]
        public void Tarea_titulo_no_supera_longitud_maxima()
        {
            using (ITransaction tran = SessionManager.Instance.GetCurrentSession().BeginTransaction())
            {
                var ex = Assert.Throws<FluentValidation.ValidationException>(() =>
                {
                    new TareasService().ActualizarTarea(
                        idTarea: 1,
                        titulo: new string('x', 120),
                        descripcion: "",
                        estado: "pendiente");
                });
            }
        }
    }
}
