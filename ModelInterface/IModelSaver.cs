using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterface
{
    public interface IModelSaver
    {
        void Save(ModelData model, string path);

        event Action<object, SaverEventArgs> SaveEvent;
    }
}
