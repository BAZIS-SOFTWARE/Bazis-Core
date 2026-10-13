using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.IO;
using OperationalController;
using OperationalController.GmshController;
using Project;
using Project.Interfaces.Tasks;
using Project.IO;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using static IronPython.Modules.PythonIterTools;

namespace TestProject_test.Geometry
{
    [TestClass]
    public class GeometryCrudTests
    {
        string testDir = @"../../../SharedData/ProjectFiles";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
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
            control.InitializeGeometry(GmshLibrary.FilePath);

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
