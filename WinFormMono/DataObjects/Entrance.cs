/*
 * Author:  Zarby89
 */

/// <summary>
/// 
/// </summary>
public struct Entrance //can be used for starting entrance as well
{
    public short room;//word value for each room

    //Missing Values : 
    public byte scrolledge_HU;//8 bytes per room, HU, FU, HD, FD, HL, FL, HR, FR
    public byte scrolledge_FU;
    public byte scrolledge_HD;
    public byte scrolledge_FD;
    public byte scrolledge_HL;
    public byte scrolledge_FL;
    public byte scrolledge_HR;
    public byte scrolledge_FR;
    public short yscroll;//2bytes each room
    public short xscroll; //2bytes
    public short yposition;//2bytes
    public short xposition;//2bytes
    public short ycamera;//2bytes
    public short xcamera;//2bytes
    public byte blockset; //1byte
    public byte floor; //1byte
    public byte dungeon; //1byte (dungeon id) //Same as music might use the project dungeon name instead
    public byte door; //1byte
    public byte ladderbg; ////1 byte, ---b ---a b = bg2, a = need to check -_-
    public byte scrolling;////1byte --h- --v- 
    public byte scrollquadrant; //1byte
    public short exit;//2byte word 
    public byte music; //1byte //Will need to be renamed and changed to add support to MSU1

    //Scrolling Quadrant notes :
    //$A9[0x01] - 0 if you are on the left half of the room. 1 if you are on the right half.
    //$AA[0x01] - 2 if you are the lower half of the room. 0 if you are on the upper half.
    //LDA $D69F, X : LSR #4     : STA $A9
    //LDA $D69F, X : AND.b #$0F : STA $AA

}