using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    class zug
    {
        /*
        0000 0000 0000 0000 0000 0111 1111 -> From 0x7F
        0000 0000 0000 0011 1111 1000 0000 -> To >> 7, 0x7F
        0000 0000 0011 1100 0000 0000 0000 -> Captured >> 14, 0xF
        0000 0000 0100 0000 0000 0000 0000 -> EP 0x40000
        0000 0000 1000 0000 0000 0000 0000 -> Pawn Start 0x80000
        0000 1111 0000 0000 0000 0000 0000 -> Promoted Piece >> 20, 0xF
        0001 0000 0000 0000 0000 0000 0000 -> Castle 0x1000000
        */

        //klassse nur zum spiecher der werte eines zuges
        public int Zug { get; set; }
        public int Wertung { get; set; }
    }
}
