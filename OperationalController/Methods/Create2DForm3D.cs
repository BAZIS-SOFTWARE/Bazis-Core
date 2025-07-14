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

        public void Create2DForm3D(string setName)
        {
            //return extractor2DFrom3D.Create(StartNumber, ModelData.ObjectData.E3DCollection.GetObjects());
            var number = ModelData.ObjectData.E2DCollection.GetMaxNumber() + 1;
            var e2d = extractor2DFrom3D.Create(number, ModelData.ObjectData.E3DCollection.GetObjects().ToArray());
            ModelData.ObjectData.E2DCollection.AddRange(setName, e2d);
        }
    }
}
