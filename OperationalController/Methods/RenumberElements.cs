using Model.Interfaces.ObjectsCollections;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void RenumberElements()
        {
            var number = 0;
            var changedSets = new List<ISetInfo>();
            foreach (var set in ModelData.ObjectData.E1DCollection.Objects)
                if (set.Renumber(ref number))
                    changedSets.Add(set);
            foreach (var set in ModelData.ObjectData.E2DCollection.Objects)
                if (set.Renumber(ref number))
                    changedSets.Add(set);
            foreach (var set in ModelData.ObjectData.E3DCollection.Objects)
                if (set.Renumber(ref number))
                    changedSets.Add(set);
            foreach (var set in changedSets)
                PublishSetChange(set, ChangeAction.Updated);
        }
    }
}
