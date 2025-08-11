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

        public static implicit operator FrameFunction(FrameFunctionBuilder builder)
        {
            return builder.frameFunction;
        }
    }
}
