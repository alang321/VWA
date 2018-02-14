using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class perft
    {
        public static long leafNodes;

        public static void Perft(int depth, ref boardStruct brett)
        {

            Debug.Assert(board.CheckBoard(ref brett));

            if (depth == 0)
            {
                leafNodes++;
                return;
            }

            movelist list = new movelist();
            movegen.GenerateAllMoves(ref list, ref brett);

            int MoveNum = 0;
            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {

                if (!makemove.MakeMove(list.zugliste[MoveNum].Zug, ref brett))
                {
                    continue;
                }
                Perft(depth - 1, ref brett);
                makemove.TakeMove(ref brett);
            }

            return;
        }

        public static void PerftTest(int depth, ref movelist list, ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));
            
            Console.Write("\nStarting Test To Depth:" + depth + "\n");
            leafNodes = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            movegen.GenerateAllMoves(ref list, ref brett);

            int move;
            int MoveNum = 0;
            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {
                move = list.zugliste[MoveNum].Zug;
                if (!makemove.MakeMove(move, ref brett))
                {
                    continue;
                }
                long newnodes = leafNodes;
                Perft(depth - 1, ref brett);
                makemove.TakeMove(ref brett);
                long oldnodes = leafNodes - newnodes;
                Console.Write(String.Format("move {0} : {1} : {2}\n", MoveNum + 1, io.PrMove(move), oldnodes));
            }

            timer.Stop();
            Console.Write(String.Format("\nTest Complete : {0} nodes visited in {1}ms\n", leafNodes, timer.ElapsedMilliseconds));
        }
    }
}
