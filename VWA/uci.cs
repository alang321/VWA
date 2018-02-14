using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

namespace VWA
{
    static class uci
    {
        public static bool quitsearch = true;

        public static void ParseGo(string input, ref suchInfo info, ref boardStruct brett)
        {
            input += " ";

            int depth = -1, movestogo = 30, movetime = -1;
            int time = -1, inc = 0;
            info.timeset = false;

            int pointer = 0;
            int pointer2 = 0;


            pointer = input.IndexOf("infinite");
            if (pointer != -1)
            {
                ;
            }

            pointer = input.IndexOf("binc");
            if (pointer != -1 && brett.seite == (int)defs.Farben.SCHWARZ)
            {
                pointer2 = input.Substring(pointer + 5).IndexOf(" ");
                inc = Convert.ToInt32(input.Substring(pointer+5, pointer2));
            }


            pointer = input.IndexOf("winc");
            if (pointer != -1 && brett.seite == (int)defs.Farben.WEISS)
            {
                pointer2 = input.Substring(pointer + 5).IndexOf(" ");
                inc = Convert.ToInt32(input.Substring(pointer + 5,pointer2));
            }


            pointer = input.IndexOf("wtime");
            if (pointer != -1 && brett.seite == (int)defs.Farben.WEISS)
            {
                pointer2 = input.Substring(pointer + 6).IndexOf(" ");
                time = Convert.ToInt32(input.Substring(pointer + 6, pointer2));
            }

            pointer = input.IndexOf("btime");
            if (pointer != -1 && brett.seite == (int)defs.Farben.SCHWARZ)
            {
                pointer2 = input.Substring(pointer + 6).IndexOf(" ");
                time = Convert.ToInt32(input.Substring(pointer + 6, pointer2));
            }

            pointer = input.IndexOf("movestogo");
            if (pointer != -1)
            {
                pointer2 = input.Substring(pointer + 10).IndexOf(" ");
                movestogo = Convert.ToInt32(input.Substring(pointer + 10, pointer2));
            }

            pointer = input.IndexOf("movetime");
            if (pointer != -1)
            {
                pointer2 = input.Substring(pointer + 9).IndexOf(" ");
                movetime = Convert.ToInt32(input.Substring(pointer + 9, pointer2));
            }

            pointer = input.IndexOf("depth");
            if (pointer != -1)
            {
                pointer2 = input.Substring(pointer + 6).IndexOf(" ");
                depth = Convert.ToInt32(input.Substring(pointer + 6, pointer2));
            }

            if (movetime != -1)
            {
                time = movetime;
                movestogo = 1;
            }

            info.depth = depth;

            if (time != -1)
            {
                info.timer.Reset();
                info.timer.Start();
                info.timeset = true;
                time /= movestogo;
                time -= 50;
                info.stoptime = time + inc;
            }

            if (depth == -1)
            {
                info.depth = defs.MAXTIEFE;
            }

            if (info.timer.IsRunning)
            {
                Console.WriteLine(String.Format("timer: Running stop:{0} tiefe:{1} timeset:{2}\n", info.stoptime, info.depth, info.timeset));
            }
            else
            {
                Console.WriteLine(String.Format("timer: Not Running stop:{0} tiefe:{1} timeset:{2}\n", info.stoptime, info.depth, info.timeset));
            }

            search.SearchPosition(ref brett, ref info);
        }

        public static void ParsePosition(string input, ref boardStruct brett)
        {
            if (input.Substring(9, 8) == "startpos")
            {
                board.ParseFen(defs.START, ref brett);
            }
            else
            {
                board.ParseFen(input.Substring(13), ref brett);
            }

            int pointer = input.IndexOf("moves");
            int move;

            if (pointer != -1)
            {
                pointer += 6;

                while (pointer < input.Length)
                {
                    move = io.ParseMove(input.Substring(pointer), ref brett);
                    if (move == defs.NOMOVE) break;
                    makemove.MakeMove(move, ref brett);
                    brett.ply = 0;
                    while (pointer < input.Length && input[pointer] != ' ') pointer++;
                    pointer++;
                }
            }

            Console.WriteLine(io.PrintBoard(ref brett));
        }

        public static void Uci_Loop(ref boardStruct brett, ref suchInfo info)
        {
            info.GAME_MODE = (int)defs.Engine.UCIMODE;

            //setbuf(stdin, NULL);
            //setbuf(stdout, NULL);

            //char line[INPUTBUFFER];
            Console.Write("id name VWA\n");
            Console.Write("id author Anton Lang\n");
            Console.Write("option name Hash type spin default 64 min 4 max {0}\n", defs.MaxHash);
            Console.Write("option name Book type check default true\n");
            Console.Write("uciok\n");

            quitsearch = true;

            TextReader a = Console.In;

            while (true)
            {
                string input = Console.ReadLine();

                

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input[0] == '\n') continue;

                if (input.Length > 6 && input.Substring(0, 7) == "isready")
                {
                    Console.WriteLine("readyok");
                    continue;
                }
                else if (input.Length > 7 && input.Substring(0, 8) == "position")
                {
                    ParsePosition(input, ref brett);
                }
                else if (input.Length > 9 && input.Substring(0, 10) == "ucinewgame")
                {
                    ParsePosition("position startpos\n", ref brett);
                }
                else if (input.Length > 1 && input.Substring(0, 2) == "go")
                {
                    Console.WriteLine("Main thread is: {0}", Thread.CurrentThread.ManagedThreadId);
                    ParseGo(input, ref info, ref brett);
                }
                else if (input.Length > 3 && input.Substring(0, 4) == "quit")
                {
                    Console.WriteLine("Seen Quit");
                    info.quit = true;
                    break;
                }
                else if (input.Length > 2 && input.Substring(0, 3) == "uci")
                {
                    Console.Write("id name VWA\n");
                    Console.Write("id author Anton Lang\n");
                    Console.Write("uciok\n");
                }
                else if (input.Length > 4 && input.Substring(0, 5) == "debug")
                {
                    //DebugAnalysisTest(pos, info);
                    break;
                }
                else if (input.Length > 25 && input.Substring(0, 26) == "setoption name Hash value ")
                {
                    /*sscanf(line, "%*s %*s %*s %*s %d", &MB);
                    if (MB < 4) MB = 4;
                    if (MB > MAX_HASH) MB = MAX_HASH;
                    printf("Set Hash to %d MB\n", MB);
                    InitHashTable(pos->HashTable, MB);*/
                }
                else if (input.Length > 25 && input.Substring(0, 26) == "setoption name Book value ")
                {
                    int index = input.IndexOf("true");
                    if (index != -1)
                    {
                        options.UseBook = true;
                    }
                    else
                    {
                        options.UseBook = false;
                    }
                }
                if (info.quit) break;
            }
        }
    }
}