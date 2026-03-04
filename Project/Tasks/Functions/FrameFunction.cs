using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.FrameCreators;
using Project.Tasks.Functions;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    public abstract class FrameFunction : Function, IFrameFunction
    {
        /// <summary>
        /// FrameFunction
        /// </summary>
        public FrameFunction()
        {
            AddParameter(new Parameter("X"));
            AddParameter(new Parameter("Y"));
            AddParameter(new Parameter("Z"));
        }
        /// <summary>
        /// X
        /// </summary>
        public double X
        {
            get { return this["X"].GetValue(); }
            set { this["X"].SetValue(value); }
        }
        /// <summary>
        /// Y
        /// </summary>
        public double Y
        {
            get { return this["Y"].GetValue(); }
            set { this["Y"].SetValue(value); }
        }
        /// <summary>
        /// Z
        /// </summary>
        public double Z
        {
            get { return this["Z"].GetValue(); }
            set { this["Z"].SetValue(value); }
        }


        /// <inheritdoc/>

        public abstract double CalcValue(Point3D elementCenter);

 
        /// <inheritdoc/>

        public abstract List<T> GetIntersectedObjects<T>(List<T> elements) where T : IModelObject;
        /// <inheritdoc/>


        public abstract bool IsOverlappingSelf(float velocity);
    }
}
