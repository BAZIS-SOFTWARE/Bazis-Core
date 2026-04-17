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

            var start = project.GmshController.Gmsh.Model.Occ.AddPoint(0, 0, 0);
            var middle = project.GmshController.Gmsh.Model.Occ.AddPoint(1, 0, 0);
            var end = project.GmshController.Gmsh.Model.Occ.AddPoint(0, 1, 0);
            //Добавляем линии по созданным точкам
            var lineA = project.GmshController.Gmsh.Model.Occ.AddLine(start, middle);
            var lineB = project.GmshController.Gmsh.Model.Occ.AddLine(middle, end);
            var lineC = project.GmshController.Gmsh.Model.Occ.AddLine(end, start);
            project.GmshController.Gmsh.Model.Occ.Synchronize();
            var contur = project.GmshController.Gmsh.Model.Occ.AddWire([lineA, lineB, lineC]);
            project.GmshController.Gmsh.Model.Occ.Synchronize();
            var surf = project.GmshController.Gmsh.Model.Occ.AddPlaneSurface([contur]);

            var w = project.GmshController.Gmsh.Model.Occ.AddThruSections([contur]);
            project.GmshController.Gmsh.Model.Occ.Synchronize();

            project.CreateGeometryObject(0, start);
            project.CreateGeometryObject(0, middle);
            project.CreateGeometryObject(0, end);
            project.CreateGeometryObject(1, lineA);
            project.CreateGeometryObject(1, lineB);
            project.CreateGeometryObject(1, lineC);
            project.CreateGeometryObject(2, surf);


            var s = project.GetModelPoint(start);
            var c = project.GetModelCurve(lineB);
            var p = project.GetModelSurface(surf);
        }
    }
}
