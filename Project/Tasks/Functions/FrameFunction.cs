using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Functions
{
    public abstract class FrameFunction : IFrameFunction
    {
        protected List<FunctionParameter> parameters = new List<FunctionParameter>();
        /// <inheritdoc/>
        public LocalFrame LocalFrame { get; set; } = new StaticFrame();
        /// <inheritdoc/>
        public HeatSources Name { get; internal set; }
/// <inheritdoc/>

        public abstract float CalcValue(Point3D elementCenter);

        /// <inheritdoc/>
        public void CreateParameter(string name, float value)
        {
            var fParam = new FunctionParameter() { Name = name, Value = value };
            parameters.Add(fParam);
        }
        /// <inheritdoc/>

        public void CreateParameter(string name)
        {
            var fParam = new FunctionParameter() { Name = name };
            parameters.Add(fParam);
        }
        /// <inheritdoc/>

        public IEnumerable<FunctionParameter> GetParameters()
        {
            foreach (var item in parameters)
            {
                yield return item;
            }
        }
/// <inheritdoc/>



        public abstract List<T> GetIntersectedObjects<T>(List<T> elements) where T : IModelObject;
/// <inheritdoc/>


        public abstract bool IsOverlappingSelf(float velocity);
    }
}
