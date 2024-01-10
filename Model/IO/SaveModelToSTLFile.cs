using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.IO
{
    public class SaveModelToSTLFile : IModelSaver
    {
        public event Action<object, ISaverEventArgs> SaveEvent;

        public void Save(IModelData model, string path)
        {
            throw new NotImplementedException();
        }
    }
}
