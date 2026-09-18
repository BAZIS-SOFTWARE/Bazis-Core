using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDB.FunctionData.Exceptions
{
    public class FunctionDBEmptyFileException : Exception
    {
        public FunctionDBEmptyFileException() { }
        public FunctionDBEmptyFileException(string message) : base(message) { }
        public FunctionDBEmptyFileException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
