﻿using Rudim.Common;
using System.Numerics;

namespace Rudim.Board
{
    internal static class SimpleEvaluation
    {
        private static readonly int[] PieceValues;
        private static readonly int[,] PositionValues;
        private static readonly int[] MidGameKingValues;
        private static readonly int[] EndGameKingValues;


        public static int Evaluate(BoardState boardState)
        {
            int score = 0;

            score += ScoreMaterial(boardState);
            score += ScorePosition(boardState);

            return boardState.SideToMove == Side.White ? score : -score;
        }

        private static int ScorePosition(BoardState boardState)
        {
            int positionalScore = 0;
            int midGamePhase = GamePhase.Calculate(boardState);
            int endGamePhase = GamePhase.TotalPhase - midGamePhase;
            for (int piece = 0; piece < Constants.Pieces; ++piece)
            {
                Bitboard whiteBoard = new Bitboard(boardState.Pieces[(int)Side.White, piece].Board);
                Bitboard blackBoard = new Bitboard(boardState.Pieces[(int)Side.Black, piece].Board);

                if (piece == Constants.Pieces - 1)
                {
                    int whiteKing = whiteBoard.GetLsb();
                    int blackKing = MirrorSquare(blackBoard.GetLsb());

                    positionalScore += (int)(((MidGameKingValues[whiteKing] * midGamePhase) + (EndGameKingValues[whiteKing] * endGamePhase)) * GamePhase.PhaseFactor);
                    positionalScore -= (int)(((MidGameKingValues[blackKing] * midGamePhase) + (EndGameKingValues[blackKing] * endGamePhase)) * GamePhase.PhaseFactor);
                    continue;
                }

                while (whiteBoard.Board > 0)
                {
                    int square = whiteBoard.GetLsb();
                    whiteBoard.ClearBit(square);
                    positionalScore += PositionValues[piece, square];
                }

                while (blackBoard.Board > 0)
                {
                    int square = blackBoard.GetLsb();
                    blackBoard.ClearBit(square);
                    square = MirrorSquare(square);
                    positionalScore -= PositionValues[piece, square];
                }
            }
            return positionalScore;
        }

        private static int MirrorSquare(int square)
        {
            int row = square >> 3;
            int col = square - (square >> 3) * 8;

            return (7 - row) * 8 + col;
        }

        private static int ScoreMaterial(BoardState boardState)
        {
            int materialScore = 0;
            for (int piece = 0; piece < Constants.Pieces; ++piece)
            {
                materialScore += PieceValues[piece] * BitOperations.PopCount(boardState.Pieces[(int)Side.White, piece].Board);
                materialScore -= PieceValues[piece] * BitOperations.PopCount(boardState.Pieces[(int)Side.Black, piece].Board);
            }
            return materialScore;
        }

        static SimpleEvaluation()
        {
            PieceValues = [100, 320, 330, 500, 900, 20000];
            PositionValues = new[,]
            { { 0,  0,  0,  0,  0,  0,  0,  0,
                50, 50, 50, 50, 50, 50, 50, 50,
                10, 10, 20, 30, 30, 20, 10, 10,
                5,  5, 10, 25, 25, 10,  5,  5,
                0,  0,  0, 20, 20,  0,  0,  0,
                5, -5,-10,  0,  0,-10, -5,  5,
                5, 10, 10,-20,-20, 10, 10,  5,
                0,  0,  0,  0,  0,  0,  0,  0},
               { -50,-40,-30,-30,-30,-30,-40,-50,
                 -40,-20,  0,  0,  0,  0,-20,-40,
                 -30,  0, 10, 15, 15, 10,  0,-30,
                 -30,  5, 15, 20, 20, 15,  5,-30,
                 -30,  0, 15, 20, 20, 15,  0,-30,
                 -30,  5, 10, 15, 15, 10,  5,-30,
                 -40,-20,  0,  5,  5,  0,-20,-40,
                 -50,-40,-30,-30,-30,-30,-40,-50},
               { -20,-10,-10,-10,-10,-10,-10,-20,
                 -10,  0,  0,  0,  0,  0,  0,-10,
                 -10,  0,  5, 10, 10,  5,  0,-10,
                 -10,  5,  5, 10, 10,  5,  5,-10,
                 -10,  0, 10, 10, 10, 10,  0,-10,
                 -10, 10, 10, 10, 10, 10, 10,-10,
                 -10,  5,  0,  0,  0,  0,  5,-10,
                 -20,-10,-10,-10,-10,-10,-10,-20},
               { 0,  0,  0,  0,  0,  0,  0,  0,
                 5, 10, 10, 10, 10, 10, 10,  5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 0,  0,  0,  5,  5,  0,  0,  0},
               { -20,-10,-10, -5, -5,-10,-10,-20,
                 -10,  0,  0,  0,  0,  0,  0,-10,
                 -10,  0,  5,  5,  5,  5,  0,-10,
                 -5,  0,  5,  5,  5,  5,  0, -5,
                 0,  0,  5,  5,  5,  5,  0, -5,
                 -10,  5,  5,  5,  5,  5,  0,-10,
                 -10,  0,  5,  0,  0,  0,  0,-10,
                 -20,-10,-10, -5, -5,-10,-10,-20}
            };
            MidGameKingValues =
            [
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -20,-30,-30,-40,-40,-30,-30,-20,
                -10,-20,-20,-20,-20,-20,-20,-10,
                20, 20,  0,  0,  0,  0, 20, 20,
                20, 30, 10,  0,  0, 10, 30, 20
            ];
            EndGameKingValues =
            [
                -50,-40,-30,-20,-20,-30,-40,-50,
                -30,-20,-10,  0,  0,-10,-20,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-30,  0,  0,  0,  0,-30,-30,
                -50,-30,-30,-30,-30,-30,-30,-50
            ];
        }
    }
}