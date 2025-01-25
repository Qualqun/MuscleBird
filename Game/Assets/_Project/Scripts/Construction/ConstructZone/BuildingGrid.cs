using UnityEngine;
using UnityEditor;
using BuildState;

public class BuildingGrid : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject cellPrefab;

    [SerializeField] int nbColumns;
    [SerializeField] int nbLines;

    [SerializeField] float cellsWight;
    [SerializeField] float cellsHeight;

    [SerializeField] float collidSizeExtand;

    int[,] gridArray;
    Vector3 gridSize;
    Vector3 cellsSize;
    Vector3 midleDownOffset;
    Vector3 origin;


    #endregion

    #region Messages

    private void Awake()
    {
        GameManager.Instance.NbCellsInAllGrid += nbColumns * nbLines;
    }

    void Start()
    {
        origin = transform.position;
        gridArray = new int[nbColumns, nbLines];
        gridSize = new Vector3(nbColumns * cellsWight, nbLines * cellsHeight);
        cellsSize = new Vector3(cellsWight, cellsHeight);
        midleDownOffset = new Vector3(-gridSize.x / 2, 0);

        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2((cellsWight * nbColumns) + collidSizeExtand, (cellsHeight * nbLines) + collidSizeExtand);
        boxCollider2D.offset = new Vector2(0f, gridSize.y / 2);

        for (int line = 0; line < nbLines; line++)
        {
            for (int column = 0; column < nbColumns; column++)
            {
                Vector3 worldPos = GetWorldPosition(column, line);
                int cellId = line * nbColumns + column;

                GameObject newCell = Instantiate(cellPrefab);
                newCell.transform.SetParent(transform);
                newCell.transform.position = worldPos;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    origin = transform.position;
    //    gridArray = new int[nbColumns, nbLines];
    //    gridSize = new Vector3(nbColumns * cellsWight, nbLines * cellsHeight);
    //    cellsSize = new Vector3(cellsWight, cellsHeight);
    //    midleDownOffset = new Vector3(-gridSize.x / 2, 0);

    //    BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
    //    boxCollider2D.size = new Vector2((cellsWight * nbColumns) + 2f, (cellsHeight * nbLines) + 2f);
    //    boxCollider2D.offset = new Vector2(0f, gridSize.y / 2);

    //    for (int line = 0; line < nbLines; line++)
    //    {
    //        for (int column = 0; column < nbColumns; column++)
    //        {
    //            Vector3 worldPos = GetWorldPosition(column, line);
    //            int cellId = line * nbColumns + column;

    //            Gizmos.DrawLine(worldPos - cellsSize / 2f, GetWorldPosition(column + 1, line) - cellsSize / 2f);
    //            Gizmos.DrawLine(worldPos - cellsSize / 2f, GetWorldPosition(column, line + 1) - cellsSize / 2f);

    //            Handles.Label(worldPos, cellId.ToString());
    //        }
    //    }
    //}

    #endregion

    #region PrivateMethods



    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellsWight, y * cellsHeight) + (origin + midleDownOffset) + cellsSize / 2f;
    }

    Vector3 GetWorldPosition(Vector3Int coord)
    {
        return new Vector3(coord.x * cellsWight, coord.y * cellsHeight) + (origin + midleDownOffset) + cellsSize / 2f;
    }

    Vector3Int GetColumnLine(Vector3 worldPos)
    {
        int column = Mathf.FloorToInt((worldPos - (origin + midleDownOffset)).x / cellsWight);
        int line = Mathf.FloorToInt((worldPos - (origin + midleDownOffset)).y / cellsHeight);



        return new Vector3Int(column, line);
    }

    void SetCellValue(Vector3 worldPos, int value)
    {
        Vector3Int coord = GetColumnLine(worldPos);
        gridArray[coord.x, coord.y] = value;
    }

    void SetCellValue(Vector3Int coord, int value)
    {
        gridArray[coord.x, coord.y] = value;
    }

    int GetCellValue(Vector3 worldPos, int value)
    {
        Vector3Int coord = GetColumnLine(worldPos);
        return gridArray[coord.x, coord.y];
    }

    int GetCellValue(Vector3Int coord, int value)
    {
        return gridArray[coord.x, coord.y];
    }
    #endregion

    #region PublicMethods
    public bool CheckIfIsOnGrid(Vector3 position)
    {
        Vector3Int coord = GetColumnLine(position);

        return (coord.x >= 0 && coord.x < nbColumns) && (coord.y >= 0 && coord.y < nbLines);
    }

    public bool CheckIfIsOnGrid(Bounds bounds)
    {
        Bounds gridBounds = GetComponent<BoxCollider2D>().bounds;
        return gridBounds.Contains(bounds.min) && gridBounds.Contains(bounds.max);
    }

    public Vector3 GetMagnetizedPosition(Vector3 position, bool betweenX = false, bool betweenY = false)
    {
        Vector3Int coord = GetColumnLine(position);
        Vector3 newPosition = GetWorldPosition(coord);

        newPosition.x = betweenX ? newPosition.x + cellsWight / 2f : newPosition.x;
        newPosition.y = betweenY ? newPosition.y + cellsHeight / 2f : newPosition.y;

        return newPosition;
    }

    public void RemoveGridCells()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    #region Properties/Accessors

    #endregion
}