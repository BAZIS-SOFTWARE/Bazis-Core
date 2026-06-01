using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDB.MaterialData.Exceptions
{
    public class MaterialDBEmptyFileException : Exception
    {
        public MaterialDBEmptyFileException() { }
        public MaterialDBEmptyFileException(string message) : base(message) { }
        public MaterialDBEmptyFileException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
