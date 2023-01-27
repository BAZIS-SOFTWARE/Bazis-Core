using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ILoader
    {
        ModelData Load(string path);

        event Action<object,LoaderEventArgs> LoadEvent;
    }
}
