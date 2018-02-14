using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class hashkeys
    {
        //zum genberiern von hashkeys, hier poskey genannt
        public static ulong[,] PieceKeys = new ulong[13, 120];
        public static ulong SideKey;
        public static ulong[] CastleKeys = new ulong[16];

        public static ulong GeneratePosKey(ref boardStruct brett) 
        {
            int sq = 0;
            ulong Key = 0;
            int piece = (int)defs.Figuren.LEER;
	
	        // pieces
	        for(sq = 0; sq<defs.BRD_SQ_NUM; ++sq) {
		        piece = brett.figuren[sq];
		        if( piece != (int)defs.Felder.NO_SQ && piece != (int)defs.Figuren.LEER && piece != (int)defs.Felder.OFFBOARD) {
                    Key ^= PieceKeys[piece,sq];
		        }
            }
	
	        if(brett.seite == (int)defs.Farben.WEISS) {
		        Key ^= SideKey;
	        }
		
	        if(brett.enPas != (int)defs.Felder.NO_SQ) 
            {
                Key ^= PieceKeys[(int)defs.Figuren.LEER, brett.enPas];
	        }

            Key ^= CastleKeys[brett.rochadePerm];
	
	        return Key;
        }
    }
}
