// LUMI :)
using System;

namespace SignalRoguelite
{
    [Serializable]
    public class EncodedSignal
    {
        public SignalColor[] colors;
        public ShapeType[] shapes;

        public EncodedSignal()
        {
            colors = new SignalColor[3];
            shapes = new ShapeType[3];
        }
    }
}