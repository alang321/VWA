using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class makros
    {
        public static int SZ2SQ(int s, int z)
        {
            return (21 + (s)) + ((z) * 10);
        }

        public static void ClearBit(ref ulong bb, int sq)
        {
            bb &= bitboards.ClearMask[sq];
        }

        public static void SetBit(ref ulong bb, int sq)
        {
            bb |= bitboards.SetMask[sq];
        }

        public static int FromSq(int zug)
        {
            return ((zug) & 0x7F);
        }

        public static int ToSq(int zug)
        {
            return (((zug)>>7) & 0x7F);
        }

        public static int Captured(int zug)
        {
            return (((zug) >> 14) & 0xF);
        }

        public static int Promoted(int zug)
        {
            return (((zug) >> 20) & 0xF);
        }

        public static bool MoveFlagEnPassant(int zug)
        {
            int probe = zug & 0x40000;
            if (probe != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFlagBauerDoppelt(int zug)
        {
            int probe = zug & 0x80000;
            if (probe != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFlagRochade(int zug)
        {
            int probe = zug & 0x1000000;
            if (probe != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFlagCapture(int zug)
        {
            int probe = zug & 0x7C000;
            if (probe != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFlagPromotion(int zug)
        {
            int probe = zug & 0xF00000;
            if (probe != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFlagNoMove(int zug)
        {
            if (zug == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int ZugGenerieren(int from, int to, int capture, int promoted, int flag)
        {
            return (from) | ((to) << 7) | ((capture) << 14) | ((promoted) << 20) | (flag);
        }


        public static void HashInPiece(int figur ,int feld, ref boardStruct brett)
        {
            brett.posKey ^= hashkeys.PieceKeys[figur,feld];
        }

        public static void HashInCastle(ref boardStruct brett)
        {
            brett.posKey ^= hashkeys.CastleKeys[brett.rochadePerm];
        }

        public static void HashInSide(ref boardStruct brett)
        {
            brett.posKey ^= hashkeys.SideKey;
        }

        public static void HashInEnPassant(ref boardStruct brett)
        {
            brett.posKey ^= hashkeys.PieceKeys[(int)defs.Figuren.LEER, brett.enPas];
        }
    }
}
