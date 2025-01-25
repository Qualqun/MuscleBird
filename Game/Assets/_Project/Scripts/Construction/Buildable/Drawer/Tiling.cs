using UnityEngine;

/*
 * @brief TileType needs an array of 15 to cover all bit flags
 * @member m_sprite => Sprite used for this specific set
 * @member m_rotation => Rotation used for this specific set
 */
[System.Serializable]
public class TileType
{
    public Sprite m_sprite;
    public float m_rotation;
    [HideInInspector] public int m_flags;
}



public class Tiling : MonoBehaviour
{
    public BuildMaterialData[] tileRuleSets;  // Array to hold multiple TileRuleSets

    public enum Winding
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
    }

    private enum BitFlags
    {
        TOP = 0b1000,
        RIGHT = 0b0100,
        BOTTOM = 0b0010,
        LEFT = 0b0001,
    }
    static public int[] pData;
    static public bool[][] placed;
    private void Awake()
    {
        placed = new bool[2][];
        placed[0] = new bool[4];
        placed[1] = new bool[4];

        for (int player = 0; player < 2; player++)
        {
            for (int i = 0; i < placed[player].Length; i++)
            {
                placed[player][i] = false;
            }
        }
    }
    BuildMaterialData.TileConfiguration[][] binds;
    /*
     * @brief
     * @param Material[4] _surroundings => Needs a size of 4 winded clockwise like : TOP, RIGHT, BOTTOM, LEFT
     * @param Material _actualMaterial => The material 
     */
    public TileType GetSpriteFromNeighbours(BuildMaterialData.Material[] _surroundings, BuildMaterialData.Material _actualMaterial)
    {
        if (_surroundings.Length != 4)
        {
            Debug.Log("Wrong surrounding count, needs 4 of them only.");
            return null;
        }

        int flags = 0;
        if (_surroundings[(int)Winding.TOP] == 0) flags |= (int)BitFlags.TOP;
        if (_surroundings[(int)Winding.RIGHT] == 0) flags |= (int)BitFlags.RIGHT;
        if (_surroundings[(int)Winding.BOTTOM] == 0) flags |= (int)BitFlags.BOTTOM;
        if (_surroundings[(int)Winding.LEFT] == 0) flags |= (int)BitFlags.LEFT;

        if (_actualMaterial == BuildMaterialData.Material.PROT)
        {
            if (binds == null)
            {
                binds = new BuildMaterialData.TileConfiguration[2][];
                binds[0] = new BuildMaterialData.TileConfiguration[4];
                binds[0][0] = BuildMaterialData.TileConfiguration.None;
                binds[0][1] = BuildMaterialData.TileConfiguration.Left;
                binds[0][2] = BuildMaterialData.TileConfiguration.TopLeft;
                binds[0][3] = BuildMaterialData.TileConfiguration.BottomLeft;

                binds[1] = new BuildMaterialData.TileConfiguration[4];
                binds[1][0] = BuildMaterialData.TileConfiguration.None;
                binds[1][1] = BuildMaterialData.TileConfiguration.Right;
                binds[1][2] = BuildMaterialData.TileConfiguration.TopRight;
                binds[1][3] = BuildMaterialData.TileConfiguration.RightBottom;
            }

            for (int i = 0; i < placed[GameManager.Instance.currentPlayerIndex].Length; i++)
            {
                if (placed[GameManager.Instance.currentPlayerIndex][i]) continue;
                foreach (var element in tileRuleSets[(int)_actualMaterial].tileTypes)
                {
                    if (element.configuration == binds[GameManager.Instance.currentPlayerIndex][i])
                    {
                        placed[GameManager.Instance.currentPlayerIndex][i] = true;
                        return element.tileType;
                    }
                }
            }
        }
        for (int i = 0; i < 16; i++)
        {
            if ((int)tileRuleSets[(int)_actualMaterial].tileTypes[i].configuration == flags)
            {
                return tileRuleSets[(int)_actualMaterial].tileTypes[i].tileType;
            }
        }
        //Random don't look
        return tileRuleSets[(int)_actualMaterial].tileTypes[flags].tileType;
    }
}
