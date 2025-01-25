using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;


struct BuildDataForSelecter
{
    public BuildDataForSelecter(BuildMaterialData materialData, BuildDrawerData drawerData)
    {
        this.materialData = materialData;
        this.drawerData = drawerData;
    }
    public BuildMaterialData materialData;
    public BuildDrawerData drawerData;
}

public class BuildSelecter : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{
    #region Fields

    [SerializeField] int nbSlots;
    [SerializeField] float percentOfAllowedSegments;

    [SerializeField] GameObject cellPrefab;

    [SerializeField] List<BuildMaterialData> buildMaterialDataList;
    [SerializeField] List<BuildDrawerData> buildDrawerDataList;

    #endregion

    #region Messages

    private void Awake()
    {
        RefreshSlots();
    }

    void Start()
    {
        StartCoroutine(RetardStartOneFrame());
        GameManager.Instance.RefreshSelecter += RefreshSelecter;
    }

    IEnumerator RetardStartOneFrame()
    {
        yield return new WaitForEndOfFrame();

        List<BuildMaterialData> materialList = new List<BuildMaterialData>(buildMaterialDataList);
        List<BuildDrawerData> drawerList = new List<BuildDrawerData>(buildDrawerDataList);

        //Step 0 : set number of combination array
        List<List<BuildDataForSelecter>> combinationsArray = new List<List<BuildDataForSelecter>>();
        for (int i = 0; i < materialList.Count; i++)
        {
            combinationsArray.Add(new List<BuildDataForSelecter>());
            for (int j = 0; j < drawerList.Count; j++)
            {
                combinationsArray[i].Add(new BuildDataForSelecter(materialList[i], drawerList[j]));
            }
        }

        //Step 1 : Exclude incorect material and Add force spawn
        for (int i = combinationsArray.Count - 1; i >= 0; i--)
        {
            BuildMaterialData material = combinationsArray[i][0].materialData;

            //Step 1.1 : Exclude bad min and max quantities
            if (material.maxQuantity <= 0 || material.minQuantity <= 0 || material.minQuantity > material.maxQuantity)
            {
                Debug.LogWarning("The Material Data : " + material.name + " have a bad min/max Value, He removed from list", this);
                combinationsArray.RemoveAt(i);
                continue;
            }

            //Step 1.2 : Priorize the force Spawn material
            if (material.forceSpawnAtSlot > 0 && material.forceSpawnAtSlot <= nbSlots)
            {
                BuildSlot slot = GetComponentsInChildren<BuildSlot>()[material.forceSpawnAtSlot - 1];

                if (material.hasRandDrawer)
                {
                    int randomDrawerIndex = Random.Range(0, combinationsArray[i].Count);
                    BuildDrawerData drawer = combinationsArray[i][randomDrawerIndex].drawerData;

                    slot.AddBuildOnSlot(drawer, material, material.minQuantity);
                }
                else
                {
                    slot.AddBuildOnSlot(material.notRandDrawer, material, material.minQuantity);
                }

                combinationsArray.RemoveAt(i);

                continue;
            }
        }

        //Step 2 : Complet empty cell with random material and drawer combination
        foreach (BuildSlot slot in GetComponentsInChildren<BuildSlot>())
        {
            if (slot.ContentCount == 0)
            {
                if (combinationsArray.Count == 0)
                {
                    Debug.LogWarning("The number of combination is inferior to the number of slots", this);
                    continue;
                }

                int randMaterialId = Random.Range(0, combinationsArray.Count);
                int randDrawerId = Random.Range(0, combinationsArray[randMaterialId].Count);
                BuildDataForSelecter randId2 = combinationsArray[randMaterialId][randDrawerId];

                BuildMaterialData material = randId2.materialData;

                if (material.hasRandDrawer)
                {
                    BuildDrawerData drawer = randId2.drawerData;

                    slot.AddBuildOnSlot(drawer, material, material.minQuantity);

                    combinationsArray[randMaterialId].RemoveAt(randDrawerId);

                    if (combinationsArray[randMaterialId].Count == 0)
                    {
                        combinationsArray.RemoveAt(randMaterialId);
                    }
                }
                else
                {
                    slot.AddBuildOnSlot(material.notRandDrawer, material, material.minQuantity);
                    combinationsArray.RemoveAt(randMaterialId);
                }
            }
        }

        //Step 3 : Count the number of segments in selecter to determine if all the constructions will fit into the grid
        int nbSegmentsInSelector = 0;
        int nbSegmentsMax = Mathf.FloorToInt(GameManager.Instance.NbCellsInAllGrid * percentOfAllowedSegments);
        foreach (BuildSlot slot in GetComponentsInChildren<BuildSlot>())
        {
            BuildDrawer drawer = slot.ContainedBuild.Drawer;
            nbSegmentsInSelector += drawer.NbSegments * slot.ContentCount;
        }
        if (nbSegmentsInSelector > nbSegmentsMax)
        {
            Debug.LogWarning("The number of segments in the selector is superior to the number of segments allowed", this);
        }

        //Step 4 : Increase the number of build per slots depending on the previous authorized number
        int nbSegmentsToAdd = nbSegmentsMax - nbSegmentsInSelector;
        for (int i = 0; i < nbSlots; i++)
        {
            BuildSlot slot = GetComponentsInChildren<BuildSlot>()[i];
            Build build = slot.ContainedBuild;

            int targetRange = Random.Range(0, build.Data.materialData.maxQuantity - slot.ContentCount);
            int nbSegments = build.Drawer.NbSegments;
            int rangeReached = 0;

            for (int j = 0; j < targetRange; j++)
            {
                if ((slot.ContentCount + j) * nbSegments <= nbSegmentsToAdd)
                    rangeReached = j;
                else
                    break;
            }

            slot.AddBuildOnSlot(rangeReached);
            nbSegmentsToAdd -= rangeReached * nbSegments;
        }

        //Step 5 : Distributes the last quantities which could not be distributed previously
        int nbFails = 0;
        while (nbFails < nbSlots)
        {
            nbFails = 0;

            for (int i = 0; i < nbSlots; i++)
            {
                BuildSlot slot = GetComponentsInChildren<BuildSlot>()[i];
                Build build = slot.ContainedBuild;

                int nbSegments = build.Drawer.NbSegments;

                if (nbSegments <= nbSegmentsToAdd && slot.ContentCount + 1 <= build.Data.materialData.maxQuantity)
                {
                    slot.AddBuildOnSlot(1);
                    nbSegmentsToAdd -= nbSegments;
                }
                else
                {
                    nbFails++;
                    continue;
                }
            }
        }

        GameManager gm = GameManager.Instance;
        gm.CurrentBuildsInfo.nbBuildsRemaining = gm.CurrentBuildsInfo.nbBuilds;
        foreach (BuildSlot slot in GetComponentsInChildren<BuildSlot>())
        {
            if (slot.ContainedBuild.Material == BuildMaterialData.Material.PROT)
            {
                gm.CurrentBuildsInfo.kingsRemaining = slot.ContentCount;
            }
        }
        gm.CurrentBuildsInfo.nbKingsAlive[gm.currentPlayerIndex] = gm.CurrentBuildsInfo.kingsRemaining;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    Build build = BuildCursor.Instance.BuildSelected;
    //    if (build != null && build.CheckCurrentState<BuildState.OnDragState>())
    //    {
    //        build.ChangeState<BuildState.HoverSelecterState>();
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    Build build = BuildCursor.Instance.BuildSelected;
    //    if (build != null && build.CheckCurrentState<BuildState.HoverSelecterState>())
    //    {
    //        build.ChangeState<BuildState.OnDragState>();
    //    }
    //}

    private void Update()
    {
        if (GameManager.Instance.constructionDone)
        {
            Destroy(gameObject);
        }
    }


    #endregion

    #region PrivateMethods
    void RefreshSlots()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < nbSlots; i++)
        {
            GameObject newCell = Instantiate(cellPrefab);
            newCell.transform.SetParent(transform, false);
        }
    }
    #endregion

    #region PublicMethods

    public int GetNbAllCellContent()
    {
        int nbContent = 0;
        foreach (BuildSlot slot in GetComponentsInChildren<BuildSlot>())
        {
            nbContent += slot.ContentCount;
        }
        return nbContent;
    }

    public void RefreshSelecter()
    {
        RefreshSlots();
        StartCoroutine(RetardStartOneFrame());
    }
    #endregion

    #region Properties/Accessors

    //(access modifier)type name{ get {return someVar;} set{ name = anotherVar}}
    #endregion


}
