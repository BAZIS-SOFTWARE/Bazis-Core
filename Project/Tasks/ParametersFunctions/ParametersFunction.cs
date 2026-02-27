using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.ParametersFunctions
{
    /// <summary>
    /// ParametersFunction
    /// </summary>
    public abstract class ParametersFunction
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public Parameter<double> this[int ind]
        {
            get => parameters[ind];
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var str = string.Join(";", parameters.Select(x => x.Name));
            return $"{Name};{str}";
        }

        protected List<Parameter<double>> parameters = new List<Parameter<double>>();

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void CreateParameter(string name, double value)
        {
            var fParam = new Parameter<double>() { Name = name, Value = value };
            parameters.Add(fParam);
        }
        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>

        public void CreateParameter(string name)
        {
            var fParam = new Parameter<double>() { Name = name };
            parameters.Add(fParam);
        }
        /// <summary>
        /// GetParameters
        /// </summary>
        /// <returns></returns>

        public IEnumerable<Parameter<double>> GetParameters()
        {
            foreach (var item in parameters)
            {
                yield return item;
            }
        }
    }
}
