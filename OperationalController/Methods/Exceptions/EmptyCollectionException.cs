using System;

namespace OperationalController.Methods.Exceptions
{
    [Serializable]
    internal class EmptyCollectionException : Exception
    {
        public EmptyCollectionException()
        {
        }

        public EmptyCollectionException(string message) : base(message)
        {
        }

        public EmptyCollectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}