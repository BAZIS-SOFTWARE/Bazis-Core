using System;

namespace ModelController.IO
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