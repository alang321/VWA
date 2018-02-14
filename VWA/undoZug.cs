using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    class undoZug
    {
        //zum erstellen einer array mit allen vergangenen zügen
        public int Zug { get; set; }
        public int RochadePerm { get; set; }
        public int EnPas { get; set; }
        public int FiftyMove { get; set; }
        public ulong PosKey { get; set; }
    }
}
