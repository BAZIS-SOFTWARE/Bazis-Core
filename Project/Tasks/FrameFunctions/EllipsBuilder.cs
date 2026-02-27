using Geometry;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Project.Tasks.FrameFunctions
{
    public class EllipsBuilder
    {
        DoubleEllips elp;

        public EllipsBuilder()
        {
            elp = new DoubleEllips();
            elp.Name = FrameFunctionName.DEL.ToString();
            elp.LocalFrame = new StaticFrame();
        }

        public EllipsBuilder SetWidth(float width)
        {
            elp.Width = width;
            return this;
        }

        public EllipsBuilder SetDepth(float depth)
        {
            elp.Depth = depth;
            return this;
        }

        public EllipsBuilder SetFrontPart(float frontPart)
        {
            elp.FrontPart = frontPart;
            return this;
        }

        public EllipsBuilder SetBackPart(float backPart)
        {
            elp.BackPart = backPart;
            return this;
        }

        public EllipsBuilder SetFrame(Frame frame)
        {
            elp.LocalFrame = new StaticFrame(frame);
            return this;
        }

        public static implicit operator DoubleEllips(EllipsBuilder builder)
        {
            return builder.elp;
        }
    }
}
