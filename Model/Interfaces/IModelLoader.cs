using Model;
using Model.Events;
using System;

namespace Model.Interfaces
{
    public interface IModelLoader
    {
        ModelData Load(string path);

        event Action<object, LoaderEventArgs> LoadEvent;
    }
}
