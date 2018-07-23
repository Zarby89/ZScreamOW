/*
 * Class        :   Layout.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using ZScream_Exporter;

public class Layout
{
    private int[] layoutLocations;

    public Layout()
    {
        int primaryPointer_location;

        switch (RegionId.myRegion)
        {
            case ((int)RegionId.Region.USA):
                primaryPointer_location = 0x26F2F;
                break;
            case ((int)RegionId.Region.Japan):
                primaryPointer_location = 0x26C0F;
                break;
            default:
                throw new NotImplementedException();
        }

        int location = PointerRead.LongRead_LoHiBank(primaryPointer_location);
    }
}
