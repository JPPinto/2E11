using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11 {
    class Game {
        static readonly String[] representation = { "2", "4", "8", "16", "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192", "16384", "32768", "65536", "131072" };
        static readonly int[] values = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072 };
        static readonly ushort boardSize = 4;
        static readonly ushort startTiles = 2;

        bool isWon;
        bool isLost;
        ulong score;

        Tile[,] board;
        Random rnd;

        // Initial constructor (only call once)
        public Game() {
            initializeBoard();

            resetBoard();
            addStartTiles();
        }
        public void newGame() {
            resetBoard();
            addStartTiles();
        }
        void initializeBoard() {
            board = new Tile[boardSize, boardSize];

            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    board[i, j] = new Tile();
                }
            }
        }
        void resetBoard() {
            rnd = new Random();
            score = 0;
            isWon = false;
            isLost = false;

            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    board[i, j].clear();
                }
            }
        }
        bool cellsAvailable() {
            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    if (board[i, j].getAvailability()) {
                        return true;
                    }
                }
            }
            return false;
        }
        ushort getNumberOfAvailableCells() {
            ushort ret = 0;
            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    if (board[i, j].getAvailability()) {
                        ret++;
                    }
                }
            }

            return ret;
        }
        void addRandomTile() {
            if (cellsAvailable()) {
                // 95% for 2 5% for 4 
                ushort value = (ushort)rnd.Next(0, 100) < 95 ? (ushort)1 : (ushort)2;

                ushort pos = (ushort)rnd.Next(0, getNumberOfAvailableCells());

                ushort ret = 0;
                for (ushort i = 0; i < boardSize; i++) {
                    for (ushort j = 0; j < boardSize; j++) {
                        if (board[i, j].getAvailability()) {
                            ret++;
                        }

                        if (ret == pos) {
                            board[i, j].setValue(value);
                        }
                    }
                }
            }
        }
        void updateScore(ushort mergedX, ushort mergedY) {
            score += (ulong)values[board[mergedX, mergedY].getValue() - 1];
        }
        void addStartTiles() {
            for (var i = 0; i < startTiles; i++) {
                addRandomTile();
            }
        }
        public Boolean getIsWon() {
            return isWon;
        }
        public void moveLeft() {
            bool needsReRun = true;
            bool actualMoveDone = false;

            while (needsReRun) {
                needsReRun = false;

                for (ushort i = 0; i < boardSize; i++) {
                    for (ushort j = 0; j < boardSize - 1; j++) {
                        // Move
                        if (board[i, j].getAvailability() && !(board[i, j + 1].getAvailability())) {
                            board[i, j].setValue(board[i, j + 1].getValue());
                            board[i, j + 1].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }

                        // Merge
                        if ((board[i,j].getValue() != 0) && board[i, j].getValue() == board[i, j + 1].getValue()) {
                            board[i, j].increaseValue();
                            board[i, j + 1].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }
                    }
                }
            }

            if (actualMoveDone) {
                afterMoveChecks();
            }
        }
        public void moveRight() {
            bool needsReRun = true;
            bool actualMoveDone = false;

            while (needsReRun) {
                needsReRun = false;
                for (ushort i = 0; i < boardSize; i++) {
                    for (ushort j = 0; j < boardSize - 1; j++) {
                        // Move
                        if (board[i, j + 1].getAvailability() && !(board[i, j].getAvailability())) {
                            board[i, j + 1].setValue(board[i, j].getValue());
                            board[i, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }

                        // Merge
                        if ((board[i, j].getValue() != 0) && board[i, j].getValue() == board[i, j + 1].getValue()) {
                            board[i, j + 1].increaseValue();
                            board[i, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }
                    }
                }
            }

            if (actualMoveDone) {
                afterMoveChecks();
            }
        }
        public void moveUp() {
            bool needsReRun = true;
            bool actualMoveDone = false;

            while (needsReRun) {
                needsReRun = false;

                for (ushort i = 0; i < boardSize - 1; i++) {
                    for (ushort j = 0; j < boardSize; j++) {
                        // Move
                        if (board[i, j].getAvailability() && !(board[i + 1, j].getAvailability())) {
                            board[i, j].setValue(board[i + 1, j].getValue());
                            board[i + 1, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }

                        // Merge
                        if ((board[i, j].getValue() != 0) && board[i, j].getValue() == board[i + 1, j].getValue()) {
                            board[i, j].increaseValue();
                            board[i + 1, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }
                    }
                }
            }

            if (actualMoveDone) {
                afterMoveChecks();
            }
        }
        public void moveDown() {
            bool needsReRun = true;
            bool actualMoveDone = false;

            while (needsReRun) {
                needsReRun = false;

                for (ushort i = 0; i < boardSize - 1; i++) {
                    for (ushort j = 0; j < boardSize; j++) {
                        // Move
                        if (board[i + 1, j].getAvailability() && !(board[i, j].getAvailability())) {
                            board[i + 1, j].setValue(board[i, j].getValue());
                            board[i, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }

                        // Merge
                        if ((board[i, j].getValue() != 0) && board[i, j].getValue() == board[i + 1, j].getValue()) {
                            board[i + 1, j].increaseValue();
                            board[i, j].clear();
                            needsReRun = true;
                            actualMoveDone = true;
                            break;
                        }
                    }
                }
            }

            if (actualMoveDone) {
                afterMoveChecks();
            }
        }
        void afterMoveChecks() {
            updateVictoryBool();
            updateLostBool();

            if (!isLost) { 
                addRandomTile();
            }
        }
        void updateVictoryBool() {

        }
        void updateLostBool() {

        }
        bool areMovesAvailable() {
            return false;
        }
        public String toString() {
            String ret = "";

            for (ushort i = 0; i < boardSize; i++) {
                for (ushort j = 0; j < boardSize; j++) {
                    if (!board[i, j].getAvailability()) {
                        ret += representation[board[i, j].getValue() - 1];
                    }
                    ret += "; ";
                }
                ret += "\n";
            }
            return ret;
        }
    }
}
