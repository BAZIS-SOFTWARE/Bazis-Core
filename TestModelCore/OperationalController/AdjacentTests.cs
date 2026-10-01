using Model.Interfaces;
using OperationalController;
using OperationalController.GmshController;
using System.Globalization;

namespace TestModeCore_test.OperationalController
{
    [TestClass]
    public class AdjacentTests
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", global::Model.Interfaces.ObjType.Поверхность,1,6,4)]
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", global::Model.Interfaces.ObjType.Кривая, 1, 2, 2)]
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", global::Model.Interfaces.ObjType.Точка, 1, 3, 0)]
        //[DataRow(@"..\..\..\Examples\CAD\Tavr.step", global::Model.Interfaces.ObjType.Узел, 1, 0, 0)]
        public void GetAdjacent(string cadFile,ObjType objType,int numb,int uCount,int dCount)
        {
            var project = new ProjectController();
            project.InitializeGeometry(@"..\..\..\..\GmshApi\gmsh.dll");
            project.ImportCAD(cadFile);

            var obj = project.GetModelObject(objType, numb);
            
            var objsDown = project.GetAdjacentGeometryObjects(obj,1);
            var objsUp = project.GetAdjacentGeometryObjects(obj, 2);

            Assert.AreEqual(uCount, objsUp?.Count());
            Assert.AreEqual(dCount, objsDown?.Count());
        }

        
    }
}
