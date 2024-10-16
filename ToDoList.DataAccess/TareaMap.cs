using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Dominio;

namespace ToDoList.DataAccess
{
    public class TareaMap : ClassMap<Tarea>
    {
        public TareaMap()
        {
            Table("Tarea");
            Id(x => x.ID, "ID").GeneratedBy.Identity();
            Map(x => x.Titulo, "Titulo");
            Map(x => x.Descripcion, "Descripcion");
            Map(x => x.Estado, "Estado");
            Map(x => x.FechaCreacion, "FechaCreacion").Not.Update();
        }
    }
}
