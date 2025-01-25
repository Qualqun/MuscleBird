using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CannonManager : Singleton<CannonManager>
{
    // defines
    const float defaultTimer = 5.0f;
    const Cannon defaultCannon = Cannon.KIWI;

    enum Cannon
    {
        KIWI,
        LLAMA
    }

    public Image buttonImage;

    [SerializeField]
    Image loadingImage;

    [SerializeField]
    Sprite[] spritesButton;

    [SerializeField]
    GameObject[] cannons;

    [SerializeField]
    Transform tractionBar;

    public GameObject swapButton;

    private GameObject instateCannons;

    private Cannon actualCannon = defaultCannon;

    float timerSpawnCannon = defaultTimer;

    public Vector2 PositionCannon { get { return transform.position; } set { transform.position = value; } }
    int nbShoot = 0;
    bool canSpawn = true;
    public bool cannonPause = false;
    public Action destroyAllLaunchedBullets;

    private void Start()
    {
        swapButton.SetActive(false);
        loadingImage.color = Color.white;
    }

    private void Update()
    {

        if (instateCannons == null)
        {
            timerSpawnCannon -= Time.deltaTime;

            loadingImage.enabled = true;
            loadingImage.fillAmount = canSpawn ? 1 - (timerSpawnCannon / defaultTimer) : 1;
            loadingImage.color = canSpawn ? Color.white : Color.red;

            if (timerSpawnCannon < 0)
            {
                destroyAllLaunchedBullets?.Invoke();
                timerSpawnCannon = defaultTimer;
                CreateCannon();
            }
        }
        
        if(!swapButton.activeSelf)
        {
            loadingImage.enabled = false;
        }
        
    }

    public void SwapCannon()
    {
        if (timerSpawnCannon >= defaultTimer)
        {
            buttonImage.sprite = spritesButton[(int)actualCannon];

            switch (actualCannon)
            {
                case Cannon.KIWI:
                    actualCannon = Cannon.LLAMA;
                    break;
                case Cannon.LLAMA:
                    actualCannon = Cannon.KIWI;
                    break;
                default:
                    break;
            }

            CreateCannon();
        }
    }

    public void StateCannon(bool _state)
    {

        for (int i = 0; i < nbShoot; i++)
        {
            cannons[i].SetActive(_state);
        }
        // instateCannons.GetComponent<CircleCollider2D>().enabled = _state;
    }

    public void CreateCannon()
    {
        if (instateCannons != null)
        {
            Destroy(instateCannons.gameObject);
        }

        if (canSpawn)
        {
            switch (actualCannon)
            {
                case Cannon.KIWI:
                    instateCannons = Instantiate(cannons[(int)Cannon.KIWI], transform.position, Quaternion.identity, transform);
                    break;
                case Cannon.LLAMA:
                    instateCannons = Instantiate(cannons[(int)Cannon.LLAMA], transform.position, Quaternion.identity, transform);
                    break;
                default:
                    break;
            }

            if(GameManager.Instance.currentPlayerIndex == 0)
            {
                Vector3 scaleCannon = instateCannons.transform.localScale;
                Vector3 scaleTractionBar = tractionBar.transform.localScale;
                scaleCannon.x = -1;
                scaleTractionBar.x = -1;

                instateCannons.transform.localScale = scaleCannon;
                tractionBar.transform.localScale = scaleCannon;
            }
            else
            {
                Vector3 scaleTractionBar = tractionBar.transform.localScale;
                scaleTractionBar.x = 1;

                tractionBar.transform.localScale = scaleTractionBar;
            }


            tractionBar.position = transform.position;
            timerSpawnCannon = defaultTimer;
            loadingImage.enabled = false;
           
        }
        
    }

    /// <summary>
    /// Change de position first
    /// </summary>
    public void ResetCannon()
    {
        actualCannon = defaultCannon;
        nbShoot = 0;
        canSpawn = true;
        buttonImage.sprite = spritesButton[1];
        CreateCannon();
    }

    bool GetSelected()
    {
        if (instateCannons != null)
        {
            if (instateCannons.GetComponent<CannonBase>().Selected == true)
            {
                return true;
            }
        }

        return false;

    }

    public bool InstatiateCannonIsSelected()
    {

        if (instateCannons != null)
        {
            return instateCannons.GetComponent<CannonBase>().Selected;
        }

        return false;
    }
    public void HadShot()
    {
        if (nbShoot < 2)
        {
            nbShoot++;  
        }
        else
        {
            canSpawn = false;
        }

       
    }
}
