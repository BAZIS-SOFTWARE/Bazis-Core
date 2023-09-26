namespace GmshApi.Api
{
    public interface IMeshQuality
    {
        //double[] GetMinJacobianValues();

        double[] GetInvertedGradientError();

        double[] GetInvertedConditionNumber();
    }
}
