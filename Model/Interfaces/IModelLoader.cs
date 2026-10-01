using System;

namespace Model.Interfaces
{
    public interface IModelLoader
    {
        IModelData Load(string path, IProgress<int> progress = null);

        event Action<object, ILoaderEventArgs> LoadEvent;
    }
}
