/*
 * Class        :   indoor_damagepits.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZScream_Exporter;

public class indoor_damagepits
{
    private const int
        embedded2 = 2,
        sizeOfEachEntry = 2;

    private readonly int
        dataAddress,
        POINTER_LOCATION,
        DATALENGTH_VALUE_LOCATION,
        maxLength;

    private SortedDictionary<ushort, ushort> entries;

    internal indoor_damagepits()
    {

        POINTER_LOCATION = ConstantsReader.GetAddress("pit_pointer");
        DATALENGTH_VALUE_LOCATION = ConstantsReader.GetAddress("pit_count");
        dataAddress = PointerRead.LongRead_LoHiBank(POINTER_LOCATION);
        maxLength = Conversion.toUShort(ROM.DATA[DATALENGTH_VALUE_LOCATION+ 1], ROM.DATA[DATALENGTH_VALUE_LOCATION + 0]) + embedded2;
        readData();
    }

    private void readData()
    {
        entries = new SortedDictionary<ushort, ushort>();
        byte[] b = ROM.Read(dataAddress, maxLength);
        for (int i = 0; i < maxLength; i += sizeOfEachEntry)
        {
            ushort room = Conversion.toUShort(b[i + 1], b[i]);
            if (room <= Dungeon.maxRoomNo)
                entries.Add(room, room);
        }
    }

    public bool doesRoomHaveDamagePits(ushort roomNo) => entries.ContainsKey(roomNo);

    public void makeRoomHaveDamagePits(ushort roomNo, bool yesMakeHavePit)
    {
        if (yesMakeHavePit)
        {
            if (roomNo <= Dungeon.maxRoomNo)
                entries.Add(roomNo, roomNo);
        }
        else
        {
            if (entries.ContainsKey(roomNo))
                entries.Remove(roomNo);
        }
    }

    public void writeAllData()
    {
        if (entries.Count > maxLength / sizeOfEachEntry)
            String.Format("Pit data too big. Please remove at least {0} pit(s).", entries.Count - (maxLength / sizeOfEachEntry));

        byte[] data = new byte[0];

        foreach (ushort i in entries.Keys)
        {
            byte[] b = BitConverter.GetBytes(i);
            data = data.Concat(new byte[] { b[1], b[0] }).ToArray();
        }

        while (entries.Count < maxLength / sizeOfEachEntry)
            data = data.Concat(new byte[] { Dungeon.nullValue, Dungeon.nullValue }).ToArray();

        ROM.Write(dataAddress, maxLength, data);
    }
}
