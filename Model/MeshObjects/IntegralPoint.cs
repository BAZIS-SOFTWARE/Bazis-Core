using Geometry;
using ModelInterfaces.IntegralObject;
using System.Collections.Generic;
using System.Drawing;

namespace Model.MeshObjects
{
    /// <summary>
    /// IntegralPoint
    /// </summary>
    public class IntegralPoint : IIntegralPoint
    {
        private Dictionary<string, float> directions = new Dictionary<string, float>();

        /// <inheritdoc/>
        public float Weigt { get; set; } = 1;
        /// <summary>
        /// this
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public float this[string dirName] 
        { 
            get { return directions[dirName]; } 
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, float>> GetDirections()
        {
            foreach (var direction in directions)
            {
                yield return direction;
            }
        }
/// <inheritdoc/>

        public void AddDirection(string name,float value)
        {
            this.directions.Add(name,value);
        }
    }
}