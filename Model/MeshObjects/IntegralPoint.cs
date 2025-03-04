using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using System.Collections.Generic;
using System.Reflection;

namespace Model.MeshObjects
{
    /// <summary>
    /// IntegralPoint
    /// </summary>
    public class IntegralPoint : IIntegralPoint
    {
        private Dictionary<string, double> coordinates = new Dictionary<string, double>();
/// <inheritdoc/>

        /// <inheritdoc/>
        public double Weigt { get; set; } = 1;
/// <inheritdoc/>

        public int Index { get; set; }
/// <inheritdoc/>

        public Matrix<double> Jacobi { get; set; }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public double this[string dirName] 
        { 
            get { return coordinates[dirName]; } 
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, double>> GetCoordinates()
        {
            foreach (var direction in coordinates)
            {
                yield return direction;
            }
        }
/// <inheritdoc/>

        public void AddCoordinate(string name,double value)
        {
            coordinates.Add(name,value);
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Weigt} : {string.Join(" ", coordinates)}";
        }
    }
}