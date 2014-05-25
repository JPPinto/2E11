using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11 {
    class Tile {
        ushort value;
        bool isAvailable;

        public Tile() {
            isAvailable = true;
        }

        int getValueRepresentation() {
            int result = 2;

            for (ushort i = value; i > 0; i++) {
                result *= result;
            }

            return result;
        }
        public bool getAvailability() {
            return isAvailable;

        }
        public void setValue(ushort val) {
            value = val;
            isAvailable = false;
        }
        public void clear() {
            isAvailable = true;
        }

        public ushort getValue() {
            return value;
        }
    }
}
