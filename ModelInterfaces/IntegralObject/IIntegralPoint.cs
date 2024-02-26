using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.IntegralObject
{
    /// <summary>
    /// IIntegralPoint
    /// </summary>
    public interface IIntegralPoint
    {
        /// <summary>
        /// Weigt
        /// </summary>
        float Weigt { get; set; }
        /// <summary>
        /// GetLocalCoordinates
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, float>> GetDirections();
        /// <summary>
        /// AddDirection
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddDirection(string name, float value);
        /// <summary>
        /// this
        /// </summary>
        /// <param name="dirInd"></param>
        /// <returns></returns>
        float this[string dirInd] { get; }
    }
}
