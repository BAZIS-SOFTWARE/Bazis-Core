using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDB.FunctionData.Exceptions
{
    public class FunctionDBFileNotFoundException : Exception
    {
        public FunctionDBFileNotFoundException() { }
        public FunctionDBFileNotFoundException(string message) : base(message) { }
        public FunctionDBFileNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
