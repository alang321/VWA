using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VWA
{
    static class PvTable
    {
        public static int GetPvLine(int depth, ref boardStruct brett)
        {
            Debug.Assert(depth < defs.MAXTIEFE && depth >= 1);

            int move = ProbePvMove(ref brett);
            int count = 0;
	
	        while(move != defs.NOMOVE && count < depth)
            {
		        Debug.Assert(count < defs.MAXTIEFE);
	
		        if(movegen.MoveExists(ref brett, move) )
                {
                    makemove.MakeMove(move, ref brett);
                    brett.PvArray[count++] = move;
		        }
                else
                {
			        break;
		        }
                move = ProbePvMove(ref brett);	
	        }
	
	        while(brett.ply > 0)
            {
		        makemove.TakeMove(ref brett);
	        }
	
	        return count;
        }

        //im tutorial mit mb size gemacht aber da das nicht so gut geht in c# setzt ich einfach die maximale anzahl an einträgen
        public static void InitHashTable(ref hashTableStruct table, int size)
        {
            table.numEntries = (size * 50000)-2;
            table.hashTable = new hashEntry[table.numEntries];
        }

        public static void ClearPvTable(ref hashTableStruct table)
        {
            Array.Clear(table.hashTable, 0, table.hashTable.Length);

            table.newWrite = 0;
        }


        public static void StoreHashEntry(int move, int score, int flags, int depth, ref boardStruct brett) 
        {
            int index = Convert.ToInt32(Convert.ToUInt64(brett.posKey) % Convert.ToUInt64(brett.PvTable.numEntries));

            if (brett.PvTable.hashTable[index].posKey == 0) {
		        brett.PvTable.newWrite++;
	        } else {
                brett.PvTable.overWrite++;
            }

            if(score > defs.ISMATE) score += brett.ply;
            else if(score < -defs.ISMATE) score -= brett.ply;

            brett.PvTable.hashTable[index] = new hashEntry(brett.posKey, move, score, depth, flags);

        }

        public static int ProbePvMove(ref boardStruct brett)
        {
            int index = Convert.ToInt32(Convert.ToUInt64(brett.posKey) % Convert.ToUInt64(brett.PvTable.numEntries));
            try
            {
                if (brett.PvTable.hashTable[index].posKey == 0) ;
            }
            catch
            {
                brett.PvTable.hashTable[index] = new hashEntry(0, 0, 0, 0, 0);
            }

            if (brett.PvTable.hashTable[index].posKey == brett.posKey )
            {
		        return brett.PvTable.hashTable[index].move;
	        }
	
	        return defs.NOMOVE;
        }

        public static bool ProbeHashEntry(ref int move, ref int score, int alpha, int beta, int depth, ref boardStruct brett)
        {
            int index = Convert.ToInt32(Convert.ToUInt64(brett.posKey) % Convert.ToUInt64(brett.PvTable.numEntries));
            try
            {
                if (brett.PvTable.hashTable[index].posKey == 0);
            }
            catch
            {
                brett.PvTable.hashTable[index] = new hashEntry(0, 0, 0, 0, 0);
            }

            if (brett.PvTable.hashTable[index].posKey == brett.posKey)
            {
                move = brett.PvTable.hashTable[index].move;
                if (brett.PvTable.hashTable[index].depth >= depth)
                {
                    brett.PvTable.hit++;

                    score = brett.PvTable.hashTable[index].score;
                    if (score > defs.ISMATE) score -= brett.ply;
                    else if (score < -defs.ISMATE) score += brett.ply;

                    switch (brett.PvTable.hashTable[index].flags)
                    {
                        case (int)defs.HASH.HFALPHA:
                            if (score <= alpha)
                            {
                                score = alpha;
                                return true;
                            }
                            break;
                        case (int)defs.HASH.HFBETA:
                            if (score >= beta)
                            {
                                score = beta;
                                return true;
                            }
                            break;
                        case (int)defs.HASH.HFEXACT:
                            return true;
                        default: throw new Exception("HASHENTRY FLAG IS WRONG IN PROBEHASHENTRY"); break;
                    }
                }
            }
            return false;
        }
    }

    class hashEntry
    {
        public ulong posKey = 0;
        public int move = 0;
        public int score = 0;
        public int depth = 0;
        public int flags = 0;

        public hashEntry(ulong posKey, int move, int score, int depth, int flags)
        {
            this.posKey = posKey;
            this.move = move;
            this.score = score;
            this.depth = depth;
            this.flags = flags;
        }
    }

    class hashTableStruct
    {
        public hashEntry[] hashTable;
        public int numEntries;
        public int newWrite;
        public int overWrite;
        public int hit;
        public int cut;
    }
}
