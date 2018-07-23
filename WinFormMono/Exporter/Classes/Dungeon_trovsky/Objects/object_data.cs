/*
 * Class        :   object_data.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using System.Collections.Generic;
using ZScream_Exporter;

public class object_data : frameWork
{
    private int[]
        secondayPointers, //room pointers
        layoutPointers;

    private const int
        bytesPerPointer = 3,
        pointerSize = 0x3C0,
        numberOfRooms = pointerSize / bytesPerPointer,
        layoutPointersLocation = 0x26F2F,
        numberOfLayouts = 0x8;

    public const int layerNo = 3;

    public int getPrimaryPointer()
    { return primaryPointer_address[1]; }

    internal object_data() : base(3)
    {
        updatePrimaryPointers();
        getPointers();
    }

    private void updatePrimaryPointers()
    {
        /* In the original ROM, the pointers point to 
         * 0xF8000 and 0xF8001. I will just use the pointer at 0x874C,
         * should be arbitrary,
         */

        primaryPointer_location = new int[]
        {
            0x8746, //0x0F8001   \
            0x874C, //0x0F8000    | is 0xF8000
            0x883F, //0x0F8001    |
            0x8845  //0x0F8000   /
        };

        primaryPointer_address = new int[primaryPointer_location.Length];

        for (int i = 0; i < primaryPointer_location.Length - 1; i++)
        {
            byte[] b = ROM.Read(primaryPointer_location[i], bytesPerPointer);
            updatePointer(b, ref primaryPointer_address, bytesPerPointer, i);
        }
    }

    /// <summary>
    /// Reads the pointer, which is byte swapped
    /// </summary>
    private void getPointers()
    {
        secondayPointers = new int[numberOfRooms];
        byte[] rawData = new byte[pointerSize];
        Array.Copy(ROM.DATA, primaryPointer_address[1], rawData, 0, pointerSize);
        updatePointer(rawData, ref secondayPointers, rawData.Length);

        layoutPointers = new int[numberOfLayouts];

        for (int i = 0; i < numberOfLayouts; i++)
            layoutPointers[i] = PointerRead.LongRead_LoHiBank(layoutPointersLocation + (i * 3));
    }

    private void updatePointer(byte[] rawData, ref int[] pointer, int length, int start = 0)
    {
        for (int i = 0, j = 0; (i < length - 1) && (j < rawData.Length - 1); i += 1, j += 3)
            pointer[i + start] = AddressLoROM.SnesToPc((rawData[j]), (rawData[j + 1]), (rawData[j + 2]));
    }

    public void movePointers(int newAddress)
    {
        if (ROM.IsEmpty(newAddress, pointerSize))
        {
            PointerRead.CheckAddressWithinRangeOf3Byte(newAddress);
            ROM.Swap(primaryPointer_address[1], newAddress, pointerSize);

            byte[] b = PointerRead.GeneratePointer3(newAddress);

            for (int i = 0; i < primaryPointer_location.Length; i++)
            {
                b[0] += (byte)((i % 2 == 0) ? 1 : -1); //works if the first byte is ($actualPointer + 1)
                ROM.Write(primaryPointer_location[i], 3, b);
            }

            updatePrimaryPointers();
        }
        else throw new Exception(TextAndTranslationManager.GetString(Dungeon.moveError));
    }

    private byte[]
        nextLayer = new byte[] { 0xFF, 0xFF },
        nextType = new byte[] { 0xFF, 0xF0 };

    public room_object_header[] readAllObjects()
    {
        ushort roomNo = 0;

        room_object_header[] bytes = new room_object_header[numberOfRooms];

        foreach (int address in secondayPointers)
        {
            bytes[roomNo] = new room_object_header();
            bytes[roomNo].generateFromBytes(ROM.DATA[address + 0], ROM.DATA[address + 1]);


            for (int k = 0; k < layerNo; k++)
            {
                bytes[roomNo].layers[k].type1 = new List<i_object_type1>();
                bytes[roomNo].layers[k].type2 = new List<i_object_type2>();
            }

            int i = 2;

            bool isType1 = true;
            int layer_i = 0;

            while (true)
            {
                byte b = ROM.DATA[address + i];

                if (b == nextLayer[0])
                {
                    if (ROM.DATA[address + i + 1] == nextLayer[1])
                    {
                        if (layer_i < layerNo)
                        {
                            layer_i++;
                            isType1 = true;
                            i++;
                            continue;
                        }
                        else throw new Exception();
                    }
                    else break;
                }
                else if (b == nextType[0])
                {
                    if (ROM.DATA[address + i + 1] == nextType[1])
                    {
                        if (isType1)
                        {
                            isType1 = false;
                            continue;
                        }
                        else throw new Exception();
                    }
                }
                else
                {
                    if (isType1)
                    {
                        i_object_type1 t = new i_object_type1
                            (
                            ROM.DATA[address + i],
                            ROM.DATA[address + i + 1],
                            ROM.DATA[address + i + 2]
                            );

                        bytes[roomNo].layers[layer_i].type1.Add(t);

                        i += 3;
                    }
                    else
                    {
                        i_object_type2 t = new i_object_type2
                            (
                            ROM.DATA[address + i],
                            ROM.DATA[address + i + 1]
                            );

                        bytes[roomNo].layers[layer_i].type2.Add(t);

                        i += 2;
                    }
                }
            }
            roomNo++;
        }
        return bytes;
    }
}