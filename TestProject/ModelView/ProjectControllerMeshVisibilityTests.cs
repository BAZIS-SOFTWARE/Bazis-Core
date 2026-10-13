using GmshApi;
using IronPython.Runtime;
using MathNet.Spatial.Euclidean;
using Model;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using Model.IO;
using Model.MeshObjects;
using OperationalController;
using OperationalController.GmshController;
using OperationalController.MeshObjsCreator.Extrude;
using System.Numerics;
using Point3D = global::Geometry.Point3D;

namespace TestProject_test.ModelView
{
    [TestClass]
    public class ProjectControllerMeshVisibilityTests
    {
        private string modelFolder = @"../../../../TestModelCore/Examples";

        [DataTestMethod]
        [DataRow(@"/CAD/cilindr.step")]
        public void Set3DVisibilityTest(string path)
        {
            var proj = new ProjectController();
            proj.InitializeGeometry(GmshLibrary.FilePath);
            proj.ImportCAD(modelFolder + @"/" + path);

            proj.GenerateMesh(3);

            var elements = proj.GetModelObjects(ObjType.Элемент3D).ToArray();
            var numbers = elements.Select(element => element.Number).ToArray();

            // Состояние представления хранится отдельно от элементов модели.
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.SetVisible(ObjType.Элемент3D, numbers, expState);

                foreach (var element in elements)
                    Assert.AreEqual(expState, proj.GetVisible(ObjType.Элемент3D, element.Number));
            }
        }

        [DataTestMethod]
        [DataRow(@"/CAD/cilindr.step")]
        public void Hide2DTest(string path)
        {
            var proj = new ProjectController();
            proj.InitializeGeometry(GmshLibrary.FilePath);
            proj.ImportCAD(modelFolder + @"/" + path);

            proj.GenerateMesh(3);

            //Скрытие\показ 2д элементов сопряженных с поверхностью и проверка элементов
            var groupName = proj.CreateGroupByGeoObjs(2, 2, 1);
            var elemTags = proj.GetModelGroup(groupName).Select(element => element.Number).ToHashSet();
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.SetVisible(ObjType.Поверхность, [1], expState);
                proj.HideMesh(2);

                foreach (var elems in proj.GetModelObjects(ObjType.Элемент2D))
                {
                    if (elemTags.Contains(elems.Number))
                        Assert.AreEqual(expState, proj.GetVisible(ObjType.Элемент2D, elems.Number));
                    else
                        Assert.AreEqual(true, proj.GetVisible(ObjType.Элемент2D, elems.Number));//Элементы не сопряженные с поверхность 1 должны быть показаны
                }
            }
        }

        [DataTestMethod]
        [DataRow(@"/CAD/cilindr.step")]
        public void Hide1DTest(string path)
        {
            var proj = new ProjectController();
            proj.InitializeGeometry(GmshLibrary.FilePath);
            proj.ImportCAD(modelFolder + @"/" + path);

            proj.GenerateMesh(3);

            //Скрытие\показ 1д элементов сопряженных с кривой и проверка элементов
            var groupName = proj.CreateGroupByGeoObjs(1, 1, 1);
            var elemTags = proj.GetModelGroup(groupName).Select(element => element.Number).ToHashSet();
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.SetVisible(ObjType.Кривая, [1], expState);
                proj.HideMesh(1);

                foreach (var elems in proj.GetModelObjects(ObjType.Элемент1D))
                {
                    if (elemTags.Contains(elems.Number))
                        Assert.AreEqual(expState, proj.GetVisible(ObjType.Элемент1D, elems.Number));
                    else
                        Assert.AreEqual(true, proj.GetVisible(ObjType.Элемент1D, elems.Number));//Элементы не сопряженные с кривой 1 должны быть показаны
                }
            }
        }
    }
}
