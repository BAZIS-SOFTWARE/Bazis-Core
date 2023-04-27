using Model;
using System.Collections.Generic;

namespace ModelController
{
    public class ModelSelectToolStripPresentator
    {
        ModelData modelData;
        public ModelSelectToolStripPresentator(ModelData modelData)
        {
            this.modelData = modelData;
        }

        public IEnumerable<string> GetObjectsInfo()
        {
            var objNames = modelData.ObjectData.GetObjectTypes();
            foreach (var objName in objNames)
            {
                yield return objName;
            }
        }
    }
}
