using System;
using System.Collections.Generic;
using System.Text;

namespace _2e11
{
    class Game
    {
        bool isWon;
        bool isLost;
        ushort boardSize = 4;

        Tile[][] board;
        

        // Empty constructor
        Game(){
            initializeBoard();
            

        }

        void initializeBoard() {
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

        void addRandomTile() {
            if (cellsAvailable()) {
                //var value = Math.random() < 0.9 ? 2 : 4;
                //var tile = new Tile(this.grid.randomAvailableCell(), value);

                //this.grid.insertTile(tile);
            }
        }
    }
}
