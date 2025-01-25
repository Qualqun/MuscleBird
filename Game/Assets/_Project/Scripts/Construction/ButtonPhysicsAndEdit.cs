using UnityEngine;
using UnityEngine.UI;

public class ButtonPhysicsAndEdit : MonoBehaviour
{
    public BuildsStack[] buildsStack;
    public Sprite[] sprites;

    private void Start()
    {
        GameManager.Instance.RefreshSelecter += ReturnToEditMode;
    }

    public void SwapBetweenPhysicsAndEdit()
    {
        GameManager gm = GameManager.Instance;

        if (gm.inEditMode)
            TestPhysics();
        else
            ReturnToEditMode();
    }

    void TestPhysics()
    {
        buildsStack[GameManager.Instance.currentPlayerIndex].TestPhysics();
        //GetComponentInChildren<Text>().text = "RETURN TO EDIT MODE";
        GameManager.Instance.inEditMode = false;
        GetComponentInChildren<Image>().sprite = sprites[1];
    }

    void ReturnToEditMode()
    {
        buildsStack[GameManager.Instance.currentPlayerIndex].ResetBuilds();
        //GetComponentInChildren<Text>().text = "TEST PHYSICS";
        GameManager.Instance.inEditMode = true;
        GetComponentInChildren<Image>().sprite = sprites[0];
    }

    private void Update()
    {
        if (GameManager.Instance.constructionDone)
        {
            gameObject.SetActive(false);
        }
    }


}
