using Geometry;
using ModelControllerInterfaces.Delone2D;

namespace ModelController.MeshObjsCreator.Delone2D
{
    /// <summary>
    /// DeloneNode2D
    /// </summary>
    public class DeloneNode2D : IDeloneNode2D
    {
        /// <inheritdoc/>
        public int Number { get; }
/// <inheritdoc/>

        public Point2D Position { get; }
        /// <summary>
        /// DeloneNode2D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public DeloneNode2D(int number, Point2D position)
        {
            Number = number;
            Position = position;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Number} {Position}";
        }
    }
}