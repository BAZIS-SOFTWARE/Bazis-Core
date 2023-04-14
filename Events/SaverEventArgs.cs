using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class SaverEventArgs : EventArgs
    {
        public SaverEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
