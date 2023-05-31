using UnityEngine;

public static class Globals
{
    public const float TileSize = 6.5f;
    public const float SpacingSize = 0.0f;
    public static float FullTileSize = TileSize + SpacingSize;
    public static float BorderOffset = 10.0f;

    public static string directoryPath = Application.persistentDataPath + "/LevelData";
    public static string DataPathText = directoryPath + "/RM_A";
    public static string AlternativeDataPathText = directoryPath + "/RM_B";
    public static int SelectedLevel = 1;
}
