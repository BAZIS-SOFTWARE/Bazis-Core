using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ISaver
    {
        void Save(ModelData model, string path);

        event Action<object, SaverEventArgs> SaveEvent;
    }
}
