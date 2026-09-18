using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDB.MaterialData.Exceptions
{
    public class MaterialDBFileNotFoundException : Exception
    {
        public MaterialDBFileNotFoundException() : base() { }
        public MaterialDBFileNotFoundException(string message) : base(message) { }
        public MaterialDBFileNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
