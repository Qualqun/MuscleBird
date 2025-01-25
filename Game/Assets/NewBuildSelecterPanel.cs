using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class NewBuildSelecterPanel : MonoBehaviour
{
    List<Tuple<BuildMaterialData, BuildDrawerData>> blackPairList = new List<Tuple<BuildMaterialData, BuildDrawerData>>();

    private void Awake()
    {
        GameManager.Instance.RefreshSelecter += ReloadSlots;
    }

    private void Update()
    {
        if (GameManager.Instance.constructionDone)
        {
            gameObject.SetActive(false);
        }
    }

    public NewBuildSlot GetBuildSlotPressed()
    {
        foreach (NewBuildSlot slot in GetComponentsInChildren<NewBuildSlot>())
        {
            if (slot.IsMouseOver())
            {
                return slot;
            }
        }
        return null;
    }

    public void ReloadSlots()
    {
        blackPairList.Clear();

        foreach (NewBuildSlot slot in GetComponentsInChildren<NewBuildSlot>())
        {
            slot.ResetContentCount();
        }
    }

    public Tuple<BuildMaterialData, BuildDrawerData> GetPairFromWhiteList(List<BuildMaterialData> ListM, List<BuildDrawerData> ListD)
    {
        ListM = ListM.OrderBy(x => Guid.NewGuid()).ToList();
        ListD = ListD.OrderBy(x => Guid.NewGuid()).ToList();

        foreach (var m in ListM)
        {
            foreach (var d in ListD)
            {
                bool findInBL = false;

                foreach (var blackPair in blackPairList)
                {
                    findInBL = m == blackPair.Item1 && d == blackPair.Item2;
                }

                if (!findInBL)
                {
                    Tuple<BuildMaterialData, BuildDrawerData> pair = new Tuple<BuildMaterialData, BuildDrawerData>(m, d);
                    blackPairList.Add(pair);
                    return pair;
                }
            }
        }
        return null;
    }
}
