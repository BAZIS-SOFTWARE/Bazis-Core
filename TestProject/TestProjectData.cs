using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.IO;
using OperationalController;
using OperationalController.GmshController;
using Project;
using Project.IO;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using static IronPython.Modules.PythonIterTools;

namespace TestProject
{

    [TestClass]
    public class TestProjectData
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 2 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1, 2, 3 }, ObjType.Узел, "load false", DisplayName = "load false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "load true", DisplayName = "load true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 5 }, ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "newMesh false", DisplayName = "newMesh false")]
        public void RemoveObjsByNumber(string path, int[] objNumbers, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();

            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelObjects(type, objNumbers);

            //assert

            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "A17", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "A17aa", ObjType.Элемент2D, "newMesh true", DisplayName = "newMesh true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "t4", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void RemoveSetByNameAndType(string path, string setName, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();
            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelSet(type, setName);

            //assert
            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void ClearMeshCollection(string path, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();

            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);

            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.ClearModelCollection(type);

            //assert
            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

        [DataTestMethod]
        [DataRow(0, 5, false, true, DisplayName = "Удаление точки без зависимостей")]
        [DataRow(0, 1, false, true, DisplayName = "Удаление точки без зависимостей (не проходит)")]
        [DataRow(1, 1, false, true, DisplayName = "Удаление кривой без зависимостей (не проходит)")]
        [DataRow(2, 1, false, true, DisplayName = "Удаление поверхности без зависимостей")]
        [DataRow(2, 1, true, true, DisplayName = "Рекурсивное удаление поверхности")]
        public void DeleteGeometryObject_ValidCases(int dim,int number,bool recursive,bool shouldBeRemoved)
        {
            // arrange
            var control = new ProjectController();
            control.CreateProject(@"newProj.bpf2");
            control.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            control.GmshController.Gmsh.Model.Add("Test");

            // подготовка тестовой геометрии:
            // point(1), curve(1), surface(1)
            var point1 = control.CreatePoint(0, 0, 0);     
            var point2 = control.CreatePoint(1, 0, 0);     
            var point3 = control.CreatePoint(1, 1, 0);    
            var point4 = control.CreatePoint(0, 1, 0);
            var point5 = control.CreatePoint(0, 1, 13);

            var line1 = control.CreateLine(point1, point2);      
            var line2 = control.CreateLine(point2, point3);     
            var line3 = control.CreateLine(point3, point4);    
            var line4 = control.CreateLine(point4, point1);     

            control.CreateSurface([line1, line2, line3, line4]); 

            // act
            control.DeleteGeometryObject(dim, number, recursive);

            // assert
            ObjType type = dim switch
            {
                0 => ObjType.Точка,
                1 => ObjType.Кривая,
                2 => ObjType.Поверхность,
                _ => throw new Exception()
            };

            var obj = control.GetModelObject(type, number);

            Assert.AreEqual(!shouldBeRemoved, obj != null);
        }
    }
}
