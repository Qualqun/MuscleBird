using TMPro;
using UnityEngine;

public class TutorialSteps : MonoBehaviour
{

    [SerializeField]
    TMP_Text tutorialTxt;

    [SerializeField]
    GameObject holeSelect;

    [SerializeField]
    GameObject constructionBackground;

    [SerializeField]
    GameObject lamaShooted;

    int tutorialSteps = 0;


    private void Start()
    {
        if (!PersistentGameData.Instance.activeTutorial)
        {
            gameObject.SetActive(false);
        }
        PersistentGameData.Instance.activeTutorial = false;
        ActivateSteps();
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ActivateSteps();
        }

        //Debug.Log("Screen Width : " + Screen.width);
    }
    //699 283
    void ActivateSteps()
    {
        switch (tutorialSteps)
        {
            case 0:
                tutorialTxt.text = "Hello, welcome to Birdland. Today I'm going to show you step by step how to play this wonderful game";

                break;

            case 1:
                tutorialTxt.text = "First, there is the construction phase";
                break;

            case 2:
                tutorialTxt.text = "Your objective is simple: prevent your opponent from touching your bodybuilding equipment.";

                break;

            case 3:

                tutorialTxt.text = "So you'll need to prepare your defenses on this grid";

                SetTextPos(612, 14, 534, 382);
                SetTargetPos(0, 0, 6,7);

                break;

            case 4:
                tutorialTxt.text = "You have different construction parts";

                SetTextPos(0, 0);
                SetTargetPos(0, -448, 25, 3);

                break;

            case 5:
                tutorialTxt.text = "You will have to put on all your equipment to be able to pass to the next player";
                SetTargetPos(-810, -454, 3);

                break;

            case 6:
                tutorialTxt.text = "Here you can test your construction";

                SetTargetPos(-796, 0, 3);

                break;
            case 7: 
                tutorialTxt.text = "Once you are ready, you can hand over to your opponent";

                SetTargetPos(860, 0, 3);
                break;

            case 8:
                tutorialTxt.text = "Now that you've seen the construction phase, let's move on to the play phase";
                break;

            case 9:
                tutorialTxt.text = "Your objective is simple! Grab all this great equipment and become the most muscular person in your region";
                constructionBackground.SetActive(false);
                SetTargetPos(27, -327, 3);
                break;

            case 10:
                tutorialTxt.text = "You'll be helped by your gymbros, who are ready to sacrifice themselves for you";
                SetTargetPos(-840, 74, 3);
                break;

            case 11:
                tutorialTxt.text = "You can choose between two gymbro";
                SetTargetPos(-891, -302, 2);
                break;

            case 12:
                tutorialTxt.text = "When it's thrown, you can press again to make it spit";
                lamaShooted.SetActive(true);
                SetTargetPos(-103, 44, 5);
                SetTextPos(560, 0, 542, 378);

                break;
            case 13:
                tutorialTxt.text = "If you have finished, or consider that you cannot do any better, you may skip your turn";
                SetTargetPos(717, 0, 3);
                SetTextPos(0, 0);
                break;
            case 14:
                tutorialTxt.text = "You know everything now, you have no excuse not to become a mountain of muscle reigning supreme over the world";
                holeSelect.SetActive(false);
                break;
            case 15:
                gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Error step");
                break;

        }


        tutorialSteps++;

    }


    void SetTargetPos(float _posX, float _posY, float _scale)
    {
        holeSelect.SetActive(true);

       
        holeSelect.transform.localPosition = new Vector2(_posX, _posY);
        holeSelect.transform.localScale = new Vector3(_scale, _scale, _scale);
    }

    void SetTargetPos(float _posX, float _posY, float _scaleX, float _scaleY)
    {
        Vector2 posScreen = new Vector2(_posX, _posY);
        posScreen.x = posScreen.x * Screen.width / 1920;
        posScreen.y = posScreen.y * Screen.height / 1080;

        holeSelect.SetActive(true);

        holeSelect.transform.localPosition = new Vector2(_posX, _posY);
        holeSelect.transform.localScale = new Vector2(_scaleX, _scaleY);
    }


    void SetTextPos(float _posX, float _posY, int _width = 699, int _height = 283, int _fontSize = 50)
    {

        tutorialTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(_posX, _posY);
        tutorialTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _height);

        tutorialTxt.fontSize = _fontSize;

    }



}
