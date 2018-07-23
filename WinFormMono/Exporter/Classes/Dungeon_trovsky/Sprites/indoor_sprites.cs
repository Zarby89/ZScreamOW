/*
 * Class        :   indoor_sprites.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZScream_Exporter;

public class indoor_sprites
{
    private int secondaryPointer;
    private const int
        size = 0x300,
        bytesPerRoomEntry = 2,
        codePointerLength = 4,
        specialPatchByte = 0x20,
        connPointerByteNo = 14,
        sprite_delim = 0xFF,
        sizeOfData = 0x1371;

    private readonly int
        codePointer,
        primaryPointer = 0x4C298;

    private bool isConnPatched;
    private readonly string patchError = TextAndTranslationManager.GetString("dungeon_error_sprite_patch");

    private int[] roomPointers;

    private readonly byte[]
        patchByConn = new byte[]
        {
            0xE2, 0x30, 0xA9, 0x29, 0x48, 0xAB,
            0xC2, 0x30, 0xAD, 0x8E, 0x04, 0x0A,
            0xA8, 0xB9, 0xFF, 0xFF, 0x6B
        },
        originalCode = new byte[]
        {
            0xA8,
            0xB9
        };


    /// <summary>
    /// Initilializes a new instance of indoor_sprites
    /// </summary>
    internal indoor_sprites()
    {
        primaryPointer = ConstantsReader.GetAddress("rooms_sprite_pointer");

        switch (RegionId.myRegion)
        {
            case (int)RegionId.Region.Japan:
            case (int)RegionId.Region.USA:
                codePointer = 0x4C296;
                break;
            case (int)RegionId.Region.German:
                codePointer = 0x4C2BE;
                break;
            default:
                throw new NotImplementedException();
        }

        updatePointers();
    }

    public int getPointer()
    { return secondaryPointer; }

    private void updatePointers()
    {
        bool match = true;
        byte[] b = new byte[originalCode.Length];
        Array.Copy(ROM.DATA, codePointer, b, 0, originalCode.Length);

        for (int i = 0; i < originalCode.Length; i++)
        {
            if (originalCode[i] != b[i])
            {
                match = false;
                break;
            }
        }

        if (match)
            secondaryPointer = PointerRead.ShortRead_LoHi(primaryPointer);
        else if (b[0] == specialPatchByte)
        {
            int Codeaddress = PointerRead.LongRead_LoHiBank(codePointer + 1);
            byte[] patch = new byte[patchByConn.Length];
            Array.Copy(ROM.DATA, Codeaddress, patch, 0, patchByConn.Length);
            for (int i = 0; i < patchByConn.Length; i++)
                if (patchByConn[i] != patch[i] && i != connPointerByteNo && i != connPointerByteNo + 1)
                    throw new Exception(patchError);
            secondaryPointer = PointerRead.ShortRead_LoHi(codePointer + connPointerByteNo);
        }
        else throw new Exception(patchError);

        roomPointers = new int[size / bytesPerRoomEntry];
        for (int i = 0; i < size / bytesPerRoomEntry; i++)
            roomPointers[i] = PointerRead.ShortRead_LoHi(secondaryPointer + (i * bytesPerRoomEntry));
        isConnPatched = !match;
    }

    /* Transfer the Sprites to another bank. Patch by Conn. */
    private void applyMovingPatch(bool isApply)
    {

    }

    public void movePointers(int newAddress)
    {
        if (!isConnPatched)
            if (ROM.IsEmpty(newAddress, size))
                if (AddressLoROM.PcToSnes_Hi(newAddress) == AddressLoROM.PcToSnes_Bank(primaryPointer))
                {
                    ROM.Swap(secondaryPointer, newAddress, size);
                    byte[] b = new byte[]
                    {
                        (byte)(AddressLoROM.PcToSnes_Lo(newAddress)),
                        (byte)(AddressLoROM.PcToSnes_Hi(newAddress + 1)),
                    };
                    ROM.Write(primaryPointer, b.Length, b);
                    updatePointers();
                }
                else throw new Exception("Address must be within the bank.");
            else throw new Exception(TextAndTranslationManager.GetString(Dungeon.moveError));
        else throw new NotImplementedException();
    }

    public SortedList<ushort, List<i_sprite>> readAllSprites()
    {
        SortedList<ushort, List<i_sprite>> sprites = new SortedList<ushort, List<i_sprite>>();
        for (ushort i = 0; i < roomPointers.Length; i++)
        {
            sprites.Add(i, new List<i_sprite>());
            List<byte> b = ROM.ReadWithDelim(roomPointers[i], sprite_delim);

            byte spriteSort = b[0];
            b.RemoveAt(0);

            while (b.Count >= i_sprite.bytesPerEntry)
            {
                i_sprite s = new i_sprite(spriteSort, b[0], b[1], b[2]);
                sprites[i].Add(s);

                b.RemoveAt(0);
                b.RemoveAt(0);
                b.RemoveAt(0);
            }
        }
        return sprites;
    }

    public void writeAllSprites(SortedList<ushort, List<i_sprite>> allSprites)
    {
        int[] pointers = new int[roomPointers.Length];
        const int blah = 2; //inspired by HM source code

        int lowestPointer = int.MaxValue;
        foreach (int i in roomPointers)
            lowestPointer = Math.Min(lowestPointer, i);

        List<byte> data = new List<byte>();

        for (ushort i = 0; i < roomPointers.Length; i++)
            if (!allSprites.ContainsKey(i))
                allSprites.Add(i, new List<i_sprite>());

        foreach (ushort i in allSprites.Keys)
        {
            if (allSprites[i].Count == 0)
                pointers[i] = lowestPointer + sizeOfData - blah;
            else
            {
                List<byte> data2 = new List<byte>();

                byte byteSortByte = 0x0;
                bool first = true;
                foreach (i_sprite s in allSprites[i])
                {
                    byte[] b = null;
                    if (first)
                    {
                        b = s.getBytes(out byteSortByte);
                        first = false;
                        data2.Add(byteSortByte);
                    }
                    else
                    {
                        b = s.getBytes(out byte temp);

                        if (temp != byteSortByte)
                            throw new Exception();
                    }

                    data2 = data2.Concat(b).ToList();
                }
                data2.Add(sprite_delim);

                pointers[i] = lowestPointer + data.Count;
                data = data.Concat(data2).ToList();
            }
        }

        if (data.Count > sizeOfData - blah)
            throw new Exception();

        while (data.Count < sizeOfData - blah)
            data.Add(sprite_delim);

        data.Add(0xAA);
        data.Add(sprite_delim);

        List<byte> pointerData = new List<byte>();
        foreach (int address in pointers)
        {
            byte[] d = new byte[]
            {
                (byte)AddressLoROM.PcToSnes_Lo(address),
                (byte)AddressLoROM.PcToSnes_Hi(address)
            };

            pointerData = pointerData.Concat(d).ToList();
        }

        ROM.Write(secondaryPointer, size, pointerData.ToArray());
        ROM.Write(lowestPointer, sizeOfData, data.ToArray());
        updatePointers();
    }
}
