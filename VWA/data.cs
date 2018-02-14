using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class data
    {
        public static char[] PceChar = {'.','B','P','L','T','D','K','b','p','l','t','d','k'};
        public static char[] SideChar = { 'w', 's', '-' };
        public static char[] RankChar = { '1', '2', '3', '4', '5', '6', '7', '8' };
        public static char[] FileChar = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public static bool[] PieceBig = { false, false, true, true, true, true, true, false, true, true, true, true, true };
        public static bool[] PieceMaj = { false, false, false, false, true, true, true, false, false, false, true, true, true };
        public static bool[] PieceMin = { false, false, true, true, false, false, false, false, true, true, false, false, false };
        public static int[] PieceVal = { 0, 100, 325, 325, 550, 1000, 50000, 100, 325, 325, 550, 1000, 50000 };
        public static int[] PieceCol = { (int)defs.Farben.BEIDE, (int)defs.Farben.WEISS, (int)defs.Farben.WEISS, (int)defs.Farben.WEISS, (int)defs.Farben.WEISS, (int)defs.Farben.WEISS, (int)defs.Farben.WEISS, (int)defs.Farben.SCHWARZ, (int)defs.Farben.SCHWARZ, (int)defs.Farben.SCHWARZ, (int)defs.Farben.SCHWARZ, (int)defs.Farben.SCHWARZ, (int)defs.Farben.SCHWARZ };

        public static bool[] PiecePawn = { false, true, false, false, false, false, false, true, false, false, false, false, false };
        public static bool[] PieceKnight = { false, false, true, false, false, false, false, false, true, false, false, false, false };
        public static bool[] PieceKing = { false, false, false, false, false, false, true, false, false, false, false, false, true };
        public static bool[] PieceRookQueen = { false, false, false, false, true, true, false, false, false, false, true, true, false };
        public static bool[] PieceBishopQueen = { false, false, false, true, false, true, false, false, false, true, false, true, false };
        public static bool[] PieceSlides = { false, false, false, true, true, true, false, false, false, true, true, true, false };



        public static int[] Mirror64 = {   56  ,   57  ,   58  ,   59  ,   60  ,   61  ,   62  ,   63  ,
                                    48  ,   49  ,   50  ,   51  ,   52  ,   53  ,   54  ,   55  ,
                                    40  ,   41  ,   42  ,   43  ,   44  ,   45  ,   46  ,   47  ,
                                    32  ,   33  ,   34  ,   35  ,   36  ,   37  ,   38  ,   39  ,
                                    24  ,   25  ,   26  ,   27  ,   28  ,   29  ,   30  ,   31  ,
                                    16  ,   17  ,   18  ,   19  ,   20  ,   21  ,   22  ,   23  ,
                                    8   ,   9   ,   10  ,   11  ,   12  ,   13  ,   14  ,   15  ,
                                    0   ,   1   ,   2   ,   3   ,   4   ,   5   ,   6   ,   7   };
    }
}
