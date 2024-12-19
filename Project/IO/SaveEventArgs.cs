using Project.Interfaces.IO;

namespace Project.IO
{
    public class SaveEventArgs : ISaveEventArgs
    {
        public SaveEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
