using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    class movelist
    {
        //movelist
        public zug[] zugliste = new zug[defs.MAXZÜGEPOSITION];
        public int anzahlZüge;

        public movelist()
        {
            for (int i = 0; i < zugliste.Length; i++) zugliste[i] = new zug();
        }
    }
}
