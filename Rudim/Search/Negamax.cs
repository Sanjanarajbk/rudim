﻿using Rudim.Board;
using Rudim.Common;
using System.Threading;

namespace Rudim.Search
{
    static class Negamax
    {
        public static Move BestMove;
        public static int Nodes = 0;
        private static int _searchDepth = 0;

        private static int Search(BoardState boardState, int depth, int alpha, int beta, CancellationToken cancellationToken)
        {
            if (boardState.IsDraw())
                return 0;

            if (depth == 0)
                return Quiescent.Search(boardState, alpha, beta, cancellationToken);

            Nodes++;
            int originalAlpha = alpha;
            Move bestEvaluation = Move.NoMove;

            boardState.GenerateMoves();
            int ply = _searchDepth - depth;
            // TODO : Flag in GenerateMoves to avoid extra iteration?
            foreach (Move move in boardState.Moves)
            {
                MoveOrdering.PopulateMoveScore(move, boardState, ply);
            }

            MoveOrdering.SortMoves(boardState);

            int numberOfLegalMoves = 0;
            foreach (Move move in boardState.Moves)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                boardState.MakeMove(move);
                if (boardState.IsInCheck(boardState.SideToMove.Other()))
                {
                    boardState.UnmakeMove(move);
                    continue;
                }
                int score = -Search(boardState, depth - 1, -beta, -alpha, cancellationToken);
                boardState.UnmakeMove(move);
                numberOfLegalMoves++;
                if (score >= beta)
                {
                    if (!move.IsCapture())
                        MoveOrdering.AddKillerMove(move, ply);
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                    bestEvaluation = move;
                }
            }

            if (numberOfLegalMoves == 0)
            {
                if (boardState.IsInCheck(boardState.SideToMove))
                    return -Constants.MaxCentipawnEval + (_searchDepth - depth);
                return 0;
            }

            if (alpha != originalAlpha)
                BestMove = bestEvaluation;

            return alpha;
        }

        public static int Search(BoardState boardState, int depth, CancellationToken cancellationToken)
        {
            _searchDepth = depth;
            Nodes = 0;
            BestMove = Move.NoMove;
            Quiescent.ResetNodes();
            int score = Search(boardState, depth, int.MinValue + 1, int.MaxValue - 1, cancellationToken);
            if (BestMove == Move.NoMove)
            {
                boardState.GenerateMoves();
                BestMove = boardState.Moves[0];
            }
            return score;
        }
    }
}