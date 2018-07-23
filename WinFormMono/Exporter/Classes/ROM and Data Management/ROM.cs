/*
 * Author:  Zarby89, Trovsky
 */

using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Stores the ROM byte array and has a few methods.
/// </summary>
/// 
namespace ZScream_Exporter
{
    public static class ROM
    {
        public static byte[] DATA;

        public static void SetRom(byte[] temp, out bool isHeadered)
        {
            isHeadered = false;
            DATA = new byte[temp.Length];
            const int HeaderSize = 0x200;

            //is ROM Headered?
            if ((temp.Length & HeaderSize) == HeaderSize)
            {
                //Rom is headered, remove header
                DATA = new byte[temp.Length - HeaderSize];
                for (int i = HeaderSize; i < temp.Length; i++)
                    DATA[i - HeaderSize] = temp[i];
                isHeadered = true;
            }
            else DATA = (byte[])temp.Clone();
        }
        public static bool CompareBytes(int location, byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (DATA[location + i] != array[i])
                    return false;
            return true;
        }

        public static byte[] Read(int address, int size)
        {
            byte[] b = new byte[size];
            Array.Copy(DATA, address, b, 0, size);
            return b;
        }

        public static byte Read(int address) => DATA[address];

        public static void Write(int address, int size, byte[] data)
        { Array.Copy(data, 0, DATA, address, size); }

        public static void Write(int address, byte data)
        { DATA[address] = data; }

        public static void Swap(int Location1, int Location2, int size)
        {
            byte[]
                b_location1 = Read(Location1, size),
                b_location2 = Read(Location1, size);

            Array.Copy(b_location1, 0, DATA, Location2, size);
            Array.Copy(b_location2, 0, DATA, Location1, size);
        }

        public static bool IsEmpty(int address, int Length)
        {
            if (Length < 1)
                throw new ArgumentOutOfRangeException();

            byte[] b = Read(address, Length);
            return b.All(singleByte => singleByte == b[0]);
        }

        /// <summary>
        /// Read ROM with delimeter
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        public static List<byte> ReadWithDelim(int pos, int delim)
        {
            List<byte> list = new List<byte>();
            int i = 0;
            while (DATA[pos + i] != delim)
                list.Add(DATA[pos + i++]);
            return list;
        }
    }
}