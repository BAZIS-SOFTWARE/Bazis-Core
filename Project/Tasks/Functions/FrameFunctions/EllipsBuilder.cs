using Geometry;
using Project.Tasks.LocalFrames;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class EllipsBuilder
    {
        DEL elp;

        public EllipsBuilder()
        {
            elp = new DEL();
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

        public static implicit operator DEL(EllipsBuilder builder)
        {
            return builder.elp;
        }
    }
}
