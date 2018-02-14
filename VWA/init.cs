using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class init
    {
        public static void AllInit(ref boardStruct brett)
        {
            InitSq120To64();
            InitBitMasks();
            InitHashKeys();
            InitFilesRanksBrd();
            PvTable.InitHashTable(ref brett.PvTable, 500);
            movegen.InitMvvLva();
            InitEvaluationMasks();
            polybook.InitPolyBook();
        }

        static void InitEvaluationMasks()
        {
            int sq, tsq, r, f;

            for (sq = 0; sq < 8; ++sq)
            {
                defs.FileMask[sq] = 0UL;
                defs.RankMask[sq] = 0UL;
            }

            for (r = (int)defs.Zeilen.ZEILE_8; r >= (int)defs.Zeilen.ZEILE_1; r--)
            {
                for (f = (int)defs.Spalten.SPALTE_A; f <= (int)defs.Spalten.SPALTE_H; f++)
                {
                    sq = r * 8 + f;
                    defs.FileMask[f] |= (1UL << sq);
                    defs.RankMask[r] |= (1UL << sq);
                }
            }

	        for(sq = 0; sq< 64; ++sq)
            {
                defs.IsolatedMask[sq] = 0UL;
                defs.WhitePassedMask[sq] = 0UL;
                defs.BlackPassedMask[sq] = 0UL;
            }

            for (sq = 0; sq < 64; ++sq)
            {
                tsq = sq + 8;

                while (tsq < 64)
                {
                    defs.WhitePassedMask[sq] |= (1UL << tsq);
                    tsq += 8;
                }

                tsq = sq - 8;
                while (tsq >= 0)
                {
                    defs.BlackPassedMask[sq] |= (1UL << tsq);
                    tsq -= 8;
                }

                if (defs.FilesBrd[defs.Sq64ToSq120[sq]] > (int)defs.Spalten.SPALTE_A)
                {
                    defs.IsolatedMask[sq] |= defs.FileMask[defs.FilesBrd[defs.Sq64ToSq120[sq]] - 1];

                    tsq = sq + 7;
                    while (tsq < 64)
                    {
                        defs.WhitePassedMask[sq] |= (1UL << tsq);
                        tsq += 8;
                    }

                    tsq = sq - 9;
                    while (tsq >= 0)
                    {
                        defs.BlackPassedMask[sq] |= (1UL << tsq);
                        tsq -= 8;
                    }
                }


                if (defs.FilesBrd[defs.Sq64ToSq120[sq]] < (int)defs.Spalten.SPALTE_H)
                {
                    defs.IsolatedMask[sq] |= defs.FileMask[defs.FilesBrd[defs.Sq64ToSq120[sq]] + 1];

                    tsq = sq + 9;
                    while (tsq < 64)
                    {
                        defs.WhitePassedMask[sq] |= (1UL << tsq);
                        tsq += 8;
                    }

                    tsq = sq - 7;
                    while (tsq >= 0)
                    {
                        defs.BlackPassedMask[sq] |= (1UL << tsq);
                        tsq -= 8;
                    }
                }
            }
        }


        static void InitFilesRanksBrd()
        {

            int index = 0;
            int file = (int)defs.Spalten.SPALTE_A;
            int rank = (int)defs.Zeilen.ZEILE_1;
            int sq = (int)defs.Felder.A1;

            for (index = 0; index < defs.BRD_SQ_NUM; ++index)
            {
                defs.FilesBrd[index] = (int)defs.Felder.OFFBOARD;
                defs.RanksBrd[index] = (int)defs.Felder.OFFBOARD;
            }

            for (rank = (int)defs.Zeilen.ZEILE_1; rank <= (int)defs.Zeilen.ZEILE_8; ++rank)
            {
                for (file = (int)defs.Spalten.SPALTE_A; file <= (int)defs.Spalten.SPALTE_H; ++file)
                {
                    sq = makros.SZ2SQ(file, rank);
                    defs.FilesBrd[sq] = file;
                    defs.RanksBrd[sq] = rank;
                }
            }
        }

        static void InitHashKeys()
        {
            Random rnd = new Random();
            byte[] bytes = new byte[8];

            for (int index = 0; index < 13; ++index)
            {
                for (int index2 = 0; index2 < 120; ++index2)
                {
                    rnd.NextBytes(bytes);
                    hashkeys.PieceKeys[index,index2] = BitConverter.ToUInt64(bytes, 0);
                }
            }
            rnd.NextBytes(bytes);
            hashkeys.SideKey = BitConverter.ToUInt64(bytes, 0);
            for (int index = 0; index < 16; ++index)
            {
                rnd.NextBytes(bytes);
                hashkeys.CastleKeys[index] = BitConverter.ToUInt64(bytes, 0);
            }

        }


        static void InitBitMasks()
        {
            int index = 0;

            for (index = 0; index < 64; index++)
            {
                bitboards.SetMask[index] = 0UL;
                bitboards.ClearMask[index] = 0UL;
            }

            for (index = 0; index < 64; index++)
            {
                bitboards.SetMask[index] |= (1UL << index);
                bitboards.ClearMask[index] = ~bitboards.SetMask[index];
            }
        }

        static void InitSq120To64()
        {
            int spalte = (int)defs.Spalten.SPALTE_A;
            int zeile = (int)defs.Zeilen.ZEILE_1;
            int sq = (int)defs.Felder.A1;
            int sq64 = 0;

            for (int index = 0; index < defs.BRD_SQ_NUM; ++index)
            {
                defs.Sq120ToSq64[index] = 65;
            }

            for (int index = 0; index < 64; ++index)
            {
                defs.Sq64ToSq120[index] = 120;
            }

            for (zeile = (int)defs.Zeilen.ZEILE_1; zeile <= (int)defs.Zeilen.ZEILE_8; ++zeile)
            {
                for (spalte = (int)defs.Spalten.SPALTE_A; spalte <= (int)defs.Spalten.SPALTE_H; ++spalte)
                {
                    sq = makros.SZ2SQ(spalte, zeile);
                    defs.Sq64ToSq120[sq64] = sq;
                    defs.Sq120ToSq64[sq] = sq64;
                    sq64++;
                }
            }
        }
    }
}
