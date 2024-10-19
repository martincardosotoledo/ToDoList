using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Dominio
{
    public abstract class Entidad
    {
        public virtual int ID { get; protected set; }
    }
}
