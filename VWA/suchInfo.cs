using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    class suchInfo
    {
        public int starttime;
        public int stoptime;
        public int depth;
        public bool timeset;
        public int movestogo;

        public Stopwatch timer = new Stopwatch();

        public long nodes;

        public bool quit;
        public bool stopped;

        public float fh;
        public float fhf;
        public int nullCut;

        public int GAME_MODE;
        public bool POST_THINKING;
    }
}
