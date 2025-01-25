using BuildState;
using UnityEngine;

public class BuildsStack : MonoBehaviour
{
    bool isPhisicEnabled;

    public void TestPhysics(bool enable = true)
    {
        isPhisicEnabled = enable;

        foreach (Build build in GetComponentsInChildren<Build>())
        {
            build.EnablePhysics(enable);
        }
    }

    public void ResetBuilds()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        foreach (Build build in GetComponentsInChildren<Build>())
        {
            //Debug.Log("TEST");
            TestPhysics(false);
            build.ChangeState<BuildState.DropedState>();
            build.ResetPreviousTransform();
        }
    }

    public int GetNbBuildInStack()
    {
        return transform.childCount;
    }

    public void AddBuildOnStack(Build build)
    {
        build.transform.SetParent(transform);
        GameManager.Instance.CurrentBuildsInfo.nbBuildsRemaining--;

        if (build.Material == BuildMaterialData.Material.PROT)
        {
            if (GameManager.Instance.CurrentBuildsInfo.kingComponents == null)
            {// ? Shoudln't be there but not enough time to refactor
                GameManager.Instance.CurrentBuildsInfo.kingComponents = new KingComponent[2][];
                for (int i = 0; i < GameManager.Instance.CurrentBuildsInfo.kingComponents.Length; i++)
                {
                    GameManager.Instance.CurrentBuildsInfo.kingComponents[i] = new KingComponent[4];
                    for (int j = 0; j < GameManager.Instance.CurrentBuildsInfo.kingComponents[i].Length; j++)
                    {
                        GameManager.Instance.CurrentBuildsInfo.kingComponents[i][j] = new KingComponent();
                    }
                }
            }

            for (global::System.Int32 i = 0; i < GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex].Length; i++)
            {
                if (GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_waitingForActivation)
                {
                    GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_active = true;
                    GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_waitingForActivation = false;
                    build.Data.kingId = i;
                    break;
                }
            }
            //Debug.Log("New king : " + build.Data.kingId +" for player " + GameManager.Instance.currentPlayerIndex);
            GameManager.Instance.CurrentBuildsInfo.kingsOnGrid++;
            GameManager.Instance.CurrentBuildsInfo.kingsRemaining--;
        }
    }

    public void RemoveBuildOnStack(Build build)
    {
        build.transform.SetParent(null);
        GameManager.Instance.CurrentBuildsInfo.nbBuildsRemaining++;

        if (build.Material == BuildMaterialData.Material.PROT)
        {
            GameManager.Instance.CurrentBuildsInfo.kingsOnGrid--;
            GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][build.Data.kingId].m_active = false;
            GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][build.Data.kingId].m_waitingForActivation = false;
            //Debug.Log("Removing king : " + build.Data.kingId);
            GameManager.Instance.CurrentBuildsInfo.kingsRemaining++;
        }
    }

    public void LoadPlayMode()
    {
        ResetBuilds();
        foreach (Build build in GetComponentsInChildren<Build>())
        {
            build.ChangeState<OnPlayModeState>();
        }
    }
    public bool IsPhysicEnabled { get => isPhisicEnabled; }
}

