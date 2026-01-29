using Model.Interfaces.MeshObjects;
using Model.Interfaces;
using OperationalController;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsUtility;
using OperationalController.ObjsCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.MeshObjects;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        IBoundaryEdgesFinder boundaryEdgesFinder { get; set; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes)
        {
            return boundaryEdgesFinder.CreateBoundaryEdges(linesNodes, ModelData);
        }
        /// <inheritdoc/>
        public List<string> FindBoundaryEdges()
        {
            if (ModelData.ObjectData.GetAllElements().Count() == 0)
                throw new Exception("Модель не содержит элементов");

            return boundaryEdgesFinder.Find(ModelData.ObjectData.GetAllElements().
                Where(x => x is ISurfaceElement).
            Select(x => (ISurfaceElement)x));
        }
    }
}
