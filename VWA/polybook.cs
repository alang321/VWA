using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VWA
{
    class PolyBookEntry
    {
        public ulong key;
        public UInt16 move;
        public UInt16 weight;
        public UInt32 learn;
    }

    static class polybook
    {
        static long NumEntries = 0;

        static PolyBookEntry[] entries;

        static int[] PolyKindOfPiece = {-1, 1, 3, 5, 7, 9, 11, 0, 2, 4, 6, 8, 10};

        public static void InitPolyBook()
        {

            options.UseBook = false;

            FileStream fs = File.Open("performance.bin", FileMode.Open);

            if (fs == null)
            {
                Console.WriteLine("Book File Not Read\n");
            }
            else
            {
                if (fs.Length < 16)
                {
                    Console.WriteLine("No Entries Found\n");
                    return;
                }

                NumEntries = fs.Length / 16;
                Console.WriteLine(String.Format("{0} Entries Found In File\n", NumEntries));

                entries = new PolyBookEntry[NumEntries];

                BinaryReader binReader = new BinaryReader(fs);

                // Set Position to the beginning of the stream.
                binReader.BaseStream.Position = 0;
                int i = 0;
                for(i = 0; i < entries.Length; i++)
                {
                    entries[i] = new PolyBookEntry();
                    entries[i].key = BitConverter.ToUInt64(binReader.ReadBytes(8),0);
                    entries[i].move = BitConverter.ToUInt16(binReader.ReadBytes(2), 0);
                    entries[i].weight = BitConverter.ToUInt16(binReader.ReadBytes(2), 0);
                    entries[i].learn= BitConverter.ToUInt16(binReader.ReadBytes(4), 0);
                }

                Console.WriteLine(String.Format("fread() {0} Entries Read in from file\n", i));

                if (NumEntries > 0)
                {
                    options.UseBook = true;
                }
            }
        }

        static int ConvertPolyMoveToInternalMove(ushort polyMove, ref boardStruct brett)
        {
            int ff = (polyMove >> 6) & 7;
            int fr = (polyMove >> 9) & 7;
            int tf = (polyMove >> 0) & 7;
            int tr = (polyMove >> 3) & 7;
            int pp = (polyMove >> 12) & 7;

            string ret;

            if (pp == 0)
            {
                ret = data.FileChar[ff].ToString() + data.RankChar[fr].ToString() + data.FileChar[tf].ToString() + data.RankChar[tr].ToString();
            }
            else
            {
                char promChar = 'q';
                switch (pp)
                {
                    case 1: promChar = 'n'; break;
                    case 2: promChar = 'b'; break;
                    case 3: promChar = 'r'; break;
                }
                ret = data.FileChar[ff].ToString() + data.RankChar[fr].ToString() + data.FileChar[tf].ToString() + data.RankChar[tr].ToString() + promChar.ToString();
            }
            return io.ParseMove(ret, ref brett);
        }

        public static int GetBookMove(ref boardStruct brett)
        {
            ushort move;
            int MAXBOOKMOVES = 32;
            int[] bookMoves = new int[MAXBOOKMOVES];
            int tempMove = defs.NOMOVE;
            int count = 0;
            ulong polyKey = PolyKeyFromBoard(ref brett);

            for (int i = 0; i < entries.Length; i++)
            {
                if (polyKey == endian_swap_u64(entries[i].key))
                {
                    move = endian_swap_u16(entries[i].move);
                    tempMove = ConvertPolyMoveToInternalMove(move, ref brett);
                    if (tempMove != defs.NOMOVE)
                    {
                        bookMoves[count++] = tempMove;
                        if (count > MAXBOOKMOVES) break;
                    }
                }
            }

            if(count != 0)
            {
                Random rnd = new Random();
                int i = rnd.Next(0, count);
                return bookMoves[i];
            }
            else
            {
                return defs.NOMOVE;
            }

        }

        static UInt16 endian_swap_u16(UInt16 x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        static UInt32 endian_swap_u32(UInt32 x)
        {
            x = (x >> 24) |
                ((x << 8) & 0x00FF0000) |
                ((x >> 8) & 0x0000FF00) |
                (x << 24);
            return x;
        }

        static ulong endian_swap_u64(ulong x)
        {
            x = (x >> 56) |
                ((x << 40) & 0x00FF000000000000) |
                ((x << 24) & 0x0000FF0000000000) |
                ((x << 8) & 0x000000FF00000000) |
                ((x >> 8) & 0x00000000FF000000) |
                ((x >> 24) & 0x0000000000FF0000) |
                ((x >> 40) & 0x000000000000FF00) |
                (x << 56);
            return x;
        }


        public static ulong PolyKeyFromBoard(ref boardStruct brett)
        {
            int sq = 0, rank = 0, file = 0;
            ulong finalKey = 0;
            int piece = (int)defs.Figuren.LEER;
            int polyPiece = 0;
            int offset = 0;

            for (sq = 0; sq < defs.BRD_SQ_NUM; ++sq)
            {
                piece = brett.figuren[sq];
                if (piece != (int)defs.Felder.NO_SQ && piece != (int)defs.Figuren.LEER && piece != (int)defs.Felder.OFFBOARD)
                {
                    polyPiece = PolyKindOfPiece[piece];
                    rank = defs.RanksBrd[sq];
                    file = defs.FilesBrd[sq];
                    finalKey ^= polykeys.Random64Poly[(64 * polyPiece) + (8 * rank) + file];
                }
            }

            // castling
            offset = 768;

            int check = brett.rochadePerm & (int)defs.Rochade.WKRC;
            if (check != 0) finalKey ^= polykeys.Random64Poly[offset + 0];

            check = brett.rochadePerm & (int)defs.Rochade.WDRC;
            if (check != 0) finalKey ^= polykeys.Random64Poly[offset + 1];

            check = brett.rochadePerm & (int)defs.Rochade.SKRC;
            if (check != 0) finalKey ^= polykeys.Random64Poly[offset + 2];

            check = brett.rochadePerm & (int)defs.Rochade.SDRC;
            if (check != 0) finalKey ^= polykeys.Random64Poly[offset + 3];

            // enpassant
            offset = 772;
            if (HasPawnForCapture(ref brett))
            {
                file = defs.FilesBrd[brett.enPas];
                finalKey ^= polykeys.Random64Poly[offset + file];
            }

            if (brett.seite == (int)defs.Farben.WEISS)
            {
                finalKey ^= polykeys.Random64Poly[780];
            }
            return finalKey;
        }

        public static bool HasPawnForCapture(ref boardStruct brett) {

            int sqWithPawn = 0;
            int targetPce = (brett.seite == (int)defs.Farben.WEISS) ? (int)defs.Figuren.wB : (int)defs.Figuren.sB;
	        if(brett.enPas != (int)defs.Felder.NO_SQ)
            {
		        if(brett.seite == (int)defs.Farben.WEISS)
                {
			        sqWithPawn = brett.enPas - 10;
		        }
                else
                {
			    sqWithPawn = brett.enPas + 10;
		        }
		        if(brett.figuren[sqWithPawn + 1] == targetPce)
                {
			        return true;
		        }
                else if(brett.figuren[sqWithPawn - 1] == targetPce)
                {
			        return true;
		        } 
	        }
	        return false;
        }
    }
}
