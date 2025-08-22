using System;
using System.Collections.Generic;

namespace ResultDB.Interfaces
{
    public interface IResultsSaver
    {
        event Action<object, string> SaveEvent;
        void Save(List<Result> results, string path, bool append);
    }
}
