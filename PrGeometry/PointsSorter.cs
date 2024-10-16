using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
//using Functions.Extensions;
//using Functions.Sort;
using Geometry.Interfaces;
using MathNet.Numerics.Distributions;

namespace Geometry
{
    public static class PointsSorter
    {
        public static bool SwapArray<T>(T[] objectArray, int x, int y)
        {

            // check for out of range
            if (objectArray.Length <= y || objectArray.Length <= x) return false;


            // swap index x and y
            T temp = objectArray[x];
            objectArray[x] = objectArray[y];
            objectArray[y] = temp;


            return true;
        }

        public static bool SwapList<T>(List<T> objectArray, int firstIndex, int secondIndex)
        {

            // check for out of range
            if (objectArray.Count <= secondIndex || objectArray.Count <= firstIndex) return false;


            // swap index x and y
            T temp = objectArray[firstIndex];
            objectArray[firstIndex] = objectArray[secondIndex];
            objectArray[secondIndex] = temp;


            return true;
        }
        /// <summary>
        /// Провести сортировку
        /// </summary>
        /// <param name="objects"></param>
        public static void RoundSort<T>(this T[] objects) where T : IPosition3D
        {
            var fInd = 0;
            var sInd = 1;

            var plane = TryGetPlane(objects);

            if (plane == null)
                return;


            while (true)
            {
                var resu = CheckVectorsOrientation(objects, fInd, sInd, plane);

                if (resu)
                {
                    if (sInd - fInd > 1)
                    {
                        SwapArray(objects, fInd + 1, sInd);
                        sInd = fInd + 1;
                    }

                    fInd++;
                }
                sInd++;
                if (fInd == objects.Length - 1 | sInd == objects.Length) break;
            }
        }

        private static Plane TryGetPlane<T>(T[] objects) where T : IPosition3D
        {
            var p0 = new Point3D(objects[0]._x, objects[0]._y, objects[0]._z);
            var p1 = new Point3D(objects[1]._x, objects[1]._y, objects[1]._z);

            for (int i = 0; i < objects.Length - 2; i++)
            {
                var p2 = new Point3D(objects[i + 2]._x, objects[i + 2]._y, objects[i + 2]._z);

                var fvectors = p1.Sub(p0);  //new Point3D(p1._x - p0._x, p1._y - p0._y, p1._z - p0._z);
                var svectors = p2.Sub(p0); //new Point3D(p2._x - p0._x, p2._y - p0._y, p2._z - p0._z);

                var normal = Vector.CrossProd(fvectors, svectors);
                if (Vector.GetVectorLenght(normal) > 1e-6)
                {
                    var plane = new Plane(p0, normal);
                    plane.Shifting = 0;
                    return plane;
                }

            }

            return null;
        }

        private static Plane TryGetPlane<T>(List<T> objects) where T : IPosition3D
        {
            var p0 = new Point3D(objects[0]._x, objects[0]._y, objects[0]._z);
            var p1 = new Point3D(objects[1]._x, objects[1]._y, objects[1]._z);

            for (int i = 0; i < objects.Count - 2; i++)
            {
                var p2 = new Point3D(objects[i + 2]._x, objects[i + 2]._y, objects[i + 2]._z);

                var fvectors = p1.Sub(p0);  //new Point3D(p1._x - p0._x, p1._y - p0._y, p1._z - p0._z);
                var svectors = p2.Sub(p0); //new Point3D(p2._x - p0._x, p2._y - p0._y, p2._z - p0._z);

                var normal = Vector.CrossProd(fvectors, svectors);
                if (Vector.GetVectorLenght(normal) > 1e-6)
                {
                    var plane = new Plane(p0, normal);
                    plane.Shifting = 0;
                    return plane;
                }

            }

            return null;
        }

        private static bool CheckVectorsOrientation<T>(T[] points, int firstInd, int secondInd, Plane plane) where T : IPosition3D
        {
            //bool isPointInside = false;
            var temp = new List<float>();

            var baseVector = new Point3D()
            {
                _x = points[secondInd]._x - points[firstInd]._x,
                _y = points[secondInd]._y - points[firstInd]._y,
                _z = points[secondInd]._z - points[firstInd]._z
            };

            for (int i = 0; i < points.Length; i++)
            {
                if (i != firstInd & i != secondInd)
                {
                    var actVector = new Point3D()
                    {
                        _x = points[i]._x - points[firstInd]._x,
                        _y = points[i]._y - points[firstInd]._y,
                        _z = points[i]._z - points[firstInd]._z
                    };

                    var prod = Vector.CrossProd(baseVector, actVector);

                    var dist = plane.PointPosition(prod);
                    temp.Add(dist);
                }
            }

            if (temp.All(x => Math.Sign(x) >= 0))
                return true;
            else if (temp.All(x => Math.Sign(x) < 0))
                return true;
            else return false;
        }
        /// <summary>
        /// Провести сортировку
        /// </summary>
        /// <param name="points"></param>
        public static void RoundSort<T>(this List<T> points) where T : IPosition3D
        {
            var fInd = 0;
            var sInd = 1;

            var plane = TryGetPlane(points);

            if (plane == null)
                return;

            while (true)
            {
                var resu = CheckVectorsOrientation(points, fInd, sInd, plane);

                if (resu)
                {
                    if(sInd - fInd > 1)
                    {
                        SwapList(points,fInd + 1, sInd);
                        sInd = fInd + 1;
                    }

                    fInd++;
                }
                sInd++;
                if (fInd == points.Count - 1 | sInd == points.Count) break;
            }
        }

        private static bool CheckVectorsOrientation<T>(List<T> points, int firstInd, int secondInd, Plane plane) where T : IPosition3D
        {
            //bool isPointInside = false;
            var temp = new List<float>();

            var baseVector = new Point3D()
            {
                _x = points[secondInd]._x - points[firstInd]._x,
                _y = points[secondInd]._y - points[firstInd]._y,
                _z = points[secondInd]._z - points[firstInd]._z
            };

            for (int i = 0; i < points.Count; i++)
            {
                if (i != firstInd & i != secondInd)
                {
                    var actVector = new Point3D()
                    {
                        _x = points[i]._x - points[firstInd]._x,
                        _y = points[i]._y - points[firstInd]._y,
                        _z = points[i]._z - points[firstInd]._z
                    };

                    var prod = Vector.CrossProd(baseVector, actVector);

                    var dist = plane.PointPosition(prod);
                    temp.Add(dist);
                }
            }

            if (temp.All(x => Math.Sign(x) >= 0))
                return true;
            else if (temp.All(x => Math.Sign(x) < 0))
                return true;
            else return false;
        }    

        public static void SortByDistance<T>(this List<T> points) where T : IPosition3D
        {          
            var fistPoint = 0;

            while (fistPoint < points.Count - 1)
            {
                var dists = new List<float>();
                for (int j = fistPoint + 1; j < points.Count; j++) // простейшая пузырьковая сортировка  
                {
                    var dx = points[fistPoint]._x - points[j]._x;
                    var dy = points[fistPoint]._y - points[j]._y;
                    var dz = points[fistPoint]._z - points[j]._z;

                    var dist = Vector.GetVectorLenght(new Point3D(dx,dy,dz));
                    dists.Add(dist);
                }              

                var minInd = dists.IndexOf(dists.Min()) + 1;

                SwapList(points,fistPoint + 1, minInd + fistPoint);

                fistPoint++;
            }
        } 
    }
}
