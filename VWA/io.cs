using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class io
    {
        public static string PrSq(int sq)
        {
            string sqStr = Number2String(defs.FilesBrd[sq], false);
            sqStr += (defs.RanksBrd[sq] + 1).ToString();

            return sqStr;
        }

        //zum printen der buchstaben statt der zahlen für zug
        public static String Number2String(int number, bool isCaps)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number));
            return c.ToString();
        }

        public static string PrMove(int zug)
        {
            string sqStr = "";

            sqStr += PrSq(makros.FromSq(zug));
            sqStr += PrSq(makros.ToSq(zug));
            int promoted = makros.Promoted(zug);

	        if(makros.MoveFlagPromotion(zug))
            {
		        if(data.PieceKnight[promoted])
                {
                    sqStr += "n";
		        }
                else if(data.PieceRookQueen[promoted] && !data.PieceBishopQueen[promoted])
                {
                    sqStr += "r";
		        }
                else if(!data.PieceRookQueen[promoted] && data.PieceBishopQueen[promoted])
                {
                    sqStr += "b";
		        }
                else
                {
                    sqStr += "q";
                }
            }

	    return sqStr;
        }

        public static string PrintMoveList(movelist list)
        {
            string ret = "";
            int index = 0;
            int score = 0;
            int move = 0;
            ret += "\nZugListe:\n";

	        for(index = 0; index < list.anzahlZüge; ++index) {

		        move = list.zugliste[index].Zug;
		        score = list.zugliste[index].Wertung;

		        ret += String.Format("Zug:{0} > {1} (Wertung:{2})\n", index+1, PrMove(move), score);
            }
            ret += String.Format("Zug Liste Züge insgesamt: {0} \n\n", list.anzahlZüge);
            return ret;
        }

        public static string PrintBoard(ref boardStruct brett)
        {
            string ret = "";

            int sq, file, rank, piece;

            ret += "\nSpiel Brett:\n\n";

            for (rank = (int)defs.Zeilen.ZEILE_8; rank >= (int)defs.Zeilen.ZEILE_1; rank--)
            {
                ret += rank + 1 + "  ";
                for (file = (int)defs.Spalten.SPALTE_A; file <= (int)defs.Spalten.SPALTE_H; file++)
                {
                    sq = makros.SZ2SQ(file, rank);
                    piece = brett.figuren[sq];
                    ret += data.PceChar[piece].ToString().PadLeft(3);
                }
                ret += "\n";
            }

            ret += "\n     ";
            for (file = (int)defs.Spalten.SPALTE_A; file <= (int)defs.Spalten.SPALTE_H; file++)
            {
                ret += io.Number2String(file, true) + "  ";
            }
            ret += "\n\n";
            ret += "Seite: " + data.SideChar[brett.seite] + "\n";
            ret += "enPas: " + (PrSq(brett.enPas) != "Å101" ? PrSq(brett.enPas) : "-") + "\n";
            ret += "Rochade: " + ((brett.rochadePerm & (int)defs.Rochade.WKRC) == (int)defs.Rochade.WKRC ? "K" : "-") + ((brett.rochadePerm & (int)defs.Rochade.WDRC) == (int)defs.Rochade.WDRC ? "D" : "-") + ((brett.rochadePerm & (int)defs.Rochade.SKRC) == (int)defs.Rochade.SKRC ? "k" : "-") + ((brett.rochadePerm & (int)defs.Rochade.SDRC) == (int)defs.Rochade.SDRC ? "d" : "-") + "\n";
            ret += String.Format("PosKey: {0:X}", brett.posKey);

            return ret;
        }

        //zum züge eingeben von Zügen aus der Konsole
        public static int ParseMove(string moveInput, ref boardStruct brett)
        {

            Debug.Assert(board.CheckBoard(ref brett));

            //damit kein out of range ist wenn der string zu kurz für zurücknehmen ist
            if (moveInput.Length < 4)
            {
                return defs.NOMOVE;
            }

            // index eines buchstaben (c ist die character variable) = (int)c % 32;

            if (Convert.ToInt32(moveInput[1].ToString()) > 8 || Convert.ToInt32(moveInput[1].ToString()) < 1) return defs.NOMOVE;
            if (Convert.ToInt32(moveInput[3].ToString()) > 8 || Convert.ToInt32(moveInput[3].ToString()) < 1) return defs.NOMOVE;
            if (((int)moveInput[0] % 32) > 8 || ((int)moveInput[0] % 32) < 1) return defs.NOMOVE;
            if (((int)moveInput[2] % 32) > 8 || ((int)moveInput[2] % 32) < 1) return defs.NOMOVE;

            int from = makros.SZ2SQ(((int)moveInput[0] % 32)-1, moveInput[1] - '1');
            int to = makros.SZ2SQ(((int)moveInput[2] % 32)-1, moveInput[3] - '1');

            Debug.Assert(validate.SqOnBoard(from) && validate.SqOnBoard(to));

            movelist list = new movelist();
            movegen.GenerateAllMoves(ref list, ref brett);
            int MoveNum = 0;
            int Move = 0;
            int PromPce = (int)defs.Figuren.LEER;

            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {
                Move = list.zugliste[MoveNum].Zug;
                if (makros.FromSq(Move) == from && makros.ToSq(Move) == to)
                {
                    PromPce = makros.Promoted(Move);
                    if (PromPce != (int)defs.Figuren.LEER)
                    {
                        if(moveInput.Length == 4 || moveInput[4] == ' ')
                        {
                            continue;
                        }
                        else if (data.PieceRookQueen[PromPce] && !data.PieceBishopQueen[PromPce] && moveInput[4] == 'r')
                        {
                            return Move;
                        }
                        else if (!data.PieceRookQueen[PromPce] && data.PieceBishopQueen[PromPce] && moveInput[4] == 'b')
                        {
                            return Move;
                        }
                        else if (data.PieceRookQueen[PromPce] && data.PieceBishopQueen[PromPce] && moveInput[4] == 'q')
                        {
                            return Move;
                        }
                        else if (data.PieceKnight[PromPce] &&  moveInput[4] == 'n')
                        {
                            return Move;
                        }
                        continue;
                    }
                    return Move;
                }
            }

            return defs.NOMOVE;
        }
    }
}
