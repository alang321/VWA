using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    class boardStruct
    {
        //Brett
        public int[] figuren = new int[defs.BRD_SQ_NUM];
        public ulong[] bauern = new ulong[3];
        public int[] KönigSq = new int[2];
        public int seite;
        public int enPas;
        public int fiftyMove;
        public int ply;
        public int hisPly;
        public ulong posKey;

        public int rochadePerm;

        public int[] pceNum = new int[13];
        public int[] bigPce = new int[2];
        public int[] majPce = new int[2];
        public int[] minPce = new int[2];
        public int[] material = new int[3];

        //Zug zurücknehmen
        public undoZug[] history = new undoZug[defs.MAXSPIELZÜGE];

        //Figur Liste
        public int[,] fListe = new int[13, 10];

        //pvtable
        public hashTableStruct PvTable = new hashTableStruct();
        
        //pvtable
        public int[] PvArray = new int[defs.MAXTIEFE];

        public int[,] searchHistory = new int[13, defs.BRD_SQ_NUM];
        public int[,] searchKillers = new int[2, defs.MAXTIEFE];


        public boardStruct()
        {
            for (int i = 0; i < history.Length; i++) history[i] = new undoZug();
        }
    }

    public static class options
    {
        public static bool UseBook;
    }
}
