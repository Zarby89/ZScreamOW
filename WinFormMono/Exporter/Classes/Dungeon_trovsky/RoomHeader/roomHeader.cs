/*
 * Class        :   roomHeader.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using ZScream_Exporter;

public class roomHeader
{
    private const int
        numberOfHeaders = 0x140,
        bankOffset1 = 2,
        bankOffset2 = 10,
        sizeOfEachHeader = 14;

    public const int
        Max_BG2 = 8,
        Max_Collision = 4,
        Max_Tag = i_roomHeader.maxAndMask2,
        Max_Effect = i_roomHeader.maxAndMask1,
        Max_Planes = i_roomHeader.maxAndMask3,
        NumberOfExits = i_roomHeader.numberOfPlanesAndExits;

    private int secondaryPointer;
    private readonly int primaryPointer, pointer_bank;

    private int[] roomHeaderPointers;

    private i_roomHeader[] roomHeaders;

    public roomHeader()
    {
        primaryPointer = ConstantsReader.GetAddress("room_header_pointer");
        pointer_bank = ConstantsReader.GetAddress("room_header_pointers_bank");
        roomHeaders = new i_roomHeader[numberOfHeaders];
        readPointer();
    }

    private void readPointer()
    {
        if (ROM.Read(primaryPointer + bankOffset1) != ROM.Read(primaryPointer + bankOffset2))
            throw new Exception();

        secondaryPointer = PointerRead.LongRead_LoHiBank(primaryPointer);
        secondaryPointer = PointerRead.LongRead_LoHiBank(primaryPointer, primaryPointer + 1, pointer_bank);

        roomHeaderPointers = new int[numberOfHeaders];
        for (int i = 0; i < numberOfHeaders; i++)
        {
            int temp = secondaryPointer + i * 2;
            roomHeaderPointers[i] = AddressLoROM.SnesToPc(
                    ROM.Read(temp + 0),
                    ROM.Read(temp + 1),
                    AddressLoROM.PcToSnes_Bank(secondaryPointer));

            roomHeaders[i] = new i_roomHeader(ROM.Read(roomHeaderPointers[i], sizeOfEachHeader));
        }
    }

    public i_roomHeader getHeader(ushort room)
    {
        if (room <= Dungeon.maxRoomNo)
            return roomHeaders[room];
        else throw new ArgumentOutOfRangeException();
    }

    public void setHeader(ushort room, i_roomHeader rm)
    {
        if (room <= Dungeon.maxRoomNo)
            roomHeaders[room] = rm;
        else throw new ArgumentOutOfRangeException();
    }
}
