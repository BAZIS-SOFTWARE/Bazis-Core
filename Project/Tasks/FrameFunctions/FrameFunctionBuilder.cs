using Geometry;
using Model.Interfaces;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Project.Tasks.FrameFunctions
{
    public class FrameFunctionBuilder
    {
        FrameFunction frameFunction;

        public FrameFunctionBuilder(string[] heatSourceAr)
        {
            FrameFunctionName processType;
            if (!Enum.TryParse(heatSourceAr[0], out processType))
                throw new Exception("Источник задан неверно!");

            if (processType == FrameFunctionName.SPH)
            {
                var builder = new SphereBuilder().SetWidth(heatSourceAr[1]);
                frameFunction = (SphereFunction)builder;
            }


            else if (processType == FrameFunctionName.CIL)
            {
                var builder = new CillindricalBuilder().SetLength(heatSourceAr[1]).
                    SetUpperDiam(heatSourceAr[2]).SetBottomDiam(heatSourceAr[3]);

                frameFunction = (CillindricalFunction)builder;
            }
            else
            {
                var cs = new CustomFrameFunction();
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
