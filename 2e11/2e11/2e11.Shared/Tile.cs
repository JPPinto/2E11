using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Tile
    {
        Position pos;

        ushort value;

        Tile()
        { 
            
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
    }
}
