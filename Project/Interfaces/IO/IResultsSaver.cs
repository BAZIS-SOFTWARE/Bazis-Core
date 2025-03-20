using Project.Results;
using System;
using System.Collections.Generic;

namespace Project.Interfaces.IO
{
    public interface IResultsSaver
    {
        event Action<object, ISaveEventArgs> SaveEvent;
        void Save(List<Result> results, string path, bool append);
    }
}
