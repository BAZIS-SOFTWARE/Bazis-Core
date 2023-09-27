using GmshApi.GmshController;

namespace GmshApi.Api
{
    internal class MeshQuality : IMeshQuality
    {
        public double[] GetInvertedConditionNumber()
        {
            return ControllerMesh.GetElementQualities("minSICN");
        }

        public double[] GetInvertedGradientError()
        {
            return ControllerMesh.GetElementQualities("minSIGE");
        }

        //public double[] GetMinJacobianValues()
        //{
        //    return GmshWrapperMesh.GetElementQualities("minDetJac");
        //}
    }
}