using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using ModelInterfaces.ObjectsFinders;

namespace TestModel
{
    [TestClass]
    public class SearchModelObjects
    {
        [TestMethod]
        public void ByNumber()
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new Model.ModelObject[]
            {
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var model = new ModelData();
            model.ObjectData.AddRange(objs);
            //act
            var element = objs.Find(8);
            //assert
            Assert.AreEqual(8, element.Number);
        }
    }
}
