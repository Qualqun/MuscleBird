using UnityEngine;
using UnityEngine.UI;

public class MatrixImageGenerator : MonoBehaviour
{
    Sprite imageSprite;
    int[,] matrix;
    public float cellSize = 100f;
    public float offset = 1;
    public Sprite sprite;
    public Color color0;
    public Color color1;

    void Awake()
    {
        //imageSprite = GetComponent<Image>().sprite;
        //color = GetComponent<Image>().color;
        //GetComponent<Image>().color = Color.clear;
    }

    public void GenerateImagesFromMatrix()
    {
        matrix = GetComponentInParent<BuildSlot>().ContainedBuild.Drawer.Data.matrix;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                GameObject imageObject = new GameObject("Image_" + i + "_" + j);
                imageObject.transform.SetParent(transform);


                Image image = imageObject.AddComponent<Image>();
                image.sprite = sprite;
                image.color = matrix[i, j] >= 1 ? color1 : color0;

                RectTransform rectTransform = image.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(j * cellSize * offset, -i * cellSize * offset, 0);

                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            }
        }


    }
}
