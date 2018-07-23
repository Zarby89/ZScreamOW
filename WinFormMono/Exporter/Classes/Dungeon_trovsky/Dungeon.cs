/*
 * Class        :   Dungeon.cs
 * Author       :   Trovsky, Zarby, Superskuj
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

/// <summary>
/// The main class that manages all other classes.
/// It hides all the "dirty" work the classes have to do.
/// </summary>
public class Dungeon
{
    public readonly object_data room;
    public readonly indoor_sprites sprite;
    public readonly indoor_blocks block;
    public readonly indoor_torches torches;
    public readonly indoor_chests chest;
    public readonly indoor_damagepits pit;
    public readonly indoor_telepathy telepathy;
    public readonly roomHeader roomheader;
    public readonly indoor_items item;
    public const string
        moveError = "error_move_error",
        edit_lock_error = "error_edit_lock";
    public const ushort nullRoomVal = ushort.MaxValue;
    public const byte nullValue = 0xFF;
    public const ushort maxRoomNo = 295;

    /// <summary>
    /// Initilializes a new instance of Dungeon
    /// </summary>
    /// <param name="rom"></param>
    public Dungeon()
    {
        room = new object_data();
        sprite = new indoor_sprites();
        block = new indoor_blocks();
        torches = new indoor_torches();
        chest = new indoor_chests();
        pit = new indoor_damagepits();
        telepathy = new indoor_telepathy();
        roomheader = new roomHeader();
        item = new indoor_items();
    }
}