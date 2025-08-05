using Geometry;
using Project.Tasks.Functions.Welding;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Project.Tasks.Functions
{
    public class FrameFunctionBuilder
    {
        FrameFunction frameFunction;

        public FrameFunctionBuilder(string[] heatSourceAr)
        {
            HeatSources processType;
            if (!Enum.TryParse(heatSourceAr[0], out processType))
                throw new Exception("Источник задан неверно!");

            if (processType == HeatSources.SPH)
            {
                var builder = new SphereBuilder().SetWidth(heatSourceAr[1]);
                frameFunction = (SphereFunction)builder;
            }

            
            else
            {
                var builder = new CillindricalBuilder().SetLength(heatSourceAr[1]).
                    SetUpperDiam(heatSourceAr[2]).SetBottomDiam(heatSourceAr[3]);

                frameFunction = (CillindricalFunction)builder;
            }
        }
        public FrameFunctionBuilder SetFrame(Frame frame)
        {
            frameFunction.LocalFrame.frame = frame;
            return this;
        }

        private static Point3D NewMethod(string[] ar)
        {
            var x = float.Parse(ar[0]);
            var y = float.Parse(ar[1]);
            var z = float.Parse(ar[2]);

            var centr = new Geometry.Point3D(x, y, z);
            return centr;
        }

        public static implicit operator FrameFunction(FrameFunctionBuilder builder)
        {
            return builder.frameFunction;
        }
    }
}
