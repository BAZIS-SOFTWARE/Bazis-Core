using Geometry;
using Model.Interfaces;
using OperationalController;
using System.Globalization;


namespace ControllerTests
{
    [TestClass]
    public class MergeObjects
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }


        public static IEnumerable<object[]> AdditionData
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
        [DynamicData(nameof(AdditionData))]
        public void MergeElements(List<Point3D> points, int dim)
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
                

            contr.CreateElementsSet("newSet0", dim);
            contr.CreateElement("newSet0", dim, numbs);
            contr.CreateElementsSet("newSet1", dim);
            contr.CreateElement("newSet1", dim, numbs);
            
            contr.MergeElements(Model.Interfaces.ObjType.Элемент1D, "newSet0", "newSet1");

            //assert
            Assert.AreNotEqual(contr.GetModelElements(dim, "newSet0").Count(), 0);
            //Assert.AreEqual(contr.GetModelElements(dim, "newSet1").Count(), 0);
        }


        [DataTestMethod]
        [DataRow(@"c:\projs\Matushkin\T_project.bpf", 0.001f, 2)]
        [DataRow(@"..\..\..\Examples\3D\cub_unsorted_freeNodesCoincident.bpf", 0.01f, 4)]
        [DataRow(@"C:\projs\AdditiveTest\1n.stl", 0.01f, 3964)]
        public void MergeNodes(string path, float dist, int count)
        {
            //assing
            var controller = new OperationalController.ProjectController();

            //act

            var ext = Path.GetExtension(path);

            if (ext == ".stl")
                controller.ImportMesh(path);
            else
                controller.Load(path);

            var coincidentNodes = controller.FindCoincidentObjects(ObjType.Узел, dist);

            controller.MergeNodes(coincidentNodes);

            //assert

            Assert.AreEqual(coincidentNodes.Count, count);

            // Очень странная проверка ее нужно ускорить

            foreach (var elem in controller.GetAllModelElements())
            {
                foreach (var item in elem.GetVertexes())
                {
                    if (!controller.ContainsModelObject(ObjType.Узел, item.Number))
                        throw new Exception($"Узел {item.Number} в элементе {elem.Number} не удален");
                }
            }
        }
    }
}
