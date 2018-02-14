using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class evaluate
    {
        static int PawnIsolated = -10;
        static int[] PawnPassed = { 0, 5, 10, 20, 35, 60, 100, 200 };
        static int RookOpenFile = 10;
        static int RookSemiOpenFile = 5;
        static int QueenOpenFile = 5;
        static int QueenSemiOpenFile = 3;
        static int BishopPair = 30;

        static int[] PawnTable = {  0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                                    10  ,   10  ,   0   ,   -10 ,   -10 ,   0   ,   10  ,   10  ,
                                    5   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   5   ,
                                    0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   0   ,   0   ,
                                    5   ,   5   ,   5   ,   10  ,   10  ,   5   ,   5   ,   5   ,
                                    10  ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   10  ,
                                    20  ,   20  ,   20  ,   30  ,   30  ,   20  ,   20  ,   20  ,
                                    0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   };

        static int[] KnightTable = {0   ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   0   ,
                                    0   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   0   ,
                                    0   ,   0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   0   ,
                                    0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   5   ,   0   ,
                                    5   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   5   ,
                                    5   ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   5   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   };

        static int[] BishopTable = {0   ,   0   ,   -10 ,   0   ,   0   ,   -10 ,   0   ,   0   ,
                                    0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
                                    0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
                                    0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
                                    0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
                                    0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
                                    0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
                                    0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   };

        static int[] RookTable = {  0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
                                    25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,
                                    0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   };

        static int[] KingE = {  -50 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -50 ,
                                -10,    0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   -10 ,
                                0   ,   10  ,   20  ,   20  ,   20  ,   20  ,   10  ,   0   ,
                                0   ,   10  ,   20  ,   40  ,   40  ,   20  ,   10  ,   0   ,
                                0   ,   10  ,   20  ,   40  ,   40  ,   20  ,   10  ,   0   ,
                                0   ,   10  ,   20  ,   20  ,   20  ,   20  ,   10  ,   0   ,
                                -10,    0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   -10 ,
                                -50 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -50  };

        static int[] KingO = {  0   ,   5   ,   5   ,   -10 ,   -10 ,   0   ,   10  ,   5   ,
                                -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,
                                -50 ,   -50 ,   -50 ,   -50 ,   -50 ,   -50 ,   -50 ,   -50 ,
                                -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
                                -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
                                -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
                                -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
                                -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 };


        static bool MaterialDraw(ref boardStruct brett)
        {
	
            if (brett.pceNum[(int)defs.Figuren.wT] == 0 && brett.pceNum[(int)defs.Figuren.sT] == 0 && brett.pceNum[(int)defs.Figuren.wD] == 0 && brett.pceNum[(int)defs.Figuren.sD] == 0)
            {
	            if (brett.pceNum[(int)defs.Figuren.sL] == 0 && brett.pceNum[(int)defs.Figuren.wL] == 0)
                {
	                if (brett.pceNum[(int)defs.Figuren.wP] < 3 && brett.pceNum[(int)defs.Figuren.sP] < 3) return true;
                }
                else if (brett.pceNum[(int)defs.Figuren.wP] == 0 && brett.pceNum[(int)defs.Figuren.sP] == 0)
                {
	                if (Math.Abs(brett.pceNum[(int)defs.Figuren.wL] - brett.pceNum[(int)defs.Figuren.sL]) < 2) return true;
	            }
                else if ((brett.pceNum[(int)defs.Figuren.wP] < 3 && brett.pceNum[(int)defs.Figuren.wL] == 0) || (brett.pceNum[(int)defs.Figuren.wL] == 1 && brett.pceNum[(int)defs.Figuren.wP] == 0))
                {
	                if ((brett.pceNum[(int)defs.Figuren.sP] < 3 && brett.pceNum[(int)defs.Figuren.sL] == 0) || (brett.pceNum[(int)defs.Figuren.sL] == 1 && brett.pceNum[(int)defs.Figuren.sP] == 0)) return true;
	            }
	        }
            else if (brett.pceNum[(int)defs.Figuren.wD] == 0 && brett.pceNum[(int)defs.Figuren.sD] == 0)
            {
                if (brett.pceNum[(int)defs.Figuren.wT] == 1 && brett.pceNum[(int)defs.Figuren.sT] == 1)
                {
                    if ((brett.pceNum[(int)defs.Figuren.wP] + brett.pceNum[(int)defs.Figuren.wL]) < 2 && (brett.pceNum[(int)defs.Figuren.sP] + brett.pceNum[(int)defs.Figuren.sL]) < 2)	return true;
                }
                else if (brett.pceNum[(int)defs.Figuren.wT] == 1 && brett.pceNum[(int)defs.Figuren.sT] == 0)
                {
                    if ((brett.pceNum[(int)defs.Figuren.wP] + brett.pceNum[(int)defs.Figuren.wL] == 0) && (((brett.pceNum[(int)defs.Figuren.sP] + brett.pceNum[(int)defs.Figuren.sL]) == 1) || ((brett.pceNum[(int)defs.Figuren.sP] + brett.pceNum[(int)defs.Figuren.sL]) == 2))) return true;
                }
                else if (brett.pceNum[(int)defs.Figuren.sT] == 1 && brett.pceNum[(int)defs.Figuren.wT] == 0)
                {
                    if ((brett.pceNum[(int)defs.Figuren.sP] + brett.pceNum[(int)defs.Figuren.sL] == 0) && (((brett.pceNum[(int)defs.Figuren.wP] + brett.pceNum[(int)defs.Figuren.wL]) == 1) || ((brett.pceNum[(int)defs.Figuren.wP] + brett.pceNum[(int)defs.Figuren.wL]) == 2))) return true;
                }
            }
            return false;
        }           

        public static int EvalPosition(ref boardStruct brett)
        {
            int pce;
            int pceNum;
            int sq;
            int score = brett.material[(int)defs.Farben.WEISS] - brett.material[(int)defs.Farben.SCHWARZ];

            if (brett.pceNum[(int)defs.Figuren.wB] == 0 && brett.pceNum[(int)defs.Figuren.sB] == 0 && MaterialDraw(ref brett))
            {
                return 0;
            }

            pce = (int)defs.Figuren.wB;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                score += PawnTable[defs.Sq120ToSq64[sq]];

                if ((defs.IsolatedMask[defs.Sq120ToSq64[sq]] & brett.bauern[(int)defs.Farben.WEISS]) == 0)
                {
                    score += PawnIsolated;
                }

                if ((defs.WhitePassedMask[defs.Sq120ToSq64[sq]] & brett.bauern[(int)defs.Farben.SCHWARZ]) == 0)
                {
                    score += PawnPassed[defs.RanksBrd[sq]];
                }

            }

            pce = (int)defs.Figuren.sB;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce, pceNum];
                score -= PawnTable[data.Mirror64[defs.Sq120ToSq64[sq]]];

                if ((defs.IsolatedMask[defs.Sq120ToSq64[sq]] & brett.bauern[(int)defs.Farben.SCHWARZ]) == 0)
                {
                    //Console.WriteLine("bP Iso:" + io.PrSq(sq));
                    score -= PawnIsolated;
                }

                if ((defs.BlackPassedMask[defs.Sq120ToSq64[sq]] & brett.bauern[(int)defs.Farben.WEISS]) == 0)
                {
                    //Console.WriteLine("bP Passed:" + io.PrSq(sq));
                    score -= PawnPassed[7 - defs.RanksBrd[sq]];
                }
            }


            pce = (int)defs.Figuren.wP;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score += KnightTable[defs.Sq120ToSq64[sq]];
            }

            pce = (int)defs.Figuren.sP;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score -= KnightTable[data.Mirror64[defs.Sq120ToSq64[sq]]];
            }

            pce = (int)defs.Figuren.wL;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score += BishopTable[defs.Sq120ToSq64[sq]];
            }

            pce = (int)defs.Figuren.sL;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score -= BishopTable[data.Mirror64[defs.Sq120ToSq64[sq]]];
            }

            pce = (int)defs.Figuren.wT;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score += RookTable[defs.Sq120ToSq64[sq]];

                if ((brett.bauern[(int)defs.Farben.BEIDE] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score += RookOpenFile;
                }
                else if ((brett.bauern[(int)defs.Farben.WEISS] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score += RookSemiOpenFile;
                }
            }

            pce = (int)defs.Figuren.sT;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                Debug.Assert(validate.SqOnBoard(sq));
                score -= RookTable[data.Mirror64[defs.Sq120ToSq64[sq]]];

                if ((brett.bauern[(int)defs.Farben.BEIDE] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score -= RookOpenFile;
                }
                else if ((brett.bauern[(int)defs.Farben.SCHWARZ] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score -= RookSemiOpenFile;
                }
            }

            pce = (int)defs.Figuren.wD;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce,pceNum];
                if ((brett.bauern[(int)defs.Farben.BEIDE] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score += QueenOpenFile;
                }
                else if ((brett.bauern[(int)defs.Farben.WEISS] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score += QueenSemiOpenFile;
                }
            }

            pce = (int)defs.Figuren.sD;
            for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
            {
                sq = brett.fListe[pce, pceNum];
                if ((brett.bauern[(int)defs.Farben.BEIDE] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score -= QueenOpenFile;
                }
                else if ((brett.bauern[(int)defs.Farben.SCHWARZ] & defs.FileMask[defs.FilesBrd[sq]]) == 0)
                {
                    score -= QueenSemiOpenFile;
                }
            }

            pce = (int)defs.Figuren.wK;
            sq = brett.fListe[pce, 0];

            if ((brett.material[(int)defs.Farben.SCHWARZ] <= (1 * data.PieceVal[(int)defs.Figuren.wT] + 2 * data.PieceVal[(int)defs.Figuren.wP] + 2 * data.PieceVal[(int)defs.Figuren.wB] + data.PieceVal[(int)defs.Figuren.wK])))
            {
                score += KingE[defs.Sq120ToSq64[sq]];
            }
            else
            {
                score += KingO[defs.Sq120ToSq64[sq]];
            }

            pce = (int)defs.Figuren.sK;
            sq = brett.fListe[pce, 0];

            if ((brett.material[(int)defs.Farben.WEISS] <= (1 * data.PieceVal[(int)defs.Figuren.wT] + 2 * data.PieceVal[(int)defs.Figuren.wP] + 2 * data.PieceVal[(int)defs.Figuren.wB] + data.PieceVal[(int)defs.Figuren.wK])))
            {
                score -= KingE[data.Mirror64[defs.Sq120ToSq64[sq]]];
            }
            else
            {
                score -= KingO[data.Mirror64[defs.Sq120ToSq64[sq]]];
            }

            if (brett.pceNum[(int)defs.Figuren.wL] >= 2) score += BishopPair;
            if (brett.pceNum[(int)defs.Figuren.sL] >= 2) score -= BishopPair;

            if (brett.seite == (int)defs.Farben.WEISS)
            {
                return score;
            }
            else
            {
                return -score;
            }
        }
    }
}
