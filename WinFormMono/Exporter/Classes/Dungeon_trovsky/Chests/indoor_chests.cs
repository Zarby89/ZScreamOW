/*
 * Class        :   indoor_chests.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZScream_Exporter;

public class indoor_chests : frameWork
{
    private readonly ushort totalMax;

    private const int
        addressOfStorageMax01 = 0x88C1,
        storageMax2 = 0x20,
        sizeOfOneChest = 3;

    private readonly int addressOfTotalMax;

    private SortedList<ushort, List<i_chest>> allChests;

    private int sizeOfOneEntry
    { get { return totalMax / numberOfPointers; } }

    public indoor_chests() : base(3)
    {
        primaryPointer_location = new int[]
        {
            ConstantsReader.GetAddress("chests_data_pointer1"),
            ConstantsReader.GetAddress("chests_data_pointer2"),
            ConstantsReader.GetAddress("chests_data_pointer3"),
        };

        addressOfTotalMax = ConstantsReader.GetAddress("chests_length_pointer");

        byte[] b = ROM.Read(addressOfTotalMax, 2);
        totalMax = Conversion.toUShort(b[1], b[0]);

        if (totalMax % numberOfPointers != 0)
            throw new Exception();

        refreshPointer3Bytes();
        readAllChests();
    }

    private void readAllChests()
    {
        allChests = new SortedList<ushort, List<i_chest>>();
        byte[] allData = new byte[0];
        /*for (int i = 0; i < primaryPointer_address.Length; i++)
        {
            byte[] data = RomIO.read(primaryPointer_address[i], sizeOfOneEntry);
            allData = allData.Concat(data).ToArray();
        }*/
        byte[] data = ROM.Read(primaryPointer_address[0], sizeOfOneEntry);
        allData = allData.Concat(data).ToArray();

        for (int i = 0; i < sizeOfOneEntry; i += sizeOfOneChest)
        {
            ushort room = i_chest.getRoom(allData[i], allData[i + 1]);

            if (room <= Dungeon.maxRoomNo)
            {
                if (!allChests.ContainsKey(room))
                    allChests.Add(room, new List<i_chest>());
                allChests[room].Add(new i_chest(allData[i], allData[i + 1], allData[i + 2]));
            }
        }
    }

    private void writeAllChests()
    {
        byte[][] blockData_set = new byte[numberOfPointers][];

        int currentEntry = 0;
        foreach (ushort room in allChests.Keys)
        {
            foreach (i_chest chest in allChests[room])
            {
                byte[] data = chest.getBytes();

                if (blockData_set[currentEntry] == null)
                {
                    /* Store the block entry. No need to check storage limits here
                     * because it's very unlikely that an entire space dedicated
                     * to block storage is less than a single block.
                     */
                    blockData_set[currentEntry] = data;
                }
                else if (blockData_set[currentEntry].Length < sizeOfOneEntry + data.Length)
                {
                    //Store the block entry if space is available.
                    blockData_set[currentEntry] = blockData_set[currentEntry].Concat(data).ToArray();
                }
                else
                {
                    //otherwise move to the next location, if it exists
                    currentEntry++;
                }
            }
            for (int i = 0; i < numberOfPointers; i++)
            {
                ROM.Write(primaryPointer_address[i], blockData_set[i].Length, blockData_set[i]);

                //clear up unused space
                for (int j = blockData_set[i].Length + 1; j < sizeOfOneEntry; j++)
                    ROM.Write(primaryPointer_address[i] + j, Dungeon.nullValue);
            }
        }
    }
}