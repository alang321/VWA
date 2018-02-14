using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class validate
    {
        public static bool SqOnBoard(int sq) 
        {
            if(defs.FilesBrd[sq] == (int)defs.Felder.OFFBOARD)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool SideValid(int side)
        {
            return (side == (int)defs.Farben.WEISS || side == (int)defs.Farben.SCHWARZ) ? true : false;
        }

        public static bool FileRankValid(int fr)
        {
            return (fr >= 0 && fr <= 7) ? true : false;
        }

        public static bool PieceValidEmpty(int pce)
        {
            return (pce >= (int)defs.Figuren.LEER && pce <= (int)defs.Figuren.sK) ? true : false;
        }

        public static bool PieceValid(int pce)
        {
            return (pce >= (int)defs.Figuren.wB && pce <= (int)defs.Figuren.sK) ? true : false;
        }
    }
}
