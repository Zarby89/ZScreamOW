lorom
;#====================================================
;This is the core script of the editor modifying the
;Tile map system to use only 16x16 tiles instead of 32x32 tiles
;it also add tile type feature, modify the position of the sprites
;and the items location on the overworld
;#====================================================
;Every comments between #= will be added in the description of the script editor
;Every comments after a org $addr will be added in description of that org in script editor

org $02F54A ;Updatetile32 (replaced by tiles16)

LDA #$7E00 : STA $04

;load overworld (index * 3)
LDA $8A : ASL A : CLC : ADC $8A : TAX
LDY #$2000
JSR Looptilesmap

;load overworld (index * 3) + 1
LDA $8A : ASL A : CLC : ADC $8A : CLC : ADC #$0003 : TAX
LDY #$2040
JSR Looptilesmap

;load overworld (index * 3) + 8
LDA $8A : ASL A : CLC : ADC $8A : CLC : ADC #$0018 : TAX
LDY #$3000
JSR Looptilesmap

;load overworld (index * 3) + 9
LDA $8A : ASL A : CLC :  ADC $8A : CLC : ADC #$001B : TAX
LDY #$3040


Looptilesmap:
{
    LDA $33FE20, X : STA $00
    LDA $33FE22, X : STA $02
    STY $03 ;store address we're writing to
    LDX #$0000
    LDY #$0000
.loop2
    .loop1
        LDA [$00], Y : STA [$03], Y ; copy tile from ROM to RAM $7F2000, Y
        INY : INY
        CPY #$0040 : BCC .loop1 ;if Y < 64 (32tiles)
    ;if Y > 64 (32tiles)
    LDA $00 : CLC : ADC #$0040 : STA $00 ;increase source address by 64bytes (32tiles)
    LDA $03 : CLC : ADC #$0080 : STA $03 ;increase destination address by 128bytes (skipping right side map) (64 tiles)
    LDY #$0000
    INX
CPX #$0020 : BCC .loop2
RTS
}

org $3DF800 ;Tiles type location
skip #$A0 ; Tiletype used for each map A0 length

org $00886E ;Change what tile type we are standing on to match our new tiles type location
JSL loadTileAttr

org $09C4AC;Moved sprites to the end of the rom so no need to split duplicate rooms
    LoadOverworldSprites:
    {
        ; calculate lower bounds for X coordinates in this map
        LDA $040A : AND.b #$07 : ASL A : STZ $0FBC : STA $0FBD
        
        ; calculate lower bounds for Y coordinates in this map
        LDA $040A : AND.b #$3F : LSR #2 : AND.b #$0E : STA $0FBF : STZ $0FBE
        
        LDA $040A : TAY
        
        LDX $C635, Y : STX $0FB9 : STZ $0FB8 
        STX $0FBB    : STZ $0FBA
        
        REP #$30
        
        ; What Overworld area are we in?
        LDA $040A : ASL A : TAY
        
        SEP #$20
        
        ; load the game state variable
        JSL setAddressAndState
        
        CMP.b #$03 : BEQ .secondPart
        CMP.b #$02 : BEQ .firstPart
        
        ; Load the "Beginning" sprites for the Overworld. ;64 Rain State
        LDA $C881, Y : STA $00
        LDA $C882, Y
        
        BRA .loadData
    
    .secondPart
    
        ; Load the "Second part" sprites for the Overworld.;143 Agahnim Defeated
        LDA $CA21, Y : STA $00
        LDA $CA22, Y
        
        BRA .loadData
    
    .firstPart
    
        ; Load the "First Part" sprites for the Overworld.;143 Zelda Rescued
        LDA $C901, Y : STA $00
        LDA $C902, Y
    
    .loadData
    
        STA $01
        
        LDY.w #$0000
    
    .nextSprite
    
        ; Read off the sprite information until we reach a #$FF byte.
        LDA [$00], Y : CMP.b #$FF : BEQ .stopLoading
        
        INY #2
        
        ; Is this a �Falling Rocks� sprite?
        LDA [$00], Y : DEY #2 : CMP.b #$F4 : BNE .notFallingRocks
        
        ; Set a "falling rocks" flag for the area and skip past this sprite
        INC $0FFD
        
        INY #3
        
        BRA .nextSprite
    
    .notFallingRocks ; Anything other than falling rocks.
    
        LDA [$00], Y : PHA : LSR #4 : ASL #2 : STA $03 : INY
        
        LDA [$00], Y : LSR #4 : CLC : ADC $03 : STA $06
        
        PLA : ASL #4 : STA $07
        
        ; All this is to tell us where to put the sprite in the sprite map.
        LDA [$00], Y : AND.b #$0F : ORA $07 : STA $05
        
        ; The sprite / overlord index as stored as one plus it�s normal index. Don�t ask me why yet.
        INY : LDA [$00], Y : LDX $05 : INC A : STA $7FDF80, X ; Load them into what I guess you might call a sprite map.
        
        ; Move on to the next sprite / overlord.
        INY
        
        BRA .nextSprite
    
    .stopLoading
    
        SEP #$10
        
        RTS
    }

org $3FC000 ;new tiles types attributes location 0x2000 size
Overworld_TileAttr:
{
 

}

org $1BC8BF ;Items Data Location Bank
dw #$003E  ;THAT NEED TO CHANGE WITH DATAOFFSET


org $02AB0D ;Load Map Properties Hook to add 0AA5 (tile typeset)
JSL loadTileTypeSet

;CODE HERE hooks are above that part ONLY 1 ORG HERE !
org $208000 ;New code added no hooks past that point

loadTileAttr:
{
    ;0AA5 (free ram) -> used to keep tile type set used in current map, loaded on map load
    ;A,X,Y on 16-bit mode
    PHY
    LDY $00 : PHY
    STX $00
    LDA $0AA5 : AND #$000F : XBA : ASL #$01 : ORA $00 : TAX
    LDA $3FC000, X ; load tile value with an offset
    TYX
    PLY : STY $00
    PLY
    RTL
}

setAddressAndState:
{
    LDA #$3E : STA $02 ;THAT NEED TO CHANGE WITH DATAOFFSET
    LDA $7EF3C5
    RTL
}

loadTileTypeSet:
{
    ;X = map index
    LDA $3DF800, X : STA $0AA5
    LDA $7EFCC0, X : STA $0AA3;Restore previous code
    RTL
}