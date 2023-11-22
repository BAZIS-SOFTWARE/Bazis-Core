using ModelInterfaces;

namespace Model.Events
{
    public class LoaderEventArgs : ILoaderEventArgs
    {
        public LoaderEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}