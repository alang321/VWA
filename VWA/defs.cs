using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    static class defs
    {
        //FEN strings zum Testen
        public static string START = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static string IllegalTest = "8/3q1p2/8/5P2/4Q3/8/8/8 w - - 1 1";
        public static string RochadeTest = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1"; 
        public static string perftTest = "8/Pk6/8/8/8/8/6Kp/8 w - - 0 1";
        public static string perftTest2 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        public static string WAC1 = "r1b1k2r/ppppnppp/2n2q2/2b5/3NP3/2P1B3/PP3PPP/RN1QKB1R w KQkq - 0 1";



        public const string name = "VWA 1.0";
        public const int BRD_SQ_NUM = 120;
        public const int MAXSPIELZÜGE = 2048;
        public const int MAXZÜGEPOSITION = 256;
        public const int MAXTIEFE = 64;

        public enum Spalten { SPALTE_A, SPALTE_B, SPALTE_C, SPALTE_D, SPALTE_E, SPALTE_F, SPALTE_G, SPALTE_H, SPALTE_NONE };
        public enum Zeilen { ZEILE_1, ZEILE_2, ZEILE_3, ZEILE_4, ZEILE_5, ZEILE_6, ZEILE_7, ZEILE_8, ZEILE_NONE };
        public enum HASH { HFNONE, HFALPHA, HFBETA, HFEXACT };

        public enum Engine { UCIMODE, XBOARDMODE, CONSOLEMODE };

        public enum Figuren { LEER, wB, wP, wL, wT, wD, wK, sB, sP, sL, sT, sD, sK };
        public enum Farben { WEISS, SCHWARZ, BEIDE };
        public enum Felder {
            A1 = 21, B1, C1, D1, E1, F1, G1, H1,
            A2 = 31, B2, C2, D2, E2, F2, G2, H2,
            A3 = 41, B3, C3, D3, E3, F3, G3, H3,
            A4 = 51, B4, C4, D4, E4, F4, G4, H4,
            A5 = 61, B5, C5, D5, E5, F5, G5, H5,
            A6 = 71, B6, C6, D6, E6, F6, G6, H6,
            A7 = 81, B7, C7, D7, E7, F7, G7, H7,
            A8 = 91, B8, C8, D8, E8, F8, G8, H8, NO_SQ, OFFBOARD};

        public enum Rochade { WKRC = 1, WDRC = 2, SKRC = 4, SDRC = 8 };
        
        //Arrays zum umrechnen von 120 feld brett zu 64feld und umgekehrt
        public static int[] Sq120ToSq64 =  new int[BRD_SQ_NUM];
        public static int[] Sq64ToSq120 =  new int[64];

        //arrays die sagen welche spalte/zeile ein feld ist
        public static int[] FilesBrd = new int[BRD_SQ_NUM];
        public static int[] RanksBrd = new int[BRD_SQ_NUM];

        //evaluation masks, erleichtern evaluierung ob zumbeispiel noch ein bauer von einer anderen farbe vor dem eigenen ist einem ist, ist wchtig weil die besser sind wegen promoten
        public static ulong[] FileMask = new ulong[8];
        public static ulong[] RankMask = new ulong[8];
        public static ulong[] BlackPassedMask = new ulong[64];
        public static ulong[] WhitePassedMask = new ulong[64];
        public static ulong[] IsolatedMask = new ulong[64];

        //flags für züge
        public static readonly int MFlagEP = 0x40000; //en passant
        public static readonly int MFlagPS = 0x80000; //bauer anfang dopelt, ps = pawn start
        public static readonly int MFlagCA = 0x1000000; //rocahde, castling
        public static readonly int MFlagCAP = 0x7C000; //capture
        public static readonly int MFlagPROM = 0xF00000; //prom
        public static readonly int NOMOVE = 0; //no move
        public static readonly int MATE = 29000; //mate
        public static readonly int INFINITE = 30000; //für alpha beta
        public static readonly int ISMATE = (INFINITE - MAXTIEFE); //für transposition table

        //mas hashtable size
        public static readonly int MaxHash = 1024; //für alpha beta
    }
}
