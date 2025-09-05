using OperationalController;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsUtility;
using OperationalController.ObjsCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        IExtractor2DFrom3D extractor2DFrom3D { get; } = new Extract2DFrom3D();
        /// <inheritdoc/>

        public bool Create2DForm3D()
        {
            if (ModelData.ObjectData.E3DCollection.Count == 0)
                return false;
            //return extractor2DFrom3D.Create(StartNumber, ModelData.ObjectData.E3DCollection.GetObjects());
            var coll = ModelData.ObjectData.E2DCollection;

            var number = coll.Count != 0 ? coll.GetMaxNumber() + 1 : 0;

            var name = $"boundary2DSet_{number}";


            var e2d = extractor2DFrom3D.Create(number, ModelData.ObjectData.E3DCollection.GetObjects().ToArray());
            ModelData.ObjectData.E2DCollection.AddRange(name, e2d);

            return true;
        }
    }
}
