using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class ItemRange
    {
        public float Max { get; set; }
        public float Min { get; set; }

        public Color Color { get; set; }

        public override string ToString()
        {
            return $"Max {Max} Min {Min} Color {Color}";
        }
    }
}