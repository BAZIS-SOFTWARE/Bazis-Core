using System;

namespace OperationalController.MeshObjsCreator.Exceptions
{
    [Serializable]
    public class CollectionEmptyArgumentException : Exception
    {
        public CollectionEmptyArgumentException()
        {
        }

        public CollectionEmptyArgumentException(string message) : base(message)
        {
        }

        public CollectionEmptyArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}