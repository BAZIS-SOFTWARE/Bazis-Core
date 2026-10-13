using Geometry;
using Model;
using Model.Interfaces;
using OperationalController;
using OperationalController.GmshController;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TestProject_test.Mesh
{
    [TestClass]
    public class CreateObjects
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }


        public static IEnumerable<object[]> Create1DAdditionData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new List<Point3D>
                        {
                            new Point3D(0, 0,0),
                            new Point3D(1, 1,1),
                        },
                        1
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Create1DAdditionData))]
        public void Create1D(List<Point3D> points, int dim)
        {
            var contr = new ProjectController();
            contr.CreateProject(@"../../../../TestModelCore/Examples/createObj.bpf2");

            var numb = 0;
            var numbs = new int[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                numbs[i] = numb;
                contr.CreateNode(numb++, points[i]._x, points[i]._y, points[i]._z);
            }


            contr.CreateElementsSet("newSet", dim);
            contr.CreateElement("newSet", dim, numbs);

            //assert
            Assert.AreNotEqual(contr.GetModelElements(dim, "newSet").Count(), 0);
        }

        public static IEnumerable<object[]> Create2DMeshOnPoligonAdditionData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 10),
                            new Point2D(10, 10),
                            new Point2D(10, 0)
                        },
                        5
                    },
                    new object[]
                    {
                        new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 10),
                            new Point2D(15, 15),
                            new Point2D(10, 0)
                        },
                        5
                    },
                    new object[]
                    {
                        new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 10),
                            new Point2D(15, 15),
                            new Point2D(15, 0)
                        },
                        5
                    }
                };
            }
        }


        [TestMethod]
        [DynamicData(nameof(Create2DMeshOnPoligonAdditionData))]
        public void Create2DMeshOnPoligon(List<Point2D> points, int num)
        {
            //act
            var contr = new ProjectController();

            contr.CreateProject(@"newProj.bpf2");
            contr.CreateQuadMeshOnPoligon(points, num);


            //assert
            // Append сохраняет исходное имя набора ("create"), а не переименовывает его в append_N
            Assert.AreNotEqual(contr.GetModelElements(2, "create").Count(), 0);
        }






        [TestMethod]
        public void ExtrudeGeometry()
        {
            var project = new ProjectController();
            project.InitializeGeometry(GmshLibrary.FilePath);
            //project.CreateProject(@"newProj.bpf2");
            var s = @"../../../../TestModelCore/Examples/CAD/model1.stp";
            
            project.ImportCAD(s);

            var start = project.CreatePoint(0, -85, 5);

            var line2 = project.CreateLine(1, start);
            var line3 = project.CreateLine(start, 7);

            var plane = project.CreateSurface(new int[] { 11, line2, line3 });

            project.ClearModelCollection(ObjType.Узел);
            project.GenerateMesh(3);

            var setName = project.ExtrudeElement3DAlongCurve(plane, [1],1,1,false);

            //assert
            // Append возвращает имя созданного набора, дополнительное переименование больше не требуется
            Assert.IsTrue(setName.StartsWith("extrude"), $"Неожиданное имя набора: {setName}");
            Assert.IsTrue(project.GetModelSetsInfo(ObjType.Элемент3D).Any(x => x.Name == setName));
            Assert.AreNotEqual(0, project.GetModelElements(3, setName).Count());
        }

        [TestMethod]
        public void ExtrudeGeometry2DThen1D()
        {
            var project = new ProjectController();
            project.InitializeGeometry(GmshLibrary.FilePath);
            project.ImportCAD(@"../../../../TestModelCore/Examples/CAD/model1.stp");

            var start = project.CreatePoint(0, -85, 5);
            var line2 = project.CreateLine(1, start);
            var line3 = project.CreateLine(start, 7);
            var plane = project.CreateSurface(new int[] { 11, line2, line3 });

            project.ClearModelCollection(ObjType.Узел);
            project.GenerateMesh(3);

            var startNodeTag = project.GetAllModelNodes().First().Number;
            var elements3DBefore = project.GetModelSetsInfo(ObjType.Элемент3D).Count();
            var elements1DBefore = project.GetModelSetsInfo(ObjType.Элемент1D).Count();

            project.ExtrudeElement3DAlongCurve(plane, [1], 1, 1, false);
            project.ExtrudeElement1DAlongCurve([1], 1, startNodeTag, 1);

            Assert.IsTrue(project.GetModelSetsInfo(ObjType.Элемент3D).Count() > elements3DBefore);
            Assert.IsTrue(project.GetModelSetsInfo(ObjType.Элемент1D).Count() > elements1DBefore);
        }


        [TestMethod]
        public void ExtrudeGeometry2()
        {
            var project = new ProjectController();
            project.InitializeGeometry(GmshLibrary.FilePath);
            //project.CreateProject(@"newProj.bpf2");
            var s = @"../../../../TestModelCore/Examples/CAD/PART_3.stp";

            project.ImportCAD(s);

            var pointOne = project.CreatePointByVector(15, 17, -10);
            var pointTwo = project.CreatePointByVector(15, 16, 10);
            var line1 = project.CreateLine(15, pointOne);
            var line2 = project.CreateLine(pointOne, pointTwo);
            var line3 = project.CreateLine(pointTwo, 15);

            var plane = project.CreateSurface(new int[] { line1, line2, line3 });

            project.ClearModelCollection(ObjType.Узел);
            project.GenerateMesh(3);

            project.ExtrudeElement3DAlongCurve(plane, [20,19,14,11,8], 15, 3, false);

            var set = project.GetModelSetsInfo(ObjType.Элемент3D).Where(x => x.Name.Contains("extrude")).Last();
        }
    }
}
