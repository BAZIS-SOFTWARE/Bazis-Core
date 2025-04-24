using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Functions
{
    public class FrameFunctionBuilder
    {
        public IPowerFunction Build(string[] heatSourceAr)
        {
            HeatSources processType;
            if (!Enum.TryParse(heatSourceAr[0], out processType))
                throw new Exception("Источник задан неверно!");

            if (processType == HeatSources.ARC)
            {
                return (Arc)new ArcBuilder().SetCurrent(heatSourceAr[2]).SetVoltage(heatSourceAr[3]).
                    SetWidth(heatSourceAr[1]);
            }

            else if (processType == HeatSources.FSWPin)
            {
                return (FSWPin)new FSWPinBuilder().SetRotSpeed(heatSourceAr[1]).SetLength(heatSourceAr[2]).
                    SetBottomDiam(heatSourceAr[3]).SetUpperDiam(heatSourceAr[4]).SetYieldFunc(heatSourceAr[5]);

            }
            else if (processType == HeatSources.FSWShoulder)
            {
                return (FSWShoulder)new FSWShoulderBuilder().SetAxisForce(heatSourceAr[1]).
                    SetRotSpeed(heatSourceAr[2]).SetLength(heatSourceAr[3]).
                    SetBottomDiam(heatSourceAr[4]).SetUpperDiam(heatSourceAr[5]).
                    SetFricModuleFunc(heatSourceAr[6]);
            }
            else
            {
                return (Lazer)new LazerBuilder().SetPower(heatSourceAr[1]).SetLength(heatSourceAr[2]).SetUpperDiam(heatSourceAr[3]).SetBottomDiam(heatSourceAr[4]);
            }
        }
    }
}
