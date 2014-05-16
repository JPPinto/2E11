using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Tile
    {
        Position pos;
        ushort value;
        bool isAvailable;

        Tile()
        {
            isAvailable = true;
        }

        int getValueRepresentation() {
            int result = 2;

            for (ushort i = value; i > 0; i++ ) {
                result *= result;
            }

            return result;
        }

        Position getPosition() {
            return pos;
        }

        public bool getAvailability()
        {
            return isAvailable;

        }
    }
}
