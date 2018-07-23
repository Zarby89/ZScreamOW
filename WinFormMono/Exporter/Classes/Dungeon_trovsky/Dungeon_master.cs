/*
 * Class        :   Dungeon_master.cs
 * Author       :   trovsky, Skarsnik(Consultant)
 * Description  :   
 */

using System;
using System.Collections.Generic;

/// <summary>
/// Get dungeon list objects
/// </summary>
public class Dungeon_master
{
    private Dungeon dung;
    private List<dungeon_object>[] master_list;

    /// <summary>
    /// originx and originy offset the dungeon objects coordinates for screen writing
    /// </summary>
    public static int originx, originy;

    public Dungeon_master(int originx = 0, int originy = 0)
    {
        Dungeon_master.originx = originx;
        Dungeon_master.originy = originy;
        dung = new Dungeon();
        master_list = new List<dungeon_object>[Dungeon.maxRoomNo + 1];

        for (ushort i = 0; i < master_list.Length; i++)
            master_list[i] = new List<dungeon_object>();

        readAllData();
    }

    /// <summary>
    /// Returns a list of dungeon objects for a specified room number
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public List<dungeon_object> getRoomData(ushort room)
    {
        if (master_list[room] == null)
            master_list[room] = new List<dungeon_object>();

        /*
         * Note that this method clones the object's from the
         * main list. This is to allow clean edit discarding
         */
        List<dungeon_object> new_ = new List<dungeon_object>();
        master_list[room].ForEach((item) => { new_.Add((dungeon_object)item.Clone()); });
        return new_;
    }

    /// <summary>
    /// After you're done modifying the dungeon list, submit the entry to the master list
    /// </summary>
    /// <param name="room"></param>
    /// <param name="list"></param>
    public void setRoomData(ushort room, List<dungeon_object> list)
    { master_list[room] = list; }


    private void readAllData()
    {
        SortedList<ushort, List<i_torch>> l_torch = dung.torches.readAllTorches();
        SortedList<ushort, List<i_block>> l_block = dung.block.readBlocks();
        SortedList<ushort, List<i_sprite>> l_sprite = dung.sprite.readAllSprites();
        dung.room.readAllObjects();

        foreach (ushort key in l_torch.Keys)
        {
            foreach (i_torch torch in l_torch[key])
            {
                dungeon_object d = new dungeon_object((int)objectType.torch, torch.rawX, torch.rawY, Convert.ToInt16(torch.isLayer2) + 1);

                if (master_list[key] == null)
                    master_list[key] = new List<dungeon_object>();
                master_list[key].Add(d);
            }
        }

        foreach (ushort key in l_block.Keys)
        {
            foreach (i_block block in l_block[key])
            {
                dungeon_object d = new dungeon_object((int)objectType.block, block.rawX, block.rawY, Convert.ToInt16(block.isLayer2) + 1);

                if (master_list[key] == null)
                    master_list[key] = new List<dungeon_object>();
                master_list[key].Add(d);
            }
        }

        foreach (ushort key in l_sprite.Keys)
        {
            foreach (i_sprite sprite in l_sprite[key])
            {
                dungeon_object d = new dungeon_object((int)objectType.sprite, sprite.rawX, sprite.rawY, Convert.ToInt16(sprite.isLayer2) + 1, sprite.spriteId);

                if (master_list[key] == null)
                    master_list[key] = new List<dungeon_object>();
                master_list[key].Add(d);
            }
        }
    }

    /// <summary>
    /// Submit changes you've made to the dungeons' to the ROM in memory.
    /// 
    /// Note: Sprite writing NOT done.
    /// </summary>
    public void writeToRom()
    {
        SortedList<ushort, List<i_torch>> l_torch = new SortedList<ushort, List<i_torch>>();
        SortedList<ushort, List<i_block>> l_block = new SortedList<ushort, List<i_block>>();
        SortedList<ushort, List<i_sprite>> l_sprite = new SortedList<ushort, List<i_sprite>>();

        ushort roomNo = 0;
        foreach (List<dungeon_object> l in master_list)
        {
            foreach (dungeon_object o in l)
            {
                switch (o.objectTypeIdentifier)
                {
                    case (int)(objectType.torch):
                        if (!l_torch.ContainsKey(roomNo))
                            l_torch.Add(roomNo, new List<i_torch>());
                        i_torch t = new i_torch(o.rawX, o.rawY, Convert.ToBoolean(o.BG_number));
                        l_torch[roomNo].Add(t);
                        break;
                    case (int)(objectType.block):
                        if (!l_block.ContainsKey(roomNo))
                            l_block.Add(roomNo, new List<i_block>());
                        i_block b = new i_block(roomNo, o.rawX, o.rawY);
                        l_block[roomNo].Add(b);
                        break;
                    case (int)(objectType.chest):
                        throw new NotImplementedException();
                    case (int)(objectType.sprite):
                        if (!l_sprite.ContainsKey(roomNo))
                            l_sprite.Add(roomNo, new List<i_sprite>());
                        i_sprite s = new i_sprite(true, (byte)o.spriteID, o.rawX, o.rawY, Convert.ToBoolean(o.BG_number));
                        l_sprite[roomNo].Add(s);
                        break;
                }
            }
            roomNo++;
        }

        dung.block.writeAllBlocks(l_block);
        dung.torches.writeAllTorches(l_torch);
        dung.sprite.writeAllSprites(l_sprite);
    }


    /// <summary>
    /// An enum for the dungeon_object class's attribute objectTypeIdentifier.
    /// This identifies whether the object you have is a torch, block, sprite, etc.
    /// </summary>
    public enum objectType
    {
        torch,
        block,
        chest,
        sprite
    }

    public enum bgLayer
    {
        BG1 = 1,
        BG2 = 2,
        BG3 = 3
    }


    /// <summary>
    /// This class stores information about dungeon objects the game stores e.g. 
    /// a torch, block, sprite, etc.
    /// </summary>
    public struct dungeon_object : ICloneable
    {
        /*
         * Not all the attributes are actually used by the game for certain types of objects.
         * Attributes not applicable for the object have a -1 value.
         * 
         * a) the objectTypeIdentifier attribute
         * b) if the attribute has the value of -1
         * 
         * Do not change attributes with a value of -1. The editor will throw an exception.
         */

        internal int rawX, rawY;

        /*
         * Torches, blocks, and sprites go on BG1 and BG2
         */

        /// <summary>
        /// What BG layer plane the object goes on
        /// </summary>
        public int BG_number;

        /// <summary>
        /// Not all the attributes are actually used by the game. Attributes not 
        /// applicable for the object have a -1 value.
        /// </summary>
        public int objectTypeIdentifier;
        private const int imageHeight = 16;
        private static int key = 0;

        /// <summary>
        /// This is a unique identifier for the dungeon object.
        /// </summary>
        public readonly string myKey;
        public readonly int gridbase;

        private int mySpriteID;

        public const int nulled = -1;
        public dungeon_object(int objectTypeIdentifier, int x = nulled, int y = nulled, int BG_number = nulled, int spriteID = nulled)
        {
            this.objectTypeIdentifier = objectTypeIdentifier;
            this.BG_number = BG_number;
            rawX = x;
            rawY = y;
            mySpriteID = spriteID;
            myKey = (key++).ToString();

            //if (objectTypeIdentifier != (int)objectType.sprite)
            //    gridbase = LOZobject.blockSize;
            gridbase = 2;
        }

        /// <summary>
        /// X Position for dungeon object
        /// 
        /// If your object is a sprite, the coordinates are
        /// based on a 16x16 grid.
        /// 
        /// If your object is NOT a sprite, the coordinates are
        /// based on a 8x8 grid.
        /// 
        /// Get:    Get's the X coordinate. No need to multiply this.
        ///         The value you get is an accurate coordinate of where 
        ///         the object is in the room.
        /// 
        /// Set:    Modify X coordinate
        /// </summary>
        public int X
        {
            get
            {
                checkIfNotNulled(rawX);
                return gridize(rawX * gridbase) + Dungeon_master.originx;
            }
            set
            {
                checkIfNotNulled(value);
                rawX = (value - Dungeon_master.originx) / gridbase;
            }
        }

        /// <summary>
        /// Y Position for dungeon object
        /// 
        /// If your object is a sprite, the coordinates are
        /// based on a 16x16 grid.
        /// 
        /// If your object is NOT a sprite, the coordinates are
        /// based on a 8x8 grid.
        /// 
        /// Get:    Get's the Y coordinate. No need to multiply this.
        ///         The value you get is an accurate coordinate of where 
        ///         the object is in the room.
        /// 
        /// Set:    Modify Y coordinate
        /// 
        /// </summary>
        public int Y
        {
            get
            {
                checkIfNotNulled(rawY);
                return gridize((rawY * gridbase)) + Dungeon_master.originy - imageHeight;
            }
            set
            {
                checkIfNotNulled(value);
                rawY = value / gridbase + imageHeight - Dungeon_master.originy;
            }
        }

        /// <summary>
        /// Make a coordinate align to the grid
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int gridize(int i)
        { return i / gridbase * gridbase; }


        /// <summary>
        /// This will throwan error if the coordinate is not aligned to the grid.
        /// </summary>
        /// <param name="value"></param>
        private void checkIfNotNulled(int value)
        {
            if (value <= nulled)
                throw new ArgumentException();
        }


        public int spriteID
        {
            get
            {
                checkIfNotNulled(mySpriteID);
                return mySpriteID;
            }
            set
            {
                checkIfNotNulled(value);
                mySpriteID = value;
            }
        }

        /// <summary>
        /// Makes a copy of the dungeon object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        { return new dungeon_object(objectTypeIdentifier, rawX, rawY, BG_number, mySpriteID); }
    }
}