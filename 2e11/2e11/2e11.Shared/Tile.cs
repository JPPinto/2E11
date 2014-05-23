using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Tile
    {
        //static int representation[] = new int[17] {2,4,8,16,32,64,128,256,512,1024,2048,4096,8192,16384,32768,65536,131072};
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
        public bool getAvailability()
        {
            return isAvailable;

        }
        public void setValue(ushort val)
        {
            value = val;
        }
        void clear() 
        {
            isAvailable = true;
        }
    }
}
