using Geometry;
using Model.Interfaces;
using Project.Interfaces.Results;
using PostProc.ScenePresenter.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class LineGradientsCreator : IGradientsCreator<ILineObject<IPoint>>
    {
        //public List<List<List<Point3D>>> CalcGradients(ResultInfo resInfo, Tuple<float[], Point3D[]> elNdRes, Element element)
        //{
        //    var grads = new List<List<List<Point3D>>>();

        //    //var elRes = proj.ResultData.GetElementNodesResults(element, resInfo);         

        //    //var ndInd = new int[] { element.ObjPtr[0], element.ObjPtr[1] };
        //    //var ndRes = new float[] { elRes.Item1[ndInd[0]], elRes.Item1[ndInd[1]] };

        //    //var gradsOnSurface = proj.ResultData.FindSubSurfs(resInfo.ScaleInfo.ValueRange().ToArray(), element.GetNodes.Select(x => x.Position).ToArray(), ndRes);

        //    //grads.Add(gradsOnSurface);

        //    return grads;
        //}

        public IEnumerable<ILineObject<IPoint>> CreateGradientObjects(ItemRange[] itemRanges, Color[] colors, List<Point3D> nodesCoords, List<float> nodesResults)
        {
            throw new NotImplementedException();
        }
    }
}
