/*
 * Class        :   indoor_telepathy.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using ZScream_Exporter;

public class indoor_telepathy
{
    private const int
        POINTER_LOCATION = 0x3B500,
        size = 0x280,
        sizeOfEachEntry = 2;
    private readonly int
        dataAddress;

    private ushort[] allData;

    internal indoor_telepathy()
    {
        dataAddress = AddressLoROM.SnesToPc(
            ROM.DATA[POINTER_LOCATION + 0],
            ROM.DATA[POINTER_LOCATION + 1],
            AddressLoROM.PcToSnes_Bank(POINTER_LOCATION));
    }

    private void readAllData()
    {
        allData = new ushort[Dungeon.maxRoomNo];
        byte[] rawData = ROM.Read(dataAddress, size);
        for (ushort room = 0; room < size; room += sizeOfEachEntry)
            allData[room] = Conversion.toUShort(rawData[room + 1], rawData[room + 0]);
    }

    public ushort getMonologueEntryForRoom(ushort room) => allData[room];

    public void setMonologueEntryForRoom(ushort room, ushort entry) => allData[room] = entry;

    internal void writeAllData()
    {
        byte[] rawData = new byte[size];

        int i = 0;
        for (ushort room = 0; room < size; room += sizeOfEachEntry, i += 2)
        {
            byte[] b = BitConverter.GetBytes(room);
            rawData[i + 0] = b[1];
            rawData[i + 1] = b[0];
        }

        ROM.Write(dataAddress, size, rawData);
    }
}