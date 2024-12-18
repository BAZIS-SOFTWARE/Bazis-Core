using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.IntegralObject
{
    /// <summary>
    /// IIntegralPoint
    /// </summary>
    public interface IIntegralPoint
    {
        /// <summary>
        /// Weigt
        /// </summary>
        double Weigt { get; set; }
        /// <summary>
        /// GetLocalCoordinates
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, double>> GetCoordinates();
        /// <summary>
        /// AddDirection
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddCoordinate(string name, double value);
        /// <summary>
        /// this
        /// </summary>
        /// <param name="dirInd"></param>
        /// <returns></returns>
        double this[string dirInd] { get; }
    }
}
