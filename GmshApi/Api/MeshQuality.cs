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
            throw new NotImplementedException();
        }

        public double[] GetInvertedGradientError()
        {
            throw new NotImplementedException();
        }

        public double[] GetMinJacobianValues()
        {
            return GmshWrapperMesh.GetElementQualities("minDetJac");
        }
    }
}
