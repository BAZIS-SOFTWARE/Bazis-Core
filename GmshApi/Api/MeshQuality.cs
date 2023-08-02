using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmshApi.Api
{
    internal class MeshQuality : IMeshQuality
    {
        public double[] GetInvertedConditionNumber()
        {
            return GmshWrapperMesh.GetElementQualities("minSICN");
        }

        public double[] GetInvertedGradientError()
        {
            return GmshWrapperMesh.GetElementQualities("minSIGE");
        }

        //public double[] GetMinJacobianValues()
        //{
        //    return GmshWrapperMesh.GetElementQualities("maxDetJac");
        //}
    }
}