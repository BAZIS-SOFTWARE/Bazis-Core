using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ILoader
    {
        IModelData Load(string path);

        event Action<object,ILoaderEventArgs> LoadEvent;
    }
}
