using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11 {
    class Tile {
        ushort value;
        bool isAvailable;

        public Tile() {
            value = 0;
            isAvailable = true;
        }
        public bool getAvailability() {
            return isAvailable;

        }
        public void setValue(ushort val) {
            value = val;
            isAvailable = false;
        }
        public void increaseValue() {
            if (!isAvailable) { 
                value++;
            }
        }
        public void clear() {
            value = 0;
            isAvailable = true;
        }

        public ushort getValue() {
            return value;
        }
    }
}
