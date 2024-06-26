using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;

namespace Model.MeshObjects
{
    public class Penta : Element3D
    {
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Пента;

        public Penta(int number, INode[] nodes) : base(number, nodes)
        {
        }

        public Penta(int number, INode[] nodes, int point) : base(number, nodes, point)
        {
        }

        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint point3D)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float CalcVolume()
        {
            var sum = 0f;

            for (int i = 0; i < nodes.Count - 3; i++)
            {
                for (int j = i + 1; j < nodes.Count - 2; j++)
                {
                    for (int k = j + 1; k < nodes.Count - 1; k++)
                    {
                        for (int l = k + 1; l < nodes.Count; l++)
                        {
                            var x = new Node[] { (Node)nodes[i], (Node)nodes[j], (Node)nodes[k], (Node)nodes[l] };
                            var tetra = new Tetra(0, x);
                            sum += tetra.CalcVolume();
                        }
                    }
                }
            }
            return sum / 4;
        }      
/// <inheritdoc/>

        public override double[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
    }
}
