
using UnityEngine;
using UnityEngine.UI;

public class ButtonEndBuild : MonoBehaviour
{
    public void CurrentPlayerFinishBuilding()
    {
        GameManager gm = GameManager.Instance;

        if (gm.CurrentBuildsInfo.kingsRemaining > 0)
        {
            //Debug.Log("There are kings left");
            return;
        }

        gm.HandleConstructionPhase();
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm.CurrentBuildsInfo.kingsRemaining > 0)
        {
            GetComponent<Image>().color = Color.grey;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }


        if (GameManager.Instance.constructionDone)
        {
            gameObject.SetActive(false);
        }
    }
}
