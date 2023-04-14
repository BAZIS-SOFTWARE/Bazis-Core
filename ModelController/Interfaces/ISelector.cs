using Model;
using Model.Interfaces;
using System.Collections.Generic;

namespace ModelController.Interfaces
{
    public interface ISelector
    {
        IEnumerable<IModelObject> GetSelectedObjs();
    }
}
