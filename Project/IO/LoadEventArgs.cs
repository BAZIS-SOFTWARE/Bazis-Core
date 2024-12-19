using Project.Interfaces.IO;

namespace Project.IO
{
    internal class LoadEventArgs : ILoadEventArgs
    {
        public string Message { get; }

        public LoadEventArgs(string message)
        {
            Message = message;
        }
    }
}