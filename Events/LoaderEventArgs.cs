using System;

namespace Model.Events
{
    public class LoaderEventArgs : EventArgs
    {
        public LoaderEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}