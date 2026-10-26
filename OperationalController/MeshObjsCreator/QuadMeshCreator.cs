using Geometry;
using Model;
using Model.MeshObjects;
using Model.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator
{
    public class QuadMeshCreator
    {
        public ObjectsCollection<Element2D> CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem)
        {
            if (points.Count != 4)
                throw new Exception("Колличество узлов не равно 4");

            var b_0 = GetBorderPoints(numberOfElem, points[0], points[1]);
            var b_2 = GetBorderPoints(numberOfElem, points[3], points[2]);

            var b_1 = GetBorderPoints(numberOfElem, points[1], points[2]);
            var b_3 = GetBorderPoints(numberOfElem, points[0], points[3]);

            var hor = new List<Segment2D>(); // набор линий гор
            var vert = new List<Segment2D>(); // набор линий гор

            for (int i = 0; i <= numberOfElem; i++)
                hor.Add(new Segment2D(b_0[i], b_2[i]));

            for (int i = 0; i <= numberOfElem; i++)
                vert.Add(new Segment2D(b_1[i], b_3[i]));

            var numbE = 0;
            var numbN = 0;

            var elems = new ObjectsCollection<Element2D>();

            for (int i = 1; i < hor.Count; i++)
            {
                for (int j = 1; j < vert.Count; j++)
                {

                    var p0 = hor[i - 1].GetIntersection(vert[j - 1]);
                    var p1 = hor[i].GetIntersection(vert[j - 1]);
                    var p2 = hor[i].GetIntersection(vert[j]);
                    var p3 = hor[i - 1].GetIntersection(vert[j]);
                    var elem = CreateElem(ref numbE, ref numbN, p0, p1, p2, p3);

                    elems.Add("create", elem);
                }
            }
            return elems;
        }

        private Quad CreateElem(ref int numbE, ref int numbN, Point2D p0, Point2D p1, Point2D p2, Point2D p3)
        {
            var nodes = new Node[]
            {
                    new Node(numbN++,new Point3D(p0._x,p0._y,0)),
                    new Node(numbN++,new Point3D(p1._x,p1._y,0)),
                    new Node(numbN++,new Point3D(p2._x,p2._y,0)),
                    new Node(numbN++,new Point3D(p3._x,p3._y,0)),
            };
            numbE++;
            return new Quad(numbE, nodes);
        }

        private List<Point2D> GetBorderPoints(int numberOfElem, Point2D fPoint, Point2D sPoint)
        {
            var border = new List<Point2D>();
            var temp = new Segment2D(fPoint, sPoint);

            var elmSize = temp.GetLength() / numberOfElem;
            var length = 0.0f;
            border.Add(fPoint);
            for (int i = 0; i < numberOfElem - 1; i++)
                border.Add(temp.GetPointOnLine(length += elmSize));

            border.Add(sPoint);

            return border;
        }
    }
}
