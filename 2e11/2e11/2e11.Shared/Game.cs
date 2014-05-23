using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Game
    {
        bool isWon;
        bool isLost;
        static ushort boardSize = 4;
        static ushort startTiles = 2;

        int score;

        Tile[][] board;
        Random rnd;

        // Empty constructor
        Game() {
            rnd = new Random();
            score = 0;
            isWon = false;
            isLost = false;

            initializeBoard();
            addStartTiles();
        }

        void initializeBoard()
        {
            board = new Tile[boardSize][];

            for (ushort i = 0; i < boardSize; i++)
            {
                board[i] = new Tile[boardSize];
            }
        }

        void insertTile(Tile tile){


        }

        bool cellsAvailable(){
            for (ushort i = 0; i < boardSize; i++){
                for (ushort j = 0; j < boardSize; j++){
                    if (board[i][j].getAvailability())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ushort getNumberOfAvailableCells()
        {
            ushort ret = 0;
            for (ushort i = 0; i < boardSize; i++)
            {
                for (ushort j = 0; j < boardSize; j++)
                {
                    if (board[i][j].getAvailability())
                    {
                        ret++;
                    }
                }
            }

            return ret;
        }

        void addRandomTile() {
            if (cellsAvailable()) {
                ushort value = (ushort) rnd.Next(1, 2);

                ushort pos = (ushort)rnd.Next(0, getNumberOfAvailableCells());

                ushort ret = 0;
                for (ushort i = 0; i < boardSize; i++)
                {
                    for (ushort j = 0; j < boardSize; j++)
                    {
                        if (board[i][j].getAvailability())
                        {
                            ret++;
                        }

                        if (ret == pos) {
                            board[i][j].setValue(value);
                        }
                    }
                }
            }
        }

        void updateScore()
        {
            //this.clearContainer(this.scoreContainer);

            var difference = score - this.score;

            //this.scoreContainer.textContent = this.score;

        }

        // Set up the initial tiles to start the game with
        void addStartTiles() {
            for (var i = 0; i < startTiles; i++) {
                this.addRandomTile();
            }
        }


    }
}
