using Rudim.Common;

namespace Rudim.Board
{
    public static class History
    {
        private const int HistorySize = 4096;
        private static readonly BoardHistory[] BoardHistories = new BoardHistory[HistorySize];
        private static int _historyIndex;


        public static void SaveBoardHistory(Piece capturedPiece, Square enPassant, Castle originalCastlingRights,
            ulong boardHash, int lastDrawKiller)
        {
            BoardHistories[_historyIndex++] = new BoardHistory
            {
                CapturedPiece = capturedPiece,
                EnPassantSquare = enPassant,
                CastlingRights = originalCastlingRights,
                BoardHash = boardHash,
                LastDrawKiller = lastDrawKiller
            };
        }

        public static BoardHistory RestoreBoardHistory()
        {
            return BoardHistories[--_historyIndex];
        }

        public static void ClearBoardHistory()
        {
            _historyIndex = 0;
        }

        public class BoardHistory
        {
            public Piece CapturedPiece { get; set; }
            public Square EnPassantSquare { get; set; }
            public Castle CastlingRights { get; internal set; }
            public ulong BoardHash { get; set; }
            public int LastDrawKiller { get; set; }
        }

        public static bool HasHashAppearedTwice(ulong boardHash, int startingIndex)
        {
            int count = 0;
            for (int i = _historyIndex - 1; i >= startingIndex; --i)
            {
                if (BoardHistories[i].BoardHash == boardHash)
                    count++;

                if (count == 2)
                    return true;
            }

            return false;
        }
    }
}