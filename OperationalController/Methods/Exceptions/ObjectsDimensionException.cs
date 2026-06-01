using System;

namespace OperationalController.Methods.Exceptions
{
    [Serializable]
    internal class ObjectsDimensionException : Exception
    {
        public ObjectsDimensionException()
        {
        }

        public ObjectsDimensionException(string message) : base(message)
        {
        }

        public ObjectsDimensionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}