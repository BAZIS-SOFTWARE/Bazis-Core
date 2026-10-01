using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestModeCore_test.Model
{
    /// <summary>
    /// Владелец поверхности для тестов, которые проверяют саму поверхность,
    /// а не элемент: ElementSurface требует элемент-хозяина, но здесь от него
    /// нужен только номер.
    /// </summary>
    public class SurfaceOwnerStub : Element
    {
        public SurfaceOwnerStub(int number, Node[] nodes) : base(number, nodes)
        {
        }

        public override ElementKind ElementKind => ElementKind.Квадрат;

        public override int Level { get; set; }

        public override int NumberOfPoints => nodes.Count;

        public override ObjType ObjType => ObjType.Элемент2D;

        public override IEnumerable<Point3D> GetCoordinates()
        {
            return nodes.Select(node => node.Position);
        }

        public override Matrix<double> CalcJacobi(IIntegralPoint integralPoint)
        {
            throw new NotImplementedException();
        }

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            throw new NotImplementedException();
        }
    }
}
