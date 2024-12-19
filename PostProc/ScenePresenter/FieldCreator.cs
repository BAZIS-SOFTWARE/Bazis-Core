using Model;
using ProjectInterfaces;
using Results.ScenePresenter.Interfaces;
using System.Collections.Generic;

namespace Results.ScenePresenter
{
    public class FieldCreator
    {
        IResultFieldCreator resultFieldCreator;

        public IEnumerable<ModelObject> CreateFieldObjects(IResult result, string objsType, string resDescr)
        {
            return resultFieldCreator.CreateFieldObjects(result, objsType, resDescr);
        }

        public void SetFieldCreator(IResultFieldCreator resultFieldCreator)
        {
            this.resultFieldCreator = resultFieldCreator;
        }
    }
}
