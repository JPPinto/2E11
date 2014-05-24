using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11 {
    class Game {
        static int[] representation = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072 };
        bool isWon;
        bool isLost;
        static ushort boardSize = 4;
        static ushort startTiles = 2;

        int score;

        Tile[,] board;
        Random rnd;

        // Initial constructor (only call once)
        Game() {
            board = new Tile[boardSize, boardSize];
            resetBoard();
            addStartTiles();
        }

        public void resetBoard() {
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
                ushort value = (ushort)rnd.Next(1, 2);

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
            score += representation[board[mergedX, mergedY].getValue()];
        }

        // Set up the initial tiles to start the game with
        void addStartTiles() {
            for (var i = 0; i < startTiles; i++) {
                this.addRandomTile();
            }
        }


    }
}
