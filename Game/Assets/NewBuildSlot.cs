using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NewBuildSlot : MonoBehaviour, BuildCursor.Behavior
{
    [SerializeField] RuleTile ruleTile;
    [SerializeField] BuildFactory buildFactory;

    [SerializeField] List<BuildMaterialData> buildMaterialDataList;
    [SerializeField] List<BuildDrawerData> buildDrawerDataList;

    [SerializeField] NewBuildSelecterPanel selecter;
    [SerializeField] Image buildDraw;
    [SerializeField] Image materialIcon;

    Tilemap tilemap;
    int contentCountOnStart;
    int contentCount;
    Build containedBuild;

    BuildMaterialData currentMaterial;
    BuildDrawerData currentDrawer;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        GetComponentInParent<Grid>().cellSize = ruleTile.m_DefaultSprite.bounds.size;
    }

    void Start()
    {
        ReloadContent();
    }

    public void ReloadContent()
    {
        ClearContent();
        RandomizeContent();
        GenerateContainedBuild();
        RefreshDisplay();
    }

    public void ClearContent()
    {
        contentCount = 0;
        currentMaterial = null;
        currentDrawer = null;
        if (containedBuild != null)
            Destroy(containedBuild.gameObject);
    }

    public void RandomizeContent()
    {
        Tuple<BuildMaterialData, BuildDrawerData> pair = selecter.GetPairFromWhiteList(buildMaterialDataList, buildDrawerDataList);

        currentMaterial = pair.Item1;
        currentDrawer = pair.Item2;

        AddBuildOnSlot(UnityEngine.Random.Range(currentMaterial.minQuantity, currentMaterial.maxQuantity + 1));
        
        contentCountOnStart = contentCount;
    }

    public void GenerateContainedBuild()
    {
        containedBuild = buildFactory.CreateBuildOnUI(currentDrawer, currentMaterial, this);
        containedBuild.gameObject.SetActive(false);
    }

    public void RefreshDisplay()
    {
        materialIcon.sprite = currentMaterial.icon;
        PlaceTiles();
        buildDraw.sprite = GenerateSpriteFromTilemap();
        tilemap.ClearAllTiles();
    }

    void PlaceTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (currentDrawer.matrix[i].values[j] >= 1)
                {
                    Vector3Int pos = new Vector3Int(j, -i, 0);
                    tilemap.SetTile(pos, ruleTile);
                    tilemap.RefreshTile(pos);

                }
            }
        }
    }

    public Sprite GenerateSpriteFromTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int tilemapSize = new Vector3Int(3, 3);

        // 1 : Creat Texture of tilemap
        int tileSize = (int)(ruleTile.m_DefaultSprite.bounds.size.x * ruleTile.m_DefaultSprite.pixelsPerUnit); //72x72
        int width = tilemapSize.x * tileSize;
        int height = tilemapSize.y * tileSize;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // 2 : Fill the texture with transparent pixels
        Color[] transparentPixels = new Color[width * height];
        for (int i = 0; i < transparentPixels.Length; i++)
        {
            transparentPixels[i] = Color.clear;
        }
        texture.SetPixels(transparentPixels);

        // 3 : Fill the texture with Tiles pixels
        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {

                Sprite tileSprite = tilemap.GetSprite(position);

                if (tileSprite != null)
                {

                    Rect spriteRect = tileSprite.rect;

                    Texture2D spriteTexture = tileSprite.texture;

                    for (int x = 0; x < spriteRect.width; x++)
                    {
                        for (int y = 0; y < spriteRect.height; y++)
                        {
                            Color color = spriteTexture.GetPixel(
                                Mathf.FloorToInt(spriteRect.x) + x,
                                Mathf.FloorToInt(spriteRect.y) + y
                            );

                            int targetX = (position.x - bounds.xMin) * tileSize + x;
                            int targetY = (position.y - bounds.yMin) * tileSize + y;

                            if (tilemap.size.y < tilemapSize.y) //DEPRECATED, for type ONE replacer
                            {
                                targetY += (tileSize);
                            }

                            texture.SetPixel(targetX, targetY, color);
                        }
                    }
                }
            }
        }

        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    public void AddBuildOnSlot(BuildDrawerData drawerData, BuildMaterialData materialData, int quantity = 1)
    {
        if (contentCount == 0 && quantity > 0)
        {
            buildDraw.color = Color.white;
        }

        contentCount += quantity;
        GameManager.Instance.CurrentBuildsInfo.nbBuilds += quantity;
        GetComponentInChildren<TextMeshProUGUI>().text = contentCount > 0 ? "x" + contentCount.ToString() : "";
        
        if (currentMaterial.material == BuildMaterialData.Material.PROT)
        {
            GameManager.Instance.CurrentBuildsInfo.kingsRemaining = contentCount;
        }
    }

    public void AddBuildOnSlot(int quantity)
    {
        AddBuildOnSlot(currentDrawer, currentMaterial, quantity);
    }

    public void RemoveBuildOnSlot(int quantity)
    {
        if (contentCount - quantity < 1)
        {
            contentCount = 0;
            buildDraw.color = Color.clear;
        }
        else
        {
            contentCount -= quantity;
        }

        GetComponentInChildren<TextMeshProUGUI>().text = contentCount > 0 ? "x" + contentCount.ToString() : "";
    }

    public void ReturnBuildOnSlot()
    {
        AddBuildOnSlot(1);
    }

    public void ResetContentCount()
    {
        Destroy(containedBuild.gameObject);
        GenerateContainedBuild();
        AddBuildOnSlot(contentCountOnStart - contentCount);
        buildDraw.rectTransform.rotation = Quaternion.identity;
    }

    public bool IsMouseOver()
    {
        if (contentCount == 0) return false;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localMousePosition = Vector2.zero;

        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            GetComponent<Image>().canvas.worldCamera,
            out localMousePosition
        );

        return rectTransform.rect.Contains(localMousePosition);
    }

    public void OnMouseButtonDown()
    {
        
    }

    public void OnMouseButtonUp()
    {
        if (contentCount == 0) return;
  
        containedBuild.OnMouseButtonUp();
        buildDraw.transform.localEulerAngles += new Vector3(0, 0, -90);
    }

    public void OnBeginDrag()
    {
        if (contentCount == 0) return;
        containedBuild.OnBeginDrag();
    }

    public void OnDrag(Vector3 dragPos)
    {
    }

    public void OnEndDrag()
    {
    }



    public int ContentCount { get => contentCount; }
}
