using Geometry;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using OperationalController;
using System.Globalization;


namespace TestModeCore_test.OperationalController
{
    [TestClass]
    public class RenumberElements
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
                        new List<Node>()
                        {
                            new Node(0,new Point3D(0, 0,0)),
                            new Node(1,new Point3D(1, 1,1)),
                            new Node(2,new Point3D(2, 2,2)),
                            new Node(3,new Point3D(3, 3,3))
                        },
                        new List<int[]>()
                        {
                            new int[]{1,0,0,1},
                            new int[]{1,0,0,1},
                            new int[]{2,0,0,1,2},
                            new int[]{2,1,1,3,2},
                            new int[]{2,1,3,2,0}
                        }                               
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AdditionData))]
        public void Renumber(List<Node> nodes, List<int[]> elems)
        {
            var contr = new ProjectController();
            contr.CreateProject(@"..\..\..\Examples\createObj.bpf2");
            
            foreach (var item in nodes)
            {
                contr.CreateNode(item.Number, item._x, item._y, item._z);
            }

            //var numb = 0;
            foreach (var item in elems)
            {
                var numbers = new int[item.Length-2];
                Array.Copy(item, 2, numbers, 0, item.Length - 2);
                
                if(!contr.GetAllModelSetsInfo().Any(x => x.Name == $"newSet{item[1]}"))
                    contr.CreateElementsSet
                        ($"newSet{item[1]}", item[0]);
                contr.CreateElement($"newSet{item[1]}", item[0], numbers);
                //numb++;
            }

            contr.RenumberElements();

            //assert
            Assert.AreEqual(contr.GetAllModelElements().Last().Number, elems.Count-1);
        }
    }
}
