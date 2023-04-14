using Model;
using ProjectPageInterface;
using System.Collections.Generic;

namespace ModelController
{
    public class ModelTreePresentator : IModelTreePresentor
    {
        ModelData modelData;
        public ModelTreePresentator(ModelData modelData)
        {
            this.modelData = modelData;
        }

        public IEnumerable<KeyValuePair<string,List<string>>> GetObjectsInfo()
        {
            foreach (var objType in modelData.ObjectData.GetObjectTypes())
            {
                var objKinds = new List<string>();
                foreach (var obj in modelData.ObjectData.FindMany(objType))
                {
                    objKinds.Add(obj.ObjKind.ToString());
                }
                yield return new KeyValuePair<string, List<string>>(objType, objKinds);
            }
        }
    

        public IEnumerable<KeyValuePair<string, string>> GetGroupsInfo()
        {
            foreach (var group in modelData.GroupData)
            {
                    yield return new KeyValuePair<string, string>(group.GroupName, group.ObjType);
            }
        }

    }
}
