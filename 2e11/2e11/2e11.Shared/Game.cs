using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11 {
    class Game {
        public static readonly String[] representation = { "2", "4", "8", "16", "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192", "16384", "32768", "65536", "131072" };
        public static readonly int[] values = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072 };
        public static readonly ushort boardSize = 4;
        public static readonly ushort startTiles = 2;

        public bool isWon;
        public bool isLost;
        public ulong score;
        ushort[,] board;
        Random rnd = new Random();

        // Initial constructor (only call once)
        public Game() {
            board = new ushort[boardSize, boardSize];
            newGame();
        }
        public void newGame() {
            resetBoard();
            addStartTiles();
        }

        public void resetBoard() {
            score = 0;
            isWon = false;
            isLost = false;

            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    board[i, j] = 0;
                }
            }
        }
        private void PutNewValue() {
            // Find all empty slots
            List<Tuple<ushort, ushort>> emptySlots = new List<Tuple<ushort, ushort>>();

            for (ushort iRow = 0; iRow < boardSize; iRow++) {
                for (ushort iCol = 0; iCol < boardSize; iCol++) {
                    if (board[iRow, iCol] == 0) {
                        emptySlots.Add(new Tuple<ushort, ushort>(iRow, iCol));
                    }
                }
            }

            // We should have at least 1 empty slot. Since we know the user is not dead
            ushort iSlot = (ushort) rnd.Next(0, emptySlots.Count);
            ushort value = rnd.Next(0, 100) < 95 ? (ushort)1 : (ushort)2;
            board[emptySlots[iSlot].Item1, emptySlots[iSlot].Item2] = value;

            // Check if we died
            updateLostBool();
        }
        public void updateScore(ushort mergedX, ushort mergedY) {
            score += (ulong)values[board[mergedX, mergedY] - 1];
        }
        public void addStartTiles() {
            for (var i = 0; i < startTiles; i++) {
                PutNewValue();
            }
        }
        public Boolean getIsWon() {
            return isWon;
        }
        public void moveLeft() {
            if (isLost) { 
                return;
            }
            ulong temp;
            if (Update(this.board, Direction.Left, out temp)) {
                PutNewValue();
            }
            score+=temp;
        }
        public void moveRight() {
            if (isLost) {
                return;
            }
            ulong temp;
            if (Update(this.board, Direction.Right, out temp)) {
                PutNewValue();
            }
            score += temp;
        }
        public void moveUp() {
            if (isLost) {
                return;
            }
            ulong temp;
            if (Update(this.board, Direction.Up, out temp)) {
                PutNewValue();
            }
            score += temp;
        }
        public void moveDown() {
            if (isLost) {
                return;
            }
            ulong temp;
            if(Update(this.board, Direction.Down, out temp)){
                PutNewValue();
            }
            score += temp;
        }
        private static bool Update(ushort[,] boardIn, Direction direction, out ulong scoreIn) {
            scoreIn = 0;
            bool hasUpdated = false;

            // You shouldn't be dead at this point. We always check if you're dead at the end of the Update()

            // Drop along row or column? true: process inner along row; false: process inner along column
            bool isAlongRow = direction == Direction.Left || direction == Direction.Right;

            // Should we process inner dimension in increasing index order?
            bool isIncreasing = direction == Direction.Left || direction == Direction.Up;

            ushort outterCount = isAlongRow ? boardSize : boardSize;
            ushort innerCount  = isAlongRow ? boardSize : boardSize;
            ushort innerStart  = (ushort) (isIncreasing ? 0 : innerCount - 1);
            ushort innerEnd    = (ushort) (isIncreasing ? innerCount - 1 : 0);

            Func<ushort, ushort> drop = isIncreasing
                ? new Func<ushort, ushort>(innerIndex => (ushort) (innerIndex - 1))
                : new Func<ushort, ushort>(innerIndex => (ushort) (innerIndex + 1));

            Func<ushort, ushort> reverseDrop = isIncreasing
                ? new Func<ushort, ushort>(innerIndex => (ushort) (innerIndex + 1))
                : new Func<ushort, ushort>(innerIndex => (ushort) (innerIndex - 1));

            Func<ushort, bool> innerCondition = index => Math.Min(innerStart, innerEnd) <= index && index <= Math.Max(innerStart, innerEnd);

            Func<ushort, ushort, ushort> getValue = isAlongRow
                ? new Func<ushort, ushort, ushort>((i, j) => boardIn[i, j])
                : new Func<ushort, ushort, ushort>((i, j) => boardIn[j, i]);

            Action<ushort, ushort, ushort> setValue = isAlongRow
                ? new Action<ushort, ushort, ushort>((i, j, v) => boardIn[i, j] = v)
                : new Action<ushort, ushort, ushort>((i, j, v) => boardIn[j, i] = v);

            for (ushort i = 0; i < outterCount; i++) {

                for (ushort j = innerStart; innerCondition(j); j = reverseDrop(j)) {

                    if (getValue(i, j) == 0) {
                        continue;
                    }

                    ushort newJ = j;
                    do {
                        newJ = drop(newJ);
                    }
                    // Continue probing along as long as we haven't hit the boundary and the new position isn't occupied
                    while (innerCondition(newJ) && getValue(i, newJ) == 0);

                    if (innerCondition(newJ) && getValue(i, newJ) == getValue(i, j)) {
                        // We did not hit the canvas boundary (we hit a node) AND no previous merge occurred AND the nodes' values are the same
                        // Let's merge
                        ushort newValue = (ushort) (getValue(i, newJ) + 1);
                        setValue(i, newJ, newValue);
                        setValue(i, j, 0);

                        hasUpdated = true;
                        scoreIn = (ulong) values[newValue];

                    } else {
                        // Reached the boundary OR...
                        // we hit a node with different value OR...
                        // we hit a node with same value BUT a prevous merge had occurred
                        // 
                        // Simply stack along
                        newJ = reverseDrop(newJ); // reverse back to its valid position
                        if (newJ != j) {
                            // there's an update
                            hasUpdated = true;
                        }

                        ushort value = getValue(i, j);
                        setValue(i, j, 0);
                        setValue(i, newJ, value);
                    }
                }
            }

            return hasUpdated;
        }

        public void afterMoveChecks() {
            updateVictoryBool();
            updateLostBool();

            if (!isLost) {
                PutNewValue();
            }
        }
        void updateVictoryBool() {

        }
        void updateLostBool() {
            ulong scoreTemp;
            foreach (Direction dir in new Direction[] { Direction.Down, Direction.Up, Direction.Left, Direction.Right })
            {
                ushort[,] clone = (ushort[,])board.Clone();
                if (Game.Update(clone, dir, out scoreTemp))
                {
                    isLost = false;
                    return;
                }
            }

            // tried all directions. none worked.
            isLost = true;
        }
        public String toString() {
            String ret = "";

            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    if (board[i, j] != 0) {
                        ret += representation[board[i, j] - 1];
                    }
                    ret += "; ";
                }
                ret += "\n";
            }
            return ret;
        }

        public ushort[,] getBoard() {
            return this.board;
        }

        public ulong getScore(){
            return score;
        }

        enum Direction {
            Up,
            Down,
            Right,
            Left,
        }
    }
}
