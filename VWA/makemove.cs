using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class makemove
    {
        public static int[] CastlePerm =   {15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15,  7, 15, 15, 15,  3, 15, 15, 11, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                                            15, 15, 15, 15, 15, 15, 15, 15, 15, 15};


        public static void ClearPiece(int sq, ref boardStruct brett)
        {
	        Debug.Assert(validate.SqOnBoard(sq));
            Debug.Assert(board.CheckBoard(ref brett));

            int pce = brett.figuren[sq];

            Debug.Assert(validate.PieceValid(pce));

            int col = data.PieceCol[pce];
            int index = 0;
            int t_pceNum = -1;

            Debug.Assert(validate.SideValid(col));

            makros.HashInPiece(pce, sq, ref brett);

            brett.figuren[sq] = (int)defs.Figuren.LEER;
            brett.material[col] -= data.PieceVal[pce];
	
	        if(data.PieceBig[pce])
            {
                brett.bigPce[col]--;

		        if(data.PieceMaj[pce])
                {
                    brett.majPce[col]--;
		        }
                else
                {
                    brett.minPce[col]--;
		        }
	        }
            else
            {
		        makros.ClearBit(ref brett.bauern[col], defs.Sq120ToSq64[sq]);
                makros.ClearBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[sq]);
	        }
	
	        for(index = 0; index < brett.pceNum[pce]; ++index)
            {
		        if(brett.fListe[pce,index] == sq) {
			        t_pceNum = index;
			        break;
		        }
	        }
	
	        Debug.Assert(t_pceNum != -1);
            Debug.Assert(t_pceNum >= 0 && t_pceNum < 10);

            brett.pceNum[pce]--;

            brett.fListe[pce, t_pceNum] = brett.fListe[pce, brett.pceNum[pce]];
        }

        public static void AddPiece(int sq, int pce, ref boardStruct brett)
        {
            Debug.Assert(validate.PieceValid(pce));
            Debug.Assert(validate.SqOnBoard(sq));

            int col = data.PieceCol[pce];
            Debug.Assert(validate.SideValid(col));

            makros.HashInPiece(pce, sq, ref brett);

            brett.figuren[sq] = pce;

            if(data.PieceBig[pce])
            {
                brett.bigPce[col]++;

		        if(data.PieceMaj[pce])
                {
                    brett.majPce[col]++;
		        }
                else
                {
                    brett.minPce[col]++;
		        }
	        }
            else
            {
		        makros.SetBit(ref brett.bauern[col], defs.Sq120ToSq64[sq]);
                makros.SetBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[sq]);
	        }

            brett.material[col] += data.PieceVal[pce];
            brett.fListe[pce, brett.pceNum[pce]++] = sq;
        }


        public static void MovePiece(int from, int to, ref boardStruct brett)
        {
            Debug.Assert(validate.SqOnBoard(from));
            Debug.Assert(validate.SqOnBoard(to));

            int index = 0;
            int pce = brett.figuren[from];
            int col = data.PieceCol[pce];

            Debug.Assert(validate.SideValid(col));
            Debug.Assert(validate.PieceValid(pce));

            makros.HashInPiece(pce, from, ref brett);
            brett.figuren[from] = (int)defs.Figuren.LEER;

            makros.HashInPiece(pce, to, ref brett);
            brett.figuren[to] = pce;
	
	        if(!data.PieceBig[pce])
            {
                makros.ClearBit(ref brett.bauern[col], defs.Sq120ToSq64[from]);
                makros.ClearBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[from]);
                makros.SetBit(ref brett.bauern[col], defs.Sq120ToSq64[to]);
                makros.SetBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[to]);
            }    
	
	        for(index = 0; index < brett.pceNum[pce]; ++index)
            {
		        if(brett.fListe[pce,index] == from)
                {
                    brett.fListe[pce,index] = to;
			        break;
		        }
	        }
        }


        public static bool MakeMove(int move, ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));

            int from = makros.FromSq(move);
            int to = makros.ToSq(move);
            int side = brett.seite;

            Debug.Assert(validate.SqOnBoard(from));
            Debug.Assert(validate.SqOnBoard(to));
            Debug.Assert(validate.SideValid(side));
            Debug.Assert(validate.PieceValid(brett.figuren[from]));
            Debug.Assert(brett.hisPly >= 0 && brett.hisPly < defs.MAXSPIELZÜGE);
            Debug.Assert(brett.ply >= 0 && brett.ply < defs.MAXTIEFE);

            brett.history[brett.hisPly].PosKey = brett.posKey;

            if ((move & defs.MFlagEP) != 0)
            {
                if (side == (int)defs.Farben.WEISS)
                {
                    ClearPiece(to - 10, ref brett);
                }
                else
                {
                    ClearPiece(to + 10, ref brett);
                }
            }
            else if ((move & defs.MFlagCA) != 0)
            {
                switch (to)
                {
                    case (int)defs.Felder.C1:
                        MovePiece((int)defs.Felder.A1, (int)defs.Felder.D1, ref brett);
                        break;
                    case (int)defs.Felder.C8:
                        MovePiece((int)defs.Felder.A8, (int)defs.Felder.D8, ref brett);
                        break;
                    case (int)defs.Felder.G1:
                        MovePiece((int)defs.Felder.H1, (int)defs.Felder.F1, ref brett);
                        break;
                    case (int)defs.Felder.G8:
                        MovePiece((int)defs.Felder.H8, (int)defs.Felder.F8, ref brett);
                        break;
                    default: Debug.Assert(false); break;
                }
            }
            
            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);
            makros.HashInCastle(ref brett);

            brett.history[brett.hisPly].Zug = move;
            brett.history[brett.hisPly].FiftyMove = brett.fiftyMove;
            brett.history[brett.hisPly].EnPas = brett.enPas;
            brett.history[brett.hisPly].RochadePerm = brett.rochadePerm;

            brett.rochadePerm &= CastlePerm[from];
            brett.rochadePerm &= CastlePerm[to];
            brett.enPas = (int)defs.Felder.NO_SQ;

            makros.HashInCastle(ref brett);

            int captured =  makros.Captured(move);
            brett.fiftyMove++;

            if (captured != (int)defs.Figuren.LEER)
            {
                Debug.Assert(validate.PieceValid(captured));
                ClearPiece(to, ref brett);
                brett.fiftyMove = 0;
            }


            brett.hisPly++;
            brett.ply++;

            Debug.Assert(brett.hisPly >= 0 && brett.hisPly < defs.MAXSPIELZÜGE);
            Debug.Assert(brett.ply >= 0 && brett.ply < defs.MAXTIEFE);

            if (data.PiecePawn[brett.figuren[from]])
            {
                brett.fiftyMove = 0;
                if ((move & defs.MFlagPS) != 0)
                {
                    if (side == (int)defs.Farben.WEISS)
                    {
                        brett.enPas = from + 10;
                        Debug.Assert(defs.RanksBrd[brett.enPas] == (int)defs.Zeilen.ZEILE_3);
                    }
                    else
                    {
                        brett.enPas = from - 10;
                        Debug.Assert(defs.RanksBrd[brett.enPas] == (int)defs.Zeilen.ZEILE_6);
                    }
                    makros.HashInEnPassant(ref brett);
                }
            }

            MovePiece(from, to, ref brett);

            int prPce = makros.Promoted(move);
            if (prPce != (int)defs.Figuren.LEER)
            {
                Debug.Assert(validate.PieceValid(prPce) && !data.PiecePawn[prPce]);
                ClearPiece(to, ref brett);
                AddPiece(to, prPce, ref brett);
            }

            if (data.PieceKing[brett.figuren[to]])
            {
                brett.KönigSq[brett.seite] = to;
            }

            brett.seite ^= 1;
            makros.HashInSide(ref brett);

            Debug.Assert(board.CheckBoard(ref brett));


            if (attack.SqAttacked(brett.KönigSq[side], brett.seite, ref brett))
            {
                TakeMove(ref brett);
                return false;
            }

            return true;
        }

        public static void TakeMove(ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));

            brett.hisPly--;
            brett.ply--;

            Debug.Assert(brett.hisPly >= 0 && brett.hisPly < defs.MAXSPIELZÜGE);
            Debug.Assert(brett.ply >= 0 && brett.ply < defs.MAXTIEFE);

            int move = brett.history[brett.hisPly].Zug;
            int from = makros.FromSq(move);
            int to = makros.ToSq(move);

            Debug.Assert(validate.SqOnBoard(from));
            Debug.Assert(validate.SqOnBoard(to));

            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);
            makros.HashInCastle(ref brett);

            brett.rochadePerm = brett.history[brett.hisPly].RochadePerm;
            brett.fiftyMove = brett.history[brett.hisPly].FiftyMove;
            brett.enPas = brett.history[brett.hisPly].EnPas;

            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);
            makros.HashInCastle(ref brett);

            brett.seite ^= 1;
            makros.HashInSide(ref brett);

            if ((defs.MFlagEP & move) != 0)
            {
                if (brett.seite == (int)defs.Farben.WEISS)
                {
                    AddPiece(to - 10, (int)defs.Figuren.sB, ref brett);
                }
                else
                {
                    AddPiece(to + 10, (int)defs.Figuren.wB, ref brett);
                }
            }

            else if ((defs.MFlagCA & move) != 0)
            {
                switch (to)
                {
                    case (int)defs.Felder.C1: MovePiece((int)defs.Felder.D1, (int)defs.Felder.A1, ref brett); break;
                    case (int)defs.Felder.C8: MovePiece((int)defs.Felder.D8, (int)defs.Felder.A8, ref brett); break;
                    case (int)defs.Felder.G1: MovePiece((int)defs.Felder.F1, (int)defs.Felder.H1, ref brett); break;
                    case (int)defs.Felder.G8: MovePiece((int)defs.Felder.F8, (int)defs.Felder.H8, ref brett); break;
                    default: Debug.Assert(false); break;
                }
            }

            MovePiece(to, from, ref brett);

            if (data.PieceKing[brett.figuren[from]])
            {
                brett.KönigSq[brett.seite] = from;
            }

            int captured = makros.Captured(move);

            if (captured != (int)defs.Figuren.LEER)
            {
                Debug.Assert(validate.PieceValid(captured));
                AddPiece(to, captured, ref brett);
            }

            if (makros.Promoted(move) != (int)defs.Figuren.LEER)
            {
                Debug.Assert(validate.PieceValid(makros.Promoted(move)) && !data.PiecePawn[makros.Promoted(move)]);
                ClearPiece(from, ref brett);
                AddPiece(from, (data.PieceCol[makros.Promoted(move)] == (int)defs.Farben.WEISS ? (int)defs.Figuren.wB : (int)defs.Figuren.sB), ref brett);
            }

            Debug.Assert(board.CheckBoard(ref brett));
        }

        public static void MakeNullMove(ref boardStruct brett)
        {

            Debug.Assert(board.CheckBoard(ref brett));
            Debug.Assert(!attack.SqAttacked(brett.KönigSq[brett.seite], brett.seite ^ 1, ref brett));

            brett.ply++;
            brett.history[brett.hisPly].PosKey = brett.posKey;

            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);

            brett.history[brett.hisPly].Zug = defs.NOMOVE;
            brett.history[brett.hisPly].FiftyMove = brett.fiftyMove;
            brett.history[brett.hisPly].EnPas = brett.enPas;
            brett.history[brett.hisPly].RochadePerm = brett.rochadePerm;
            brett.enPas = (int)defs.Felder.NO_SQ;

            brett.seite ^= 1;
            brett.hisPly++;
            makros.HashInSide(ref brett);

            Debug.Assert(board.CheckBoard(ref brett));
            Debug.Assert(brett.hisPly >= 0 && brett.hisPly < defs.MAXSPIELZÜGE);
            Debug.Assert(brett.ply >= 0 && brett.ply < defs.MAXTIEFE);

            return;
        }

        public static void TakeNullMove(ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));

            brett.hisPly--;
            brett.ply--;

            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);

            brett.rochadePerm = brett.history[brett.hisPly].RochadePerm;
            brett.fiftyMove = brett.history[brett.hisPly].FiftyMove;
            brett.enPas = brett.history[brett.hisPly].EnPas;

            if (brett.enPas != (int)defs.Felder.NO_SQ) makros.HashInEnPassant(ref brett);
            brett.seite ^= 1;
            makros.HashInSide(ref brett);

            Debug.Assert(board.CheckBoard(ref brett));
            Debug.Assert(brett.hisPly >= 0 && brett.hisPly < defs.MAXSPIELZÜGE);
            Debug.Assert(brett.ply >= 0 && brett.ply < defs.MAXTIEFE);
        }
    }
}
