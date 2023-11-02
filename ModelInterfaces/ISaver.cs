using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ISaver
    {
        void Save(IModelData model, string path);

        event Action<object, ISaverEventArgs> SaveEvent;
    }
}
