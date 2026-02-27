using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.FrameCreators;
using Project.Tasks.ParametersFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.FrameFunctions
{
    public abstract class FrameFunction : ParametersFunction, IFrameFunction
    {
        /// <inheritdoc/>
        public LocalFrame LocalFrame { get; set; } = new StaticFrame();
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{base.ToString()};{LocalFrame}";
        }


        /// <inheritdoc/>

        public abstract double CalcValue(Point3D elementCenter);

 
        /// <inheritdoc/>

        public abstract List<T> GetIntersectedObjects<T>(List<T> elements) where T : IModelObject;
        /// <inheritdoc/>


        public abstract bool IsOverlappingSelf(float velocity);
    }
}
