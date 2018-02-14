using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace VWA
{
    class Program
    {
        public static int index = 0;

        static void Main(string[] args)
        {
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            
            boardStruct brett = new boardStruct();
            movelist list = new movelist();
            suchInfo info = new suchInfo();

            init.AllInit(ref brett);

            bool loop = true;
            while (loop)
            {
                loop = false;
                Console.Write("\nGeben sie \"uci\" für den uci modus und \"vwa\" für den Konsolen modus ein: ");
                string input = Console.ReadLine();
                if(input.Length > 2 && string.Equals(input.Substring(0, 3), "uci", StringComparison.CurrentCultureIgnoreCase))
                {
                    uci.Uci_Loop(ref brett, ref info);
                }
                if (input.Length > 2 && string.Equals(input.Substring(0, 3), "vwa", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.Clear();
                    xboard.Console_Loop(ref brett, ref info);
                }
                else
                {
                    loop = true;
                }
            }

            
            
        }
    }
}
