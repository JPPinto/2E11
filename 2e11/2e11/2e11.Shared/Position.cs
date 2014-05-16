using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Position
    {
        ushort x;
        ushort y;

        Position(ushort x, ushort y)
        {
            setPosition(x, y);
        }

        void setPosition(ushort xi, ushort yi)
        {
            this.x = xi;
            this.y = yi;
        }
        ushort getX()
        {
            return x;
        }

        ushort getY()
        {
            return y;
        }
    }
}
