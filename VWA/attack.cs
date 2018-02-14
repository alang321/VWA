using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class attack
    {
        static readonly int[] KnDir = { -8, -19, -21, -12, 8, 19, 21, 12 };
        static readonly int[] RkDir = { -1, -10, 1, 10 };
        static readonly int[] BiDir = { -9, -11, 11, 9 };
        static readonly int[] KiDir = { -1, -10, 1, 10, -9, -11, 11, 9 };

        public static bool SqAttacked(int sq, int side, ref boardStruct brett)
        {

            int pce, index, t_sq, dir;

            Debug.Assert(validate.SqOnBoard(sq));
            Debug.Assert(validate.SideValid(side));
            Debug.Assert(board.CheckBoard(ref brett));

            // bauern
            if (side == (int)defs.Farben.WEISS)
            {
                if (brett.figuren[sq - 11] == (int)defs.Figuren.wB || brett.figuren[sq - 9] == (int)defs.Figuren.wB)
                {
                    return true;
                }
            }
            else
            {
                if (brett.figuren[sq + 11] == (int)defs.Figuren.sB || brett.figuren[sq + 9] == (int)defs.Figuren.sB)
                {
                    return true;
                }
            }

            // pferd
            for (index = 0; index < 8; ++index)
            {
                pce = brett.figuren[sq + KnDir[index]];
                //Debug.Assert(PceValidEmptyOffbrd(pce));
                if (pce != (int)defs.Felder.OFFBOARD && data.PieceKnight[pce] && data.PieceCol[pce] == side)
                {
                    return true;
                }
            }

            // Turm Dame
            for (index = 0; index < 4; ++index)
            {
                dir = RkDir[index];
                t_sq = sq + dir;
                //Debug.Assert(SqIs120(t_sq));
                pce = brett.figuren[t_sq];
                //Debug.Assert(PceValidEmptyOffbrd(pce));
                while (pce != (int)defs.Felder.OFFBOARD)
                {
                    if (pce != (int)defs.Figuren.LEER)
                    {
                        if (data.PieceRookQueen[pce] && data.PieceCol[pce] == side)
                        {
                            return true;
                        }
                        break;
                    }
                    t_sq += dir;
                    //Debug.Assert(SqIs120(t_sq));
                    pce = brett.figuren[t_sq];
                }
            }

            // läufer dame
            for (index = 0; index < 4; ++index)
            {
                dir = BiDir[index];
                t_sq = sq + dir;
                //Debug.Assert(SqIs120(t_sq));
                pce = brett.figuren[t_sq];
                //Debug.Assert(PceValidEmptyOffbrd(pce));
                while (pce != (int)defs.Felder.OFFBOARD)
                {
                    if (pce != (int)defs.Figuren.LEER)
                    {
                        if (data.PieceBishopQueen[pce] && data.PieceCol[pce] == side)
                        {
                            return  true;
                        }
                        break;
                    }
                    t_sq += dir;
                    //Debug.Assert(SqIs120(t_sq));
                    pce = brett.figuren[t_sq];
                }
            }

            // könig
            for (index = 0; index < 8; ++index)
            {
                pce = brett.figuren[sq + KiDir[index]];
                //Debug.Assert(PceValidEmptyOffbrd(pce));
                if (pce != (int)defs.Felder.OFFBOARD && data.PieceKing[pce] && data.PieceCol[pce] == side)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
