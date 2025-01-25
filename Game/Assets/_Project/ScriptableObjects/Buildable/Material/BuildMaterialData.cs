using UnityEngine;

[CreateAssetMenu(fileName = "BuildableMaterialData", menuName = "Scriptable Objects/BuildableMaterialData")]
public class BuildMaterialData : ScriptableObject
{
    public enum TileConfiguration
    {
        None = 0,        // 0000
        Top = 1 << 3,    // 1000
        Right = 1 << 2,  // 0100
        Bottom = 1 << 1, // 0010
        Left = 1,        // 0001
        TopRight = 0b1100,
        TopBottom = 0b1010,
        TopLeft = 0b1001,
        RightBottom = 0b0110,
        RightLeft = 0b0101,
        BottomLeft = 0b0011,
        TopRightBottom = 0b1110,
        TopRightLeft = 0b1101,
        TopBottomLeft = 0b1011,
        RightBottomLeft = 0b0111,
        AllSides = 0b1111 // 1111
    }

    [System.Serializable]
    public class NamedTileType
    {
        public TileConfiguration configuration;
        public TileType tileType;
    }

    public NamedTileType[] tileTypes;

    public enum Material
    {
        NONE,
        WOOD,
        GLASS,
        STONE,
        PROT,
    }

    public Sprite sprite;
    public Sprite alternatSprite;
    public Sprite icon;
    public Color color = Color.white;
    public Material material;
    public int maxHp;
    public int score;
    public int minQuantity;
    public int maxQuantity;
    public int forceSpawnAtSlot;

    public bool hasRandDrawer = true;

    [ConditionalHide("hasRandDrawer", AttributeShowMode.READ_ONLY, AttributeShowMode.EDIT)]
    public BuildDrawerData notRandDrawer;

}

