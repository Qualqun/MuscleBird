using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildDrawer : MonoBehaviour
{
    #region InternalStruct

    public struct DynamicData
    {
        public int nbColumns;
        public int nbRows;
        public int[,] matrix;
    }

    #endregion

    #region Fields

    [SerializeField] GameObject segmentPrefab;
    [SerializeField] GameObject scoreDestructPrefab;

    DynamicData data;
    Color defaultColor;
    Color currentColor;
    int nbSegments;
    TextMeshPro scoreDestruct;

    BuildDrawerData baseDataUnused;
    Sprite sprite;

    #endregion

    #region Messages
    private void OnEnable()
    {
        if (scoreDestruct != null)
        {
            Destroy(scoreDestruct);
        }
    }

    #endregion

    #region PrivateMethods
    void ConvertBaseDataToDynamicData(BuildDrawerData data)
    {
        this.data.nbColumns = data.nbColumns;
        this.data.nbRows = data.nbRows;

        this.data.matrix = new int[this.data.nbRows, this.data.nbColumns];

        for (int i = 0; i < this.data.nbRows; i++)
        {
            for (int j = 0; j < this.data.nbColumns; j++)
            {
                this.data.matrix[i, j] = data.matrix[i].values[j];
            }
        }
    }
    #endregion

    #region PublicMethods

    public void CopyTo(BuildDrawer drawerToCopy)
    {
        segmentPrefab = drawerToCopy.segmentPrefab;
        baseDataUnused = drawerToCopy.baseDataUnused;
        data = drawerToCopy.data;
        sprite = drawerToCopy.sprite;
        defaultColor = drawerToCopy.defaultColor;
        currentColor = drawerToCopy.currentColor;
        nbSegments = drawerToCopy.nbSegments;

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        nbSegments = 0;

        Vector3 spriteSize = Vector3.Scale(segmentPrefab.GetComponent<SpriteRenderer>().bounds.size, transform.lossyScale);
        Vector3 uncenterOffset = new Vector3(spriteSize.x, -spriteSize.y) / 2f;
        Vector3 midleOffset = new Vector3(-data.nbColumns, data.nbRows) / 2f;
        Vector3 scaledMidleOffset = Vector3.Scale(midleOffset, spriteSize);

        BuildMaterialData.Material material = GetComponent<Build>().Data.materialData.material;
        for (int i = 0; i < data.nbRows; i++)
        {
            for (int j = 0; j < data.nbColumns; j++)
            {
                int cellValue = data.matrix[i, j];

                if (cellValue > 0)
                {
                    GameObject newSegment = Instantiate(segmentPrefab);

                    Vector3 betweenOffset = new Vector3(j, -i);
                    Vector3 scaledBetweenOffset = Vector3.Scale(betweenOffset, spriteSize);
                    Vector3 segmentPos = scaledBetweenOffset + scaledMidleOffset;
                    Vector3 wordPos = transform.position + segmentPos + uncenterOffset;

                    SpriteRenderer sr = newSegment.GetComponent<SpriteRenderer>();
                    List<BuildMaterialData.Material> surroundings = new List<BuildMaterialData.Material>();

                    //TOP, RIGHT, BOTTOM, LEFT
                    surroundings.Add((IsCellInGrid(i - 1, j, data.nbRows, data.nbColumns)) ? (BuildMaterialData.Material)data.matrix[i - 1, j] : BuildMaterialData.Material.NONE);
                    surroundings.Add((IsCellInGrid(i, j + 1, data.nbRows, data.nbColumns)) ? (BuildMaterialData.Material)data.matrix[i, j + 1] : BuildMaterialData.Material.NONE);
                    surroundings.Add((IsCellInGrid(i + 1, j, data.nbRows, data.nbColumns)) ? (BuildMaterialData.Material)data.matrix[i + 1, j] : BuildMaterialData.Material.NONE);
                    surroundings.Add((IsCellInGrid(i, j - 1, data.nbRows, data.nbColumns)) ? (BuildMaterialData.Material)data.matrix[i, j - 1] : BuildMaterialData.Material.NONE);
                    
                    TileType tiling = GameManager.Instance.BuildFactory.gameObject.GetComponent<Tiling>().GetSpriteFromNeighbours(surroundings.ToArray(), material);

                    newSegment.GetComponent<SpriteRenderer>().sprite = tiling.m_sprite;

                    newSegment.transform.SetParent(transform, false);
                    newSegment.transform.position = wordPos;

                    nbSegments++;
                }
            }
        }
    }
    private bool IsCellInGrid(int x, int y, int xMax, int yMax)
    {
        if (x < 0 || x >= xMax || y < 0 || y >= yMax) return false;
        return true;
    }

    public void TurnQuarterDegreeClockwise()
    {
        transform.eulerAngles += new Vector3(0, 0, -90f);

        int rows = data.nbColumns;
        int columns = data.nbRows;
        int[,] rotatedMatrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                rotatedMatrix[i, j] = data.matrix[data.nbRows - 1 - j, i];
            }
        }

        data.nbRows = rows;
        data.nbColumns = columns;
        data.matrix = rotatedMatrix;

        //RefreshDisplay();
    }

    public Vector3 GetSize()
    {
        return new Vector3(data.nbColumns * 50f, data.nbRows * 50f);
    }

    public Vector3 GetCutedSize()
    {
        int nbColumns = 0, nbRows = data.nbRows;

        for (int i = 0; i < data.nbRows; i++)
        {
            int tempColumn = 0;
            for (int j = 0; j < data.nbColumns; j++)
            {
                if (data.matrix[i, j] >= 1)
                {
                    tempColumn++;
                    if (nbColumns < tempColumn)
                    {
                        nbColumns = tempColumn;
                    }
                }
            }

            if (tempColumn == 0)
            {
                nbRows--;
            }
        }

        return new Vector3(nbColumns * 50f, nbRows * 50f);
    }

    public void SetColor(Color color)
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = color;
        }
        currentColor = color;
    }

    public void SetColor(float r, float g, float b, float a = 1f)
    {
        SetColor(new Color(r, g, b, a));
    }

    public void SetColorRed(float red)
    {
        Color color = currentColor;
        color.r = red;
        SetColor(color);
    }

    public void SetColorGreen(float green)
    {
        Color color = currentColor;
        color.g = green;
        SetColor(color);
    }

    public void SetColorBlue(float blue)
    {
        Color color = currentColor;
        color.b = blue;
        SetColor(color);
    }

    public void SetColorAlpha(float alpha)
    {
        Color color = currentColor;
        color.a = alpha;
        SetColor(color);
    }

    public void ChangeDefaultColor(Color color)
    {
        SetColor(color);
        defaultColor = color;
    }

    public void SetDefaultColor()
    {
        SetColor(defaultColor);
    }

    public void SetData(BuildDrawerData drawerData)
    {
        baseDataUnused = drawerData;
        ConvertBaseDataToDynamicData(drawerData);
    }

    public void SetDynamicData(DynamicData drawerData)
    {
        data = drawerData;
    }

    public void DisplayScore()
    {
        if (scoreDestruct == null)
        {
            scoreDestruct = Instantiate(scoreDestructPrefab, transform.position, Quaternion.identity).GetComponent<TextMeshPro>();
            scoreDestruct.text = GetComponent<Build>().Data.materialData.score.ToString();
        }
    }

    public void SetOrderLayer(string sortingLayerName)
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingLayerName = sortingLayerName;
        }
    }
    #endregion

    #region Properties/Accessors

    public Sprite Sprite { get { return sprite; } set { sprite = value; } }
    public BuildDrawerData BaseDataUnused { get { return baseDataUnused; } set { baseDataUnused = value; } }
    public DynamicData Data { get { return data; } }
    public int NbSegments { get { return nbSegments; } }
    //(access modifier)type name{ get {return someVar;} set{ name = anotherVar}}
    #endregion


}
