using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHudManager : Singleton<GameHudManager>
{
    [SerializeField]
    private GameObject m_pauseLayout;
    [SerializeField]
    private GameObject m_pauseButton;

    [SerializeField]
    private Image player1Tag;
    [SerializeField]
    private TMP_Text player1Text;

    [SerializeField]
    private Image player2Tag;
    [SerializeField]
    private TMP_Text player2Text;

    [SerializeField] private GameObject m_soundOn;
    [SerializeField] private GameObject m_soundOff;
    [SerializeField] private GameObject m_musicOn;
    [SerializeField] private GameObject m_musicOff;

    [SerializeField] private GameObject m_endScreen;
    [SerializeField] private GameObject m_player1Win;
    [SerializeField] private GameObject m_player2Win;

    [Header("Kings Player1")]
    public Image m_player1King1;
    public Image m_player1King2;
    public Image m_player1King3;

    [Header("Kings Player2")]
    public Image m_player2King1;
    public Image m_player2King2;
    public Image m_player2King3;

    [SerializeField] public TMP_Text player1TimerText, player2TimerText;
    [SerializeField] public TMP_Text player1ScoreText, player2ScoreText;

    #region Messages
    void Start()
    {
        m_endScreen.SetActive(false);
        m_pauseButton.SetActive(true);
        m_pauseLayout.SetActive(false);
        bool soundCondition = (PoolManager.gameVolume == 1.0f ? true : false);
        m_soundOn.SetActive(soundCondition);
        m_soundOff.SetActive(!soundCondition);
        m_musicOn.SetActive(soundCondition);
        m_musicOff.SetActive(!soundCondition);
        m_player1Win.SetActive(false);
        m_player2Win.SetActive(false);

        player1TimerText.gameObject.SetActive(false);
        player2TimerText.gameObject.SetActive(false);

        m_player1King1.color = new Color(m_player1King1.color.r, m_player1King1.color.g, m_player1King1.color.b, 0.0f);
        m_player1King2.color = new Color(m_player1King2.color.r, m_player1King2.color.g, m_player1King2.color.b, 0.0f);
        m_player1King3.color = new Color(m_player1King3.color.r, m_player1King3.color.g, m_player1King3.color.b, 0.0f);
        m_player2King1.color = new Color(m_player2King1.color.r, m_player2King1.color.g, m_player2King1.color.b, 0.0f);
        m_player2King2.color = new Color(m_player2King2.color.r, m_player2King2.color.g, m_player2King2.color.b, 0.0f);
        m_player2King3.color = new Color(m_player2King3.color.r, m_player2King3.color.g, m_player2King3.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    public void PauseClick()
    {
        m_pauseButton.SetActive(!m_pauseButton.activeSelf);
        m_pauseLayout.SetActive(!m_pauseLayout.activeSelf);
        CannonManager.Instance.cannonPause = m_pauseLayout.activeSelf;
        
    }

    public async void Leave()
    {
        TouchManager.Instance.canSlide = false;
        GameManager.Instance.ButtonEndTurn.gameObject.SetActive(true);
        CannonManager.Instance.buttonImage.gameObject.SetActive(true);
        await SceneLoader.Instance.LoadSceneGroup(0);
    }

    #region PublicMethods
    public void DisablePlayerTag(Player.PlayerID playerID)
    {
        if (playerID == Player.PlayerID.Player1)
        {          
            player1Tag.color = Color.white;
            player1Text.color = Color.blue;

            player2Tag.color = Color.grey;
            player2Text.color = Color.grey;
        }
        else
        {
            player1Tag.color = Color.grey;
            player1Text.color = Color.grey;

            player2Tag.color = Color.white;
            player2Text.color = Color.red;
        }
    }
    public void SoundToggle()
    {
        if (PoolManager.gameVolume == 1.0f)
        {
            m_soundOn.SetActive(false);
            m_soundOff.SetActive(true);
            PoolManager.gameVolume = 0.0f;
        }
        else
        {
            m_soundOn.SetActive(true);
            m_soundOff.SetActive(false);
            PoolManager.gameVolume = 1.0f;
        }
    }
    public void MusicToggle()
    {
        if (PoolManager.gameVolume == 1.0f)
        {
            m_musicOn.SetActive(false);
            m_musicOff.SetActive(true);
            PoolManager.gameVolume = 0.0f;
        }
        else
        {
            m_musicOn.SetActive(true);
            m_musicOff.SetActive(false);
            PoolManager.gameVolume = 1.0f;
        }
    }

    public async void BackToMenu()
    {
        TouchManager.Instance.canSlide = false;
        GameManager.Instance.ButtonEndTurn.gameObject.SetActive(true);
        await SceneLoader.Instance.LoadSceneGroup(0);
    }
    public void CheckForWinner()
    {
        int winnerId = -1;
        if (m_player1King1.color.a == 1.0f
           && m_player1King2.color.a == 1.0f
            && m_player1King3.color.a == 1.0f)
        {
            winnerId = 1;
        }
        else if (m_player2King1.color.a == 1.0f
           && m_player2King2.color.a == 1.0f
            && m_player2King3.color.a == 1.0f)
        {
            winnerId = 0;
        }
        else
        {
            return;
        }
        TouchManager.Instance.canSlide = false;
        CannonManager.Instance.cannonPause = true;
        m_endScreen.SetActive(true);
        m_pauseButton.SetActive(false);
        m_pauseLayout.SetActive(false);
        GameManager.Instance.ButtonEndTurn.gameObject.SetActive(false);
        CannonManager.Instance.buttonImage.gameObject.SetActive(false);
        (winnerId != 0 ? m_player1Win : m_player2Win).SetActive(true);
        //Debug.Log("Winner set");
    }
    #endregion


}
