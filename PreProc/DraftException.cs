using System;

namespace PreProc
{
    [Serializable]
    public class DraftException : Exception
    {
        public DraftException()
        {
        }

        public DraftException(string message) : base(message)
        {
        }

        public DraftException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}