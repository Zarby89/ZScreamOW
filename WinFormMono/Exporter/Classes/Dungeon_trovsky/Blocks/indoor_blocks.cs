/*
 * Class        :   indoor_blocks.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZScream_Exporter;

public class indoor_blocks : frameWork
{
    private const int
        maxBytesPerBlockFor3 = 0x0C,
        bytesPerBlock = 4,
        addressOfTotalMax = 0x8896; //why is this here?
    private readonly int
        maxBlockEntries,
        sizeOfBlockData,
        maxBytesPerBlockFor012;

    private readonly int[] dataSize;

    public indoor_blocks() : base(4)
    {
        primaryPointer_location = new int[]
        {
            ConstantsReader.GetAddress("blocks_pointer1"),
            ConstantsReader.GetAddress("blocks_pointer2"),
            ConstantsReader.GetAddress("blocks_pointer3"),
            ConstantsReader.GetAddress("blocks_pointer4")
        };

        //maxBytesPerBlockFor012 = ReadMixedNumbers.readTwoByte(addressOfStorageMaxFor012);
        maxBytesPerBlockFor012 = 0x80;

        dataSize = new int[]
        {
            maxBytesPerBlockFor012,
            maxBytesPerBlockFor012,
            maxBytesPerBlockFor012,
            maxBytesPerBlockFor3
        };

        for (int i = 0; i < numberOfPointers; i++)
            sizeOfBlockData += dataSize[i];
        maxBlockEntries = sizeOfBlockData / bytesPerBlock;
        refreshPointer3Bytes();
    }

    private int getNumberOfPointers() => primaryPointer_location.Length;

    public void movePointers(uint pointerNo, int newAddress)
    {
        if (!(pointerNo <= getNumberOfPointers()))
            throw new ArgumentOutOfRangeException();

        int size = dataSize[pointerNo];

        if (ROM.IsEmpty(newAddress, size))
        {
            PointerRead.CheckAddressWithinRangeOf3Byte(newAddress);
            ROM.Swap(primaryPointer_address[pointerNo], newAddress, size);
            byte[] newPointer = PointerRead.GeneratePointer3(newAddress);
            ROM.Write(primaryPointer_location[pointerNo], newPointer.Length, newPointer);
            refreshPointer3Bytes();
        }
        else throw new Exception(TextAndTranslationManager.GetString(Dungeon.moveError));
    }

    /// <summary>
    /// Read the rom data to generate a Linked List of block objects
    /// </summary>
    public SortedList<ushort, List<i_block>> readBlocks()
    {
        byte[] allData = null;

        //Merge all data into one array because I want to keep things simple and readable.
        for (int i = 0; i < numberOfPointers; i++)
        {
            byte[] b = ROM.Read(primaryPointer_address[i], dataSize[i]);
            if (allData == null)
                allData = b;
            else allData = allData.Concat(b).ToArray();
        }

        SortedList<ushort, List<i_block>> allDungeonBlocks = new SortedList<ushort, List<i_block>>();

        for (ushort i = 0; i <= Dungeon.maxRoomNo; i++)
            allDungeonBlocks.Add(i, new List<i_block>());

        //now read every 4 byte pair and create 
        for (int i = 0; i < allData.Length; i += bytesPerBlock)
        {
            /*
             * Skip adding the block if the room number is the value of 0xFFFF
             * The game doesn't have unused/empty data, but for the purposes of this
             * editor, we'll use 0xFFFF.
             */
            if (!i_block.isNulledOut(allData[i + 0], allData[i + 1]))
            {
                i_block blocky = new i_block(allData[i + 0], allData[i + 1], allData[i + 2], allData[i + 3]);

                if (!allDungeonBlocks.ContainsKey(blocky.room))
                    allDungeonBlocks.Add(blocky.room, new List<i_block>());
                allDungeonBlocks[blocky.room].Add(blocky);
            }
        }
        return allDungeonBlocks;
    }

    public void writeAllBlocks(SortedList<ushort, List<i_block>> allDungeonBlocks)
    {
        List<i_block> allBlocks = new List<i_block>();

        foreach (ushort i in allDungeonBlocks.Keys)
            foreach (i_block blocky in allDungeonBlocks[i])
                allBlocks.Add(blocky);

        const string error = "dungeon_error_block_overload";

        if (allBlocks.Count > maxBlockEntries)
            throw new Exception(TextAndTranslationManager.GetString(error));

        byte[][] blockData_set = new byte[numberOfPointers][];

        int currentEntry = 0;

        /* Convert block data to byte arrays and attempt to insert into array blockData_set.
         * 
         * blockData_set is of length pointerNumber (the number of pointers for the blocks.
         * Each index represents the locations to insert the location. Therefore, all data
         * at index i is stored at the address primaryPointer_address[i].
         */

        foreach (i_block blocky in allBlocks)
        {
            /* You get this error if you run out of space, or in other words,
             * if you are inserting too many blocks.
             */
            if (currentEntry > numberOfPointers)
                throw new Exception(TextAndTranslationManager.GetString(error));

            byte[] blockData;
            blocky.getBytes(out blockData);

            if (blockData_set[currentEntry] == null)
            {
                /* Store the block entry. No need to check storage limits here
                 * because it's very unlikely that an entire space dedicated
                 * to block storage is less than a single block.
                 */
                blockData_set[currentEntry] = blockData;
            }
            else if (blockData_set[currentEntry].Length < dataSize[currentEntry] + blockData.Length)
            {
                //Store the block entry if space is available.
                blockData_set[currentEntry] = blockData_set[currentEntry].Concat(blockData).ToArray();
            }
            else
            {
                //otherwise move to the next location, if it exists
                currentEntry++;

                if (currentEntry > numberOfPointers)
                    throw new Exception(TextAndTranslationManager.GetString(error));
                else blockData_set[currentEntry] = blockData;
            }
        }

        for (int i = 0; i < numberOfPointers; i++)
        {
            if (blockData_set[i] != null)
                ROM.Write(primaryPointer_address[i], blockData_set[i].Length, blockData_set[i]);

            //clear up unused space

            int size = blockData_set[i] == null ? 0 : blockData_set[i].Length;
            for (int j = size; j < dataSize[i]; j++)
                ROM.Write(primaryPointer_address[i] + j, Dungeon.nullValue);
        }
    }
}
