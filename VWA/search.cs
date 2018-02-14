using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace VWA
{
    static class search
    {
        static int rootDepth;
        static void PickNextMove(int moveNum, ref movelist list)
        {

            zug temp = new zug();
            int index = 0;
            int bestScore = 0;
            int bestNum = moveNum;

            for (index = moveNum; index < list.anzahlZüge; ++index)
            {
                if (list.zugliste[index].Wertung > bestScore)
                {
                    bestScore = list.zugliste[index].Wertung;
                    bestNum = index;
                }
            }

            Debug.Assert(moveNum >= 0 && moveNum < list.anzahlZüge);
            Debug.Assert(bestNum >= 0 && bestNum < list.anzahlZüge);
            Debug.Assert(bestNum >= moveNum);

            temp = list.zugliste[moveNum];
            list.zugliste[moveNum] = list.zugliste[bestNum];
            list.zugliste[bestNum] = temp;
        }

        static void CheckUp(ref suchInfo info)
        {
            if (info.timeset && info.timer.ElapsedMilliseconds > info.stoptime)
            {
                info.timer.Stop();
                info.stopped = true;
            }
        }

        public static bool IsRepetition(ref boardStruct brett)
        {
	        for(int index = brett.hisPly - brett.fiftyMove; index<brett.hisPly-1; ++index)
            {
		        Debug.Assert(index >= 0 && index < defs.MAXSPIELZÜGE);
		        if(brett.posKey == brett.history[index].PosKey)
                {
			        return true;
		        }
            }
	        return false;
        }

        static void ClearForSearch(ref boardStruct brett, ref suchInfo info)
        {

            int index = 0;
            int index2 = 0;

            for (index = 0; index < 13; ++index)
            {
                for (index2 = 0; index2 < defs.BRD_SQ_NUM; ++index2)
                {
                    brett.searchHistory[index,index2] = 0;
                }
            }

            for (index = 0; index < 2; ++index)
            {
                for (index2 = 0; index2 < defs.MAXTIEFE; ++index2)
                {
                    brett.searchKillers[index, index2] = 0;
                }
            }

            brett.PvTable.overWrite = 0;
            brett.PvTable.hit = 0;
            brett.PvTable.cut = 0;
            brett.ply = 0;

            PvTable.ClearPvTable(ref brett.PvTable);

            info.stopped = false;
            info.nodes = 0;
            info.fh = 0;
            info.fhf = 0;
        }

        public static void SearchPosition(ref boardStruct brett, ref suchInfo info)
        {
            int bestMove = defs.NOMOVE;
            int bestScore = -defs.INFINITE;
            int currentDepth = 0;
            int pvMoves = 0;
            int pvNum = 0;

            ClearForSearch(ref brett, ref info);

            if (options.UseBook)
            {
                bestMove = polybook.GetBookMove(ref brett);
            }

            //Console.WriteLine(String.Format("Such tiefe:{0}\n",info.depth));

            // iterative deepening

            if(bestMove == defs.NOMOVE)
            {
                for (currentDepth = 1; currentDepth <= info.depth; ++currentDepth)
                {
                    // alpha	 beta
                    rootDepth = currentDepth;
                    bestScore = AlphaBeta(-defs.INFINITE, defs.INFINITE, currentDepth, ref brett, ref info, true);

                    if (info.stopped)
                    {
                        break;
                    }

                    pvMoves = PvTable.GetPvLine(currentDepth, ref brett);
                    bestMove = brett.PvArray[0];


                    if (info.GAME_MODE == (int)defs.Engine.UCIMODE)
                    {
                        //Console.Write(String.Format("Score: {0} Tief: {1} Nodes: {2} Time: {3}",  bestScore, currentDepth , info.nodes, info.timer.Elapsed));
                        Console.Write(String.Format("\ninfo score cp {0} depth {1} nodes {2} time {3} ", bestScore, currentDepth, info.nodes, info.timer.ElapsedMilliseconds));
                    }
                    else if (info.GAME_MODE == (int)defs.Engine.XBOARDMODE && info.POST_THINKING)
                    {
                        Console.WriteLine("{0} {1} {2} {3} ",currentDepth, bestScore, info.timer.ElapsedMilliseconds/10, info.nodes);
                    }
                    else if (info.POST_THINKING)
                    {
                        Console.Write(String.Format("\nscore:{0} depth:{1} nodes:{2} time:{3}ms ", bestScore, currentDepth, info.nodes, info.timer.ElapsedMilliseconds));
                    }
                    if (info.GAME_MODE == (int)defs.Engine.UCIMODE || info.POST_THINKING)
                    {
                        pvMoves = PvTable.GetPvLine(currentDepth, ref brett);
                    
                        if (info.GAME_MODE != (int)defs.Engine.XBOARDMODE)
                        {
                            Console.Write("pv");
                        }
                        for (pvNum = 0; pvNum < pvMoves; pvNum++)
                        {
                            Console.Write(" " + io.PrMove(brett.PvArray[pvNum]));
                        }

                        Console.Write("");
                    }

                    //printf("Hits:%d Overwrite:%d NewWrite:%d Cut:%d\nOrdering %.2f NullCut:%d\n",pos.HashTable.hit,pos.HashTable.overWrite,pos.HashTable.newWrite,pos.HashTable.cut,
                    //(info.fhf/info.fh)*100,info.nullCut);
                }
            }
            

            if (info.GAME_MODE == (int)defs.Engine.UCIMODE)
            {
                Console.WriteLine("\nbestmove {0}\n", io.PrMove(bestMove));
            }
            else if (info.GAME_MODE == (int)defs.Engine.XBOARDMODE)
            {
                Console.WriteLine("move " + io.PrMove(bestMove) + "\n");
                makemove.MakeMove(bestMove, ref brett);
            }
            else
            {
                Console.WriteLine("\n\n***!! Vwa makes move " + io.PrMove(bestMove) + "!!***\n\n");
                makemove.MakeMove(bestMove, ref brett);
                Console.WriteLine(io.PrintBoard(ref brett));
            }

        }

        public static int AlphaBeta(int alpha, int beta, int depth, ref boardStruct brett, ref suchInfo info, bool DoNull) 
        {
            Debug.Assert(board.CheckBoard(ref brett));
            Debug.Assert(alpha<beta);
            Debug.Assert(depth >= 0);

            if (depth <= 0)
            {
                return Quiescence(alpha, beta, ref brett, ref info);
                //return evaluate.EvalPosition(ref brett);
            }

            if ((info.nodes % 2047) == 0)
            {
                CheckUp(ref info);
            }

            info.nodes++;

            if ((IsRepetition(ref brett) || brett.fiftyMove >= 100) && brett.ply != 0)
            {
                return 0;
            }

            if (brett.ply > defs.MAXTIEFE- 1)
            {
                return evaluate.EvalPosition(ref brett);
            }

            bool inSchach = attack.SqAttacked(brett.KönigSq[brett.seite], brett.seite ^ 1, ref brett);
            //wenn könig in schach ist gleich tiefer weil das sonst unnötig ist weil eh in schach
            if (inSchach)
            {
                depth++;
            }

            int Score = -defs.INFINITE;
            int PvMove = defs.NOMOVE;

            if (PvTable.ProbeHashEntry(ref PvMove, ref Score, alpha, beta, depth, ref brett))
            {
                brett.PvTable.cut++;
                return Score;
            }

            if (DoNull && !inSchach && brett.ply > 0 && (brett.bigPce[brett.seite] > 0) && depth >= 4)
            {
                makemove.MakeNullMove(ref brett);
                Score = -AlphaBeta(-beta, -beta + 1, depth - 4, ref brett, ref info, false);
                makemove.TakeNullMove(ref brett);
                if (info.stopped)
                {
                    return 0;
                }

                if (Score >= beta && Math.Abs(Score) < defs.ISMATE)
                {
                    info.nullCut++;
                    return beta;
                }
            }


            movelist list = new movelist();
            movegen.GenerateAllMoves(ref list, ref brett);

            int MoveNum = 0;
            int Legal = 0;
            int OldAlpha = alpha;
            int BestMove = defs.NOMOVE;
            Score = -defs.INFINITE;

            int BestScore = -defs.INFINITE;

            //checkt ob man in der best möglichewn line ist dann sucht es diesesen zug zuerst
            if (PvMove != defs.NOMOVE)
            {
                for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
                {
                    if(list.zugliste[MoveNum].Zug == PvMove)
                    {
                        list.zugliste[MoveNum].Wertung = 2000000;
                        break;
                    }
                }
            }

            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {
                PickNextMove(MoveNum, ref list);

                if (!makemove.MakeMove(list.zugliste[MoveNum].Zug, ref brett))
                {
                    continue;
                }

                Legal++;
                Score = -AlphaBeta(-beta, -alpha, depth - 1, ref brett, ref info, true);
                makemove.TakeMove(ref brett);

                if (info.stopped)
                {
                    return 0;
                }

                if (Score > BestScore)
                {
                    BestScore = Score;
                    BestMove = list.zugliste[MoveNum].Zug;
                    if (Score > alpha)
                    {
                        if (Score >= beta)
                        {
                            if(Legal == 1)
                            {
                                info.fhf++;
                            }
                            info.fh++;

                            if (!makros.MoveFlagCapture(list.zugliste[MoveNum].Zug))
                            {
                                brett.searchKillers[1, brett.ply] = brett.searchKillers[0, brett.ply];
                                brett.searchKillers[0, brett.ply] = list.zugliste[MoveNum].Zug;
                            }

                            PvTable.StoreHashEntry(BestMove, beta, (int)defs.HASH.HFBETA, depth, ref brett);

                            return beta;
                        }
                        alpha = Score;

                        if (!makros.MoveFlagCapture(list.zugliste[MoveNum].Zug))
                        {
                            brett.searchHistory[brett.figuren[makros.FromSq(BestMove)], makros.ToSq(BestMove)] += depth;
                        }
                    }
                }
            }

            if (Legal == 0)
            {
                if (inSchach)
                {
                    return -defs.MATE + brett.ply;
                }
                else
                {
                    return 0;
                }
            }

            if (alpha != OldAlpha)
            {
                PvTable.StoreHashEntry(BestMove, BestScore, (int)defs.HASH.HFEXACT, depth, ref brett);
            }
            else
            {
                PvTable.StoreHashEntry(BestMove, alpha, (int)defs.HASH.HFALPHA, depth, ref brett);
            }

            return alpha;
        }

        public static int Quiescence(int alpha, int beta, ref boardStruct brett, ref suchInfo info)
        {

            Debug.Assert(board.CheckBoard(ref brett));
            Debug.Assert(beta > alpha);

            if ((info.nodes & 2047) == 0)
            {
                CheckUp(ref info);
            }

            info.nodes++;

            if (IsRepetition(ref brett) || brett.fiftyMove >= 100)
            {
                return 0;
            }

            if (brett.ply > defs.MAXTIEFE - 1)
            {
                return evaluate.EvalPosition(ref brett);
            }

            int Score = evaluate.EvalPosition(ref brett); 

            Debug.Assert(Score > -defs.INFINITE && Score < defs.INFINITE);

            if (Score >= beta)
            {
                return beta;
            }

            if (Score > alpha)
            {
                alpha = Score;
            }

            movelist list = new movelist();
            movegen.GenerateAllCaps(ref list, ref brett);

            int MoveNum = 0;
            int Legal = 0;
            Score = -defs.INFINITE;

            for (MoveNum = 0; MoveNum < list.anzahlZüge; ++MoveNum)
            {

                PickNextMove(MoveNum, ref list);

                if (!makemove.MakeMove(list.zugliste[MoveNum].Zug, ref brett))
                {
                    continue;
                }

                Legal++;
                Score = -Quiescence(-beta, -alpha, ref brett, ref info);
                makemove.TakeMove(ref brett);

                if (info.stopped)
                {
                    return 0;
                }

                if (Score > alpha)
                {
                    if (Score >= beta)
                    {
                        if (Legal == 1)
                        {
                            info.fhf++;
                        }
                        info.fh++;
                        return beta;
                    }
                    alpha = Score;
                }
            }
            return alpha;
        }
    }
}
