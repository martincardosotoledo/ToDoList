using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Comun.Excepciones
{
    public class ToDoException : ApplicationException
    {
        public ErrorType ErrorType { get; private set; }

        public ToDoException(string message, Exception innerException, ErrorType errorType = ErrorType.Error) : base(message, innerException)
        {
            this.ErrorType = errorType;
        }
    }

    public enum ErrorType
    {
        Error,
        NotFound
    }
}
