using Model;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.IO
{
    public class LoadModelFromCDBTextFile : IModelLoader
    {
        public event Action<object, LoaderEventArgs> LoadEvent;

        public ModelData Load(string path)
        {
            // для Виктора. Реализовать метод загрузки. Проверяет Антон
            throw new NotImplementedException();
        }
    }
}
