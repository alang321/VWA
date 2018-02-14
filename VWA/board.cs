using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class board
    {
        //zum kompletten umdrehen einer position damit man evaluierungsfunktion nur für weis schreiben muss
        public static void MirrorBoard(ref boardStruct brett)
        {

            int[] tempPiecesArray = new int [64];
            int tempSide = brett.seite ^ 1;
            int[] SwapPiece = { (int)defs.Figuren.LEER, (int)defs.Figuren.sB, (int)defs.Figuren.sP, (int)defs.Figuren.sL, (int)defs.Figuren.sT, (int)defs.Figuren.sD, (int)defs.Figuren.sK, (int)defs.Figuren.wB, (int)defs.Figuren.wP, (int)defs.Figuren.wL, (int)defs.Figuren.wT, (int)defs.Figuren.wD, (int)defs.Figuren.wK };
            int tempCastlePerm = 0;
            int tempEnPas = (int)defs.Felder.NO_SQ;

            int sq;
            int tp;

            if ((brett.rochadePerm & (int)defs.Rochade.WKRC) != 0) tempCastlePerm |= (int)defs.Rochade.SKRC;
            if ((brett.rochadePerm & (int)defs.Rochade.WDRC) != 0) tempCastlePerm |= (int)defs.Rochade.SDRC;

            if ((brett.rochadePerm & (int)defs.Rochade.SKRC) != 0) tempCastlePerm |= (int)defs.Rochade.SKRC;
            if ((brett.rochadePerm & (int)defs.Rochade.SDRC) != 0) tempCastlePerm |= (int)defs.Rochade.SDRC;

            if (brett.enPas != (int)defs.Felder.NO_SQ)
            {
                tempEnPas = defs.Sq64ToSq120[data.Mirror64[defs.Sq120ToSq64[brett.enPas]]];
            }

            for (sq = 0; sq < 64; sq++)
            {
                tempPiecesArray[sq] = brett.figuren[defs.Sq64ToSq120[data.Mirror64[sq]]];
            }

            ResetBoard(ref brett);

            for (sq = 0; sq < 64; sq++)
            {
                tp = SwapPiece[tempPiecesArray[sq]];
                brett.figuren[defs.Sq64ToSq120[sq]] = tp;
            }

            brett.seite = tempSide;
            brett.rochadePerm = tempCastlePerm;
            brett.enPas = tempEnPas;

            brett.posKey = hashkeys.GeneratePosKey(ref brett);

            UpdateListsMaterial(ref brett);

            Debug.Assert(CheckBoard(ref brett));
        }

        public static void ResetBoard(ref boardStruct brett)
        {

            for (int index = 0; index < defs.BRD_SQ_NUM; ++index)
            {
                brett.figuren[index] = (int)defs.Felder.OFFBOARD;
            }

            for (int index = 0; index < 64; ++index)
            {
                brett.figuren[defs.Sq64ToSq120[index]] = (int)defs.Figuren.LEER;
            }

            for (int index = 0; index < 2; ++index)
            {
                brett.bigPce[index] = 0;
                brett.majPce[index] = 0;
                brett.minPce[index] = 0;
            }

            for (int index = 0; index < 3; ++index)
            {
                brett.bauern[index] = 0UL;
                brett.material[index] = 0;
            }

            for (int index = 0; index < 13; ++index)
            {
                brett.pceNum[index] = 0;
            }

            brett.KönigSq[(int)defs.Farben.WEISS] = brett.KönigSq[(int)defs.Farben.SCHWARZ] = (int)defs.Felder.NO_SQ;

            brett.seite = (int)defs.Farben.BEIDE;
            brett.enPas = (int)defs.Felder.NO_SQ;
            brett.fiftyMove = 0;

            brett.ply = 0;
            brett.hisPly = 0;

            brett.rochadePerm = 0;

            brett.posKey = 0UL;
        }

        public static bool CheckBoard(ref boardStruct brett)
        {
            int[] t_pceNum = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] t_bigPce = { 0, 0 };
            int[] t_majPce = { 0, 0 };
            int[] t_minPce = { 0, 0 };
            int[] t_material = { 0, 0, 0 };

            int sq64, t_piece, t_pce_num, sq120, colour, pcount;

            ulong[] t_pawns = { 0UL, 0UL, 0UL };

            t_pawns[(int)defs.Farben.WEISS] = brett.bauern[(int)defs.Farben.WEISS];
	        t_pawns[(int)defs.Farben.SCHWARZ] = brett.bauern[(int)defs.Farben.SCHWARZ];
	        t_pawns[(int)defs.Farben.BEIDE] = brett.bauern[(int)defs.Farben.BEIDE];

	        // check piece lists
	        for(t_piece = (int)defs.Figuren.wB; t_piece <= (int)defs.Figuren.sK; ++t_piece) 
            {
		        for(t_pce_num = 0; t_pce_num < brett.pceNum[t_piece]; ++t_pce_num) 
                {
			        sq120 = brett.fListe[t_piece, t_pce_num];
                    Debug.Assert(brett.figuren[sq120] == t_piece);
                }
            }
            
	        // check piece count and other counters
	        for(sq64 = 0; sq64< 64; ++sq64) {
		        sq120 = defs.Sq64ToSq120[sq64];
                t_piece = brett.figuren[sq120];
		        t_pceNum[t_piece]++;
		        colour = data.PieceCol[t_piece];
		        if(data.PieceBig[t_piece]) t_bigPce[colour]++;
		        if(data.PieceMin[t_piece]) t_minPce[colour]++;
		        if(data.PieceMaj[t_piece]) t_majPce[colour]++;

                t_material[colour] += data.PieceVal[t_piece];
	        }

	        for(t_piece = (int)defs.Figuren.wB; t_piece <= (int)defs.Figuren.sK; ++t_piece)
            {
                Debug.Assert(t_pceNum[t_piece] == brett.pceNum[t_piece]);
	        }

	        // check bitboards count
	        pcount = bitboards.CountBits(t_pawns[(int)defs.Farben.WEISS]);
            Debug.Assert(pcount == brett.pceNum[(int)defs.Figuren.wB]);

            pcount = bitboards.CountBits(t_pawns[(int)defs.Farben.SCHWARZ]);
            Debug.Assert(pcount == brett.pceNum[(int)defs.Figuren.sB]);

            pcount = bitboards.CountBits(t_pawns[(int)defs.Farben.BEIDE]);
            Debug.Assert(pcount == (brett.pceNum[(int)defs.Figuren.wB] + brett.pceNum[(int)defs.Figuren.sB]));

            // check bitboards squares
            while (t_pawns[(int)defs.Farben.SCHWARZ] != 0)
            {
                sq64 = bitboards.PopBit(ref t_pawns[(int)defs.Farben.SCHWARZ]);
                Debug.Assert(brett.figuren[defs.Sq64ToSq120[sq64]] == (int)defs.Figuren.sB);
            }

            while (t_pawns[(int)defs.Farben.WEISS] != 0)
            {
                sq64 = bitboards.PopBit(ref t_pawns[(int)defs.Farben.WEISS]);
                Debug.Assert(brett.figuren[defs.Sq64ToSq120[sq64]] == (int)defs.Figuren.wB);
            }

            while (t_pawns[(int)defs.Farben.BEIDE] != 0)
            {
                sq64 = bitboards.PopBit(ref t_pawns[(int)defs.Farben.BEIDE]);
                Debug.Assert(brett.figuren[defs.Sq64ToSq120[sq64]] == (int)defs.Figuren.sB || brett.figuren[defs.Sq64ToSq120[sq64]] == (int)defs.Figuren.wB);
            }


            Debug.Assert(t_material[(int)defs.Farben.WEISS] == brett.material[(int)defs.Farben.WEISS] && t_material[(int)defs.Farben.SCHWARZ] == brett.material[(int)defs.Farben.SCHWARZ]);
            Debug.Assert(t_minPce[(int)defs.Farben.WEISS] == brett.minPce[(int)defs.Farben.WEISS] && t_minPce[(int)defs.Farben.SCHWARZ] == brett.minPce[(int)defs.Farben.SCHWARZ]);
            Debug.Assert(t_majPce[(int)defs.Farben.WEISS] == brett.majPce[(int)defs.Farben.WEISS] && t_majPce[(int)defs.Farben.SCHWARZ] == brett.majPce[(int)defs.Farben.SCHWARZ]);
            Debug.Assert(t_bigPce[(int)defs.Farben.WEISS] == brett.bigPce[(int)defs.Farben.WEISS] && t_bigPce[(int)defs.Farben.SCHWARZ] == brett.bigPce[(int)defs.Farben.SCHWARZ]);

            Debug.Assert(brett.seite == (int)defs.Farben.WEISS || brett.seite == (int)defs.Farben.SCHWARZ);
            Debug.Assert(hashkeys.GeneratePosKey(ref brett) == brett.posKey);

            Debug.Assert(brett.enPas == (int)defs.Felder.NO_SQ || (defs.RanksBrd[brett.enPas] == (int)defs.Zeilen.ZEILE_6 && brett.seite == (int)defs.Farben.WEISS) || (defs.RanksBrd[brett.enPas] == (int)defs.Zeilen.ZEILE_3 && brett.seite == (int)defs.Farben.SCHWARZ));

            Debug.Assert(brett.figuren[brett.KönigSq[(int)defs.Farben.WEISS]] == (int)defs.Figuren.wK);
            Debug.Assert(brett.figuren[brett.KönigSq[(int)defs.Farben.SCHWARZ]] == (int)defs.Figuren.sK);

            Debug.Assert(brett.rochadePerm >= 0 && brett.rochadePerm <= 15);
            return true;
        }


        public static int ParseFen(string fen, ref boardStruct brett)
        {
            int rank = (int)defs.Zeilen.ZEILE_8;
            int file = (int)defs.Spalten.SPALTE_A;
            int piece = 0;
            int count = 0;
            int i = 0;
            int sq64 = 0;
            int sq120 = 0;

            int pos = 0;

            ResetBoard(ref brett);

            while (rank >= (int)defs.Zeilen.ZEILE_1)
            {
                count = 1;
                switch (fen[pos])
                {
                    case 'p': piece = (int)defs.Figuren.sB; break;
                    case 'r': piece = (int)defs.Figuren.sT; break;
                    case 'n': piece = (int)defs.Figuren.sP; break;
                    case 'b': piece = (int)defs.Figuren.sL; break;
                    case 'k': piece = (int)defs.Figuren.sK; break;
                    case 'q': piece = (int)defs.Figuren.sD; break;
                    case 'P': piece = (int)defs.Figuren.wB; break;
                    case 'R': piece = (int)defs.Figuren.wT; break;
                    case 'N': piece = (int)defs.Figuren.wP; break;
                    case 'B': piece = (int)defs.Figuren.wL; break;
                    case 'K': piece = (int)defs.Figuren.wK; break;
                    case 'Q': piece = (int)defs.Figuren.wD; break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        piece = (int)defs.Figuren.LEER;
                        count = Convert.ToInt32(fen[pos].ToString());
                        break;

                    case '/':
                    case ' ':
                        rank--;
                        file = (int)defs.Spalten.SPALTE_A;
                        pos++;
                        continue;

                    default:
                        throw new Exception("FEN invalid  -  " + fen[pos].ToString());
                }

                for (i = 0; i < count; i++)
                {
                    sq64 = rank * 8 + file;
                    sq120 = defs.Sq64ToSq120[sq64];
                    if (piece != (int)defs.Figuren.LEER)
                    {
                        brett.figuren[sq120] = piece;
                    }
                    file++;
                }
                pos++;

            }

            brett.seite = (fen[pos] == 'w') ? (int)defs.Farben.WEISS : (int)defs.Farben.SCHWARZ;

            pos += 2;

            for (i = 0; i < 4; i++)
            {
                if (fen[pos] == ' ')
                {
                    break;
                }
                switch (fen[pos])
                {
                    case 'K': brett.rochadePerm |= (int)defs.Rochade.WKRC; break;
                    case 'Q': brett.rochadePerm |= (int)defs.Rochade.WDRC; break;
                    case 'k': brett.rochadePerm |= (int)defs.Rochade.SKRC; break;
                    case 'q': brett.rochadePerm |= (int)defs.Rochade.SDRC; break;
                    default: break;
                }
                pos++;
            }
            pos++;

            if (fen[pos] != '-')
            {
                file = fen[pos] - 'a';
                rank = fen[pos+1] - '1';

                brett.enPas = makros.SZ2SQ(file, rank);
            }

            brett.posKey = hashkeys.GeneratePosKey(ref brett);

            UpdateListsMaterial(ref brett);

            return 0;
        }

        public static void UpdateListsMaterial(ref boardStruct brett)
        {
            int piece, sq, index, colour;

            for (index = 0; index < defs.BRD_SQ_NUM; ++index)
            {
                sq = index;
                piece = brett.figuren[index];
                if (piece != (int)defs.Felder.OFFBOARD && piece != (int)defs.Figuren.LEER)
                {
                    colour = data.PieceCol[piece];

                    if (data.PieceBig[piece]) brett.bigPce[colour]++;
                    if (data.PieceMin[piece]) brett.minPce[colour]++;
                    if (data.PieceMaj[piece]) brett.majPce[colour]++;

                    brett.material[colour] += data.PieceVal[piece];

                    brett.fListe[piece, brett.pceNum[piece]] = sq;
                    brett.pceNum[piece]++;

                    if (piece == (int)defs.Figuren.wK) brett.KönigSq[(int)defs.Farben.WEISS] = sq;
                    if (piece == (int)defs.Figuren.sK) brett.KönigSq[(int)defs.Farben.SCHWARZ] = sq;

                    if (piece == (int)defs.Figuren.wB)
                    {
                        makros.SetBit(ref brett.bauern[(int)defs.Farben.WEISS], defs.Sq120ToSq64[sq]);
                        makros.SetBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[sq]);
                    }

                    else if (piece == (int)defs.Figuren.sB)
                    {
                        makros.SetBit(ref brett.bauern[(int)defs.Farben.SCHWARZ], defs.Sq120ToSq64[sq]);
                        makros.SetBit(ref brett.bauern[(int)defs.Farben.BEIDE], defs.Sq120ToSq64[sq]);
                    }
                }
            }
        }
    }
}
