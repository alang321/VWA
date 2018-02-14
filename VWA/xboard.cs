using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class xboard
    {
        static int ThreeFoldRep(ref boardStruct brett) 
        {
            Debug.Assert(board.CheckBoard(ref brett));
            int r = 0;

	        for (int i = 0; i < brett.hisPly; ++i)	
            {
	            if (brett.history[i].PosKey == brett.posKey) r++;
            
            }
	        return r;
        }

        static bool DrawMaterial(ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));

            if (brett.pceNum[(int)defs.Figuren.wB] != 0 || brett.pceNum[(int)defs.Figuren.sB] != 0) return false;
            if (brett.pceNum[(int)defs.Figuren.wD] != 0 || brett.pceNum[(int)defs.Figuren.sD] != 0 || brett.pceNum[(int)defs.Figuren.wT] != 0 || brett.pceNum[(int)defs.Figuren.sT] != 0) return false;
            if (brett.pceNum[(int)defs.Figuren.wL] > 1 || brett.pceNum[(int)defs.Figuren.wL] > 1) { return false; }
            if (brett.pceNum[(int)defs.Figuren.wP] > 1 || brett.pceNum[(int)defs.Figuren.sP] > 1) { return false; }
            if (brett.pceNum[(int)defs.Figuren.wP] != 0 && brett.pceNum[(int)defs.Figuren.wL] != 0) { return false; }
            if (brett.pceNum[(int)defs.Figuren.sP] != 0 && brett.pceNum[(int)defs.Figuren.sL] != 0) { return false; }

            return true;
        }

        static bool checkresult(ref boardStruct brett)
        {
            Debug.Assert(board.CheckBoard(ref brett));

            if (brett.fiftyMove > 100)
            {
                Console.WriteLine("1/2-1/2 {fifty move rule (claimed by Vwa)}\n"); return true;
            }

            if (ThreeFoldRep(ref brett) >= 2)
            {
                Console.WriteLine("1/2-1/2 {3-fold repetition (claimed by Vwa)}\n"); return true;
            }

            if (DrawMaterial(ref brett))
            {
                Console.WriteLine("1/2-1/2 {insufficient material (claimed by Vwa)}\n"); return true;
            }

            movelist list = new movelist();
            movegen.GenerateAllMoves(ref list, ref brett);

            int MoveNum = 0;
            int found = 0;
            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {

                if (!makemove.MakeMove(list.zugliste[MoveNum].Zug, ref brett))
                {
                    continue;
                }
                found++;
                makemove.TakeMove(ref brett);
                break;
            }

            if (found != 0) return false;

            bool InCheck = attack.SqAttacked(brett.KönigSq[brett.seite], brett.seite ^ 1, ref brett);

            if (InCheck)
            {
                if (brett.seite == (int)defs.Farben.WEISS)
                {
                    Console.WriteLine("0-1 {black mates (claimed by Vwa)}\n"); return true;
                }
                else
                {
                    Console.WriteLine("0-1 {white mates (claimed by Vwa)}\n"); return true;
                }
            }
            else
            {
                Console.WriteLine("\n1/2-1/2 {stalemate (claimed by Vwa)}\n"); return true;
            }
            return false;
        }

        static void PrintOptions()
        {
            Console.WriteLine("feature ping=1 setboard=1 colors=0 usermove=1 memory=1\n");
            Console.WriteLine("feature done=1\n");
        }

        public static void XBoard_Loop(ref boardStruct brett, ref suchInfo info)
        {
            PrintOptions(); // HACK

            int[] movestogo = { 30, 30 };
            int depth = -1, movetime = -1;
            int time = -1, inc = 0;
            int engineSide = (int)defs.Farben.BEIDE;
            int timeLeft;
            int sec;
            int mps = 0;
            int move = defs.NOMOVE;
            char[] inBuf = new char[80], command = new char[80];
            int MB;

            engineSide = (int)defs.Farben.SCHWARZ;
            board.ParseFen(defs.START, ref brett);
            depth = -1;
            time = -1;

            while (true)
            {
                if (brett.seite == engineSide && !checkresult(ref brett))
                {
                    info.timer.Start();
                    //info.starttime = GetTimeMs();
                    info.depth = depth;

                    if (time != -1)
                    {
                        info.timeset = true;
                        time /= movestogo[brett.seite];
                        time -= 50;
                        info.stoptime = time + inc;
                    }

                    if (depth == -1 || depth > defs.MAXTIEFE)
                    {
                        info.depth = defs.MAXTIEFE;
                    }

                    Console.WriteLine(String.Format("time:{0} start:{1} stop:{2} depth:{3} timeset:{4} movestogo:{5} mps:{6}\n", time, 0, info.stoptime, info.depth, Convert.ToInt32(info.timeset), movestogo[brett.seite], mps));
                    search.SearchPosition(ref brett, ref info);

                    if (mps != 0)
                    {
                        movestogo[brett.seite ^ 1]--;
                        if (movestogo[brett.seite ^ 1] < 1)
                        {
                            movestogo[brett.seite ^ 1] = mps;
                        }
                    }

                }
                //if (!fgets(inBuf, 80, stdin))continue;

                string input = Console.ReadLine();

                //sscanf(inBuf, "%s", command);

                Console.WriteLine(string.Format("command seen:{0}\n", input));

                if (input.Length > 3 && input.Substring(0,4) == "quit")
                {
                    info.quit = true;
                    break;
                }

                if (input.Length > 4 && input.Substring(0, 5) == "force")
                {
                    engineSide = (int)defs.Farben.BEIDE;
                    continue;
                }

                if (input.Length > 7 && input.Substring(0, 8) == "protover")
                {
                    PrintOptions();
                    continue;
                }

                if (input.Length > 6 && input.Substring(0, 7) == "polykey")
                {
                    Console.WriteLine(io.PrintBoard(ref brett));
                    //polybook.GetBookMove(ref brett);
                    continue;
                }

                if (input.Length > 1 && input.Substring(0, 2) == "sd")
                {
                    //sscanf(inBuf, "sd %d", &depth);
                    //Console.WriteLine("DEBUG depth:" + depth + "\n");
                    continue;
                }

                /*if (!strcmp(command, "st"))
                {
                    sscanf(inBuf, "st %d", &movetime);
                    printf("DEBUG movetime:%d\n", movetime);
                    continue;
                }

                if (!strcmp(command, "time"))
                {
                    sscanf(inBuf, "time %d", &time);
                    time *= 10;
                    printf("DEBUG time:%d\n", time);
                    continue;
                }*/

                if (input.Length > 6 && input.Substring(0, 7) == "polykey")
                {
                    Console.WriteLine(io.PrintBoard(ref brett));
                    //GetBookMove(pos);
                    continue;
                }

                /*if (!strcmp(command, "memory"))
                {
                    sscanf(inBuf, "memory %d", &MB);
                    if (MB < 4) MB = 4;
                    if (MB > MAX_HASH) MB = MAX_HASH;
                    printf("Set Hash to %d MB\n", MB);
                    InitHashTable(pos->HashTable, MB);
                    continue;
                }*/

                /*if (!strcmp(command, "level"))
                {
                    sec = 0;
                    movetime = -1;
                    if (sscanf(inBuf, "level %d %d %d", &mps, &timeLeft, &inc) != 3)
                    {
                        sscanf(inBuf, "level %d %d:%d %d", &mps, &timeLeft, &sec, &inc);
                        printf("DEBUG level with :\n");
                    }
                    else
                    {
                        printf("DEBUG level without :\n");
                    }
                    timeLeft *= 60000;
                    timeLeft += sec * 1000;
                    movestogo[0] = movestogo[1] = 30;
                    if (mps != 0)
                    {
                        movestogo[0] = movestogo[1] = mps;
                    }
                    time = -1;
                    printf("DEBUG level timeLeft:%d movesToGo:%d inc:%d mps%d\n", timeLeft, movestogo[0], inc, mps);
                    continue;
                }

                if (!strcmp(command, "ping"))
                {
                    printf("pong%s\n", inBuf + 4);
                    continue;
                }*/
                if (input.Length > 2 && input.Substring(0, 3) == "new")
                {
                    PvTable.ClearPvTable(ref brett.PvTable);
                    engineSide = (int)defs.Farben.SCHWARZ;
                    board.ParseFen(defs.START, ref brett);
                    depth = -1;
                    time = -1;
                    
                    continue;
                }

                if (input.Length > 7 && input.Substring(0, 8) == "setboard")
                {
                    engineSide = (int)defs.Farben.BEIDE;
                    board.ParseFen(input.Substring(9), ref brett);
                    continue;
                }

                if (input.Length > 1 && input.Substring(0, 2) == "go")
                {
                    engineSide = brett.seite;
                    continue;
                }

                if (input.Length > 7 && input.Substring(0, 8) == "usermove")
                {
                    movestogo[brett.seite]--;
                    move = io.ParseMove(input.Substring(9), ref brett);
                    if (move == defs.NOMOVE) continue;
                    makemove.MakeMove(move, ref brett);
                    brett.ply = 0;
                }
            }
        }

        public static void Console_Loop(ref boardStruct brett, ref suchInfo info)
        {

            Console.WriteLine("Welcome to Vwa In Console Mode!\n");
            Console.WriteLine("Type help for commands\n");

            info.GAME_MODE = (int)defs.Engine.CONSOLEMODE;
            info.POST_THINKING = true;

            int depth = defs.MAXTIEFE, movetime = 3000;
            int engineSide = (int)defs.Farben.BEIDE;
            int move = defs.NOMOVE;

            engineSide = (int)defs.Farben.SCHWARZ;
            board.ParseFen(defs.START, ref brett);

            while (true)
            {
                if (brett.seite == engineSide && !checkresult(ref brett))
                {
                    info.timer.Reset();
                    info.timer.Start();
                    info.starttime = 1;
                    info.depth = depth;

                    if (movetime != 0)
                    {
                        info.timeset = true;
                        info.stoptime = movetime;
                    }

                    search.SearchPosition(ref brett, ref info);
                }

                Console.Write("\nVwa > ");

                string input = Console.ReadLine();

                if (input.Length > 3 && input.Substring(0, 4) == "help")
                {
                    Console.Write("\nCommands:\n");
                    Console.Write("quit - quit game\n");
                    Console.Write("force - computer will not think\n");
                    Console.Write("print - show board\n");
                    Console.Write("post - show thinking\n");
                    Console.Write("nopost - do not show thinking\n");
                    Console.Write("new - start new game\n");
                    Console.Write("go - set computer thinking\n");
                    Console.Write("depth x - set depth to x\n");
                    Console.Write("time x - set thinking time to x seconds (depth still applies if set)\n");
                    Console.Write("view - show current depth and movetime settings\n");
                    Console.Write("setboard x - set position to fen x\n");
                    Console.Write("** note ** - to reset time and depth, set to 0\n");
                    Console.Write("enter moves using b7b8q notation\n\n\n");
                    continue;
                }

                if (input.Length > 3 && input.Substring(0, 4) == "eval")
                {
                    Console.WriteLine(io.PrintBoard(ref brett));
                    Console.WriteLine("Eval:" + evaluate.EvalPosition(ref brett));
                    board.MirrorBoard(ref brett);
                    Console.WriteLine(io.PrintBoard(ref brett));
                    Console.WriteLine("Eval:" + evaluate.EvalPosition(ref brett));
                    continue;
                }

                if (input.Length > 7 && input.Substring(0, 8) == "setboard")
                {
                    engineSide = (int)defs.Farben.BEIDE;
                    board.ParseFen(input.Substring(9), ref brett);
                    continue;
                }

                if (input.Length > 3 && input.Substring(0, 4) == "quit")
                {
                    info.quit = true;
                    break;
                }

                if (input.Length > 3 && input.Substring(0, 4) == "post")
                {
                    info.POST_THINKING = true;
                    continue;
                }

                if (input.Length > 4 && input.Substring(0, 5) == "print")
                {
                    Console.WriteLine(io.PrintBoard(ref brett));
                    continue;
                }

                if (input.Length > 5 && input.Substring(0, 6) == "nopost")
                {
                    info.POST_THINKING = false;
                    continue;
                }

                if (input.Length > 4 && input.Substring(0, 5) == "force")
                {
                    engineSide = (int)defs.Farben.BEIDE;
                    continue;
                }

                if (input.Length > 3 && input.Substring(0, 4) == "view")
                {
                    if (depth == defs.MAXTIEFE) Console.WriteLine("depth not set ");
                    else Console.WriteLine("depth " + depth);

                    if (movetime != 0) Console.WriteLine("movetime " + movetime / 1000 + "\n");
                    else Console.WriteLine("movetime not set\n");

                    continue;
                }

                if (input.Length > 4 && input.Substring(0, 5) == "depth")
                {
                    if(input.Length > 6)
                    {
                        depth = Convert.ToInt32(input.Substring(6));
                    }
                    if (depth == 0) depth = defs.MAXTIEFE;
                    continue;
                }

                if (input.Length > 3 && input.Substring(0, 4) == "time")
                {
                    if (input.Length > 5)
                    {
                        movetime = Convert.ToInt32(input.Substring(5));
                    }
                    movetime *= 1000;
                    continue;
                }

                if (input.Length > 2 && input.Substring(0, 3) == "new")
                {
                    PvTable.ClearPvTable(ref brett.PvTable);
                    engineSide = (int)defs.Farben.SCHWARZ;
                    board.ParseFen(defs.START, ref brett);
                    continue;
                }

                if (input.Length > 1 && input.Substring(0, 2) == "go")
                {
                    engineSide = brett.seite;
                    continue;
                }

                move = io.ParseMove(input, ref brett);
                if (move == defs.NOMOVE)
                {
                    Console.WriteLine("Command unknown:" + input + "\n");
                    continue;
                }
                makemove.MakeMove(move, ref brett);
                brett.ply = 0;
            }
        }
    }
}
