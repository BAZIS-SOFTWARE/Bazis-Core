using System;
using Model.GmshController;
using Model.GmshWrapper;
using PrFunctionLib;

namespace Model.GmshObject
{ 
    public class Mesh : Model, IMeshApi
    {
        public void ChangeTriangulationAlgorithm(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay)
        {
            GmshWrapperModel.ModelSetCurrent(Id.ToString());
            var algo = Convert.ToDouble(algorithm);
            GmshWrapperGeneral.OptionSetNumber("Mesh.Algorithm3D", algo);
        }

        public void ChangeTriangulationAlgorithm(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay)
        {
            GmshWrapperModel.ModelSetCurrent(Id.ToString());
            var algo = Convert.ToDouble(algorithm);
            GmshWrapperGeneral.OptionSetNumber("Mesh.Algorithm", algo);
        }
        //Последний параметр лучше сделать ID
        public void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, Curve[] curve)
        {
            GmshWrapperModel.ModelSetCurrent(Id.ToString());
            for (var i = 0; i < curve.Length; ++i)
            {
                switch (ta)
                {
                    case TransfiniteAlgorithm.Progressive:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Progression", parameter);
                        break;
                    case TransfiniteAlgorithm.Bump:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Bump", parameter);
                        break;
                    case TransfiniteAlgorithm.Beta:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Beta", parameter);
                        break;
                }
            }
        }
    }
}
