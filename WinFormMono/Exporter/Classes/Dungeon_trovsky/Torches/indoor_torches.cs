/*
 * Class        :   indoor_torches.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZScream_Exporter;

public class indoor_torches : frameWork
{
    private const int
        addressOfStorageMax01 = 0x88C1,
        storageMax2 = 0x20,
        sizeOfSingleTorchEntry = 2,
        sizeOfnewEntry = 2;
    private readonly int[]
        maxStorage;

    private readonly int storageMax01;
    private const byte delim = 0xFF;
    private int numberOfBytesUsed;
    private readonly ushort storageTotalMax;

    public indoor_torches() : base(3)
    {
        switch (RegionId.myRegion)
        {
            case (int)RegionId.Region.USA:
                primaryPointer_location = new int[] { 0x15B16, 0x15B1D, 0x15B24 };
                break;
            case (int)RegionId.Region.Japan:
                primaryPointer_location = new int[] { 0x1587A, 0x15881, 0x15888 };
                break;
            case (int)RegionId.Region.German:
                primaryPointer_location = new int[] { 0x15B98, 0x15B9F, 0x15BA6 };
                break;
            default:
                throw new NotImplementedException();
        }
        
        storageTotalMax = Conversion.toUShort(ROM.DATA[addressOfStorageMax01 + 1], ROM.DATA[addressOfStorageMax01 + 0]);
        storageMax01 = 0x80;
        maxStorage = new int[] { storageMax01, storageMax01, storageMax2 };
        refreshPointer3Bytes();
    }

    public SortedList<ushort, List<i_torch>> readAllTorches()
    {
        numberOfBytesUsed = 0;
        LinkedList<byte> allData = new LinkedList<byte>();

        for (int i = 0; i < primaryPointer_address.Length; i++)
        {
            byte[] data = new byte[maxStorage[i]];
            Array.Copy(ROM.DATA, primaryPointer_address[i], data, 0, maxStorage[i]);
            foreach (byte b in data)
                allData.AddLast(b);
        }

        SortedList<ushort, List<i_torch>> dungeonTorches = new SortedList<ushort, List<i_torch>>();
        bool readRoom = true;

        for (ushort i = 0; i <= Dungeon.maxRoomNo; i++)
            dungeonTorches.Add(i, new List<i_torch>());


        while (allData.Count != 0)
        {
            List<i_torch> temp = new List<i_torch>();

            ushort room = Dungeon.nullRoomVal;

            if (readRoom)
            {
                byte b1 = allData.First.Value;
                allData.RemoveFirst();
                byte b2 = allData.First.Value;
                allData.RemoveFirst();

                room = Conversion.toUShort(b2, b1);

                if (room <= Dungeon.maxRoomNo)
                {
                    if (!dungeonTorches.ContainsKey(room))
                    {
                        dungeonTorches.Add(room, new List<i_torch>());
                        numberOfBytesUsed += sizeOfnewEntry;
                    }
                }
                else
                {
                    /* Hyrule Magic removes torches by sending the torches
                     * to room $FFFF, so I just have to deal with the nulled
                     * torched.
                     */
                    if (room != Dungeon.nullRoomVal)
                        throw new Exception();
                }
                readRoom = false;
            }

            if (!readRoom)
            {
                while (allData.Count != 0)
                {
                    byte b1 = allData.First.Value;
                    allData.RemoveFirst();
                    byte b2 = allData.First.Value;
                    allData.RemoveFirst();

                    bool end = false;
                    if (allData.Count != 0)
                    {
                        if (b1 == delim)
                            if (b2 == delim)
                                end = true;
                    }
                    else end = true;

                    if (!end)
                    {
                        /* Don't insert the torches that belong in the "null room",
                         * but *do* continue the algorithm as normal
                         */
                        if (room != Dungeon.nullRoomVal)
                        {
                            numberOfBytesUsed += sizeOfSingleTorchEntry;

                            dungeonTorches[room].Add(new i_torch(b1, b2));
                            byte byte1, byte2;
                            new i_torch(b1, b2).getBytes(out byte1, out byte2);
                        }
                    }
                    else break;
                }
                room = Dungeon.nullRoomVal;
                readRoom = true;
            }
        }
        return dungeonTorches;
    }

    public void writeAllTorches(SortedList<ushort, List<i_torch>> dungeonTorches)
    {
        List<byte[]> aa = new List<byte[]>();
        foreach (ushort key in dungeonTorches.Keys)
        {
            byte[] q = BitConverter.GetBytes(key);

            foreach (i_torch t in dungeonTorches[key])
            {
                t.getBytes(out byte b1, out byte b2);
                q = q.Concat(new byte[] { b1, b2 }).ToArray();
            }
            aa.Add(q.Concat(new byte[] { delim, delim }).ToArray());
        }
        //maxStorage
        byte[][] dataToWrite = new byte[][] { new byte[0], new byte[0], new byte[0] };

        List<byte[]> sorted = aa.OrderByDescending(b => b.Length).ToList();

        for (int i = 0; i < dataToWrite.Length; i++)
        {
            while (aa.Count != 0)
            {
                bool added = false;
                int ii = 0;
                foreach (byte[] b in sorted)
                {
                    if (aa.Count != 0 && ii < aa.Count)
                    {
                        if (b.Length + dataToWrite[i].Length <= maxStorage[i])
                        {
                            dataToWrite[i] = dataToWrite[i].Concat(b).ToArray();
                            sorted.RemoveAt(ii);
                            added = true;
                            break;
                        }
                        else ii++;
                    }
                    else break;
                }
                if (!added) //if you haven't added an entry, that means you can't insert any more data
                    break;
            }

            while (dataToWrite[i].Length < maxStorage[i])
                dataToWrite[i] = dataToWrite[i].Concat(new byte[] { 0xFF }).ToArray();
        }

        if (sorted.Count != 0)
            throw new Exception();

        for (int j = 0; j < dataToWrite.Length; j++)
            Array.Copy(dataToWrite[j], primaryPointer_address[j], ROM.DATA, 0, maxStorage[j]);
    }
}
