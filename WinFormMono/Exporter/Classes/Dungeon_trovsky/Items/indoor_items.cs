/*
 * Class        :   indoor_items.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using ZScream_Exporter;

public class indoor_items
{
    private const int
        primaryPointer_location = 0xE6C2,
        primaryPointer_length = 8,
        sizeOfPointerTable = 0x282,
        numberOfRooms = 320,
        bytesPerRoomEntry = 2,
        maxSize = 0x8C7,
        delimter = 0xFF,
        bytesPerItem = 3;

    private int primaryPointer_val;

    private int[] secondayPointers;

    public indoor_items()
    {
        readPrimaryPointers();
        readSecondaryPointers();
    }

    private void readPrimaryPointers()
    { primaryPointer_val = PointerRead.LongRead_LoHiBank(primaryPointer_location); }

    public void movePointers(int newAddress)
    {
        if (ROM.IsEmpty(newAddress, sizeOfPointerTable))
        {
            PointerRead.CheckAddressWithinRangeOf3Byte(newAddress);
            ROM.Swap(primaryPointer_val, newAddress, sizeOfPointerTable);
            byte[] b = PointerRead.GeneratePointer3(newAddress);
            ROM.Write(primaryPointer_location, b.Length, b);
            ROM.Write(primaryPointer_location + primaryPointer_length - 1, b[b.Length - 1]);
        }
    }

    private void readSecondaryPointers()
    {
        secondayPointers = new int[numberOfRooms];

        for (int i = 0; i < numberOfRooms; i++)
            secondayPointers[i] = PointerRead.ShortRead_LoHi(primaryPointer_val + (i * bytesPerRoomEntry));
    }


    public SortedList<ushort, List<i_item>> readAllItems()
    {
        SortedList<ushort, List<i_item>> sprites = new SortedList<ushort, List<i_item>>();
        for (ushort i = 0; i < numberOfRooms; i++)
            sprites.Add(i, new List<i_item>());

        int smallestPointer = int.MaxValue;

        foreach (int i in secondayPointers)
            smallestPointer = Math.Min(i, smallestPointer);

        byte[] data = ROM.Read(smallestPointer, maxSize * 2);

        ushort roomNo = 0;
        foreach (int address in secondayPointers)
        {
            List<byte> b = new List<byte>();
            int i = address - smallestPointer;

            while (true)
            {
                if (data[i] == delimter)
                {
                    if (data[i + 1] == delimter)
                    {
                        if (b.Count % 3 == 0)
                        {
                            for (int u = 0; u < b.Count; u += bytesPerItem)
                                sprites[roomNo].Add(new i_item(b[u], b[u + 1], b[u + 2]));
                            break;
                        }
                        else
                        {
                            if (b.Count != 1)
                                throw new Exception();
                            else break;
                        }
                    }
                }
                b.Add(data[i++]);
            }
            roomNo++;
        }

        return sprites;
    }

    public void writeAllItems()
    {
        throw new NotImplementedException();
    }
}
