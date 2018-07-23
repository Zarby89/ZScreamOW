using ZScream_Exporter;

public class frameWork
{
    protected int[]
        primaryPointer_address,
        primaryPointer_location;

    protected readonly int numberOfPointers;

    protected frameWork(int i)
    {
        numberOfPointers = i;
        primaryPointer_address = new int[numberOfPointers];
    }

    protected void refreshPointer3Bytes()
    {
        for (int i = 0; i < primaryPointer_address.Length; i++)
            primaryPointer_address[i] = PointerRead.LongRead_LoHiBank(primaryPointer_location[i]);
    }
}
