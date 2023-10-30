using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.ObjectsFinders;

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
                new Element3D( 4,nodes, ObjKind.Tetra),
                new Element3D(5,nodes,  ObjKind.Tetra),
                new Element3D( 6,nodes, ObjKind.Tetra),
                new Element3D(7,nodes,  ObjKind.Tetra),
                new Element3D(8,nodes,  ObjKind.Tetra)
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
