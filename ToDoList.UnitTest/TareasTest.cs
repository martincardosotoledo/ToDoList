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
using ToDoList.Comun.Excepciones;
using ToDoList.DataAccess;
using ToDoList.Dominio;

namespace ToDoList.UnitTest
{
    [TestFixture]
    public class TareasTest
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
        public void Aplicacion_Tarea_no_se_actualiza_con_estado_invalido()
        {
            var tareaService = new TareasService();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var tareaDTO = tareaService.Traer(1);

                tareaService.ActualizarTarea(
                    idTarea: tareaDTO.ID,
                    titulo: tareaDTO.Titulo,
                    descripcion: tareaDTO.Descripcion,
                    estado: "estado invalido");
            });
        }

        [Test]
        public void Aplicacion_Tarea_titulo_no_supera_longitud_maxima()
        {
            var tareaService = new TareasService();

            var ex = Assert.Throws<ToDoException>(() =>
            {
                var tareaDTO = tareaService.Traer(1);

                new TareasService().ActualizarTarea(
                    idTarea: tareaDTO.ID,
                    titulo: new string('x', 120),
                    descripcion: tareaDTO.Descripcion,
                    estado: "pendiente");
            });
        }

        [Test]
        public void Dominio_Tarea_no_se_actualiza_con_estado_invalido()
        {
            var ex = Assert.Throws<ToDoException>(() =>
            {
                var tarea = new TareaRepository().Traer(1);

                tarea.Estado = (EstadoTarea)424;
            });
        }
    }
}
