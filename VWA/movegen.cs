using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class movegen
    {
        //zum kürzeren schreiben von slide figuren
        static readonly int[] NumDir = {0, 0, 8, 4, 4, 8, 8, 0, 8, 4, 4, 8, 8};
        static readonly int[] LoopSlideIndex = { 0, 4 };
        static readonly int[] LoopSlidePce= {(int)defs.Figuren.wL, (int)defs.Figuren.wT, (int)defs.Figuren.wD, 0, (int)defs.Figuren.sL, (int)defs.Figuren.sT, (int)defs.Figuren.sD, 0};
        static readonly int[][] PceDir = new int[][] { new int[]{ 0, 0, 0, 0, 0, 0, 0 },
                                                       new int[]{ 0, 0, 0, 0, 0, 0, 0 },
                                                       new int[]{ -8, -19,	-21, -12, 8, 19, 21, 12 },
                                                       new int[]{ -9, -11, 11, 9, 0, 0, 0, 0 },
                                                       new int[]{ -1, -10,	1, 10, 0, 0, 0, 0 },
                                                       new int[]{ -1, -10,	1, 10, -9, -11, 11, 9 },
                                                       new int[]{ -1, -10,	1, 10, -9, -11, 11, 9 },
                                                       new int[]{ 0, 0, 0, 0, 0, 0, 0 },
                                                       new int[]{ -8, -19,	-21, -12, 8, 19, 21, 12 },
                                                       new int[]{ -9, -11, 11, 9, 0, 0, 0, 0 },
                                                       new int[]{ -1, -10,	1, 10, 0, 0, 0, 0 },
                                                       new int[]{ -1, -10,	1, 10, -9, -11, 11, 9 },
                                                       new int[]{ -1, -10,	1, 10, -9, -11, 11, 9 }};

        //nur eine loop für nicht slider
        static readonly int[] LoopNonSlidePce = { (int)defs.Figuren.wP, (int)defs.Figuren.wK, 0, (int)defs.Figuren.sP, (int)defs.Figuren.sK, 0};
        static readonly int[] LoopNonSlideIndex = { 0, 3 };

        static int[] VictimScore = { 0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 };
        public static int[,] MvvLvaScores = new int[13,13];

        public static void InitMvvLva()
        {
            int Attacker;
            int Victim;
            for (Attacker = (int)defs.Figuren.wB; Attacker <= (int)defs.Figuren.sK; ++Attacker)
            {
                for (Victim = (int)defs.Figuren.wB; Victim <= (int)defs.Figuren.sK; ++Victim)
                {
                    MvvLvaScores[Victim,Attacker] = VictimScore[Victim] + 6 - (VictimScore[Attacker] / 100);
                }
            }
        }

        public static bool MoveExists(ref boardStruct brett, int move)
        {
            movelist list = new movelist();
            GenerateAllMoves(ref list, ref brett);

            int MoveNum = 0;
	        for(MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {
                if ( !makemove.MakeMove(list.zugliste[MoveNum].Zug, ref brett))  
                {
                    continue;
                }
                makemove.TakeMove(ref brett);
		        if(list.zugliste[MoveNum].Zug == move)
                {
                        return true;
		        }
            }
	        return false;
        }

        static void AddQuietMove(int zug, ref movelist list, ref boardStruct brett)
        {
            list.zugliste[list.anzahlZüge].Zug = zug;

            if(brett.searchKillers[0, brett.ply] == zug)
            {
                list.zugliste[list.anzahlZüge].Wertung = 900000;
            }
            else if(brett.searchKillers[1, brett.ply] == zug)
            {
                list.zugliste[list.anzahlZüge].Wertung = 800000;
            }
            else
            {
                list.zugliste[list.anzahlZüge].Wertung = brett.searchHistory[brett.figuren[makros.FromSq(zug)], makros.ToSq(zug)];
            }

            list.anzahlZüge++;
        }

        static void AddCaptureMove(int zug, ref movelist list, ref boardStruct brett)
        {
            list.zugliste[list.anzahlZüge].Zug = zug;
            list.zugliste[list.anzahlZüge].Wertung = MvvLvaScores[makros.Captured(zug), brett.figuren[makros.FromSq(zug)]] + 1000000;
            list.anzahlZüge++;
        }

        static void AddEnPassantMove(int zug, ref movelist list, ref boardStruct brett)
        {
            list.zugliste[list.anzahlZüge].Zug = zug;
            list.zugliste[list.anzahlZüge].Wertung = 105 + 1000000;
            list.anzahlZüge++;
        }

        static void AddWhitePawnCapMove(int from, int to, int cap, ref movelist list, ref boardStruct brett)
        {
	        if(defs.RanksBrd[from] == (int)defs.Zeilen.ZEILE_7) {
		        AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.wD, 0), ref list, ref brett);
		        AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.wT, 0), ref list, ref brett);
		        AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.wL, 0), ref list, ref brett);
		        AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.wP, 0), ref list, ref brett);
	        } else {
		        AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.LEER,0), ref list, ref brett);
	        }
        }

        static void AddWhitePawnMove(int from, int to, ref movelist list, ref boardStruct brett)
        {
            if (defs.RanksBrd[from] == (int) defs.Zeilen.ZEILE_7)
            {
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.wD, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.wT, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.wL, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.wP, 0), ref list, ref brett);
            }
            else
            {
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, 0), ref list, ref brett);
            }
        }

        static void AddBlackPawnCapMove(int from, int to, int cap, ref movelist list, ref boardStruct brett)
        {
            if (defs.RanksBrd[from] == (int)defs.Zeilen.ZEILE_2)
            {
                AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.sD, 0), ref list, ref brett);
                AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.sT, 0), ref list, ref brett);
                AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.sL, 0), ref list, ref brett);
                AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.sP, 0), ref list, ref brett);
            }
            else
            {
                AddCaptureMove(makros.ZugGenerieren(from, to, cap, (int)defs.Figuren.LEER, 0), ref list, ref brett);
            }
        }

        static void AddBlackPawnMove(int from, int to, ref movelist list, ref boardStruct brett)
        {
            if (defs.RanksBrd[from] == (int)defs.Zeilen.ZEILE_2)
            {
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.sD, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.sT, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.sL, 0), ref list, ref brett);
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.sP, 0), ref list, ref brett);
            }
            else
            {
                AddQuietMove(makros.ZugGenerieren(from, to, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, 0), ref list, ref brett);
            }
        }

        public static void GenerateAllMoves(ref movelist list, ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));
            list.anzahlZüge = 0;

            int pce = (int)defs.Figuren.LEER;
            int side = brett.seite;
            int sq = 0; int t_sq = 0;
            int pceNum = 0;
            int dir = 0;
            int index = 0;
            int pceIndex = 0;

            if (side == (int)defs.Farben.WEISS)
            {
                for (pceNum = 0; pceNum < brett.pceNum[(int)defs.Figuren.wB]; ++pceNum)
                {
                    sq = brett.fListe[(int)defs.Figuren.wB,pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    if (brett.figuren[sq + 10] == (int)defs.Figuren.LEER)
                    {
                        AddWhitePawnMove(sq, sq + 10, ref list, ref brett);
                        if (defs.RanksBrd[sq] == (int)defs.Zeilen.ZEILE_2 && brett.figuren[sq + 20] == (int)defs.Figuren.LEER)
                        {
                            AddQuietMove(makros.ZugGenerieren(sq, (sq + 20), (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagPS), ref list, ref brett);
                        }
                    }

                    if (validate.SqOnBoard(sq + 9) && data.PieceCol[brett.figuren[sq + 9]] == (int)defs.Farben.SCHWARZ)
                    {
                        AddWhitePawnCapMove(sq, sq + 9, brett.figuren[sq + 9], ref list, ref brett);
                    }
                    if (validate.SqOnBoard(sq + 11) && data.PieceCol[brett.figuren[sq + 11]] == (int)defs.Farben.SCHWARZ)
                    {
                        AddWhitePawnCapMove(sq, sq + 11, brett.figuren[sq + 11], ref list, ref brett);
                    }

                    if (brett.enPas != (int)defs.Felder.NO_SQ)
                    {
                        if (sq + 9 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq + 9, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                        if (sq + 11 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq + 11, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                    }
                }

                if ((brett.rochadePerm  & (int)defs.Rochade.WKRC) != 0)
                {
                    if (brett.figuren[(int)defs.Felder.F1] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.G1] == (int)defs.Figuren.LEER)
                    {
                        if (!attack.SqAttacked((int)defs.Felder.E1, (int)defs.Farben.SCHWARZ, ref brett) && !attack.SqAttacked((int)defs.Felder.F1, (int)defs.Farben.SCHWARZ, ref brett))
                        {
                            AddQuietMove(makros.ZugGenerieren((int)defs.Felder.E1, (int)defs.Felder.G1, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagCA), ref list, ref brett);
                        }
                    }
                }

                if ((brett.rochadePerm & (int)defs.Rochade.WDRC) != 0)
                {
                    if (brett.figuren[(int)defs.Felder.D1] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.C1] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.B1] == (int)defs.Figuren.LEER)
                    {
                        if (!attack.SqAttacked((int)defs.Felder.E1, (int)defs.Farben.SCHWARZ, ref brett) && !attack.SqAttacked((int)defs.Felder.D1, (int)defs.Farben.SCHWARZ, ref brett))
                        {
                            AddQuietMove(makros.ZugGenerieren((int)defs.Felder.E1, (int)defs.Felder.C1, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagCA), ref list, ref brett);
                        }
                    }
                }

            }
            else
            {
                for (pceNum = 0; pceNum < brett.pceNum[(int)defs.Figuren.sB]; ++pceNum)
                {
                    sq = brett.fListe[(int)defs.Figuren.sB,pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    if (brett.figuren[sq - 10] == (int)defs.Figuren.LEER)
                    {
                        AddBlackPawnMove(sq, sq - 10, ref list, ref brett);
                        if (defs.RanksBrd[sq] == (int)defs.Zeilen.ZEILE_7 && brett.figuren[sq - 20] == (int)defs.Figuren.LEER)
                        {
                            AddQuietMove(makros.ZugGenerieren(sq, (sq - 20), (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagPS), ref list, ref brett);
                        }
                    }

                    if (validate.SqOnBoard(sq - 9) && data.PieceCol[brett.figuren[sq - 9]] == (int)defs.Farben.WEISS)
                    {
                        AddBlackPawnCapMove(sq, sq - 9, brett.figuren[sq - 9], ref list, ref brett);
                    }

                    if (validate.SqOnBoard(sq - 11) && data.PieceCol[brett.figuren[sq - 11]] == (int)defs.Farben.WEISS)
                    {
                        AddBlackPawnCapMove(sq, sq - 11, brett.figuren[sq - 11], ref list, ref brett);
                    }
                    if (brett.enPas != (int)defs.Felder.NO_SQ)
                    {
                        if (sq - 9 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq - 9, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                        if (sq - 11 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq - 11, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                    }
                }

                // castling
                if ((brett.rochadePerm & (int)defs.Rochade.SKRC) != 0)
                {
                    if (brett.figuren[(int)defs.Felder.F8] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.G8] == (int)defs.Figuren.LEER)
                    {
                        if (!attack.SqAttacked((int)defs.Felder.E8, (int)defs.Farben.WEISS, ref brett) && !attack.SqAttacked((int)defs.Felder.F8, (int)defs.Farben.WEISS, ref brett))
                        {
                            AddQuietMove(makros.ZugGenerieren((int)defs.Felder.E8, (int)defs.Felder.G8, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagCA), ref list, ref brett);
                        }
                    }
                }

                if ((brett.rochadePerm & (int)defs.Rochade.SDRC) != 0)
                {
                    if (brett.figuren[(int)defs.Felder.D8] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.C8] == (int)defs.Figuren.LEER && brett.figuren[(int)defs.Felder.B8] == (int)defs.Figuren.LEER)
                    {
                        if (!attack.SqAttacked((int)defs.Felder.E8, (int)defs.Farben.WEISS, ref brett) && !attack.SqAttacked((int)defs.Felder.D8, (int)defs.Farben.WEISS, ref brett))
                        {
                            AddQuietMove(makros.ZugGenerieren((int)defs.Felder.E8, (int)defs.Felder.C8, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagCA), ref list, ref brett);
                        }
                    }
                }
            }

            /* Loop for slide pieces */
            pceIndex = LoopSlideIndex[side];
            pce = LoopSlidePce[pceIndex++];
            while (pce != 0)
            {
                Debug.Assert(validate.PieceValid(pce));

                for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
                {
                    sq = brett.fListe[pce, pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    for (index = 0; index < NumDir[pce]; ++index)
                    {
                        dir = PceDir[pce][index];
                        t_sq = sq + dir;

                        while (validate.SqOnBoard(t_sq))
                        {
                            // (int)defs.Farben.SCHWARZ ^ 1 == (int)defs.Farben.WEISS       (int)defs.Farben.WEISS ^ 1 == (int)defs.Farben.SCHWARZ
                            if (brett.figuren[t_sq] != (int)defs.Figuren.LEER)
                            {
                                if (data.PieceCol[brett.figuren[t_sq]] == (side ^ 1))
                                {
                                    AddCaptureMove(makros.ZugGenerieren(sq, t_sq, brett.figuren[t_sq], (int)defs.Figuren.LEER, 0), ref list, ref brett);
                                }
                                break;
                            }
                            AddQuietMove(makros.ZugGenerieren(sq, t_sq, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, 0), ref list, ref brett);
                            t_sq += dir;
                        }
                    }
                }
                pce = LoopSlidePce[pceIndex++];
            }
            
            /* Loop for non slide */
            pceIndex = LoopNonSlideIndex[side];
            pce = LoopNonSlidePce[pceIndex++];

            while (pce != 0)
            {
                Debug.Assert(validate.PieceValid(pce));

                for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
                {
                    sq = brett.fListe[pce,pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    for (index = 0; index < NumDir[pce]; ++index)
                    {
                        dir = PceDir[pce][index];
                        t_sq = sq + dir;

                        if (!validate.SqOnBoard(t_sq))
                        {
                            continue;
                        }

                        // BLACK ^ 1 == WHITE       WHITE ^ 1 == BLACK
                        if (brett.figuren[t_sq] != (int)defs.Figuren.LEER)
                        {
                            if (data.PieceCol[brett.figuren[t_sq]] == (side ^ 1))
                            {
                                AddCaptureMove(makros.ZugGenerieren(sq, t_sq, brett.figuren[t_sq], (int)defs.Figuren.LEER, 0), ref list, ref brett);
                            }
                            continue;
                        }
                        AddQuietMove(makros.ZugGenerieren(sq, t_sq, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, 0), ref list, ref brett);
                    }
                }

                pce = LoopNonSlidePce[pceIndex++];
            }
        }

        public static void GenerateAllCaps(ref movelist list, ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));
            list.anzahlZüge = 0;

            int pce = (int)defs.Figuren.LEER;
            int side = brett.seite;
            int sq = 0; int t_sq = 0;
            int pceNum = 0;
            int dir = 0;
            int index = 0;
            int pceIndex = 0;

            if (side == (int)defs.Farben.WEISS)
            {
                for (pceNum = 0; pceNum < brett.pceNum[(int)defs.Figuren.wB]; ++pceNum)
                {
                    sq = brett.fListe[(int)defs.Figuren.wB, pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    if (validate.SqOnBoard(sq + 9) && data.PieceCol[brett.figuren[sq + 9]] == (int)defs.Farben.SCHWARZ)
                    {
                        AddWhitePawnCapMove(sq, sq + 9, brett.figuren[sq + 9], ref list, ref brett);
                    }
                    if (validate.SqOnBoard(sq + 11) && data.PieceCol[brett.figuren[sq + 11]] == (int)defs.Farben.SCHWARZ)
                    {
                        AddWhitePawnCapMove(sq, sq + 11, brett.figuren[sq + 11], ref list, ref brett);
                    }

                    if (brett.enPas != (int)defs.Felder.NO_SQ)
                    {
                        if (sq + 9 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq + 9, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                        if (sq + 11 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq + 11, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                    }
                }

            }
            else
            {
                for (pceNum = 0; pceNum < brett.pceNum[(int)defs.Figuren.sB]; ++pceNum)
                {
                    sq = brett.fListe[(int)defs.Figuren.sB, pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    if (validate.SqOnBoard(sq - 9) && data.PieceCol[brett.figuren[sq - 9]] == (int)defs.Farben.WEISS)
                    {
                        AddBlackPawnCapMove(sq, sq - 9, brett.figuren[sq - 9], ref list, ref brett);
                    }

                    if (validate.SqOnBoard(sq - 11) && data.PieceCol[brett.figuren[sq - 11]] == (int)defs.Farben.WEISS)
                    {
                        AddBlackPawnCapMove(sq, sq - 11, brett.figuren[sq - 11], ref list, ref brett);
                    }
                    if (brett.enPas != (int)defs.Felder.NO_SQ)
                    {
                        if (sq - 9 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq - 9, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                        if (sq - 11 == brett.enPas)
                        {
                            AddEnPassantMove(makros.ZugGenerieren(sq, sq - 11, (int)defs.Figuren.LEER, (int)defs.Figuren.LEER, defs.MFlagEP), ref list, ref brett);
                        }
                    }
                }
            }

            /* Loop for slide pieces */
            pceIndex = LoopSlideIndex[side];
            pce = LoopSlidePce[pceIndex++];
            while (pce != 0)
            {
                Debug.Assert(validate.PieceValid(pce));

                for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
                {
                    sq = brett.fListe[pce, pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    for (index = 0; index < NumDir[pce]; ++index)
                    {
                        dir = PceDir[pce][index];
                        t_sq = sq + dir;

                        while (validate.SqOnBoard(t_sq))
                        {
                            // (int)defs.Farben.SCHWARZ ^ 1 == (int)defs.Farben.WEISS       (int)defs.Farben.WEISS ^ 1 == (int)defs.Farben.SCHWARZ
                            if (brett.figuren[t_sq] != (int)defs.Figuren.LEER)
                            {
                                if (data.PieceCol[brett.figuren[t_sq]] == (side ^ 1))
                                {
                                    AddCaptureMove(makros.ZugGenerieren(sq, t_sq, brett.figuren[t_sq], (int)defs.Figuren.LEER, 0), ref list, ref brett);
                                }
                                break;
                            }
                            t_sq += dir;
                        }
                    }
                }
                pce = LoopSlidePce[pceIndex++];
            }

            /* Loop for non slide */
            pceIndex = LoopNonSlideIndex[side];
            pce = LoopNonSlidePce[pceIndex++];

            while (pce != 0)
            {
                Debug.Assert(validate.PieceValid(pce));

                for (pceNum = 0; pceNum < brett.pceNum[pce]; ++pceNum)
                {
                    sq = brett.fListe[pce, pceNum];
                    Debug.Assert(validate.SqOnBoard(sq));

                    for (index = 0; index < NumDir[pce]; ++index)
                    {
                        dir = PceDir[pce][index];
                        t_sq = sq + dir;

                        if (!validate.SqOnBoard(t_sq))
                        {
                            continue;
                        }

                        // BLACK ^ 1 == WHITE       WHITE ^ 1 == BLACK
                        if (brett.figuren[t_sq] != (int)defs.Figuren.LEER)
                        {
                            if (data.PieceCol[brett.figuren[t_sq]] == (side ^ 1))
                            {
                                AddCaptureMove(makros.ZugGenerieren(sq, t_sq, brett.figuren[t_sq], (int)defs.Figuren.LEER, 0), ref list, ref brett);
                            }
                            continue;
                        }
                    }
                }

                pce = LoopNonSlidePce[pceIndex++];
            }
        }
    }
}
