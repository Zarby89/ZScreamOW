/*
 * Class        :   i_object_type1.cs
 * Author       :   Zarby89, Trovsky
 * Description  :   
 */
public sealed class i_object_type2
    {
        public byte direction { get; set; }
        public byte pos { get; set; }
        public byte type { get; set; }

        public byte posX { get; set; }
        public byte posY { get; set; }

        public i_object_type2(byte b1, byte b2)
        {
            direction = (byte)((b1 & 0x3) << 1);
            pos = (byte)((b2 & 0xF0) >> 3);
            type = b2;
        }
    }