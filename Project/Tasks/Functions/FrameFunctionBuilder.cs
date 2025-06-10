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

            if (processType == HeatSources.ARC)
            {
                var builder = new ArcSBuilder().SetCurrent(heatSourceAr[2]).SetVoltage(heatSourceAr[3]).
                        SetWidth(heatSourceAr[1]);
                frameFunction = (ArcS)builder;
            }

            else if (processType == HeatSources.FSWPin)
            {
                var builder = new FSWPinBuilder().SetRotSpeed(heatSourceAr[1]).SetLength(heatSourceAr[2]).
                    SetBottomDiam(heatSourceAr[3]).SetUpperDiam(heatSourceAr[4]).SetYieldFunc(heatSourceAr[5]);

                frameFunction = (FSWPin)builder;
            }
            else if (processType == HeatSources.FSWShoulder)
            {
                var builder = new FSWShoulderBuilder().SetAxisForce(heatSourceAr[1]).
                    SetRotSpeed(heatSourceAr[2]).SetLength(heatSourceAr[3]).
                    SetBottomDiam(heatSourceAr[4]).SetUpperDiam(heatSourceAr[5]).
                    SetFricModuleFunc(heatSourceAr[6]);

                frameFunction = (FSWShoulder)builder;

            }
            else
            {
                var builder = new LazerBuilder().SetPower(heatSourceAr[1]).SetLength(heatSourceAr[2]).
                    SetUpperDiam(heatSourceAr[3]).SetBottomDiam(heatSourceAr[4]);

                frameFunction = (Lazer)builder;
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
