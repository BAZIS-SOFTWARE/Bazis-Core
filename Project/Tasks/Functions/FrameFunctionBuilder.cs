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
            FunctionType processType;
            if (!Enum.TryParse(heatSourceAr[0], out processType))
                throw new Exception("Источник задан неверно!");

            if (processType == FunctionType.SPH)
            {
                var builder = new SphereBuilder().SetWidth(heatSourceAr[1]);
                frameFunction = (SphereFunction)builder;
            }


            else if (processType == FunctionType.CIL)
            {
                var builder = new CillindricalBuilder().SetLength(heatSourceAr[1]).
                    SetUpperDiam(heatSourceAr[2]).SetBottomDiam(heatSourceAr[3]);

                frameFunction = (CillindricalFunction)builder;
            }
            else
            {
                var cs = new CustomFunction();
                cs.CreateEngine(heatSourceAr[1]);
                frameFunction = cs;
            }
        }

        public static implicit operator FrameFunction(FrameFunctionBuilder builder)
        {
            return builder.frameFunction;
        }
    }
}
