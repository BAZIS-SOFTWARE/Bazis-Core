using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshWrapper;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperGeometryTests
    {
        [TestInitialize]
        public void Init()
        {
            GmshWrapperConnection.Initialize();
        }
        [TestCleanup]
        public void Cleanup()
        {
            GmshWrapperConnection.Finalize();
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new double[] { -14.2188, -2.7094, -13.2231 },
                                      new double[] { 10.6366, 19.599, 10.7593 })]
        [DataRow("Korpus_Nasosa.step", "2", new double[] { -88.2608, -0.679586, -61.2934 },
                                            new double[] { 88.2608, 273.279, 61.2934 })]
        [DataRow("model.igs", "3", new double[] { -1.72724e-05, -179.902, -0.0020751 },
                                   new double[] { 2591.2, 2540, 508 })]
        [DataRow("sphere.geo", "4", new double[] { -1, -1, -1 },
                                    new double[] { 1, 1, 1 })]
        public void ModelBoundingBoxTest(string fileName, string modelName, double[] minExp, double[] maxExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            double[] min, max;
            GmshWrapperGeometry.GetBoundingBox(-1, -1, out min, out max);
            for(var i = 0; i < 3; ++i)
                Assert.AreEqual(minExp[i], min[i], 1e-02);
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(maxExp[i], max[i], 1e-02);
        }
    }
}
