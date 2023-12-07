using Geometry;
using ModelInterfaces;
using ModelInterfaces.IntegralObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MeshObjects
{
    public class Quad : Element2D
    {
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Квадрат;

        public Quad(int number, Node[] nodes) : base(number, nodes)
        {
            surface = new ElementSurface(0, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] });

            var intPoint = new IntegralPoint();
            intPoint.AddDirection("ksi", 0.0f);
            intPoint.AddDirection("eta", 0.0f);
            intPoint.Weigt = CalcSquare();
            AddIntegralPoint(intPoint);
        }

        public Quad(int number, Node[] nodes,int points) : base(number, nodes,points)
        {
            surface = new ElementSurface(0, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] });

            if(points == 4)
            {
                for (int i = 0; i < points; i++)
                {
                    //TO DO
                }
            }

        }     
/// <inheritdoc/>

        public override float CalcSquare()
        {
            var sum = 0f;

            for (int i = 0; i < nodes.Count - 3; i++)
            {
                for (int j = i + 1; j < nodes.Count - 2; j++)
                {
                    for (int k = j + 1; k < nodes.Count - 1; k++)
                    {

                        var x = new Node[] { nodes[i], nodes[j], nodes[k] };
                        var tetra = new Triangle(0, x);
                        sum += tetra.CalcSquare();

                    }
                }
            }
            return sum / 2;
        }

        
/// <inheritdoc/>

        public override float DetJ(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
    }
}
