using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class bitboards
    {
        public static readonly int[] BitTable = {63, 30, 3, 32, 25, 41, 22, 33, 15, 50, 42, 13, 11, 53, 19, 34, 61, 29, 2,51, 21, 43, 45, 10, 18, 47, 1, 54, 9, 57, 0, 35, 62, 31, 40, 4, 49, 5, 52,26, 60, 6, 23, 44, 46, 27, 56, 16, 7, 39, 48, 24, 59, 14, 12, 55, 38, 28,58, 20, 37, 17, 36, 8};

        //masken zum setten von bits und clearn von bitboards
        public static ulong[] SetMask = new ulong[64];
        public static ulong[] ClearMask = new ulong[64];

        public static int PopBit(ref ulong bb)
        {
            ulong b = bb ^ (bb - 1);
            uint fold = Convert.ToUInt32(((b & 0xffffffff) ^ (b >> 32)));
            bb &= (bb - 1);
            return BitTable[(fold * 0x783a9b23) >> 26];
        }

        public static int CountBits(ulong a)
        {
            int count = 0;
            while (a != 0)
            {
                count++;
                a &= (a - 1);
            }
            return count;
        }

        public static string PrintBitBoard(ulong bb)
        {
            ulong shiftMe = 1;

            string ret = "";
            int zeile = 0;
            int spalte = 0;
            int sq = 0;
            int sq64 = 0;

            ret += "\n";
            for (zeile = (int)defs.Zeilen.ZEILE_8; zeile >= (int)defs.Zeilen.ZEILE_1; --zeile)
            {
                for (spalte = (int)defs.Spalten.SPALTE_A; spalte <= (int)defs.Spalten.SPALTE_H; ++spalte)
                {
                    sq = makros.SZ2SQ(spalte, zeile);	
                    sq64 = defs.Sq120ToSq64[sq];
                    if (((shiftMe << sq64) & bb) != 0)
                        ret += "X";
                    else
                        ret += "-";
                }
                ret += "\n";
            }
            ret += "\n\n";

            return ret;
        }
    }
}
