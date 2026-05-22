using System;

namespace OperationalController.MeshObjsCreator
{
    [Serializable]
    internal class NegativeRadiusArgumentException : Exception
    {
        public NegativeRadiusArgumentException()
        {
        }

        public NegativeRadiusArgumentException(string message) : base(message)
        {
        }

        public NegativeRadiusArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}