using BuildState;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class BuildSlot : MonoBehaviour/*, BuildCursor.Behavior*/
{
    #region Fields
    Build containedBuild;
    Build generatedBuild;
    Vector3 baseElementScale;
    int contentCount = 0;
    #endregion

    #region Messages
    private void Update()
    {
        if (containedBuild != null && containedBuild.isActiveAndEnabled)
        {
            //Craquage
            containedBuild.transform.position = Camera.main.ScreenToWorldPoint(transform.position);
        }
        //Craquage 2
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.y);

    }
    #endregion

    #region PrivateMethods

    #endregion

    #region PublicMethods
    public void AddBuildOnSlot(BuildDrawerData drawerData, BuildMaterialData materialData, int quantity = 1)
    {
        if (contentCount == 0 && quantity > 0)
        {

            if (containedBuild != null)
            {
                containedBuild.gameObject.SetActive(true);
            }
            else
            {
                //containedBuild = GameManager.Instance.BuildFactory.CreateBuildOnUI(drawerData, materialData, this);
                containedBuild.transform.SetParent(transform, false);
                GetComponentInChildren<MatrixImageGenerator>().GenerateImagesFromMatrix();
            }
        }

        contentCount += quantity;
        GameManager.Instance.CurrentBuildsInfo.nbBuilds += quantity;
        GetComponentInChildren<TextMeshProUGUI>().text = contentCount > 0 ? "x" + contentCount.ToString() : "";

    }

    public void AddBuildOnSlot(int quantity)
    {
        AddBuildOnSlot(containedBuild.Drawer.BaseDataUnused, containedBuild.Data.materialData, quantity);
    }

    public void RemoveBuildOnSlot(int quantity)
    {
        if (contentCount - quantity < 1)
        {
            contentCount = 0;
            containedBuild.gameObject.SetActive(false);
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
        generatedBuild = null;
    }

    public void OnDrag(Vector3 dragPos)
    {
        throw new System.NotImplementedException();
    }

    public void PickBuildButton()
    {
        BuildCursor.Instance.PickBuildOnUI(containedBuild);
    }
    #endregion

    #region Properties/Accessors
    public int ContentCount { get => contentCount; }
    public Build ContainedBuild { get => containedBuild; }
    public Build GeneratedBuild { get => generatedBuild; set => generatedBuild = value; }

    #endregion


}
