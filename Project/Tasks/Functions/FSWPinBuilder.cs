using Geometry;
using System;
using System.Globalization;

namespace Project.Tasks.Functions
{
    public class FSWPinBuilder
    {
        FSWPin pin;

        public FSWPinBuilder()
        {
            pin = new FSWPin();
            pin.Name = "FSWPin";
            pin.Frame = new Frame();
        }

        public FSWPinBuilder SetRotSpeed(string rotSpeed)
        {
            pin.RotSpeed = float.Parse(rotSpeed, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWPinBuilder SetYieldFunc(string yieldFunc)
        {
            pin.CreateParameter(yieldFunc);
            return this;
        }

        public FSWPinBuilder SetBottomDiam(string diam)
        {
            pin.BottomDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWPinBuilder SetUpperDiam(string diam)
        {
            pin.UpperDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWPinBuilder SetLength(string length)
        {
            pin.Length = float.Parse(length, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWPinBuilder SetFrame(Frame frame)
        {
            pin.Frame = frame;
            return this;
        }

        public static implicit operator FSWPin(FSWPinBuilder builder)
        {
            return builder.pin;
        }
    }
}
