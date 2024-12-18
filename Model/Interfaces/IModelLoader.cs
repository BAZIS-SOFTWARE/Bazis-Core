using System;

namespace Model.Interfaces
{
    public interface IModelLoader
    {
        IModelData Load(string path);

        event Action<object, ILoaderEventArgs> LoadEvent;
    }
}
