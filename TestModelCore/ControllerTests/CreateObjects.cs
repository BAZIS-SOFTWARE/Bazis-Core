using Geometry;
using Model;
using OperationalController;
using OperationalController.GmshController;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ControllerTests
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
            contr.CreateProject(@"..\..\..\Examples\createObj.bpf2");

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
            Assert.AreNotEqual(contr.GetModelElements(2, "append_0").Count(), 0);
        }

        [TestMethod]
        public void CreateGeometryObjectsTest()
        {
            var project = new ProjectController();
            project.CreateProject(@"newProj.bpf2");
            project.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");

            project.GmshController.Gmsh.Model.Add("line");



            var start = project.CreatePoint(0, 0, 0);
            var middle = project.CreatePoint(2, 2, 0);
            var end = project.CreatePoint(2, 0, 0);

            var line1 = project.CreateLine(start, middle);
            var line2 = project.CreateLine(middle, end);
            var line3 = project.CreateLine(end, start);

            var plane = project.CreateSurface(new int[] { line1, line2, line3 });

            var s = project.GetModelPoint(start);
            var c = project.GetModelCurve(line2);
            var p = project.GetModelSurface(plane);
        }
    }
}
