using ZScream_Exporter;

public static class Dungeon_constants
{
    public const int
        SPRITEBASE = 16,
        BLOCKBASE = 8;

    public static string[] pot_item_names { get; private set; }
    public static string[] pot_item_special_names { get; private set; }
    public static string[] sprite_names { get; private set; }

    public static void generateStrings()
    {
        pot_item_names = new string[22];
        sprite_names = new string[byte.MaxValue];
        pot_item_special_names = new string[5];

        for (int i = 0; i < pot_item_names.Length; i++)
            pot_item_names[i] = TextAndTranslationManager.GetString("pot_item_op" + i.ToString("000"));

        for (int i = 0; i < pot_item_special_names.Length; i++)
            pot_item_special_names[i] = TextAndTranslationManager.GetString("pot_item_special_op" + i);

        for (int i = 0; i <= 242; i++)
            sprite_names[i] = TextAndTranslationManager.GetString("sprite_names_op" + i.ToString("000"));
    }
}
